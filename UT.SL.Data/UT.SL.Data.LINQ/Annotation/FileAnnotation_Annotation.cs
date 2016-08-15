using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(FileAnnotation_Annotation))]
	public partial class FileAnnotation
	{
	}

	 public partial class FileAnnotation_Annotation {
        
        public FileAnnotation_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.FileAnnotation))]
        public int Id { get; set; }
        
        [Display(Name = "Text", ResourceType = typeof(UT.SL.Model.Resource.FileAnnotation))]
        public string Text { get; set; }
        
        [Display(Name = "ObjectId", ResourceType = typeof(UT.SL.Model.Resource.FileAnnotation))]
        [Required(ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="FieldIsRequired")]
        public int ObjectId { get; set; }
        
        [Display(Name = "ObjectType", ResourceType = typeof(UT.SL.Model.Resource.FileAnnotation))]
        public System.Nullable<int> ObjectType { get; set; }
        
        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.FileAnnotation))]
        public System.Nullable<System.DateTime> CreateDate { get; set; }
        
        [Display(Name = "FromPage", ResourceType = typeof(UT.SL.Model.Resource.FileAnnotation))]
        public System.Nullable<int> FromPage { get; set; }
        
        [Display(Name = "FromeLine", ResourceType = typeof(UT.SL.Model.Resource.FileAnnotation))]
        public System.Nullable<int> FromeLine { get; set; }
        
        [Display(Name = "FromLetter", ResourceType = typeof(UT.SL.Model.Resource.FileAnnotation))]
        public System.Nullable<int> FromLetter { get; set; }
        
        [Display(Name = "ToPage", ResourceType = typeof(UT.SL.Model.Resource.FileAnnotation))]
        public System.Nullable<int> ToPage { get; set; }
        
        [Display(Name = "ToLine", ResourceType = typeof(UT.SL.Model.Resource.FileAnnotation))]
        public System.Nullable<int> ToLine { get; set; }
        
        [Display(Name = "ToLetter", ResourceType = typeof(UT.SL.Model.Resource.FileAnnotation))]
        public System.Nullable<int> ToLetter { get; set; }
    }
}
