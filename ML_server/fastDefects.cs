//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Windows.Foundation;

//namespace ML_Server
//{
//    public class DefectProperties
//    {
//        // added to utilize new DL model
//        // TODO - UPDATE by removing Cognex object artifacts

//        public string filePath { get; set; }
//        public double x { get; set; }

//        public double y { get; set; }

//        public double height { get; set; }

//        public double width { get; set; }

//        public string bestTagName { get; set; }
//        public double bestTagScore { get; set; }

//        public double Redscore { get; set; }

//        public List<string> tags { get; set; }
//        public List<double> GreenScores { get; set; }

//    }


//    internal class fastDefects
//    {
//        // modified from production level

//        public string temp_image_path = "ddd";
//        public bool activate_detection = true;
//        public bool isNoseScan = false;

//        public List<DefectProperties> customDefects;


//        public IEnumerable<DefectProperties> GetDefects(Bitmap bitmap, bool isNoseScan)
//        {

//            var watch = new System.Diagnostics.Stopwatch();

//            watch.Start();

//            //string basePath = Environment.CurrentDirectory;

//            //bitmap.SaveAsJpg(temp_image_path);

//            // make sure defect detection is enabled in constants.

//            if (activate_detection) { customDefects = makePredictions(temp_image_path, isNoseScan); }

//            else { customDefects = new List<DefectProperties>(); }

//            watch.Stop();

//            //System.Windows.MessageBox.Show("defects found." + " exe time in seconds: " + (watch.ElapsedMilliseconds / 1000).ToString());

//            // now construct defect object for UI

//            //var defects = new List<DefectProperties>();
//            //double s = 96; //128

//            //foreach (DefectProperties d in customDefects)
//            //{

//            //    var rect = new Rect(
//            //        (int)(d.x - s / 2),
//            //        (int)(d.y - s / 2),
//            //        (int)s,
//            //        (int)s);


//            //    //var thumbnail = DefectDetectorCommon.CreateDefectThumbnail(bitmap, rect);

//            //    //defects.Add(new Defect(rect, d.bestTagName, thumbnail));
//            //}

//            return customDefects;
//        }


//        public List<DefectProperties> makePredictions(string imagePath, bool isNose)
//        {

//            //System.Windows.MessageBox.Show("running ML defect detector. current wait is 40-60seconds.");

//            var psi = new ProcessStartInfo(); // run the python script

//            psi.FileName = ML.pythonEnvPath;

//            // Provide script and arguments
//            var script = ML.predictionScriptPath;
//            var mdl1 = ML.m1Path;
//            var mdl2 = ML.m2Path;
//            //var mdl2 = ML.singleModelPath; // using inception here

//            var mdl3 = ML.m3Path;

//            var defects = ML.defectsThreadCSVpath;
//            if (isNose) { defects = ML.defectsNoseCSVpath; }

//            // delete old defects file

//            //File.Delete(defects);

//            var mlModelPath = ML.mlModelPath;

//            Console.WriteLine("PREDICTION SEQUENCE - sending test image to python prediction...");

//            psi.Arguments = $"\"{script}\" \"{imagePath}\" \"{mlModelPath}\" \"{mdl2}\" \"{mdl3}\" \"{defects}\"";

//            // process config

//            psi.UseShellExecute = false;
//            psi.CreateNoWindow = true;
//            psi.RedirectStandardOutput = true;
//            psi.RedirectStandardError = true;

//            // execute process and get output

//            var errors = "";
//            var results = "";

//            using (var process = Process.Start(psi))
//            {
//                errors = process.StandardError.ReadToEnd();
//                results = process.StandardOutput.ReadToEnd();
//            }

//            List<DefectProperties> myList = new List<DefectProperties>();

//            string[] lines = File.ReadAllLines(defects);

//            if (lines.Length > 2) // make sure they exist
//            {
//                for (int i = 1; i < lines.Length; i++) // first line is header
//                {
//                    string hm_line = lines[i].Trim();

//                    string[] hm_vals = hm_line.Split(',');

//                    double x = Convert.ToDouble(hm_vals[1]);
//                    double y = Convert.ToDouble(hm_vals[2]);
//                    string label = hm_vals[4];

//                    DefectProperties addMe = new DefectProperties();
//                    addMe.x = x;
//                    addMe.y = y;
//                    addMe.bestTagName = label;

//                    myList.Add(addMe);

//                }
//            }

//            return myList;

//        }
//    }
//}
