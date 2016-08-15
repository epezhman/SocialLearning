using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class QuestionSearchModel : PagingItems {
        
        [Display(Name = "Title", ResourceType = typeof(UT.SL.Model.Resource.Questions))]
        public string SearchTitle { get; set; }
        
        [Display(Name = "Body", ResourceType = typeof(UT.SL.Model.Resource.Questions))]
        public string SearchBody { get; set; }
        
        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.Questions))]
        public System.Nullable<System.DateTime> SearchCreateDate { get; set; }
        
        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.Questions))]
        public System.Nullable<System.Int32> SearchType { get; set; }
    }
}
