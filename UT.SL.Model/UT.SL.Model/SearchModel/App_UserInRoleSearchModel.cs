using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class App_UserInRoleSearchModel : PagingItems {
        
        [Display(Name = "CreatDate", ResourceType = typeof(UT.SL.Model.Resource.App_UserInRole))]
        public System.Nullable<System.DateTime> SearchCreateDate { get; set; }
    }
}
