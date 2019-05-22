using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

namespace RMQ_WorkQueuesSend
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.UserName = "guest";
            factory.Password = "guest";

            using (var connection=factory.CreateConnection())
            {
                using (var channel=connection.CreateModel())
                {
                    channel.QueueDeclare("WorkQueues", false, false, false, null);
                    while (true)
                    {
                        string message = GetMessage(args);
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish("", "WorkQueues", null, body);
                        Console.WriteLine("发送：" + message);
                    }
                }
            }
        }

        private static string GetMessage(string[] args)
        {
            return  Console.ReadLine();
        }
    }
}
