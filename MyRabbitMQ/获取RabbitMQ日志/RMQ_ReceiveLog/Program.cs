using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RMQ_ReceiveLog
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
            var factory = new ConnectionFactory();
            factory.HostName = location[1];
            factory.UserName = "guest";
            factory.Password = "guest";

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare("ReceiveLog", false, false, true, null);//预先建立好消息队列

            channel.QueueBind("ReceiveLog", "amq.rabbitmq.log", "error");
            channel.QueueBind("ReceiveLog", "amq.rabbitmq.log", "warning");
            channel.QueueBind("ReceiveLog", "amq.rabbitmq.log", "info");


            var consumer=new EventingBasicConsumer(channel);
            channel.BasicConsume("ReceiveLog", false, consumer);
            channel.BasicQos(0, 1, false);//设置该客户在同一时间 RabbitMQ只发给该客户一个消息，在该客户没有响应消息之前不要给他分发消息将这条新的消息发送给下一个不那么忙碌的工作者。

            consumer.Received += consumer_Received;
        }

        static void consumer_Received(object sender, BasicDeliverEventArgs args)
        {
            IBasicConsumer basicConsumer = sender as IBasicConsumer;
            var body = args.Body;
            string message = args.RoutingKey + " **" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "** " + Encoding.UTF8.GetString(body);
            Console.WriteLine(message);

            if (basicConsumer != null) basicConsumer.Model.BasicAck(args.DeliveryTag, false);//消息响应 确认消费者已收到消息
        }
    }
}
