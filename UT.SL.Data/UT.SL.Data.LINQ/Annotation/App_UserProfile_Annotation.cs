using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(App_UserProfile_Annotation))]
	public partial class App_UserProfile
	{
	}

	 public partial class App_UserProfile_Annotation {
        
        public App_UserProfile_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.App_UserProfile))]
        public int Id { get; set; }
    }
}
