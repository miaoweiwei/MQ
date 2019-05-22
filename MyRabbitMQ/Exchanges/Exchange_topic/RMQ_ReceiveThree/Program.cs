using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RMQ_ReceiveThree
{
    class Program
    {
        static void Main(string[] args)
        {
            var queueName = "Exchang_queue1";//消息队列的名字
            var exchangeName = "MyExchange"; //交换器的名字
            var exchangeType = "topic";//topic、fanout
            var routingKey = "abc.*";        //关键字

            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchangeName, exchangeType, false, true, null);
            //需要先建立消息队列
            channel.QueueDeclare(queueName, false, false, true, null);
            channel.QueueBind(queueName, exchangeName, routingKey);

            /*channel.BasicQos(0,1, false);//设置该客户在同一时间 RabbitMQ只发给该客户一个消息，
             *在该客户没有响应消息之前不要给他分发消息将这条新的消息发送给下一个不那么忙碌的工作者。  参数说明 
             *prefetchSize 预读的个数
             *prefetchCount：会告诉RabbitMQ不要同时给一个消费者推送多于N个消息，即一旦有N个消息还没有ack，则该consumer将block掉，直到有消息ack
             *global：true\false 是否将上面设置应用于channel，简单点说，就是上面限制是channel级别的还是consumer级别
            */
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queueName, false, consumer);//接收到信息时不发送信息响应
            consumer.Received += consumer_Received;

        }

        static void consumer_Received(object sender, BasicDeliverEventArgs args)
        {
            IBasicConsumer basicConsumer = sender as IBasicConsumer;
            var body = args.Body;
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("收到：" + message);

            //basicConsumer.Model.BasicAck(args.DeliveryTag, false);//把信息处理完后再发送消息响应
        }
    }
}

