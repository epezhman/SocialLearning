using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class App_UserEnrolementSearchModel : PagingItems {
        
        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.App_UserEnrolements))]
        public System.Nullable<System.Int32> SearchType { get; set; }
        
        [Display(Name = "CreatDate", ResourceType = typeof(UT.SL.Model.Resource.App_UserEnrolements))]
        public System.Nullable<System.DateTime> SearchCreateDate { get; set; }
        
        [Display(Name = "Status", ResourceType = typeof(UT.SL.Model.Resource.App_UserEnrolements))]
        public System.Nullable<System.Int32> SearchStatus { get; set; }
    }
}
