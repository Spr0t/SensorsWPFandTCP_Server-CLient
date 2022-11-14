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
using SensorsWPF.TCPLogic;

namespace SensorsWPF.Logic
{
    public class SensorAndLabelAndClient
    {
        public Sensor Sensor { get; set; }
        public Label Label { get; set; }
        public ClientObject Client { get; set; }
    }

    public static class SensorFactory
        
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();
        private static SensorsSettings sensorsSettings;
        private static List<Sensor> _sensorsList;
        private static List<Label> _labelsList;
        private static List<ClientObject> _clientList;
        public static List<SensorAndLabelAndClient> _sensorsAndLabelsAndClients;

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

        public static void AppendClient(ClientObject client)
        {
            _clientList ??= new List<ClientObject>();
            _clientList.Add(client);
            SyncSensorsAndClients();
        }

        public static void SyncSensorsAndLabels()
        {
            _sensorsAndLabelsAndClients ??= new List<SensorAndLabelAndClient>();

            for (var i = 0; i < _sensorsList.Count; i++)
            {
                _sensorsAndLabelsAndClients.Add(new SensorAndLabelAndClient()
                {
                    Sensor = _sensorsList[i],
                    Label = _labelsList[i]
                });
            }
        }

        public static void SyncSensorsAndClients()
        {
            _sensorsAndLabelsAndClients = _sensorsAndLabelsAndClients ?? throw new Exception("List was not initialized");

            /// TODO Refactoring make client wait for a free sensor
            if (_clientList.Count > _sensorsList.Count)
            {
                throw new Exception("Too many clients");
                logger.Info("Too many clients, Disconnected");
            }

            for (var i = 0; i < _clientList.Count; i++)
            {
                if (_sensorsAndLabelsAndClients[i].Client == null)
                {
                    _sensorsAndLabelsAndClients[i].Client = _clientList[i];
                    _sensorsAndLabelsAndClients[i].Sensor.Client = _clientList[i];
                }
            }
        }

        public static void RunAllSensors()
        {
            for (var i = 0; i < _sensorsAndLabelsAndClients.Count; i++)
            {
                _sensorsAndLabelsAndClients[i].Sensor.Start(_sensorsAndLabelsAndClients[i].Label);         
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
