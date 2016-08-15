using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class CommentLogSearchModel : PagingItems {
        
        [Display(Name = "ChangeText", ResourceType = typeof(UT.SL.Model.Resource.CommentLog))]
        public string SearchChangeText { get; set; }
        
        [Display(Name = "ChangeDate", ResourceType = typeof(UT.SL.Model.Resource.CommentLog))]
        public System.Nullable<System.DateTime> SearchChangeDate { get; set; }
        
        [Display(Name = "ChangedBy", ResourceType = typeof(UT.SL.Model.Resource.CommentLog))]
        public System.Nullable<System.Int32> SearchChangedBy { get; set; }
    }
}
