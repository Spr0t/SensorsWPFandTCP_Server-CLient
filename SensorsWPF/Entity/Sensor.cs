using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Timers;
using NLog;
using System.Windows.Threading;
using System.Reflection.Metadata;
using System.Net.Sockets;
using SensorsWPF.Logic;
using SensorsWPF.TCPLogic;

namespace SensorsWPF.Entity
{
    public class BaseSensor 
    {
        protected int ID { get; set; }
        protected SensorType Type { get; set; }
        protected EncoderType EncoderType { get; set; }
        protected int Frequency { get; set; }
        protected double MillisecondsDelay => 1000 / Frequency;

        protected int Value;
        protected string Quality;
    }

    public class Sensor : BaseSensor
    {
        private Label Label { get; set; }
        
        private Edges Edges;

        private Random Random;

        public ClientObject Client;

        public Sensor(int iD, SensorType type, int minValue, int maxValue, EncoderType encoderType, int frequency)
        {
            ID = iD;
            Type = type;
            EncoderType = encoderType;
            Frequency = frequency;
            Edges = new Edges(minValue, maxValue);
            Random = new Random();
        }

        internal void Start(Label label)
        {
            Label = label;
            SensorStart();
        }

        private void SensorStart()
        {
            var timer = new Timer() { Interval = MillisecondsDelay };
            timer.Start();
            timer.Elapsed += async (o, e) =>
            {
                await Task.Run(() =>
                {
                    SensorBody();
                });
            };
        }

        private void SensorBody()
        {          
            Value = Random.Next(Edges.Bottom, Edges.Top + 1);
            Quality = WarningOrAlarm();

            var message = $"$ FIX, \t{ID}, \t{Type}, \t{Value}, \t{Quality} *";
            Label.Dispatcher.Invoke(DispatcherPriority.Normal, 
                new Action(() => 
                {
                    Label.Content = message;
                }));

            if (Client != null)
            {
                ServerObject.SendMessage(message, Client);
            }
        }

        private string WarningOrAlarm()
        {
            if(Value > Edges.TopTopEdge || Value < Edges.BottomBottomEdge) { return "Alarm"; }
            if(Value > Edges.TopBottomEdge || Value < Edges.BottomTopEdge) { return "Warning"; }
            return "Normal";
        }  
    }
}

