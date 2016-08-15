using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(App_UserInRole_Annotation))]
	public partial class App_UserInRole
	{
	}

	 public partial class App_UserInRole_Annotation {
        
        public App_UserInRole_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.App_UserInRole))]
        public int Id { get; set; }
        
        [Display(Name = "CreatDate", ResourceType = typeof(UT.SL.Model.Resource.App_UserInRole))]
        public System.Nullable<System.DateTime> CreatDate { get; set; }
        
        [Display(Name = "GuidId", ResourceType = typeof(UT.SL.Model.Resource.App_UserInRole))]
        [Required(ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="FieldIsRequired")]
        public System.Guid GuidId { get; set; }
    }
}
