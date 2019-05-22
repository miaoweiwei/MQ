using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroMQ;


namespace ZeroMQDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = ZmqContext.Create())
            {
                using (var socket = context.CreateSocket(SocketType.REQ))
                {
                    socket.Connect("tcp://127.0.0.1:5000"); 
                    socket.Send("My Reply", Encoding.UTF8);
                    var replyMsg = socket.Receive(Encoding.UTF8);
                } 
            }
        }
    }
}
