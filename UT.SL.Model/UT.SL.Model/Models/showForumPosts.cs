using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UT.SL.Data.LINQ;

namespace UT.SL.Model
{
    public class showForumPosts
    {
        public UT.SL.Data.LINQ.ForumDiscussionPost post { set; get; }
        public int indent { set; get; }
        public int? parentPostId { set; get; }
 
    }
}
