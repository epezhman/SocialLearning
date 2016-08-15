using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(Vote_Annotation))]
	public partial class Vote
	{
	}

	 public partial class Vote_Annotation {
        
        public Vote_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.Vote))]
        public int Id { get; set; }
        
        [Display(Name = "Updown", ResourceType = typeof(UT.SL.Model.Resource.Vote))]
        [Required(ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="FieldIsRequired")]
        public bool Updown { get; set; }
        
        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.Vote))]
        public System.Nullable<System.DateTime> CreateDate { get; set; }
    }
}
