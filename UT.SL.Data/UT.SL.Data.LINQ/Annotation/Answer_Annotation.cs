using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(Answer_Annotation))]
	public partial class Answer
	{
	}

	 public partial class Answer_Annotation {
        
        public Answer_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.Answer))]
        public int Id { get; set; }
        
        [Display(Name = "Title", ResourceType = typeof(UT.SL.Model.Resource.Answer))]
        [StringLength(-1, ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="MaxLength-1")]
        public string Title { get; set; }
        
        [Display(Name = "CorrectAnsver", ResourceType = typeof(UT.SL.Model.Resource.Answer))]
        public string CorrectAnsver { get; set; }
        
        [Display(Name = "IsEffective", ResourceType = typeof(UT.SL.Model.Resource.Answer))]
        public System.Nullable<bool> IsEffective { get; set; }
        
        [Display(Name = "GuidId", ResourceType = typeof(UT.SL.Model.Resource.Answer))]
        [Required(ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="FieldIsRequired")]
        public System.Guid GuidId { get; set; }
    }
}
