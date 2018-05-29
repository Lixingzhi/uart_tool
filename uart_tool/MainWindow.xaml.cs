using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO.Ports;
using Microsoft.Win32;
using System.IO;

namespace uart_tool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public string current_uart;
        public string current_bound;
        public string current_stopbits;
        public string current_databits;
        public string current_checkbits;
        public static bool uart_open = false;
        public static SerialPort com = null;
        public string datastring;

        public bool timestramp = false;
        public System.Timers.Timer timer;

        public MainWindow()
        {
            InitializeComponent();

            comboBox_bound.SelectedIndex = 7;
            current_bound = comboBox_bound.Text;
            comboBox_stopbits.SelectedIndex = 0;
            current_stopbits = comboBox_stopbits.Text;
            comboBox_databits.SelectedIndex = 0;
            current_databits = comboBox_databits.Text;
            comboBox_check.SelectedIndex = 0;
            current_checkbits = comboBox_check.Text;
        }

        private void comboBox_uartlist_DropDownOpened(object sender, EventArgs e)
        {
            string[] Arrayuart = SerialPort.GetPortNames(); //获取串口列表

            comboBox_uartlist.Items.Clear();
            foreach (string i in Arrayuart)
            {
                comboBox_uartlist.Items.Add(i);
            }
        }

        private void comboBox_uartlist_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBox_uartlist.SelectedIndex >= 0)
            {
                current_uart = comboBox_uartlist.SelectedValue.ToString();
            }
            
        }

        private void button_uartopen_Click(object sender, RoutedEventArgs e)
        {
            if (current_uart == null)
            {
                MessageBox.Show("请选择串口");
                return;
            }

            if (false == uart_open)
            {
                com = new SerialPort(current_uart);  

                com.BaudRate = int.Parse(current_bound);
                com.DataBits = int.Parse(current_databits);

                switch (current_stopbits)
                { 
                    case "2":
                        com.StopBits = StopBits.Two;
                        break;

                    case "1.5":
                        com.StopBits = StopBits.OnePointFive;
                        break;

                    case "1":
                        com.StopBits = StopBits.One;
                        break;

                    default:
                        com.StopBits = StopBits.None;
                        break;
                }

                switch (current_checkbits)
                {
                    case "无":
                        com.Parity = Parity.None;
                        break;

                    case "奇校验":
                        com.Parity = Parity.Odd;
                        break;

                    case "偶校验":
                        com.Parity = Parity.Even;
                        break;

                    default:
                        break;

                }

                try
                {
                    
                    uart_open = true;
                    com.DataReceived += new SerialDataReceivedEventHandler(com_DataReceived);
                    com.ReceivedBytesThreshold = 1;
                    com.Open();

                    button_uartopen.Foreground = Brushes.Red;
                }
                catch (Exception e1)
                {
                    MessageBox.Show("打开串口错误");
                }

            }
            else 
            {
                com.Close();
                uart_open = false;

                button_uartopen.Foreground = Brushes.Black;
            }
        }

        

        void com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //throw new NotImplementedException();
            SerialPort sp = (SerialPort)sender;
            int n = sp.BytesToRead;

            int count = com.BytesToRead;
            if (count <= 0)
                return;
            byte[] databuffer = new byte[count];
            com.Read(databuffer, 0, count);
            datastring = System.Text.Encoding.Default.GetString(databuffer);

            //线程安全，否则无法修改TexttBox1
            this.TextBox1.Dispatcher.Invoke(
                new Action(
                    delegate
                    {
                        //listBox1.Items.Add(datastring);
                        if ((bool)button_hexshow.IsChecked)
                        {
                            char[] values = datastring.ToCharArray();
                            foreach (char letter in values)
                            {
                                int value = Convert.ToInt32(letter);
                                TextBox1.AppendText(String.Format(" {0:X}", value));
                            }
                        }
                        else 
                        {
                            TextBox1.AppendText(datastring);
                        }

                        if (timestramp)
                        {
                            TextBox1.AppendText("[");
                            TextBox1.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                            TextBox1.AppendText("]");
                            TextBox1.AppendText("\r\n");
                        }
                        else
                        { 
                        
                        }

                        //TextBox1.AppendText(datastring);
                        //需要通过可视树找到listbox里面的那个ScrollViewer，然后通过ScrollToEnd滚动到最后
                        Decorator decorator = (Decorator)VisualTreeHelper.GetChild(TextBox1, 0);
                        ScrollViewer scrollViewer = (ScrollViewer)decorator.Child;
                        scrollViewer.ScrollToEnd();
                    }));
        }

        private void comboBox_databits_DropDownClosed(object sender, EventArgs e)
        {
            current_databits = comboBox_databits.Text;
        }

        private void comboBox_check_DropDownClosed(object sender, EventArgs e)
        {
            current_checkbits = comboBox_check.Text;
        }

        private void comboBox_stopbits_DropDownClosed(object sender, EventArgs e)
        {
            current_stopbits = comboBox_stopbits.Text;
        }

        private void comboBox_bound_DropDownClosed(object sender, EventArgs e)
        {
            current_bound = comboBox_bound.Text;
        }

        private void button_Send_Click(object sender, RoutedEventArgs e)
        {
            byte[] buffer = System.Text.Encoding.Default.GetBytes(textBox_Send.Text);

            if (true == uart_open)
            {
                com.Write(buffer, 0, buffer.Length);
            }
            else 
            {
                MessageBox.Show("请打开串口");
            }
            
        }

        private void TextBox1_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
            ScrollViewer1.ScrollToEnd();
        }

        private void button_cleanreceive_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
            TextBox1.Clear();
        }

        private void button_save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = " txt files(*.txt)|*.txt|All files(*.*)|*.*";

            save.ShowDialog();

            StreamWriter sw = new StreamWriter(save.FileName);
            sw.Write(TextBox1.Text);
            sw.Close();
        }

        private void button_changebackgroud_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
            TextBox1.Foreground = Brushes.Black;
            TextBox1.Background = Brushes.White;

        }

        private void button_changebackgroud_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
            TextBox1.Foreground = Brushes.LightGreen;
            TextBox1.Background = Brushes.Black;
        }

        private void button_timestamp_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
            timestramp = false;
        }

        private void button_timestamp_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
            timestramp = true;
        }

        private void button_send_clean_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
            textBox_Send.Clear();
        }

        private byte[] send_buffer;

        private void textBox_Send_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // 在此处添加事件处理程序实现。
            send_buffer = System.Text.Encoding.Default.GetBytes(textBox_Send.Text);
        }

        private void CheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
            timer = new System.Timers.Timer();
            timer.Enabled = true;
            //timer.Interval = 100; //执行间隔时间,单位为毫秒; 这里实际间隔为10分钟  
            timer.Interval = Int32.Parse(textbox_sendInterval.Text);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //throw new NotImplementedException();

            if (true == uart_open)
            {
                com.Write(send_buffer, 0, send_buffer.Length);
            }
            else
            {
                timer.Stop();
                MessageBox.Show("请打开串口");
                
            }
        }

        private void CheckBox_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
            timer.Stop();
        }


    }
}
