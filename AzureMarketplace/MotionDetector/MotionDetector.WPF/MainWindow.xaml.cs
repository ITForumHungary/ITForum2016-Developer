using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;

namespace MotionDetector.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<MotionFragment> fragments = new List<MotionFragment>();
        Timer mainTimer;
        MotionData data;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void media_MediaOpened(object sender, RoutedEventArgs e)
        {

            #region parse
            //TODO: Open file (?)
            string exampleJSON = @"{
                'version': 2,
                'timescale': 23976,
                'offset': 0,
                'framerate': 24,
                'width': 1280,
                'height': 720,
                'regions': [
                    {
                        'id': 0,
                        'type': 'polygon',
                        'points': [
                            {'x': 0, 'y': 0},
                            {'x': 0.5, 'y': 0},
                            {'x': 0, 'y': 1}
                        ]
                    }
                ],
                'fragments': [
                    {
                        'start': 0,
                        'duration': 226765
                    },
                    {
                        'start': 226765,
                        'duration': 47952,
                        'interval': 999,
                        'events': [
                            [
                                {
                                    'type': 2,
                                    'typeName': 'motion',
                                    'locations': [
                                        {
                                            'x': 0.004184,
                                            'y': 0.007463,
                                            'width': 0.991667,
                                            'height': 0.985185
                                        }
                                    ],
                                    'regionId': 0
                                }
                            ]
                        ]
                    }
                ]
            }";

            StreamReader jsonReader = new StreamReader("Resources/MI_motiondetection.json");
            String jsonData = jsonReader.ReadToEnd(); 

            //Parse the fragments
            data = JsonConvert.DeserializeObject<MotionData>(jsonData);

            MotionEventList.ItemsSource = data.fragments;

            //Parse the fragments
            /*JObject parsedJson = JObject.Parse(exampleJSON);
            IList<JToken> frags = parsedJson["fragments"].Children().ToList();
            //timescale = Convert.ToInt32(parsedJson["timescale"].ToString());
            foreach (JToken frag in frags)
            {
                //Deserialize to a MotionFragment object
                MotionFragment fragment = JsonConvert.DeserializeObject<MotionFragment>(frag.ToString());
                //Add the deserialized object to the (global) fragments list
                fragments.Add(fragment);
            }*/
            #endregion

            //SecurityVideo.Play();
            //mainTimer = new Timer(timer_tick, null, 0, 100);
        }

        void timer_tick(Object o)
        {
            foreach (MotionFragment f in fragments)
            {
                var start = Convert.ToInt32(f.start);
                var end = start + Convert.ToInt32(f.duration);
                var current = SecurityVideo.Position.TotalSeconds * data.timescale;
                if ((start <= current) && (end >= current))
                {
                    if (f.events != null)
                    {
                        MotionStatus.Text = "Mozgás van";
                        break;
                    }
                    else
                    {
                        MotionStatus.Text = "Nincs mozgás";
                    }
                }
                else
                {
                    MotionStatus.Text = "Nincs mozgás";
                }
            }
        }

        private void MotionEventList_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
            SecurityVideo.Pause();
        }
    }
}
