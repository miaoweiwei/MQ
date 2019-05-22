using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ZeroMQ;

namespace ZMQ_SUB2
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ZmqForClr();
        }

        private static void ZmqForNet()
        {
//
            // Synchronized subscriber
            //
            // Author: metadings
            //

            using (var context = new ZContext())
            using (var subscriber = new ZSocket(context, ZSocketType.SUB))
            using (var syncclient = new ZSocket(context, ZSocketType.REQ))
            {
                // First, connect our subscriber socket
                subscriber.Connect("tcp://127.0.0.1:5561");
                subscriber.SubscribeAll();

                // 0MQ is so fast, we need to wait a while…
                Thread.Sleep(1);

                // Second, synchronize with publisher
                syncclient.Connect("tcp://127.0.0.1:5562");

                // - send a synchronization request
                syncclient.Send(new ZFrame());

                // - wait for synchronization reply
                syncclient.ReceiveFrame();

                // Third, get our updates and report how many we got
                int i = 0;
                while (true)
                {
                    using (ZFrame frame = subscriber.ReceiveFrame())
                    {
                        string text = frame.ReadString();
                        if (text == "END")
                        {
                            break;
                        }

                        frame.Position = 0;
                        Console.WriteLine("Receiving {0}...", frame.ReadInt32());

                        ++i;
                    }
                }
                Console.WriteLine("Received {0} updates.", i);
            }
        }

        private static void ZmqForClr()
        {
            using (var context = ZmqContext.Create())
            {
                using (var subscriber = context.CreateSocket(ZeroMQ.SocketType.SUB))
                using (var syncclient = context.CreateSocket(ZeroMQ.SocketType.REQ))
                {
                    // First, connect our subscriber socket
                    subscriber.Connect("tcp://127.0.0.1:5561");
                    subscriber.SubscribeAll();

                    // 0MQ is so fast, we need to wait a while…
                    Thread.Sleep(1);

                    // Second, synchronize with publisher
                    syncclient.Connect("tcp://127.0.0.1:5562");

                    // - send a synchronization request
                    syncclient.Send(new Frame(Encoding.UTF8.GetBytes(@"")));

                    // - wait for synchronization reply
                    syncclient.ReceiveFrame();

                    // Third, get our updates and report how many we got
                    int i = 0;
                    while (true)
                    {

                        Frame frame = subscriber.ReceiveFrame();
                        

                        //var s=frame.ReceiveStatus;
                        byte[] b = frame.Buffer;
                        string text = Encoding.UTF8.GetString(b);
                        //string text = frame.ReadString();
                        if (text == "END")
                        {
                            break;
                        }

                        //frame.Position = 0;
                        Console.WriteLine("Receiving {0}...", text);

                        ++i;

                    }
                    Console.WriteLine("Received {0} updates.", i);
                }
            }
        }
    }
}
