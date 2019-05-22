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
            var exchangeName = "Exchange_Headers1"; //交换器的名字
            var exchangeType = "headers";//topic、fanout、direct、headers  fanout为广播类型
            //var routingKey = "abc.a";        //关键字

            string[] location = new string[]
            {
                "localhost",
                "123.206.216.30"
            };

            string[,] keyDictionary = new string[,]
                    {
                        {"1", "a"},
                        {"2", "b"},
                        {"3", "c"},
                        {"4", "d"},
                        {"5", "e"},
                        {"6", "f"},
                        {"7", "g"},
                        {"8", "h"},
                        {"9", "i"},
                        {"10", "j"}
                    };

            var factory = new ConnectionFactory()
            {
                HostName = location[0],
                UserName = "guest",
                Password = "guest"
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchangeName, exchangeType, false, true, null);

                    var properties = channel.CreateBasicProperties();
                    properties.Headers = new Dictionary<string, object>();

                    int i = 0;
                    while (true)
                    {
                        string[] message = GetMessage().Split(' ');
                        i = 0;
                        while ((i<message.Length)&&(i<10))
                        {

                            properties.Headers.Clear();
                            properties.Headers.Add(keyDictionary[i, 0], keyDictionary[i,1]);

                            channel.BasicPublish(exchangeName, "", properties, Encoding.UTF8.GetBytes(message[i]));
                            Console.WriteLine("发送到" + exchangeName + "：" + message[i]);

                            i++;
                        }
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
