using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using System.IO;
using System.Runtime.Serialization;

namespace TelemetryOverBluetoothServer
{
    public class ImproperlyFormattedCSVException : Exception
    {
        public ImproperlyFormattedCSVException(string csv)
        {
            this.csv = csv;
        }

        public string csv;
    }

    class BluetoothCSVListener
    {
        public class CSVRecievedEventArgs : EventArgs
        {
            private DataPoint msg;

            public CSVRecievedEventArgs(DataPoint dataPoint)
            {
                msg = dataPoint;
            }

            public DataPoint Message
            {
                get { return msg; }
            }
        }

        private bool terminateThread = false;

        static readonly Guid serviceUuid = new Guid("{b62c4e8d-62cc-404b-bbbf-bf3e3bbb1374}");

        public delegate void CSVRecievedEventHandler(object sender, CSVRecievedEventArgs a);
        public event CSVRecievedEventHandler OnCSVRecieved;

        public void Run()
        {
            Guid serviceClass;
            serviceClass = serviceUuid;

            BluetoothListener bluetoothListener = new BluetoothListener(serviceClass);
            bluetoothListener.Start();

            BluetoothClient conn = bluetoothListener.AcceptBluetoothClient();

            Stream peerStream = conn.GetStream();

            StringBuilder stringBuffer = new StringBuilder();

            while (!terminateThread)
            {
                byte[] buffer = new byte[10];
                int readIn = peerStream.Read(buffer, 0, 10);

                if (readIn > 0)
                {
                    string s = System.Text.Encoding.UTF8.GetString(buffer).ToString();
                    stringBuffer.Append(s);

                    int loc = s.IndexOf('\n');
                    while (loc != -1)
                    {
                        string total = stringBuffer.ToString();
                        loc = total.IndexOf('\n');
                        DeserializeCSVAndCallBack(total.Substring(0, loc));
                        total = total.Remove(0, loc + 1);

                        loc = total.IndexOf('\n');
                        stringBuffer.Clear();
                        stringBuffer.Append(total);
                    }
                }

                Thread.Sleep(100);
            }
        }

        public void TerminateThread()
        {
            terminateThread = true;
        }

        void DeserializeCSVAndCallBack(string csv)
        {
            csv = csv.Replace("\0", "");

            //Todo: more accurate csv splitting (take into account quotes and commas in them).
            string[] tokens = csv.Split(',');

            DataPoint dataPoint;

            try
            {
                dataPoint = DataPointFactory.CreateDataPoint(tokens);
            }
            catch (ImproperlyFormattedCSVArrayException e)
            {
                throw new ImproperlyFormattedCSVException(csv);
            }

            OnCSVRecieved(this, new CSVRecievedEventArgs(dataPoint));
        }
    }
}
