using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing.Impl;

namespace Hello
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var factory = new ConnectionFactory();//创建连接
            factory.HostName = "localhost";       //设置目标，如果RabbitMQ不在本机，只需要设置目标机器的IP地址或者机器名称即可
            factory.UserName = "guest";              //设置用户名
            factory.Password = "guest";          //设置对应的密码

            using (var connection = factory.CreateConnection())//创建连接
            {
                using (var channel = connection.CreateModel())//创建通道
                {
                  
                    //队列声明 发送端队列声明要与此处的一致
                    channel.QueueDeclare("hello", false, false, true, null);//要发送的信息 是否持久化 是否私有的 连接关闭时是否删除队列 参数

                    string message = "共有100个信息";
                    var body = Encoding.UTF8.GetBytes(message);//组织 要发送的信息
                    channel.BasicPublish("", "hello", null, body);//把信息发送到队列channel里
                    Console.WriteLine(" 信息 {0}", message);




                    int msgNum = 100;
                    while (true)
                    {
                        Thread.Sleep(300);

                        message = "发送：这是第" + msgNum + "个Hello World";
                        body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish("", "hello", null, body);
                        Console.WriteLine(" 信息 {0}", message);

                        msgNum--;
                        if (msgNum == 0)
                        {
                            message = "信息发送完毕";
                            body = Encoding.UTF8.GetBytes(message);
                            channel.BasicPublish("", "hello", null, body);//第一个参数是交换区的名字 第二个是消息队列的名字 第三个是参数  第四个是发送的信息  
                            Console.WriteLine(" 信息 {0}", message);

                            Console.ReadLine();

                            break;
                        }
                    }
                }
            }
        }
    }
}
