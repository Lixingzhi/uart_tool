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
            current_uart = comboBox_uartlist.SelectedValue.ToString();
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
                

                com.Open();
                uart_open = true;

                button_uartopen.Foreground = Brushes.Red;
            }
            else 
            {
                com.Close();
                uart_open = false;

                button_uartopen.Foreground = Brushes.Black;
            }
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
    }
}
