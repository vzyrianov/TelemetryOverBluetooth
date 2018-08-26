using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Collections.Concurrent;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using System.IO;
using System.Runtime.Serialization;

namespace TelemetryOverBluetoothServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BluetoothCSVListener bluetoothCSVListener;
        Thread bluetoothThread;

        List<DataPoint> dataPointLog;

        public MainWindow()
        {
            InitializeComponent();

            dataPointLog = new List<DataPoint>();

            bluetoothCSVListener = new BluetoothCSVListener();
            bluetoothCSVListener.OnCSVRecieved += DataPointRecieved;

            bluetoothThread = new Thread(new ThreadStart(bluetoothCSVListener.Run));
        }

        private void DataPointRecieved(object sender, BluetoothCSVListener.CSVRecievedEventArgs e)
        {
            switch (e.Message.DataPointType)
            {
                case "speed":
                    SpeedDataRecieved((SpeedDataPoint)e.Message);
                    break;
            }

            dataPointLog.Add(e.Message);

            Dispatcher.Invoke(() =>
                listBox1.Items.Add(e.Message.ToString())
            );
        }

        private void SpeedDataRecieved(SpeedDataPoint dataPoint)
        {
            Dispatcher.Invoke(() => {
                rectangle1.Height = dataPoint.Speed;
                textBlock1.Text = dataPoint.Speed.ToString();
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bluetoothThread.Start();
        }
    }
}
