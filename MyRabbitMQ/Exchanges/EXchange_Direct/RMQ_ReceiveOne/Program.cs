using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RMQ_ReceiveOne
{
    class Program
    {
        static void Main(string[] args)
        {
            var queueName = "ExchangDirect_queue1";//消息队列的名字
            var exchangeName = "ExchangeDirect"; //交换器的名字
            var exchangeType = "direct";//topic、fanout、direct  fanout为广播类型
            var routingKey = "abc.a";        //关键字

            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            var connection = factory.CreateConnection();


            var channel = connection.CreateModel();
            channel.QueueDeclare(queueName, false, false, true, null);
            channel.QueueBind(queueName,exchangeName,routingKey);


            var consumer=new EventingBasicConsumer(channel);
            channel.BasicConsume(queueName, false, consumer);
            consumer.Received += consumer_Received;
        }

        private static void consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            IBasicConsumer basicConsumer = sender as IBasicConsumer;
            var body = e.Body;
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("收到ExchangeDirect：" + message);

            if (basicConsumer != null) basicConsumer.Model.BasicAck(e.DeliveryTag, false);
        }
    }
}
