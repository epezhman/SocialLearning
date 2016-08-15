using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class CategorySearchModel : PagingItems {
        
        [Display(Name = "Title", ResourceType = typeof(UT.SL.Model.Resource.Category))]
        public string SearchTitle { get; set; }
    }
}
