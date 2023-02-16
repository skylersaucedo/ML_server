using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ML_server
{
    public class UDPListener
    {
        private const int listenPort = 80;

        public void initalizeListener()
        {
            // check if this works!

            StartListener();
        }

        public static void StartListener()
        {
            IPAddress broadcast = IPAddress.Parse("127.0.0.1");

            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(broadcast, listenPort);

            try
            {
                while (true)
                {
                    Console.WriteLine("waiting for broadcast...");

                    byte[] bytes = listener.Receive(ref groupEP);

                    Console.WriteLine($"Received broadcast from {groupEP} :");
                    Console.WriteLine($" {Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");
                }
            }

            catch (SocketException e)
            {
                Console.WriteLine($"An error has occured: {e}");
            }

            finally
            {
                listener.Close();
            }
        }

        public static void Main()
        {
            StartListener();
        }


    }
}
