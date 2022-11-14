using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorsWPF.Logic
{
    public class SensorsSettings
    {
        public SensorSetting[] Sensors { get; set; }
    }

    public class SensorSetting
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public string EncoderType { get; set; }
        public int Frequency { get; set; }
    }
}
