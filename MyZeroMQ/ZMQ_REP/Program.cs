using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZeroMQ;


namespace ZMQ_REP
{
    class Program
    {
        private static void Main(string[] args)
        {
            List<string>  bindEndPoints = new List<string> { "tcp://127.0.0.1:5000" };

            using (var context = ZmqContext.Create())
            {
                using (var socket = context.CreateSocket(ZeroMQ.SocketType.REP))
                {
                    foreach (var bindEndPoint in bindEndPoints)
                        socket.Bind(bindEndPoint);
                    while (true)
                    {
                        Thread.Sleep(1000);
                        var rcvdMsg = socket.Receive(Encoding.UTF8);
                        Console.WriteLine("Received: " + rcvdMsg);
                        var replyMsg = rcvdMsg + "------------------";
                        Console.WriteLine("Sending :" + replyMsg + Environment.NewLine);
                        socket.Send(replyMsg, Encoding.UTF8);
                    }
                }
            }
        }
    }

}
