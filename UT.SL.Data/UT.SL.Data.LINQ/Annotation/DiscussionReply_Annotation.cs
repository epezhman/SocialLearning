using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(ForumDiscussionPost_Annotation))]
	public partial class ForumDiscussionPost
	{
	}

	 public partial class ForumDiscussionPost_Annotation {
        
        public ForumDiscussionPost_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.ForumDiscussionPost))]
        public int Id { get; set; }
        
        [Display(Name = "Text", ResourceType = typeof(UT.SL.Model.Resource.ForumDiscussionPost))]
        public string Text { get; set; }
        
        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.ForumDiscussionPost))]
        public System.Nullable<System.DateTime> CreateDate { get; set; }
    }
}
