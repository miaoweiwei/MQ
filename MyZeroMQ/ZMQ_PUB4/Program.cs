using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ZeroMQ;

namespace ZMQ_PUB4
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = ZmqContext.Create())
            {
                using (var publisher = context.CreateSocket(SocketType.PUB))
                using (var syncservice = context.CreateSocket(SocketType.REP))
                {
                    publisher.Bind("tcp://*:3385");
                    publisher.SendReady += publisher_SendReady;

                    syncservice.Bind("tcp://*:4950");
                    syncservice.SendReady += syncservice_SendReady;
                    syncservice.ReceiveReady += syncservice_ReceiveReady;

                    Poller poller = new Poller(new ZmqSocket[] { publisher, syncservice });

                    while (true)
                    {
                        poller.Poll();
                    }
                }
            }
        }

        static void syncservice_ReceiveReady(object sender, SocketEventArgs e)
        {
            
            Console.WriteLine("发布者：接收--请求应答模式");
            var syncservice = e.Socket.ReceiveFrame();
            Console.WriteLine("发布者：接收--请求应答模式--{0}", Encoding.UTF8.GetString(syncservice));
            //throw new NotImplementedException();
        }

        static void syncservice_SendReady(object sender, SocketEventArgs e)
        {
            Console.WriteLine("发布者：发送--请求应答模式");
            e.Socket.SendFrame(new Frame(Encoding.UTF8.GetBytes("请求应答模式发送数据……")));
        }

        static void publisher_SendReady(object sender, SocketEventArgs e)
        {
            Thread.Sleep(2000);
            Console.WriteLine("发布者：发送--P发布订阅模式UB");
            var zmqMessage = new ZmqMessage();
            zmqMessage.Append(Encoding.UTF8.GetBytes("发布者发布数据……"));
            e.Socket.SendMessage(zmqMessage);
            //throw new NotImplementedException();
        }
    }
}
