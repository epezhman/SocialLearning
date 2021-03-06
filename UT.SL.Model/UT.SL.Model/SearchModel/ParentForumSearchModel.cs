using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class ForumsearchModel : PagingItems {
        
        [Display(Name = "Title", ResourceType = typeof(UT.SL.Model.Resource.Forum))]
        public string SearchTitle { get; set; }

        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.Forum))]
        public System.Nullable<System.Int32> SearchType { get; set; }

        [Display(Name = "CreatDate", ResourceType = typeof(UT.SL.Model.Resource.Forum))]
        public System.Nullable<System.DateTime> SearchCreateDate { get; set; }

        public int? CourseId { get; set; }
    }
}
