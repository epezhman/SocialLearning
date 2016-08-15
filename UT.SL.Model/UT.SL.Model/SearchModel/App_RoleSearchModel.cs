using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class App_RoleSearchModel : PagingItems {
        
        [Display(Name = "Title", ResourceType = typeof(UT.SL.Model.Resource.App_Role))]
        public string SearchTitle { get; set; }
        
        [Display(Name = "Description", ResourceType = typeof(UT.SL.Model.Resource.App_Role))]
        public string SearchDescription { get; set; }
    }
}
