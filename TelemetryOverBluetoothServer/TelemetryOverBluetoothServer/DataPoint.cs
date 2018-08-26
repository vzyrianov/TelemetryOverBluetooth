using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelemetryOverBluetoothServer
{
    public class ImproperlyFormattedCSVArrayException : Exception
    {
        public ImproperlyFormattedCSVArrayException() { }
    }

    static class DataPointFactory
    {
        public static DataPoint CreateDataPoint(string[] csv)
        {
            switch(csv[0])
            {
                case "speed":
                    return new SpeedDataPoint(csv);
                    break;
            }

            throw new ImproperlyFormattedCSVArrayException();
        }
    }

    public abstract class DataPoint
    {
        public abstract string DataPointType { get; }
    }

    public class SpeedDataPoint : DataPoint
    {
        private readonly string dataPointType = "speed";
        string[] data;

        public SpeedDataPoint(string[] csv)
        {
            if (csv.Length == 2)
            {
                data = new string[2];
                csv.CopyTo(data, 0);
            }
            else
            {
                throw new ImproperlyFormattedCSVArrayException();
            }
        }

        public override string DataPointType
        {
            get { return dataPointType; }
        }

        public double Speed
        {
            get { return Convert.ToDouble(data[1]); }
        }

        public override string ToString()
        {
            return data[0] + ", " + data[1];
        }
    };
}
