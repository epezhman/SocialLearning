using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UT.SL.Data.LINQ;

namespace UT.SL.Model.Models
{
    public class ForumDiscussionsModel
    {
        public ForumDiscussion Discussion { get; set; }
        public List<ForumDiscussionPost> Posts { get; set; }        
    }
}
