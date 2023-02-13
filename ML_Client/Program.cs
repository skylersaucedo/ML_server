﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Client side!");

        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        IPAddress broadcast = IPAddress.Parse("193.169.0.0");
        Console.WriteLine("Enter message to send to broadcast address");
        var message = Console.ReadLine();

        while (message != string.Empty)
        {
            byte[] sendbuf = Encoding.ASCII.GetBytes(message);
            IPEndPoint ep = new IPEndPoint(broadcast, 11000);

            s.SendTo(sendbuf, ep);

            Console.WriteLine("Message sent to the broadcast address");

            message = Console.ReadLine();
        }

        Console.ReadLine();
    }
}