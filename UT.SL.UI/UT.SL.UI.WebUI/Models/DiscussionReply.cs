using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Data.LINQ;

namespace UT.SL.UI.WebUI.Models
{
    public  class ForumDiscussionPost
    {
        SocialLearningDataContext dc = new Data.LINQ.SocialLearningDataContext();
        //public  int? getParentPartId(int postId)
        //{
        //    return dc.ForumDiscussionPosts.Where(e => e.Id == postId).Single().ParentReplyId;
        //}
        //public List<UT.SL.Data.LINQ.ForumDiscussionPost> getChildren(int testId)
        //{

        //    /// testid has to be set when calling this method
        //    /// i.parentPartId => i.getParentPartId(testid)
        //    /// 
            
        //    var partChildren = dc.ForumDiscussionPosts.Where(i => i.ParentReplyId == 1);
        //    if (partChildren.Count() > 0)
        //    {
        //        return partChildren.ToList();
        //    }
        //    else
        //    {
        //        return new List<UT.SL.Data.LINQ.ForumDiscussionPost>();
        //    }
        //}
    }
}