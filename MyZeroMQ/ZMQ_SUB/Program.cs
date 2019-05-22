using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ZeroMQ;

namespace ZMQ_SUB
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> subscriptionPrefixes = new List<string>() ;
            List<string> connectEndPoints = new List<string> { "tcp://172.16.73.90:5000" };
            using (var ctx = ZmqContext.Create())
            {
                using (var socket = ctx.CreateSocket(ZeroMQ.SocketType.SUB))
                {
                    if (subscriptionPrefixes.Count() == 0) 
                        socket.SubscribeAll(); 
                    else
                        foreach (var subscriptionPrefix in subscriptionPrefixes)
                            socket.Subscribe(Encoding.UTF8.GetBytes(subscriptionPrefix)); 

                    foreach (var endPoint in connectEndPoints) 
                        socket.Connect(endPoint);

                    while (true)
                    {
                        Thread.Sleep(2000); 
                        var msg = socket.Receive(Encoding.UTF8); 
                        Console.WriteLine("Received: " + msg);
                    }
                }
            }
        }
    }
}
