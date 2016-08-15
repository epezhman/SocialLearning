using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(LearningGroup_Annotation))]
	public partial class LearningGroup
	{
	}

	 public partial class LearningGroup_Annotation {
        
        public LearningGroup_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.LearningGroup))]
        public int Id { get; set; }
        
        [Display(Name = "Title", ResourceType = typeof(UT.SL.Model.Resource.LearningGroup))]
        public string Title { get; set; }
        
        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.LearningGroup))]
        public System.Nullable<int> Type { get; set; }
                       
        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.LearningGroup))]
        public System.Nullable<System.DateTime> CreateDate { get; set; }
    }
}
