using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RMQExchange_Header_Receive
{
    class Program
    {
        static void Main(string[] args)
        {
            var queueName = "Exchang_Headers_queue";
            var exchangeName = "Exchange_Headers";
            var exchangeType = "headers";//topic、fanout、direct  fanout为广播类型

            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            var connetion = factory.CreateConnection();
            var channel = connetion.CreateModel();

            channel.ExchangeDeclare(exchangeName, exchangeType, false, true, null);

            var htDictionary = new Dictionary<string,object>();

            htDictionary.Add("x-match", "any");
            htDictionary.Add("key1", "123");
            htDictionary.Add("key2", "123abc");

            channel.QueueDeclare(queueName, false, false, true, null);
            channel.QueueBind(queueName, exchangeName, "", htDictionary);

            var consumer =new EventingBasicConsumer(channel);
            channel.BasicConsume(queueName, false, consumer);//订阅

            consumer.Received += consumer_Received;
            
        }

        static void consumer_Received(object sender, BasicDeliverEventArgs args)
        {
            IBasicConsumer basicConsumer = sender as IBasicConsumer;
            var body = args.Body;
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("收到："+message);

            //消息响应
            if (basicConsumer != null) basicConsumer.Model.BasicAck(args.DeliveryTag, false);
        }
    }
}
