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

namespace SensorsWPF.Logic
{
    public class SensorAndLabel
    {
        public Sensor Sensor { get; set; }
        public Label Label { get; set; }
    }

    public static class SensorFactory
    {
        private static SensorsSettings sensorsSettings;
        private static List<Sensor> _sensorsList;
        private static List<Label> _labelsList;
        public static List<SensorAndLabel> _sensorsAndLabels;

        public static void CreateAllSensors(out int sensorsCount)
        {
            sensorsSettings = ReadConfig();

            _sensorsList ??= new List<Sensor>();
            foreach (var sensorSetting in sensorsSettings.Sensors)
            {
                var encoderType = (EncoderType)Enum.Parse(typeof(EncoderType), sensorSetting.EncoderType.FirstLetterToUpper());
                var sensorType = (SensorType)Enum.Parse(typeof(SensorType), sensorSetting.Type.FirstLetterToUpper());
                var sensor = new Sensor(sensorSetting.ID, sensorType, sensorSetting.MinValue, sensorSetting.MaxValue, encoderType, sensorSetting.Frequency);

                _sensorsList.Add(sensor);
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
            for (var i = 0; i < _sensorsAndLabels.Count; i++)
            {
                _sensorsAndLabels[i].Sensor.Start(_sensorsAndLabels[i].Label);         
            }
        }

        private static SensorsSettings ReadConfig()
        {
            try
            {
                return JsonSerializer.Deserialize<SensorsSettings>(File.ReadAllText("sensorConfig.json")) ?? throw new ArgumentNullException("Check sensorConfig.json");
            }
            catch
            {
                throw;
            }
        }
    }
}
