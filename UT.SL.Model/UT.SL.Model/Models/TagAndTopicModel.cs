using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UT.SL.Data.LINQ;

namespace UT.SL.Model
{
    public class TagAndTopicModel
    {
        public Tag Tag { get; set; }
        public Topic Topic { get; set; }
        public bool IsTag { get; set; }
    }
}
