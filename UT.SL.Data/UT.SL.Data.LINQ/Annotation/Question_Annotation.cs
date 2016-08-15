using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(Question_Annotation))]
	public partial class Question
	{
	}

	 public partial class Question_Annotation {
        
        public Question_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.Questions))]
        public int Id { get; set; }
        
        [Display(Name = "Title", ResourceType = typeof(UT.SL.Model.Resource.Questions))]
        public string Title { get; set; }
        
        [Display(Name = "Body", ResourceType = typeof(UT.SL.Model.Resource.Questions))]
        public string Body { get; set; }
        
        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.Questions))]
        public System.Nullable<System.DateTime> CreateDate { get; set; }
        
        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.Questions))]
        public System.Nullable<int> Type { get; set; }
    }
}
