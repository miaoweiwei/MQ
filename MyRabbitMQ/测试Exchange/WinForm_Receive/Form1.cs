using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WinForm_Receive
{
    public partial class Form1 : Form
    {
        string[,] mageStrings = new string[10, 2];
        static string[,] keyDictionary = new string[,]
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

        //static Dictionary<string, object> keydicStrings = new Dictionary<string, object>//要显示的Key
        static string[,] keydicStrings = new string[,]
                    {
                        {"1", "a"},
                        {"2", "b"},
                        {"3", "c"},
                        {"4", "d"},
                        {"5", "e"}
                    };
        string queueName = "Exchang_Headers_queue";
        string exchangeName = "Exchange_Headers1";
        string exchangeType = "headers";//topic、fanout、direct  fanout为广播类型
        static string[] location = new string[]
            {
                "localhost",
                "123.206.216.30"
            };

        static ConnectionFactory factory = new ConnectionFactory()
        {
            HostName = location[0],
            UserName = "guest",
            Password = "guest"
        };
        static IConnection connetion = factory.CreateConnection();
        static IModel channel = connetion.CreateModel();
        EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

        public Form1()
        {
            InitializeComponent();
            string[] xiangStrings = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
            comboBox1.Items.AddRange(xiangStrings);
            comboBox2.Items.AddRange(xiangStrings);
            comboBox3.Items.AddRange(xiangStrings);
            comboBox4.Items.AddRange(xiangStrings);
            comboBox5.Items.AddRange(xiangStrings);


            //comboBox1.Text = "1";
            //comboBox2.Text = "2";
            //comboBox3.Text = "3";
            //comboBox4.Text = "4";
            //comboBox5.Text = "5";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text = "asdad";
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            //IConnection connetion = factory.CreateConnection();
            //IModel channel = connetion.CreateModel();

            channel.ExchangeDeclare(exchangeName, exchangeType, false, true, null);
            channel.QueueDeclare(queueName, false, false, true, null);

            Dictionary<string, object> keyValue = new Dictionary<string, object>();
            keyValue.Add("x-match", "any");
            for (int i = 0; i < 5; i++)
            {
                keyValue.Add(keydicStrings[i, 0], keydicStrings[i, 1]);
            }
            channel.QueueBind(queueName, exchangeName, "", keyValue);


            //EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queueName, false, consumer);//订阅
            consumer.Received += consumer_Received1;
        }

        void consumer_Received1(object sender, BasicDeliverEventArgs args)
        {
            IBasicConsumer basicConsumer = sender as IBasicConsumer;
            var key = args.BasicProperties.Headers;
            if (key.Count != 1)
            {

            }

            try
            {
                if (key.Keys.Contains(InvokeHelper.Get(comboBox1, "Text").ToString()))
                {
                    object reValue;
                    key.TryGetValue(InvokeHelper.Get(comboBox1, "Text").ToString(), out reValue);

                    var body = args.Body;
                    var message = Encoding.UTF8.GetString(body);

                    display(1, message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if (key.Keys.Contains(InvokeHelper.Get(comboBox2, "Text").ToString()))
            {
                object reValue;
                key.TryGetValue(InvokeHelper.Get(comboBox2, "Text").ToString(), out reValue);

                var body = args.Body;
                var message = Encoding.UTF8.GetString(body);
                display(2, message);
            }

            if (key.Keys.Contains(InvokeHelper.Get(comboBox3, "Text").ToString()))
            {
                object reValue;
                key.TryGetValue(InvokeHelper.Get(comboBox3, "Text").ToString(), out reValue);

                var body = args.Body;
                var message = Encoding.UTF8.GetString(body);
                display(3, message);
            }
            if (key.Keys.Contains(InvokeHelper.Get(comboBox4, "Text").ToString()))
            {
                object reValue;
                key.TryGetValue(InvokeHelper.Get(comboBox4, "Text").ToString(), out reValue);

                var body = args.Body;
                var message = Encoding.UTF8.GetString(body);
                display(4, message);
            }
            if (key.Keys.Contains(InvokeHelper.Get(comboBox5, "Text").ToString()))
            {
                object reValue;
                key.TryGetValue(InvokeHelper.Get(comboBox5, "Text").ToString(), out reValue);

                var body = args.Body;
                var message = Encoding.UTF8.GetString(body);
                display(5, message);
            }


            //for (int i = 0; i < 5; i++)
            //{
            //    if (key.Keys.Contains(keydicStrings[i,0]))
            //    {
            //        object reValue;
            //        key.TryGetValue(keydicStrings[i, 0], out reValue);
            //        if (keydicStrings[i, 1] == Encoding.UTF8.GetString(reValue as byte[]))
            //        {
            //            var body = args.Body;
            //            var message = Encoding.UTF8.GetString(body);

            //            display(i+1, message);
            //            break;
            //        }
            //    }
            //}
            //消息响应
            if (basicConsumer != null) basicConsumer.Model.BasicAck(args.DeliveryTag, false);
        }
        private void display(int textBox, string message)
        {
            mageStrings[int.Parse(keydicStrings[textBox - 1, 0]) - 1, 0] = textBox.ToString();
            mageStrings[textBox, 1] = message;


            switch (textBox)
            {
                case 1:
                    SetText1(message);
                    //textBox1.Text = message;
                    break;
                case 2:
                    SetText2(message);
                    break;
                case 3:
                    SetText3(message);
                    break;
                case 4:
                    SetText4(message);
                    break;
                case 5:
                    SetText5(message);
                    break;
            }
        }

        delegate void SetTextCallBack(string text);

        #region 设置TextBox

        private void SetText1(string text)
        {
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallBack stcb = new SetTextCallBack(SetText1);
                this.Invoke(stcb, new object[] { text });
            }
            else
            {
                this.textBox1.Text = text;
            }
        }

        private void SetText2(string text)
        {
            if (this.textBox2.InvokeRequired)
            {
                SetTextCallBack stcb = new SetTextCallBack(SetText2);
                this.Invoke(stcb, new object[] { text });
            }
            else
            {
                this.textBox2.Text = text;
            }
        }

        private void SetText3(string text)
        {
            if (this.textBox3.InvokeRequired)
            {
                SetTextCallBack stcb = new SetTextCallBack(SetText3);
                this.Invoke(stcb, new object[] { text });
            }
            else
            {
                this.textBox3.Text = text;
            }
        }

        private void SetText4(string text)
        {
            if (this.textBox4.InvokeRequired)
            {
                SetTextCallBack stcb = new SetTextCallBack(SetText4);
                this.Invoke(stcb, new object[] { text });
            }
            else
            {
                this.textBox4.Text = text;
            }
        }

        private void SetText5(string text)
        {
            if (this.textBox5.InvokeRequired)
            {
                SetTextCallBack stcb = new SetTextCallBack(SetText5);
                this.Invoke(stcb, new object[] { text });
            }
            else
            {
                this.textBox5.Text = text;
            }
        }

        #endregion

        #region 改变订阅

        private void SetCnsumer()
        {
            bool repeat = true;
            keydicStrings = new string[5, 2];
            for (int j = 0; j < 10; j++)
            {
                if (comboBox1.Text == keyDictionary[j, 0])
                {
                    for (int i = 0; i < keydicStrings.GetLength(0); i++)
                    {
                        if (comboBox1.Text == keydicStrings[i, 0])
                        {
                            repeat = false;
                            break;
                        }
                    }
                    if (repeat)
                    {
                        keydicStrings[0, 0] = keyDictionary[j, 0];
                        keydicStrings[0, 1] = keyDictionary[j, 1];
                    }
                    repeat = true;
                    break;
                }
            }
            for (int j = 0; j < 10; j++)
            {
                if (comboBox2.Text == keyDictionary[j, 0])
                {
                    for (int i = 0; i < keydicStrings.GetLength(0); i++)
                    {
                        if (comboBox2.Text == keydicStrings[i, 0])
                        {
                            repeat = false;
                            break;
                        }
                    }
                    if (repeat)
                    {
                        keydicStrings[1, 0] = keyDictionary[j, 0];
                        keydicStrings[1, 1] = keyDictionary[j, 1];
                    }
                    repeat = true;
                    break;
                }
            }
            for (int j = 0; j < 10; j++)
            {
                if (comboBox3.Text == keyDictionary[j, 0])
                {
                    for (int i = 0; i < keydicStrings.GetLength(0); i++)
                    {
                        if (comboBox3.Text == keydicStrings[i, 0])
                        {
                            repeat = false;
                            break;
                        }
                    }
                    if (repeat)
                    {
                        keydicStrings[2, 0] = keyDictionary[j, 0];
                        keydicStrings[2, 1] = keyDictionary[j, 1];
                    }
                    repeat = true;
                    break;
                }
            }
            for (int j = 0; j < 10; j++)
            {
                if (comboBox4.Text == keyDictionary[j, 0])
                {
                    for (int i = 0; i < keydicStrings.GetLength(0); i++)
                    {
                        if (comboBox4.Text == keydicStrings[i, 0])
                        {
                            repeat = false;
                            break;
                        }
                    }
                    if (repeat)
                    {
                        keydicStrings[3, 0] = keyDictionary[j, 0];
                        keydicStrings[3, 1] = keyDictionary[j, 1];
                    }
                    repeat = true;
                    break;
                }
            }
            for (int j = 0; j < 10; j++)
            {
                if (comboBox5.Text == keyDictionary[j, 0])
                {
                    for (int i = 0; i < keydicStrings.GetLength(0); i++)
                    {
                        if (comboBox5.Text == keydicStrings[i, 0])
                        {
                            repeat = false;
                            break;
                        }
                    }
                    if (repeat)
                    {
                        keydicStrings[4, 0] = keyDictionary[j, 0];
                        keydicStrings[4, 1] = keyDictionary[j, 1];
                    }
                    repeat = true;
                    break;
                }
            }
            //channel.QueueDelete(queueName);
            //channel.ExchangeDeclare(exchangeName, exchangeType, false, true, null);
            //channel.QueueDeclare(queueName, false, false, true, null);
            //consumer.HandleBasicCancel(consumer.ConsumerTag);//取消订阅
            Dictionary<string, object> keyValue = new Dictionary<string, object>();
            keyValue.Add("x-match", "any");
            for (int i = 0; i < 5; i++)
            {
                if (keydicStrings[i, 0] == null)
                    continue;
                keyValue.Add(keydicStrings[i, 0], keydicStrings[i, 1]);
            }
            channel.QueueBind(queueName, exchangeName, "", keyValue);
            channel.BasicConsume(queueName, false, consumer);//订阅
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            SetCnsumer();
        }

        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            SetCnsumer();
        }

        private void comboBox4_TextChanged(object sender, EventArgs e)
        {
            SetCnsumer();
        }

        private void comboBox3_TextChanged(object sender, EventArgs e)
        {
            SetCnsumer();
        }

        private void comboBox5_TextChanged(object sender, EventArgs e)
        {
            SetCnsumer();
        }

        #endregion

        private string setTextBX(int i)
        {
            switch (i)
            {
                case 1:
                    return textBox1.Text;
                case 2:
                    return textBox2.Text;
                case 3:
                    return textBox3.Text;
                case 4:
                    return textBox4.Text;
                case 5:
                    return textBox5.Text;
                default:
                    return "";
            }
        }

    }
}
