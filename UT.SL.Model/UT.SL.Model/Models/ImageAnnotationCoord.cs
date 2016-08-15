using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UT.SL.Model
{
    public class ImageAnnotationCoord
    {
        public string top { get; set; }
        public string left { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string text { get; set; }
        public string id { get; set; }
        public string editable { get; set; }
        public int objectId { get; set; }
        public int objectType { get; set; }
    }
}
