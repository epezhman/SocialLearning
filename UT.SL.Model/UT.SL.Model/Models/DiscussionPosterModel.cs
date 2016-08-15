using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UT.SL.Data.LINQ;

namespace UT.SL.Model
{
    public class DiscussionPosterModel
    {
        public List<App_User> Users { get; set; }
        public int TotalPosts { get; set; }
        public int TheRemainingUserCount { get; set; }

        public DiscussionPosterModel() {
            Users = new List<App_User>();
        }
    }
}
