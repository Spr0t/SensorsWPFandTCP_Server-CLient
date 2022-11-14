using SensorsWPF.Server;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SensorsWPF.Server;

namespace SensorsWPF
{
    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeSensors();
            StartServer();
        }


        private void InitializeSensors()
        {
            SensorFactory.CreateAllSensors(out int sensorsCount);
            

            for (var i = 0; i < sensorsCount; i++)
            {
                RowDefinition newGridRow = new RowDefinition()
                {
                    Name = "GridRow" +i
                };
       
                MainRoot.RowDefinitions.Add(newGridRow);

                var newLabel = new Label()
                {
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalContentAlignment = VerticalAlignment.Top,
                    Name = "LabelNumber" + i,
                    Width = 500,
                    Height = 50,
                };
                Grid.SetRow(newLabel, i);

                MainRoot.Children.Add(newLabel);
                SensorFactory.AppendLabel(newLabel);
            }

            SensorFactory.SyncSensorsAndLabels();
            SensorFactory.RunAllSensors();

        }

        private void StartServer()
        {
            Server.Server s = new Server.Server();
            


        }

    }
}
