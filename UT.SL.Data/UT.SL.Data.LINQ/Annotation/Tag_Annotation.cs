using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(Tag_Annotation))]
	public partial class Tag
	{
	}

	 public partial class Tag_Annotation {
        
        public Tag_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.Tag))]
        public int Id { get; set; }
        
        [Display(Name = "Titile", ResourceType = typeof(UT.SL.Model.Resource.Tag))]
        public string Title { get; set; }
        
        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.Tag))]
        public System.Nullable<int> Type { get; set; }
        
        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.Tag))]
        public System.Nullable<System.DateTime> CreateDate { get; set; }

        [Display(Name = "IsValid", ResourceType = typeof(UT.SL.Model.Resource.Tag))]
        public int IsValid { get; set; }
    }
}
