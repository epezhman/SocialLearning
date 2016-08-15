using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(App_UserLog_Annotation))]
	public partial class App_UserLog
	{
	}

	 public partial class App_UserLog_Annotation {
        
        public App_UserLog_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.App_UserLog))]
        public int Id { get; set; }
        
        [Display(Name = "About", ResourceType = typeof(UT.SL.Model.Resource.App_UserLog))]
        public string About { get; set; }
        
        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.App_UserLog))]
        public System.Nullable<int> Type { get; set; }
        
        [Display(Name = "CreatDate", ResourceType = typeof(UT.SL.Model.Resource.App_UserLog))]
        public System.Nullable<System.DateTime> CreatDate { get; set; }
    }
}
