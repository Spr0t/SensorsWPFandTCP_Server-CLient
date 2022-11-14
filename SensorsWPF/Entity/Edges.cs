using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SensorsWPF.Entity
{
    internal class Edges
    {
        public int Bottom { get; set; }             // 0%
        public double BottomBottomEdge { get; set; }    // 10%
        public double BottomTopEdge { get; set; }       // 25%
        public double TopBottomEdge { get; set; }       // 75%
        public double TopTopEdge { get; set; }          // 90%
        public int Top { get; set; }                // 100%

        internal Edges(int minValue, int maxValue)
        {
            Bottom = minValue;
            BottomBottomEdge = (maxValue - minValue) * 0.10 + minValue;
            BottomTopEdge = (maxValue - minValue) * 0.25 + minValue;
            TopBottomEdge = (maxValue - minValue) * 0.75 + minValue;
            TopTopEdge = (maxValue - minValue) * 0.90 + minValue;
            Top = maxValue;
        }
    }

 


}
