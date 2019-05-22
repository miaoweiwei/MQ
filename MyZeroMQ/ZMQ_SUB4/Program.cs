using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ZeroMQ;

namespace ZMQ_SUB4
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = ZmqContext.Create())
            {
                using (var subscriber = context.CreateSocket(ZeroMQ.SocketType.SUB))
                using (var syncclient = context.CreateSocket(ZeroMQ.SocketType.REQ))
                {
                    //subscriber.Connect("tcp://172.16.73.88:3385");
                    subscriber.Connect("tcp://192.168.179.128:3385");

                    subscriber.SubscribeAll();
                    Thread.Sleep(1);

                    //syncclient.Connect("tcp://172.16.73.88:4950");
                    syncclient.Connect("tcp://192.168.179.128:4950");
                    

                    syncclient.SendFrame(new Frame(Encoding.UTF8.GetBytes("Sync me")));
                    var repMsg = Encoding.UTF8.GetString(syncclient.ReceiveFrame().Buffer);

                    while (true)
                    {
                        var pubMsg = subscriber.ReceiveMessage();

                        Console.WriteLine(Encoding.UTF8.GetString(pubMsg[0].Buffer));

                    }

                }
            }
        }


    }
}
