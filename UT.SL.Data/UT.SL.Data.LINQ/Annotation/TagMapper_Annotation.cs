using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(TagMapper_Annotation))]
	public partial class TagMapper
	{
	}

	 public partial class TagMapper_Annotation {
        
        public TagMapper_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.TagMapper))]
        public int Id { get; set; }
        
        [Display(Name = "ObjectId", ResourceType = typeof(UT.SL.Model.Resource.TagMapper))]
        [Required(ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="FieldIsRequired")]
        public int ObjectId { get; set; }
        
        [Display(Name = "ObjectType", ResourceType = typeof(UT.SL.Model.Resource.TagMapper))]
        [Required(ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="FieldIsRequired")]
        public int ObjectType { get; set; }
        
        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.TagMapper))]
        public System.Nullable<System.DateTime> CreateDate { get; set; }
    }
}
