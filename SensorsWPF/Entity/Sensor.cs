using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.DirectoryServices.ActiveDirectory;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static SensorsWPF.Sensor;
using Label = System.Windows.Controls.Label;
using Timer = System.Timers.Timer;



namespace SensorsWPF.Entity
{
    public class Sensor<T> : MainWindow 
        where T : IComparable<T>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public int MinValue { get; set; }

        public int MaxValue { get; set; }

        public int ID { get; set; }

        public SensorType Type { get; set; }

        private Random rnd;

        public Frequency Frequency { get; set; }

        private T SensorValue { get; set; }

        private string QualityValue { get; set; }

        public Label text { get; set; } 

        public string EncoderType { get; set; }

         private double _bottomBottomEdge { get; set;}
         private double _bottomTopEdge { get; set;}
         private double _topBottomEdge { get; set;}
         private double _topTopEdge { get; set; }

        Server speedServer = new Server();
        public int Port { get; set; }
        public string RequestResult { get; set; }


        public void Configuration()
        {
            IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("C:\\Users\\Dell\\source\\repos\\SensorsWPF\\SensorsWPF\\sensorConfig.json")
            .AddEnvironmentVariables()
            .Build();

            var settings = config.GetRequiredSection($"Sensors:{ID - 1}").Get<Sensor<T>>();

            ID = settings.ID;
            Type = settings.Type;
            MinValue = settings.MinValue;
            MaxValue = settings.MaxValue;
            EncoderType = settings.EncoderType;
            Frequency = settings.Frequency;

            _bottomBottomEdge = (MaxValue - MinValue) * 0.10 + MinValue;
            _bottomTopEdge = (MaxValue - MinValue) * 0.25 + MinValue;

            _topBottomEdge = (MaxValue - MinValue) * 0.75 + MinValue;
            _topTopEdge = (MaxValue - MinValue) * 0.90 + MinValue;
            speedServer.port = Port;
            speedServer.Start();
           

        }
        public Sensor()
        {

        }

        public Sensor(Label label, int id,int port)
        {
            Port = port;
            ID = id;
            text = label;

        }

        public void SensorStart()
        {
            var timer = new Timer() { Interval = (double)Frequency };

            timer.Start();
            timer.Elapsed += async (o, e) =>
            {
                try
                {
                    await Task.Run(() =>
                    {
                        SensorBody();
                        SendMessageToClient();
                    });
                }
                catch(Exception ex)
                {
                    throw new ArgumentNullException(ex.Message);
                }
            };
        }

        public void SensorBody()
        {
            rnd = new Random();
            SensorValue = (T)Convert.ChangeType(rnd.Next(MinValue, MaxValue + 1), typeof(T));
            QualityValue = WarningOrAlarm();

            RequestResult = $"$ FIX, {ID}, {Type}, {SensorValue}, {QualityValue} *";

            Dispatcher.Invoke(() =>
            {
                text.Content = $"$ FIX, {ID}, {Type}, {SensorValue}, {QualityValue} *";
                Logger.Info(text);
            });
           
        }

        private string WarningOrAlarm()
        {
            // MEGA KING FORMULA !!! 
            // return SensorValue.CompareTo((T)Convert.ChangeType((MaxValue - MinValue) * 0.10 + MinValue, typeof(T))) > 0 && SensorValue.CompareTo((T)Convert.ChangeType((MaxValue - MinValue) * 0.25 + MinValue, typeof(T))) < 0 || SensorValue.CompareTo((T)Convert.ChangeType((MaxValue - MinValue) * 0.75 + MinValue, typeof(T))) > 0 && SensorValue.CompareTo((T)Convert.ChangeType((MaxValue - MinValue) * 0.90 + MinValue, typeof(T))) < 0;

            if (SensorValue.CompareTo((T)Convert.ChangeType(_topTopEdge, typeof(T))) > 0 || SensorValue.CompareTo((T)Convert.ChangeType(_bottomBottomEdge, typeof(T))) < 0) { return "Alarm"; } 
            if (SensorValue.CompareTo((T)Convert.ChangeType(_topBottomEdge, typeof(T))) > 0 || SensorValue.CompareTo((T)Convert.ChangeType(_bottomTopEdge, typeof(T))) < 0) { return "Warning"; } 
            return "Normal";
        }


        public void SendMessageToClient()
        {
            speedServer.Result = RequestResult;
            speedServer.Send();
        }

       
    }
}

