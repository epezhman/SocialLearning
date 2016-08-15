using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UT.SL.Model
{
    public class CourseSummaryModel
    {
        public int ActivityCount { get; set; }
        public int ParticipantCount { get; set; }
        public int ResourceCount { get; set; }
        public int ForumCount { get; set; }
        public int AssignmentCount { get; set; }
        public int LearningGroupsCount { get; set; }
        public List<int> ParticipantIsd { get; set; }

        public CourseSummaryModel()
        {
            ParticipantIsd = new List<int>();
        }
    }
}
