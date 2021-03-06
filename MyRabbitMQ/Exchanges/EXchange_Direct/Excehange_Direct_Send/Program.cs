﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

namespace Excehange_Direct_Send
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

            using (var connetion = factory.CreateConnection())
            {
                using (var channel = connetion.CreateModel())
                {
                    //Exchange的名字 类型 是否持久化 是否自动删除 参数
                    channel.ExchangeDeclare(exchangeName, exchangeType, false, true, null);

                    while (true)
                    {
                        string message = GetMessage();

                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchangeName, routingKey, null, body);
                        Console.WriteLine("发送到Exchange：" + message);
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
