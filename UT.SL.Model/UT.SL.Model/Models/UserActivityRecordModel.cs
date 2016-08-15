using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UT.SL.Model
{
    public class UserActivityRecordModel
    {
        public DateTime Date { get; set; }
        public int CountOfVote { get; set; }
        public int CountOfShare { get; set; }
        public int CountOfComments { get; set; }
        public int CountOfTags { get; set; }
        public int CountOfCreatedObjects { get; set; }
        public int CountOfReadenObjects { get; set; }
        public int Score { get; set; }
        public int CountOfAssignmentsubmissionSubmit { get; set; }
        public int CountOfForumDisscussion { get; set; }
        public List<ObjectViewModel> Objects { get; set; }

        public UserActivityRecordModel()
        {
            CountOfVote = 0;
            CountOfShare = 0;
            CountOfComments = 0;
            CountOfTags = 0;
            CountOfCreatedObjects = 0;
            CountOfReadenObjects = 0;
            Score = 0;
            CountOfAssignmentsubmissionSubmit = 0;
            CountOfForumDisscussion = 0;
            Objects = new List<ObjectViewModel>();
        }
    }
    
}
