using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RMQ_BasicQos_ReceiveSecond
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.UserName = "guest";
            factory.Password = "guest";

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("Lasting_queue1", true, false, true, null);

                    channel.BasicQos(0, 1, false);//设置该客户在同一时间 RabbitMQ只发给该客户一个消息，在该客户没有响应消息之前不要给他分发消息将这条新的消息发送给下一个不那么忙碌的工作者。

                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume("Lasting_queue1", false, consumer);

                    while (true)
                    {
                        var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);

                        //int dots = message.Split(',').Length - 1;
                        //Thread.Sleep(1000 * dots);

                        Console.WriteLine("接收：" + message);

                        channel.BasicAck(ea.DeliveryTag, false);//消息消息响应
                    }
                }
            }
        }
    }
}
