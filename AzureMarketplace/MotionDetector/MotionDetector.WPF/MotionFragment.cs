using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionDetector.WPF
{
    public class MotionFragment
    {
        public string start { get; set; }
        public string duration { get; set; }
        public List<List<MotionEvent>> events { get; set; }
    }

    public class MotionEvent
    {
        public string type { get; set; }
        public string typeName { get; set; }
        public string regionId { get; set; }
        public List<MotionEventLocation> locations { get; set; }
    }

    public class MotionEventLocation
    {
        public string x { get; set; }
        public string y { get; set; }
        public string width { get; set; }
        public string height { get; set; }
    }
}
