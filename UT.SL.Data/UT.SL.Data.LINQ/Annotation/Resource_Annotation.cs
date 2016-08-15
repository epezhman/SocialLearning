using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(Resource_Annotation))]
	public partial class Resource
	{
	}

	 public partial class Resource_Annotation {
        
        public Resource_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.Resource))]
        public int Id { get; set; }
        
        [Display(Name = "Title", ResourceType = typeof(UT.SL.Model.Resource.Resource))]
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public string Title { get; set; }
        
        [Display(Name = "Body", ResourceType = typeof(UT.SL.Model.Resource.Resource))]
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public string Body { get; set; }
        
        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.Resource))]
        public System.Nullable<int> Type { get; set; }
        
        [Display(Name = "FileContent", ResourceType = typeof(UT.SL.Model.Resource.Resource))]
        public byte[] FileContent { get; set; }
        
        [Display(Name = "GuidId", ResourceType = typeof(UT.SL.Model.Resource.Resource))]
        [Required(ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="FieldIsRequired")]
        public System.Guid GuidId { get; set; }
    }
}
