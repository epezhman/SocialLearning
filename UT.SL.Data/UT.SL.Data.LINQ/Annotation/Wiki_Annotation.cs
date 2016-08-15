using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(Wiki_Annotation))]
	public partial class Wiki
	{
	}

	 public partial class Wiki_Annotation {
        
        public Wiki_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.Wiki))]
        public int Id { get; set; }
        
        [Display(Name = "HyperLink", ResourceType = typeof(UT.SL.Model.Resource.Wiki))]
        [StringLength(500, ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="MaxLength500")]
        public string HyperLink { get; set; }
        
        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.Wiki))]
        [Required(ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="FieldIsRequired")]
        public int Type { get; set; }
        
        [Display(Name = "Text", ResourceType = typeof(UT.SL.Model.Resource.Wiki))]
        public string Text { get; set; }
        
        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.Wiki))]
        public System.Nullable<System.DateTime> CreateDate { get; set; }
    }
}
