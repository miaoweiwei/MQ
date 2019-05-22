using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing;

namespace RMQ_BasicQos_ReceiveOne
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] location = new string[]
            {
                "localhost",
                "123.206.216.30"
            };
            var factory=new ConnectionFactory();
            factory.HostName = location[1];
            factory.UserName = "guest";
            factory.Password = "guest";

            using (var connection=factory.CreateConnection())
            {
                using (var channel=connection.CreateModel())
                {
                    channel.QueueDeclare("Lasting_queue", true, false, true, null);

                    
                    channel.BasicQos(0, 1, false);//设置该客户在同一时间 RabbitMQ只发给该客户一个消息，在该客户没有响应消息之前不要给他分发消息将这条新的消息发送给下一个不那么忙碌的工作者。

                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume("Lasting_queue", false, consumer);

                
                    while (true)
                    {
                        var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine("接收：" + message);
                        channel.BasicAck(ea.DeliveryTag, false);//消息消息响应
                    }
                }
            }
        }

        

        

     

    }
}
