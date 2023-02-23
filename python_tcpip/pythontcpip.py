"""
DL - Thread UNET model. 
7 Classes:
    1. not pipe
    2. clean
    3. dent
    4. crack
    5. scratch
    6. paint
    7. debris
Using model: UNET_256x256_20nov_2022_final.hdf5
12/15 - updated model "C:/Users/TSI/Desktop/UNET_256x256_15dec_2022.h5"
12/19 - defect detector, optimized for runtime
      - prediction made. Total elapsed time in seconds,  68.8836419582367
12/20 - prediction made. Total elapsed time in seconds,  17.010241985321045 - HMI
      - prediction made. Total elapsed time in seconds,  16.02392268180847 - HMI updated.
12/28 - updating model with UNET ensemble of weighted models with various backbones.
      - note: need pip install segmentation-models for this approach
      - Model backbones: Resnet, InceptionV3, VGG
      - each model makes a prediction, then we combine predictions to get highest mIOU peroformance.
12/29 - Ensembeled UNET models greatly improved defect detector performance. Need to reduce run time, now it is 30-45 seconds. 
01/30 - Added DBSCAN, removed object det algo, 12.5 second prediction time.
"""
#ROOT  = r"C:\Users\TSI\Desktop"

import os
import cv2
import numpy as np
import matplotlib.pyplot as plt
import pandas as pd
import tensorflow as tf
from tensorflow import keras
import time # measure runtime
import math
import sys
import socket
from sklearn.cluster import DBSCAN

