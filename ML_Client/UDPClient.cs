using ML_server;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace ML_Client
{
    public class UDPClient
    {

        public static void makeClient()
        {
            Console.WriteLine("Client side!");

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPAddress broadcast = IPAddress.Parse("127.0.0.1");
            //Console.WriteLine("Enter message to send to broadcast address");
            //var message = Console.ReadLine();

            string folderpath = "C:\\Users\\Administrator\\Desktop\\feb16-udpstuff";
            
            string imgwarmuppath = Path.Combine(folderpath, "thread_clean_warmup_gpu.jpg");
            string imgpath = Path.Combine(folderpath, "tempImage.jpg");

            string[] imagepaths = { imgwarmuppath, imgpath, imgpath };
            string message = "";

            int cnt = 0;

            foreach (string img in imagepaths)
            {
                message = img;
                Console.WriteLine($"sending message:  {img}");
                System.Threading.Thread.Sleep(5000);


                while (message != string.Empty && message != null)
                {
                    byte[] sendbuf = Encoding.ASCII.GetBytes(message);
                    IPEndPoint ep = new IPEndPoint(broadcast, 80);

                    s.SendTo(sendbuf, ep);

                    Console.WriteLine("Message sent to the broadcast address");

                    //message = Console.ReadLine();

                    message = string.Empty;

                    cnt++;
                }

                if (cnt == 0)
                {
                    // add wait here
                    System.Threading.Thread.Sleep(40000);
                }
                
                else
                {
                    System.Threading.Thread.Sleep(30000);
                }

            }



        }
        public static void Main(string[] args)
        {
            makeClient();   

            Console.ReadLine();
        }
    }

}
