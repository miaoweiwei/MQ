using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using RabbitMQ;
using RabbitMQ.Client;

namespace RMQ_BasicQos_Send
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] location=new string[]
            {
                "localhost",
                "123.206.216.30"
            };
            var factory = new ConnectionFactory();
            factory.HostName = location[0];
            factory.UserName = "guest";
            factory.Password = "guest";
            using (var connetion = factory.CreateConnection())
            {
                using (var channel = connetion.CreateModel())
                {
                    //第二个参数设置队列持久化 避免服务器重启 信息丢失
                    channel.QueueDeclare("Lasting_queue", true, false, true, null);//要发送的信息 是否持久化 是否私有的 连接关闭时是否删除队列 参数
                    channel.QueueDeclare("Lasting_queue1", true, false, true, null);//要发送的信息 是否持久化 是否私有的 连接关闭时是否删除队列 参数
                    channel.QueueDeclare("Lasting_queue2", true, false, true, null);//要发送的信息 是否持久化 是否私有的 连接关闭时是否删除队列 参数
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2;
                    properties.SetPersistent(true); //设置消息持久化 

                    while (true)
                    {
                        string[] message = GetMessage().Split(' ');
                        byte[] body;

                        if (!connetion.IsOpen)
                        {
                            Console.WriteLine("与服务器连接断开");
                            continue;
                        }
                        if (message.Length<=0)
                            continue;
                        body = Encoding.UTF8.GetBytes(message[0]);
                        channel.BasicPublish("", "Lasting_queue", properties, body);
                        Console.WriteLine("发送：" + message[0]);

                        if (message.Length <= 1)
                            continue;
                        body = Encoding.UTF8.GetBytes(message[1]);
                        channel.BasicPublish("", "Lasting_queue1", properties, body);
                        Console.WriteLine("发送1：" + message[1]);

                        if (message.Length <= 2)
                            continue;
                        body = Encoding.UTF8.GetBytes(message[2]);
                        channel.BasicPublish("", "Lasting_queue2", properties, body);
                        Console.WriteLine("发送2：" + message[2]);
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
