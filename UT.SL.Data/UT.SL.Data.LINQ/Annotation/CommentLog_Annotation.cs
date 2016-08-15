using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(CommentLog_Annotation))]
	public partial class CommentLog
	{
	}

	 public partial class CommentLog_Annotation {
        
        public CommentLog_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.CommentLog))]
        public int Id { get; set; }
        
        [Display(Name = "ChangeText", ResourceType = typeof(UT.SL.Model.Resource.CommentLog))]
        public string ChangeText { get; set; }
        
        [Display(Name = "ChangeDate", ResourceType = typeof(UT.SL.Model.Resource.CommentLog))]
        public System.Nullable<System.DateTime> ChangeDate { get; set; }
    }
}
