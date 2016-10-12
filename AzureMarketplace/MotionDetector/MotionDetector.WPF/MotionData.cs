using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionDetector.WPF
{
    public class MotionData
    {
        public int version { get; set; }
        public int timescale { get; set; }
        public int offset { get; set; }
        public int framerate { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public Region[] regions { get; set; }
        public Fragment[] fragments { get; set; }
    }

    public class Region
    {
        public int id { get; set; }
        public string type { get; set; }
        public Point[] points { get; set; }
    }

    public class Point
    {
        public float x { get; set; }
        public float y { get; set; }
    }

    public class Fragment
    {
        public int start { get; set; }
        public int duration { get; set; }
        public int interval { get; set; }
        public Event[][] events { get; set; }
    }

    public class Event
    {
        public int type { get; set; }
        public string typeName { get; set; }
        public Location[] locations { get; set; }
        public int regionId { get; set; }
    }

    public class Location
    {
        public float x { get; set; }
        public float y { get; set; }
        public float width { get; set; }
        public float height { get; set; }
    }

}
