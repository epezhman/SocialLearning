using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(ForumDiscussion_Annotation))]
	public partial class ForumDiscussion
	{
	}

	 public partial class ForumDiscussion_Annotation {
        
        public ForumDiscussion_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.ForumDiscussion))]
        public int Id { get; set; }

        [Display(Name = "Title", ResourceType = typeof(UT.SL.Model.Resource.ForumDiscussion))]
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public string Title { get; set; }
        
        [Display(Name = "Body", ResourceType = typeof(UT.SL.Model.Resource.ForumDiscussion))]
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public string Body { get; set; }
        
        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.ForumDiscussion))]
        public System.Nullable<System.DateTime> CreateDate { get; set; }
        
        [Display(Name = "ViewCount", ResourceType = typeof(UT.SL.Model.Resource.ForumDiscussion))]
        public int ViewCount { get; set; }
    }
}
