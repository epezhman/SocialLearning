using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class ForumDiscussionPostSearchModel : PagingItems {
        
        [Display(Name = "Text", ResourceType = typeof(UT.SL.Model.Resource.ForumDiscussionPost))]
        public string SearchText { get; set; }
        
        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.ForumDiscussionPost))]
        public System.Nullable<System.DateTime> SearchCreateDate { get; set; }
    }
}
