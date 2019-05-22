using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace HelloReceive
{
    class Program
    {
        static void Main(string[] args)
        {
            #region 声明链接

            var factory = new ConnectionFactory(); //创建连接
            factory.HostName = "localhost"; //设置目标，如果RabbitMQ不在本机，只需要设置目标机器的IP地址或者机器名称即可
            factory.UserName = "guest"; //设置用户名
            factory.Password = "guest"; //设置对应的密码

            #endregion

            using (var connection=factory.CreateConnection())//建立连接
            {
                using (var channel=connection.CreateModel())//建立通道
                {

                    #region 消息队列与客户端绑定
                    //队列声明 此处的队列声明要与发送端一致
                    channel.QueueDeclare("hello", false, false, true, null); //要发送的信息 是否持久化 是否私有的 连接关闭时是否删除队列 参数

                    var consumer = new QueueingBasicConsumer(channel); //在通道channel里添加消费者
                    channel.BasicConsume("hello", true, consumer);//消费者订阅队列 // 消息队列的名字 是否关闭消息响应 消费者的名字

                    

                    //channel.BasicCancel(consumer.ConsumerTag);//停止订阅
                 
                    #endregion

                    Console.WriteLine(" waiting for message.");

                    #region 接收信息
                    while (true)
                    {

                        var ea = (BasicDeliverEventArgs) consumer.Queue.Dequeue(); //队列里的信息出列
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine("接收： {0}", message);

                    }
                    #endregion
                }
            }
        }
    }
}
