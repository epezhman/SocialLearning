using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UT.SL.Model
{
    public class ToEmergeIds
    {
        public List<string> LearningGroups { get; set; }
        public List<string> SocialGroups { get; set; }
        public List<string> ShareUserIds { get; set; }

        public ToEmergeIds()
        {
            LearningGroups = new List<string>();
            SocialGroups = new List<string>();
            ShareUserIds = new List<string>();
        }
    }

    public class SharedGroupIds
    {
        public List<int> LearningGroups { get; set; }
        public List<int> SocialGroups { get; set; }
        public List<int> ShareUserIds { get; set; }
        public List<int> CourseIds { get; set; }

        public SharedGroupIds()
        {
            LearningGroups = new List<int>();
            SocialGroups = new List<int>();
            ShareUserIds = new List<int>();
            CourseIds = new List<int>();
        }
    }

}
