using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class TagSearchModel : PagingItems {
        
        [Display(Name = "Titile", ResourceType = typeof(UT.SL.Model.Resource.Tag))]
        public string SearchTitile { get; set; }
        
        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.Tag))]
        public System.Nullable<System.Int32> SearchType { get; set; }
        
        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.Tag))]
        public System.Nullable<System.DateTime> SearchCreateDate { get; set; }

        [Display(Name = "IsValid", ResourceType = typeof(UT.SL.Model.Resource.Tag))]
        public System.Nullable<System.Boolean> SearchIsValid { get; set; }

    }
}
