using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SensorsWPF.Logic;
using SensorsWPF.TCPLogic;

namespace SensorsWPF
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeSensors();
            InitializeServer();
        }


        private void InitializeSensors()
        {
            SensorFactory.CreateAllSensors(out int sensorsCount);
            
            for (var i = 0; i < sensorsCount; i++)
            {
                var newLabel = new Label()
                {
                    Name = "LabelNumber" + i,                
                    Width = 500,
                    Height = 50,
                    Margin = new Thickness(10, 5, 0, 0),
                    RenderTransform = new TranslateTransform(0, 30*i)
                };
                MainRoot.Children.Add(newLabel);

                SensorFactory.AppendLabel(newLabel);
            }

            SensorFactory.SyncSensorsAndLabels();
            SensorFactory.RunAllSensors();
        }

        private void InitializeServer()
        {
            Server.Start();
        }

    }
}