def reshape_split(image: np.ndarray, kernel_size: tuple):
    h,w,c = image.shape
    x, y = kernel_size
    tiled_array = image.reshape(h//x, x, w//y, y, c)
    tiled_array = tiled_array.swapaxes(1,2)
    return tiled_array

def resize_image(image,s=256):
    """use this to pad image for quick reshaping """
    h,w,c = image.shape
    h_n = math.ceil(h/s)*s
    w_n = math.ceil(w/s)*s
    x = np.zeros((h_n, w_n,c),dtype=np.uint8)
    # add previous values to new image
    x[0:h,0:w,:] = image
    return x

def create_mask(pred_mask):
    pred_mask = tf.math.argmax(pred_mask, axis=-1)
    pred_mask = pred_mask[..., tf.newaxis]
    return pred_mask[0]

def normalize_array(data):
    """converts essentially to greyscale"""
    n = 255*(data/np.nanmax(data))
    return n.astype(np.uint8)

def df_defects_loc_by_pred(a):
    """look at each prediction, return list of values where defect exists, in x and y"""
    outlist = []
    
    for i in range(1,6):
        zipme = list(zip(*np.where(a == i)))
        for (x,y) in zipme:
            outlist.append([y,x,i]) # order is reversed here, image/point issue... resolved.
            
    df = pd.DataFrame(outlist,columns=["x","y","m"])
    return df

def collapse_prediction(pred, kernel):
    """reshape prediction from model output, show as 2D array with classes assigned per pixel"""
    pred = np.reshape(pred, kernel)
    mask = create_mask(pred)            
    y_c = np.nanmax(mask, axis=2) # collapse mask into 2D array.
    prediction = y_c.astype(np.uint8) # change dtype
    return prediction


def plot_defects(image_path, df_defects, title_label):
    """
    create rectangles around defects and overlay on image
    """
    s = 92 // 2
    
    image = cv2.imread(image_path, cv2.COLOR_BGR2RGB)    
    img_c = image.copy()
    
    #colors = ['red', 'green', 'blue', 'fuchsia', 'orange', 'dark blue, 'black']
    colors = [(255,0,0),(0,255,0),(0,0,255),(255,0,255),(255,120,0),(100,50,25),(0,0,0)]

    defects = df_defects.values.tolist()

    for i, count in enumerate(defects):

        x = defects[i][0]
        y = defects[i][1]
        m = int(defects[i][2])
    
        start_p = (x-s,y+s)
        stop_p = (x+s,y-s)
        thickness = 2
        
        img_c = cv2.rectangle(img_c, start_p, stop_p, colors[int(m)], thickness)

    plt.figure(figsize=(12, 8), dpi=256)

    plt.subplot(211),plt.imshow(image)
    plt.title('Original Image'), plt.xticks([]), plt.yticks([])
    plt.subplot(212),plt.imshow(img_c)
    plt.title(title_label), plt.xticks([]), plt.yticks([])

    plt.show()

    #output_path = 'C:\\Users\\TSI\\Desktop\\predictions_dec18.png'
    output_path = '\predictions_dec29.png'

    #output_path =  'C:\\Users\\endle\\Desktop\\dl - thread\\output_prediction\\bounding_final_dec10.png'
    cv2.imwrite(output_path, img_c)
    
def make_label(n):
    """should be a dictionary"""
    label = ['clean', 'dent', 'scratch', 'paint','debris', 'pits', 'not pipe/blue line']
    return label[n]

    
def defectDetector(image_path, mdl_path, defects_path):
    """
    1/13/23 - most current method.
    1/26/23 - adding dbscan clustering technique to reduce number of defects
    """
    
    lst_defects = []

    labels = list(['clean', 'dent', 'scratch', 'paint', 'debris', 'pits', 'not pipe/blue line'])
    #colors = ['red', 'green', 'blue', 'fuchsia', 'orange', 'dark blue, 'black']
    colors = [(255,0,0),(0,255,0),(0,0,255),(255,0,255),(255,120,0),(100,50,25),(0,0,0)]
    s = 256
    k = (s,s)
    n = len(labels)    

    mdl = tf.keras.models.load_model(mdl_path, compile=False)    
    
    x = cv2.imread(image_path)
    img = cv2.cvtColor(x,cv2.COLOR_BGR2RGB)    
    img = resize_image(img,s)
    array = reshape_split(img, k)
    a, b, c, d, e = array.shape
    
    
    for i in range(a):
        
        x = array[i][:][:][:][:] / 255 # (64, 256, 256, 3)
                
        y = mdl.predict_on_batch(x) # (64, 256, 256, 7)
                        
        for j, p in enumerate(y):
            
            prediction = collapse_prediction(p, (-1,s,s,n))
                    
            if np.nanmax(prediction) != 0: # if all clean, don't look for contours

                df_preds = df_defects_loc_by_pred(prediction)

                for idx, row in df_preds.iterrows():

                    x = row['x']
                    y = row['y']
                    m = row['m']

                    if m != 0 and m != 6: # not clean, not blueline or not pipe
                
                        lst_defects.append([x+int(j*s), y+int(i*s), m, labels[m]])

    df = pd.DataFrame(lst_defects,columns=["x","y","m","label"])

    # --------- adding DBscan here

    bol_clustering = True
    bol_limit_size = False
    max_n_defects = 25 # only show 25 top defects

    if bol_clustering:
        
        eps = 20 #15 good! #15 good #12.5 before
        min_n = 25 #15 good! #10 good # 6 

        X = df[['x', 'y', 'm']]
        mdl_dbscan = DBSCAN(eps=eps, min_samples=min_n).fit(X)
        labels = mdl_dbscan.labels_
        df["clustering"] = labels
        
        df = df[df.clustering != -1]
        
        if bol_limit_size: # limit output for faster runttime
            
            df = df[df.clustering < max_n_defects]
        
        df_group = df.groupby(by=["clustering"], dropna=False).mean()
        
        df_group["n"] = round(df_group["m"]).astype('int')
        df_group["label"] = df_group["n"].apply(lambda n: make_label(n))
        df_group['x'] = df_group['x'].astype('int')
        df_group['y'] = df_group['y'].astype('int')
        df_group['m'] = df_group['m'].astype('int')
                
        df_out = df_group[['x', 'y', 'm', 'label']]
        
        df = df_out

    result = df.to_json(orient="index") # convert to json packet

    df.to_csv(defects_path)
    
    return result, df

def main():
    
    # SERVER make this listen for signal to make defects

    mdl_path =r'C:\Users\Administrator\Desktop\feb16-udpstuff\UNET_6100imgs_25e_32b__Jan22b.h5'
    csvs_path = r'C:\Users\Administrator\Desktop\feb23csvs'
    _ip = "127.0.0.1"
    _port = 80

    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.bind((_ip, _port))
    s.listen()

    while True:
        conn, addr = s.accept()          
        data = conn.recv(1024)

        if len(data) > 0:
            
            themessage = data.decode("utf-8")
            m = themessage.split('*')

            image_path = m[0]
            isNose = m[1]

            start = time.perf_counter()
            time_str = time.strftime("%Y%m%d%H%M%S")
            defects_path = os.path.join(csvs_path, time_str + "defects.csv")
            result, df = defectDetector(image_path, mdl_path, defects_path)
            n_defects = len(df)
            stop = time.perf_counter()
            delta = stop - start
            outmessage = " *elapsed time: " + str(delta) + ", num defects: " + str(n_defects) + ", path: " + image_path + " , nose: " + str(isNose) + " , csv: " + defects_path
            
            #plot_defects(image_path, df, image_path)

            # transfer complete
                       
            conn.send(outmessage.encode("utf-8"))
            conn.send("<|ACK|>".encode("utf-8"))

if __name__ == "__main__":
    sys.exit(main())