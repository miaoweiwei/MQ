using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RMQ_Lasting_Receuve
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
                    channel.QueueDeclare("Lasting_queue", true, false, false, null); //设置第二个参数为true 设置队列持久化 

                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume("Lasting_queue", false, consumer);// 消息队列的名字 是否关闭消息响应 消费者的名字

                    while (true)
                    {
                        var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);

                        int doit = message.Split(',').Length - 1;
                        Thread.Sleep(doit * 1000);

                        Console.WriteLine("接收端1：" + message + Environment.NewLine);
                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                }
            }
        }
    }
}