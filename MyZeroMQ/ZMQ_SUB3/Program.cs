using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ZeroMQ;

namespace ZMQ_SUB3
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
                    // First, connect our subscriber socket
                    subscriber.Connect("tcp://172.16.73.88:5561");
                    subscriber.SubscribeAll();

                    // 0MQ is so fast, we need to wait a while…
                    Thread.Sleep(1);

                    // Second, synchronize with publisher
                    syncclient.Connect("tcp://172.16.73.88:5562");

                    // - send a synchronization request
                    syncclient.Send(new Frame(Encoding.UTF8.GetBytes(@"")));

                    // - wait for synchronization reply
                    syncclient.ReceiveFrame();

                    // Third, get our updates and report how many we got
                    int i = 0;
                    while (true)
                    {

                        //Frame frame = subscriber.ReceiveFrame();

                        ZmqMessage zmqMessage = subscriber.ReceiveMessage();
                        var s1 = Encoding.UTF8.GetString(zmqMessage[0]);

                        
                        //var s=frame.ReceiveStatus;
                        //byte[] b = frame.Buffer;
                        //string text = Encoding.UTF8.GetString(b);
                        //string text = frame.ReadString();
                        //if (text == "END")
                        //{
                        //    break;
                        //}

                        //frame.Position = 0;
                        //Console.WriteLine("Receiving {0}...", text);
                        Console.WriteLine("Receiving {0}...", s1);

                        ++i;

                    }
                    Console.WriteLine("Received {0} updates.", i);
                }
            }
        }
    }
}
