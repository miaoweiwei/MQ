using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ZeroMQ;


namespace ZMQ_PUB
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> bindEndPoints = new List<string> { "tcp://172.16.73.90:5000" };

            using (var ctx = ZmqContext.Create())
            {
                using (var socket = ctx.CreateSocket(SocketType.PUB))
                {
                    foreach (var endPoint in bindEndPoints) 
                        socket.Bind(endPoint); 
                    long msgCptr = 0;
                    int msgIndex = 0;
                    while (true)
                    {
                        if (msgCptr == long.MaxValue)
                            msgCptr = 0;
                        msgCptr++; 

                        //if (options.maxMessage >= 0)
                        //    if (msgCptr > options.maxMessage)
                        //        break; 
                        //if (msgIndex == options.altMessages.Count()) 
                        //    msgIndex = 0;
                        //var msg = options.altMessages[msgIndex++].Replace("#nb#", msgCptr.ToString("d2"));
                        //Thread.Sleep(options.delay);

                        var msg = "发布的信息" + msgCptr;

                        Console.WriteLine("Publishing: " + msg);
                        Thread.Sleep(2000); 
                        socket.Send(msg, Encoding.UTF8);
                    }
                }
            }
        }
    }
}
