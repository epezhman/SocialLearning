using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(App_User_Annotation))]
	public partial class App_User
	{
	}

	 public partial class App_User_Annotation {
        
        public App_User_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.App_User))]
        public int Id { get; set; }
        
        [Display(Name = "UserName", ResourceType = typeof(UT.SL.Model.Resource.App_User))]
        [StringLength(200, ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="MaxLength200")]
        [Required(ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="FieldIsRequired")]
        public string UserName { get; set; }
        
        [Display(Name = "FirstName", ResourceType = typeof(UT.SL.Model.Resource.App_User))]
        [StringLength(200, ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="MaxLength200")]
        public string FirstName { get; set; }
        
        [Display(Name = "LastName", ResourceType = typeof(UT.SL.Model.Resource.App_User))]
        [StringLength(200, ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="MaxLength200")]
        public string LastName { get; set; }
        
        [Display(Name = "Password", ResourceType = typeof(UT.SL.Model.Resource.App_User))]
        [StringLength(200, ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="MaxLength200")]
        [Required(ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="FieldIsRequired")]
        public string Password { get; set; }
        
        [Display(Name = "Email", ResourceType = typeof(UT.SL.Model.Resource.App_User))]
        [StringLength(200, ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="MaxLength200")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9-]+)*\\.([a-z]{2,4})$", ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="EmailFormatError")]
        public string Email { get; set; }
        
        [Display(Name = "IsAdmin", ResourceType = typeof(UT.SL.Model.Resource.App_User))]
        [Required(ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="FieldIsRequired")]
        public bool IsAdmin { get; set; }
        
        [Display(Name = "IsDeleted", ResourceType = typeof(UT.SL.Model.Resource.App_User))]
        [Required(ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="FieldIsRequired")]
        public bool IsDeleted { get; set; }
        
        [Display(Name = "IsActive", ResourceType = typeof(UT.SL.Model.Resource.App_User))]
        [Required(ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="FieldIsRequired")]
        public bool IsActive { get; set; }
        
        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.App_User))]
        public System.Nullable<System.DateTime> CreateDate { get; set; }
        
        [Display(Name = "LastLogin", ResourceType = typeof(UT.SL.Model.Resource.App_User))]
        public System.Nullable<System.DateTime> LastLogin { get; set; }
        
        [Display(Name = "GuidId", ResourceType = typeof(UT.SL.Model.Resource.App_User))]
        [Required(ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="FieldIsRequired")]
        public System.Guid GuidId { get; set; }

        [Display(Name = "UserPic", ResourceType = typeof(UT.SL.Model.Resource.App_User))]
        public System.Data.Linq.Binary UserPic { get; set; }
    }
}
