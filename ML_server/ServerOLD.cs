//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading.Tasks;

//namespace ML_server
//{
//    class ServerOLD
//    {
//        public static int Port { get; set; }
//        public static int MaxPlayers { get; set; }
//        public static Dictionary<int, ClientOLD> clients = new Dictionary<int, ClientOLD>();
//        private static TcpListener tcpListener;

//        public static void Start(int _MaxPlayers, int _port)
//        {
//            Port = _port;
//            MaxPlayers = _MaxPlayers;

//            tcpListener = new TcpListener(IPAddress.Any, Port);

//            Console.WriteLine("Starting server");

//            InitializeServerData();

//            tcpListener.Start();
           
//            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

//            Console.WriteLine($"Server started on {Port}.");
//        }

//        private static void TCPConnectCallback(IAsyncResult _result)
//        {
//            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
//            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

//            for (int i = 1; i <= MaxPlayers; i++)
//            {
//                if (clients[i].tcp.socket == null)
//                {
//                    clients[i].tcp.Connect(_client);
//                    return;
//                }
//            }

//            Console.WriteLine($"{_client.Client.RemoteEndPoint} failed to connect. Server full...");

//        }

//        private static void InitializeServerData()
//        {
//            for (int i = 1; i <= MaxPlayers; i++)
//            {
//                clients.Add(i, new ClientOLD(i));
//            }
//        }

//    }
//}
