using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RMQ_BasicQos_ReceiveThree
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
            factory.HostName = location[0];
            factory.UserName = "guest";
            factory.Password = "guest";

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();


            channel.QueueDeclare("Lasting_queue1", true, false, true, null);
            channel.QueueDeclare("Lasting_queue2", true, false, true, null);//要发送的信息 是否持久化 是否私有的 连接关闭时是否删除队列 参数
           
            channel.BasicQos(0, 1, false);//设置该客户在同一时间 RabbitMQ只发给该客户一个消息，在该客户没有响应消息之前不要给他分发消息将这条新的消息发送给下一个不那么忙碌的工作者。

            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume("Lasting_queue1", false, consumer);
            channel.BasicConsume("Lasting_queue2", false, consumer);

            consumer.Received += consumer_Received;
            
        }

        static void consumer_Received(object sender, BasicDeliverEventArgs args)
        {
            IBasicConsumer basicConsumer = sender as IBasicConsumer;
            var body = args.Body;
            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine("接收：" + args.DeliveryTag.ToString()+"\t" + message);
            basicConsumer.Model.BasicAck(args.DeliveryTag, false);//消息消息响应
        }
    }
}
