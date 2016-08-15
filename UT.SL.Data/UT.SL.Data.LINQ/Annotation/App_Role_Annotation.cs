using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(App_Role_Annotation))]
	public partial class App_Role
	{
	}

	 public partial class App_Role_Annotation {
        
        public App_Role_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.App_Role))]
        public int Id { get; set; }
        
        [Display(Name = "Title", ResourceType = typeof(UT.SL.Model.Resource.App_Role))]
        [StringLength(200, ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="MaxLength200")]
        [Required(ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="FieldIsRequired")]
        public string Title { get; set; }
        
        [Display(Name = "Description", ResourceType = typeof(UT.SL.Model.Resource.App_Role))]
        public string Description { get; set; }
    }
}
