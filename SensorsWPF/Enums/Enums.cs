using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorsWPF
{ 
    public enum SensorType
    {
        Speed = 1,
        Position = 2,
        Depth = 3
    }

    public enum Frequency
    {
        Speed = 500,
        Position = 1000,
        Depth = 100
    }

    public enum EncoderType
    {
        Unfixed = 0,
        Fixed = 1
    }
}
