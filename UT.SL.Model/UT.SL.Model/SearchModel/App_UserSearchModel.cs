using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class App_UserSearchModel : PagingItems {
        
        [Display(Name = "UserName", ResourceType = typeof(UT.SL.Model.Resource.App_User))]
        public string SearchUserName { get; set; }
        
        [Display(Name = "FirstName", ResourceType = typeof(UT.SL.Model.Resource.App_User))]
        public string SearchFirstName { get; set; }
        
        [Display(Name = "LastName", ResourceType = typeof(UT.SL.Model.Resource.App_User))]
        public string SearchLastName { get; set; }
        
        [Display(Name = "Password", ResourceType = typeof(UT.SL.Model.Resource.App_User))]
        public string SearchPassword { get; set; }
        
        [Display(Name = "Email", ResourceType = typeof(UT.SL.Model.Resource.App_User))]
        public string SearchEmail { get; set; }
        
        [Display(Name = "IsAdmin", ResourceType = typeof(UT.SL.Model.Resource.App_User))]
        public System.Nullable<System.Boolean> SearchIsAdmin { get; set; }
        
        [Display(Name = "IsDeleted", ResourceType = typeof(UT.SL.Model.Resource.App_User))]
        public System.Nullable<System.Boolean> SearchIsDeleted { get; set; }
        
        [Display(Name = "IsActive", ResourceType = typeof(UT.SL.Model.Resource.App_User))]
        public System.Nullable<System.Boolean> SearchIsActive { get; set; }
        
        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.App_User))]
        public System.Nullable<System.DateTime> SearchCreateDate { get; set; }
        
        [Display(Name = "LastLogin", ResourceType = typeof(UT.SL.Model.Resource.App_User))]
        public System.Nullable<System.DateTime> SearchLastLogin { get; set; }
    }
}
