using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class ForumDiscussionsearchModel : PagingItems {
        
        [Display(Name = "Subject", ResourceType = typeof(UT.SL.Model.Resource.ForumDiscussion))]
        public string SearchSubject { get; set; }
        
        [Display(Name = "Body", ResourceType = typeof(UT.SL.Model.Resource.ForumDiscussion))]
        public string SearchBody { get; set; }
        
        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.ForumDiscussion))]
        public System.Nullable<System.DateTime> SearchCreateDate { get; set; }
        
        [Display(Name = "ViewCount", ResourceType = typeof(UT.SL.Model.Resource.ForumDiscussion))]
        public System.Nullable<System.Int32> SearchViewCount { get; set; }
    }
}
