using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroMQ;

namespace ZMQ_PUB3
{
    class Program
    {

        const int SyncPub_SubscribersExpected = 1;	// We wait for 3 subscribers
        static void Main(string[] args)
        {
            using (var context = ZmqContext.Create())
            {
                using (var publisher = context.CreateSocket(SocketType.PUB))
                using (var syncservice = context.CreateSocket(SocketType.REP))
                {
                    publisher.SendHighWatermark = 1100000;
                    publisher.Bind("tcp://*:5561");

                    syncservice.Bind("tcp://*:5562");
                    int subscribers = SyncPub_SubscribersExpected;
                    do
                    {
                        Console.WriteLine("Waiting for {0} subscriber" + (subscribers > 1 ? "s" : string.Empty) + "...", subscribers);

                        // - wait for synchronization request
                        syncservice.ReceiveFrame();

                        // - send synchronization reply
                        syncservice.Send(new Frame(Encoding.UTF8.GetBytes(@"success")));

                    }
                    while (--subscribers > 0);

                    // Now broadcast exactly 20 updates followed by END
                    Console.WriteLine("Broadcasting messages:");

                    ZmqMessage ZmqMessage=new ZmqMessage();
                    for (int i = 0; i < 20; ++i)
                    {
                        Console.WriteLine("Sending {0}...", i);
                        ZmqMessage.Append(new Frame(Encoding.UTF8.GetBytes(i.ToString())));
                        //publisher.Send(new Frame(Encoding.UTF8.GetBytes(i.ToString())));
                        
                    }
                    publisher.SendMessage(ZmqMessage);
                    publisher.Send(new Frame(Encoding.UTF8.GetBytes(@"END")));
                }




            }
        }
    }
}
