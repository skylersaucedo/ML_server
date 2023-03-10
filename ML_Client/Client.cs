//using ML_Server;
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
    public class Client
    {
        // updated with tcp-ip patterns

        public Socket _clientSocket;

        public static async Task Main(string[] args)
        {
            // test on various images
            IPEndPoint ipEndPoint = new(IPAddress.Parse("127.0.0.1"), 80);

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

            bool isNose = true;

            // now connect to ML server

            foreach (string img in imagepaths)
            {
                var message = img + "*" + isNose.ToString();

                Console.WriteLine($"CLIENT: sending message:  {img}");

                while (message != string.Empty && message != null)
                {
                    using (Socket client = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                    {

                        bool blockingState = client.Blocking;

                        Console.WriteLine($"CLIENT: blockingState:  {blockingState}");

                        if (blockingState)
                        {
                            await client.ConnectAsync(ipEndPoint);
                            await maketcpClientAsync(client, message);
                            message = ""; //clear message
                        }

                    }
                }
            }
        }

        public static async Task maketcpClientAsync(Socket client, string message)
        {
            while (true)
            {
                var messageBytes = Encoding.UTF8.GetBytes(message);
                _ = await client.SendAsync(messageBytes, SocketFlags.None);
                Console.WriteLine($"CLIENT: \"{message}\"");

                // Receive ack.
                var buffer = new byte[1_024];
                var received = await client.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);

                Console.WriteLine($"CLIENT: \"{response}\"");
                if (response == "<|ACK|>") { break; }
            }
        }
    }
}
