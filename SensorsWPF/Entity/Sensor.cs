using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Timers;
using NLog;
using System.Windows.Threading;
using System.Reflection.Metadata;
using SensorsWPF.Server;
using System.Net.Sockets;

namespace SensorsWPF.Entity
{
    public class BaseSensor
    {
        public int ID { get; set; }
        public SensorType Type { get; set; }
        public EncoderType EncoderType { get; set; }
        public int Frequency { get; set; }

        public virtual void Run()
        {
        }
    }

    public class Sensor : BaseSensor 
    {
        private int MinValue { get; set; }
        private int MaxValue { get; set; }
        private int SensorValue { get; set; }
        private int MillisecondsDelay => 1000 / Frequency;
        private string QualityValue { get; set; }
        public string SensorResult { get; set; }
        private Label Label { get; set; }
        private Random _random;

        public Sensor(int iD, SensorType type, int minValue, int maxValue, EncoderType encoderType, int frequency)
        {
            ID = iD;
            Type = type;
            MinValue = minValue;
            MaxValue = maxValue;
            EncoderType = encoderType;
            Frequency = frequency;
            _random = new Random();
        }


        public Sensor()
        {

        }

        public override void Run()
        {
            Label = this.AskFactoryForLabel();
            SensorStart();          
        }

        public void SensorStart()
        {
            var timer = new Timer() { Interval = (double)MillisecondsDelay };
            timer.Start();
            timer.Elapsed += async (o, e) =>
            {
                await Task.Run(() =>
                {
                    SensorBody();
                });
            };
        }

        public void SensorBody()
        {          
            SensorValue = _random.Next(MinValue, MaxValue + 1);
            QualityValue=  WarningOrAlarm();
            Label.Dispatcher.Invoke(DispatcherPriority.Normal, 
                new Action(() => 
                {
                    SensorResult = $"$ FIX, {ID}, {Type}, {SensorValue}, {QualityValue} *";
                    Label.Content = $"$ FIX, {ID}, {Type}, {SensorValue}, {QualityValue} *";

                }));

            if (ServerObject.client != null)
            {
                ServerObject.SendMessage(SensorResult, ServerObject.client);
            }



        }

        private string WarningOrAlarm()
        {

           var _bottomBottomEdge = (MaxValue - MinValue) * 0.10 + MinValue;
           var _topTopEdge = (MaxValue - MinValue) * 0.90 + MinValue;

           var _bottomTopEdge = (MaxValue - MinValue) * 0.25 + MinValue;
           var _topBottomEdge = (MaxValue - MinValue) * 0.75 + MinValue;
           



            if(SensorValue > _topTopEdge || SensorValue < _bottomBottomEdge) { return "Alarm"; }
            if(SensorValue > _topBottomEdge || SensorValue < _bottomTopEdge) { return "Warning"; }
            return "Normal";

        }





    }
}

