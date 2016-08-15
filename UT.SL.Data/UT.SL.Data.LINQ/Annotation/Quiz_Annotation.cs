using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(Quiz_Annotation))]
	public partial class Quiz
	{
	}

	 public partial class Quiz_Annotation {
        
        public Quiz_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.Quiz))]
        public int Id { get; set; }
        
        [Display(Name = "DueDate", ResourceType = typeof(UT.SL.Model.Resource.Quiz))]
        public System.Nullable<System.DateTime> DueDate { get; set; }
        
        [Display(Name = "Deadline", ResourceType = typeof(UT.SL.Model.Resource.Quiz))]
        public System.Nullable<System.DateTime> Deadline { get; set; }
        
        [Display(Name = "Duration", ResourceType = typeof(UT.SL.Model.Resource.Quiz))]
        public System.Nullable<int> Duration { get; set; }
        
        [Display(Name = "EffectiveScore", ResourceType = typeof(UT.SL.Model.Resource.Quiz))]
        public System.Nullable<bool> EffectiveScore { get; set; }
        
        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.Quiz))]
        public System.Nullable<int> Type { get; set; }
        
        [Display(Name = "GuidId", ResourceType = typeof(UT.SL.Model.Resource.Quiz))]
        [Required(ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="FieldIsRequired")]
        public System.Guid GuidId { get; set; }
        
        [Display(Name = "TopicId", ResourceType = typeof(UT.SL.Model.Resource.Quiz))]
        public System.Nullable<int> TopicId { get; set; }
    }
}
