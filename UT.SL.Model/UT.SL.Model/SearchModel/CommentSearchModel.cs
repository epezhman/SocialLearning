using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class CommentSearchModel : PagingItems {
        
        [Display(Name = "Title", ResourceType = typeof(UT.SL.Model.Resource.Comment))]
        public string SearchTitle { get; set; }
        
        [Display(Name = "CreatDate", ResourceType = typeof(UT.SL.Model.Resource.Comment))]
        public System.Nullable<System.DateTime> SearchCreateDate { get; set; }
    }
}
