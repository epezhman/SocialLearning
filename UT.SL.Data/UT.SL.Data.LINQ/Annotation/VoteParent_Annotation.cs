using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(VoteParent_Annotation))]
	public partial class VoteParent
	{
	}

	 public partial class VoteParent_Annotation {
        
        public VoteParent_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.VoteParent))]
        public int Id { get; set; }
        
        [Display(Name = "ObjectId", ResourceType = typeof(UT.SL.Model.Resource.VoteParent))]
        [Required(ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="FieldIsRequired")]
        public int ObjectId { get; set; }
        
        [Display(Name = "ObjectType", ResourceType = typeof(UT.SL.Model.Resource.VoteParent))]
        public System.Nullable<int> ObjectType { get; set; }
        
        [Display(Name = "UpvoteCount", ResourceType = typeof(UT.SL.Model.Resource.VoteParent))]
        [Required(ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="FieldIsRequired")]
        public int UpvoteCount { get; set; }
        
        [Display(Name = "DownvoteCount", ResourceType = typeof(UT.SL.Model.Resource.VoteParent))]
        [Required(ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="FieldIsRequired")]
        public int DownvoteCount { get; set; }
        
        [Display(Name = "Count", ResourceType = typeof(UT.SL.Model.Resource.VoteParent))]
        [Required(ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="FieldIsRequired")]
        public int Count { get; set; }
    }
}
