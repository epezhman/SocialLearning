using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UT.SL.Model
{
    public class FilterObjectModel
    {
        public bool IsResource { get; set; }
        public bool IsActivity { get; set; }
        public bool IsHot { get; set; }
        public bool IsRecommended { get; set; }
        public bool IsForums { get; set; }
        public bool IsAssignments { get; set; }
        public List<int> SocialGroupIds { get; set; }
        public List<int> LearningGroupIds { get; set; }

        public FilterObjectModel()
        {
            SocialGroupIds = new List<int>();
            LearningGroupIds = new List<int>();
        }
    }
}
