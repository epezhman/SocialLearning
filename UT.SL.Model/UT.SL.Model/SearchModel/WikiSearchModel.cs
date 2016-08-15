using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class WikiSearchModel : PagingItems {
        
        [Display(Name = "HyperLink", ResourceType = typeof(UT.SL.Model.Resource.Wiki))]
        public string SearchHyperLink { get; set; }
        
        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.Wiki))]
        public System.Nullable<System.Int32> SearchType { get; set; }
        
        [Display(Name = "Text", ResourceType = typeof(UT.SL.Model.Resource.Wiki))]
        public string SearchText { get; set; }
        
        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.Wiki))]
        public System.Nullable<System.DateTime> SearchCreateDate { get; set; }
    }
}
