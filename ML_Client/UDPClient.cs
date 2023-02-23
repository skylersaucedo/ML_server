using ML_server;
using System;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ML_Client
{
    public class UDPClient
    {
        // updating with tcp-ip patterns

        public Socket _clientSocket;

        //public static void makeUDPClient()
        //{
        //    // NO LONGER USING.
        //    Console.WriteLine("Client side!");

        //    Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        //    IPAddress broadcast = IPAddress.Parse("127.0.0.1");
        //    //Console.WriteLine("Enter message to send to broadcast address");
        //    //var message = Console.ReadLine();

        //    string folderpath = "C:\\Users\\Administrator\\Desktop\\feb16-udpstuff";
            
        //    string imgwarmuppath = Path.Combine(folderpath, "thread_clean_warmup_gpu.jpg");
        //    string imgpath = Path.Combine(folderpath, "tempImage.jpg");
        //    string imgpathbug = Path.Combine(folderpath, "thread_bug.jpg");
        //    string imgpathscratches = Path.Combine(folderpath, "thread_scratches_dents.jpg");

        //    string[] imagepaths = {
        //        imgpathscratches, imgpath, imgpathbug,
        //        imgpathscratches, imgpathbug, imgwarmuppath,
        //        imgpath, imgpathbug, imgpathscratches
        //    };

        //    string message = "";
        //    int cnt = 0;

        //    bool isNose = true;

        //    foreach (string img in imagepaths)
        //    {
        //        message = img+"*" + isNose.ToString();
        //        //message = img;

        //        Console.WriteLine($"sending message:  {img}");
        //        System.Threading.Thread.Sleep(5000);

        //        while (message != string.Empty && message != null)
        //        {
        //            byte[] sendbuf = Encoding.ASCII.GetBytes(message);
        //            IPEndPoint ep = new IPEndPoint(broadcast, 80);

        //            s.SendTo(sendbuf, ep);

        //            Console.WriteLine("Message sent to the broadcast address");

        //            //message = Console.ReadLine();

        //            message = string.Empty;

        //            cnt++;
        //        }

        //        if (cnt == 0)
        //        {
        //            // add wait here
        //            System.Threading.Thread.Sleep(40000);
        //        }
                
        //        else
        //        {
        //            System.Threading.Thread.Sleep(30000);
        //        }
        //    }
        //}

        public static async Task Main(string[] args)
        {
            IPEndPoint ipEndPoint = new(IPAddress.Parse("127.0.0.1"), 80);
            using Socket client = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            await client.ConnectAsync(ipEndPoint);

            string folderpath = "C:\\Users\\Administrator\\Desktop\\feb16-udpstuff";

            string imgwarmuppath = Path.Combine(folderpath, "thread_clean_warmup_gpu.jpg");
            string imgpath = Path.Combine(folderpath, "tempImage.jpg");
            string imgpathbug = Path.Combine(folderpath, "thread_bug.jpg");
            string imgpathscratches = Path.Combine(folderpath, "thread_scratches_dents.jpg");

            string[] imagepaths = {
                imgpathscratches, imgpath, imgpathbug,
                imgpathscratches, imgpathbug, imgwarmuppath,
                imgpath, imgpathbug, imgpathscratches
            };

            int cnt = 0;

            bool isNose = true;

            // now connect to ML server with these

            foreach (string img in imagepaths)
            {
                var message = img + "*" + isNose.ToString();

                Console.WriteLine($"sending message:  {img}");

                while (message != string.Empty && message != null)
                {
                    await Task.WhenAll(maketcpClientAsync(client, message));
                }

            }

        }

        public static async Task maketcpClientAsync(Socket client, string message)
        {
            
            while (true)
            {

                var messageBytes = Encoding.UTF8.GetBytes(message);
                _ = await client.SendAsync(messageBytes, SocketFlags.None);
                Console.WriteLine($"Socket client sent message: \"{message}\"");

                // Receive ack.
                var buffer = new byte[1_024];
                var received = await client.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);
                if (response == "<|ACK|>")
                {
                    Console.WriteLine(
                        $"Socket client received acknowledgment: \"{response}\"");
                    //break;
                }
            }

            //client.Shutdown(SocketShutdown.Both); // not sure why my socket keeps closing...

        }
    }
}
