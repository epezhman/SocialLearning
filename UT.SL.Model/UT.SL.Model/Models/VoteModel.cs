using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UT.SL.Model
{
    public class VoteModel
    {
        public bool UpVote { get; set; }
        public bool DownVote { get; set; }
        public double VoteAverageCount { get; set; }
        public int MyReaction { get; set; }
    }
}
