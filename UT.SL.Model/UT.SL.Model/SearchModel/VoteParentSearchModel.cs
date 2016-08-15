using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class VoteParentSearchModel : PagingItems {
        
        [Display(Name = "ObjectType", ResourceType = typeof(UT.SL.Model.Resource.VoteParent))]
        public System.Nullable<System.Int32> SearchObjectType { get; set; }
        
        [Display(Name = "UpvoteCount", ResourceType = typeof(UT.SL.Model.Resource.VoteParent))]
        public System.Nullable<System.Int32> SearchUpvoteCount { get; set; }
        
        [Display(Name = "DownvoteCount", ResourceType = typeof(UT.SL.Model.Resource.VoteParent))]
        public System.Nullable<System.Int32> SearchDownvoteCount { get; set; }
        
        [Display(Name = "Count", ResourceType = typeof(UT.SL.Model.Resource.VoteParent))]
        public System.Nullable<System.Int32> SearchCount { get; set; }
    }
}
