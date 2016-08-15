using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(CategoryMapper_Annotation))]
	public partial class CategoryMapper
	{
	}

	 public partial class CategoryMapper_Annotation {
        
        public CategoryMapper_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.CategoryMapper))]
        public int Id { get; set; }
    }
}
