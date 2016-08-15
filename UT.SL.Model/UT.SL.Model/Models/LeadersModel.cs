using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UT.SL.Data.LINQ;

namespace UT.SL.Model
{
    public class LeadersModel
    {
        public int ObjectId { get; set; }
        public int ObjectType { get; set; }
        public DateTime CreateDate { get; set; }
        public App_User User { get; set; }
        public string Title { get; set; }
        public int Rank { get; set; }
        public bool IsPopular { get; set; }
        public bool IsActive { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsTop { get; set; }
        public int VoteCount { get; set; }
        public int CommentCount { get; set; }
        public int TagCount { get; set; }
        public int CreatedCount { get; set; }
        public int TotalScore { get; set; }

        public LeadersModel()
        {
            IsPopular = false;
            IsActive = false;
            TotalScore = 0;
            IsTop = false;
        }

    }
}
