using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class App_UserInfoSearchModel : PagingItems {
        
        [Display(Name = "FirstName", ResourceType = typeof(UT.SL.Model.Resource.App_UserInfo))]
        public string SearchFirstName { get; set; }
        
        [Display(Name = "LastName", ResourceType = typeof(UT.SL.Model.Resource.App_UserInfo))]
        public string SearchLastName { get; set; }
        
        [Display(Name = "BirsthDate", ResourceType = typeof(UT.SL.Model.Resource.App_UserInfo))]
        public System.Nullable<System.DateTime> SearchBirsthDate { get; set; }
    }
}
