using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

namespace RMQ_Lasting_Send
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
                    //不设置持久化的话在RabbitMQ Server关闭或者崩溃后重启 原来在服务里的消息会全部丢失掉 设置持久化可以避免这种情况
                    //我们之前已经定义了一个非持久化的hello队列。RabbitMQ不允许我们使用不同的参数重新定义一个已经存在的同名队列，如果这样做就会报错。现在，定义另外一个不同名称的队列：
                    channel.QueueDeclare("Lasting_queue", true, false, false, null);//要发送的信息 是否持久化 是否私有的 连接关闭时是否删除队列 参数

                    var properties = channel.CreateBasicProperties();
                    properties.SetPersistent(true); //设置消息持久化 

                    while (true)
                    {
                        string message = GetMessage(args);

                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish("", "Lasting_queue", properties, body);//第一个参数是交换区的名字 第二个是消息队列的名字 第三个是参数  第四个是发送的信息  
                        Console.WriteLine("发送：" + message);
                    }
                }
            }
        }

        private static string GetMessage(string[] args)
        {
            return Console.ReadLine();
        }
    }
}
