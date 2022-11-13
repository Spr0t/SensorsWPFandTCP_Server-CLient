using SensorsWPF.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static SensorsWPF.Sensor;
using System.Threading;

namespace SensorsWPF
{
    
    public partial class MainWindow : Window
    {

        public string speedResult { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SpeedSensor_Loaded(object sender, RoutedEventArgs e)
        {
                var s = new Sensor<sbyte>(SpeedSensor, 1,8080);
                s.Configuration();
                s.SensorStart();
        }





        private void PositionSensor_Loaded(object sender, RoutedEventArgs e)
        {
            var s = new Sensor<short>(PositionSensor, 2,8090);
            s.Configuration();
            s.SensorStart();
        }


        private void DepthSensor_Loaded(object sender, RoutedEventArgs e)
        {
            var s = new Sensor<byte>(DepthSensor, 3,8070);
            s.Configuration();
            s.SensorStart();
        }

       
        
    }
}
