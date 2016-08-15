using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(Comment_Annotation))]
	public partial class Comment
	{
	}

	 public partial class Comment_Annotation {
        
        public Comment_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.Comment))]
        public int Id { get; set; }
        
        [Display(Name = "Title", ResourceType = typeof(UT.SL.Model.Resource.Comment))]
        public string Title { get; set; }
        
        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.Comment))]
        public System.Nullable<System.DateTime> CreateDate { get; set; }
    }
}
