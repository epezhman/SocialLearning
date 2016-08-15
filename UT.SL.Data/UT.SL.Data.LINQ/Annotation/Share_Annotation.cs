using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(Share_Annotation))]
	public partial class Share
	{
	}

	 public partial class Share_Annotation {
        
        public Share_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.Share))]
        public int Id { get; set; }
        
        [Display(Name = "ObjectId", ResourceType = typeof(UT.SL.Model.Resource.Share))]
        [Required(ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="FieldIsRequired")]
        public int ObjectId { get; set; }
        
        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.Share))]
        public System.Nullable<System.DateTime> CreateDate { get; set; }
        
        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.Share))]
        public System.Nullable<int> Type { get; set; }
    }
}
