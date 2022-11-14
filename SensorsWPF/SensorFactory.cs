using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Controls;
using Microsoft.Extensions.Configuration;
using NLog;
using SensorsWPF.Entity;
using SensorsWPF.Logic;

namespace SensorsWPF
{
    public static class SensorFactory
    {
        private static List<BaseSensor> _sensorsList;
        private static List<Label> _labelsList;
        public static List<SensorAndLabel> _sensorsAndLabels;

        public static void CreateAllSensors(out int sensorsCount)
        {
            _sensorsList ??= new List<BaseSensor>();
            SensorsSettings sensorsSettings;

            try
            {
                var settingsJson = File.ReadAllText("sensorConfig.json");
                sensorsSettings = JsonSerializer.Deserialize<SensorsSettings>(settingsJson);
            }
            catch
            {
                throw;
            }

            foreach (var sensorsSetting in sensorsSettings.Sensors)
            {
                var Id = sensorsSetting.ID;
                var EncoderType = (EncoderType)Enum.Parse(typeof(EncoderType), sensorsSetting.EncoderType.FirstLetterToUpper());

                switch ((SensorType)Enum.Parse(typeof(SensorType), sensorsSetting.Type.FirstLetterToUpper()))
                {
                    case SensorType.Speed:
                        Sensor sensorSpeed = new Sensor(Id, SensorType.Speed, sensorsSetting.MinValue, sensorsSetting.MaxValue, EncoderType, sensorsSetting.Frequency);
                        _sensorsList.Add(sensorSpeed);
                        break;

                    case SensorType.Position:
                        var sensorPosition = new Sensor(Id, SensorType.Position, sensorsSetting.MinValue, sensorsSetting.MaxValue, EncoderType, sensorsSetting.Frequency);
                        _sensorsList.Add(sensorPosition);
                        break;

                    case SensorType.Depth:
                        var sensorDepth = new Sensor(Id, SensorType.Depth, sensorsSetting.MinValue, sensorsSetting.MaxValue, EncoderType, sensorsSetting.Frequency);
                        _sensorsList.Add(sensorDepth);
                        break;
                }
            }
            sensorsCount = _sensorsList.Count;
        }

        public static void AppendLabel(Label label)
        {
            _labelsList ??= new List<Label>();
            _labelsList.Add(label);
        }

        public static void SyncSensorsAndLabels()
        {
            _sensorsAndLabels ??= new List<SensorAndLabel>();

            for (var i = 0; i < _sensorsList.Count; i++)
            {
                _sensorsAndLabels.Add(new SensorAndLabel()
                {
                    Sensor = _sensorsList[i],
                    Label = _labelsList[i]
                });
            }
        }

        public static void RunAllSensors()
        {
            foreach (var sensor in _sensorsList)
            {
                sensor.Run();
            }
        }

        public static Label AskFactoryForLabel(this BaseSensor sensor)
        {
            var sensorAndLabel = _sensorsAndLabels.FirstOrDefault(x => x.Sensor == sensor);

            return sensorAndLabel != null ? sensorAndLabel.Label : throw new ArgumentNullException();
        }
    }

    public class SensorAndLabel
    {
        public BaseSensor Sensor { get; set; }
        public Label Label { get; set; }
    }
}
