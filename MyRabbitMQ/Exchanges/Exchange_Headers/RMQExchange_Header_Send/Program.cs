using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Xml.Serialization;
using RabbitMQ.Client;

namespace RMQExchange_Header_Send
{
    class Program
    {
        static void Main(string[] args)
        {
            var queueName = "Exchang_Headers_queue";//消息队列的名字
            var exchangeName = "Exchange_Headers"; //交换器的名字
            var exchangeType = "headers";//topic、fanout、direct、headers  fanout为广播类型
            //var routingKey = "abc.a";        //关键字

            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchangeName,exchangeType,false,true,null);

                    var properties = channel.CreateBasicProperties();
                    properties.Headers = new Dictionary<string, object>();


                    while (true)
                    {
                        string[] message = GetMessage().Split(' ');

                        properties.Headers.Clear();
                        properties.Headers.Add("key1", "123");

                        channel.BasicPublish(exchangeName, "", properties, Encoding.UTF8.GetBytes(message[0]));
                        Console.WriteLine("发送到" + exchangeName + "：" + message[0]);


                        properties.Headers.Clear();
                        properties.Headers.Add("key2","123abc");

                        channel.BasicPublish(exchangeName, "", properties, Encoding.UTF8.GetBytes(message[1]));
                        Console.WriteLine("发送到" + exchangeName + "：" + message[1]);

                    }
                }
            }

        }
        private static string GetMessage()
        {
            return Console.ReadLine();
        }
    }
}
