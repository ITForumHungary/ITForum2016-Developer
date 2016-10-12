using System;
using System.Windows;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Controls;
using System.Timers;
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Media;

namespace MotionDetector.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        public static DispatcherTimer mainTimer;
        public static MotionData data;
        public static MotionData getData()
        {
            return data;
        }
        public MainWindow()
        {
            InitializeComponent();

            #region parse

            StreamReader jsonReader = new StreamReader("Resources/MI_motiondetection.json");
            String jsonData = jsonReader.ReadToEnd();

            //parse the fragments
            data = JsonConvert.DeserializeObject<MotionData>(jsonData);

            MotionEventList.ItemsSource = data.fragments;
            #endregion
        }

        private void media_MediaOpened(object sender, RoutedEventArgs e)
        {
            //start the timer
            mainTimer = new DispatcherTimer(
                TimeSpan.FromMilliseconds(100),
                DispatcherPriority.Normal,
                (_,__) => Update(), 
                this.Dispatcher);
            
            mainTimer.Start();
        }

        private void Update()
        {
            foreach (Fragment f in data.fragments)
            {
                var start = Convert.ToInt32(f.start);
                var end = start + Convert.ToInt32(f.duration);
                var current = SecurityVideo.Position.TotalSeconds * data.timescale;
                if ((start <= current) && (end >= current))
                {
                    if (f.events != null)
                    {
                        MotionStatus.Text = "Mozgás van";
                        MotionStatus.Foreground = new SolidColorBrush(Colors.Red);
                        break;
                    }
                    else
                    {
                        MotionStatus.Text = "Nincs mozgás";
                        MotionStatus.Foreground = new SolidColorBrush(Colors.Green);
                    }
                }
                else
                {
                    MotionStatus.Text = "Nincs mozgás";
                    MotionStatus.Foreground = new SolidColorBrush(Colors.Green);
                }
            }
        }

        private void PlayFragmentButton_Click(object sender, RoutedEventArgs e)
        {
            var clickedFragment = ((e.Source as Button)?.DataContext as Fragment);
            if (clickedFragment != null)
            {
                SecurityVideo.Position = TimeSpan.FromSeconds(clickedFragment.start/data.timescale);
            }
        }
        
    }
    public class TicToTimeConverter : IValueConverter
    {
        public object Convert(object tic, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (tic is int)
            {
                
                TimeSpan time = TimeSpan.FromSeconds(System.Convert.ToInt32(tic) / MainWindow.getData().timescale);
                return time.ToString();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

}
