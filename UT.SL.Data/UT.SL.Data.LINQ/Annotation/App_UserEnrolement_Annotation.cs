using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(App_UserEnrolement_Annotation))]
	public partial class App_UserEnrolement
	{
	}

	 public partial class App_UserEnrolement_Annotation {
        
        public App_UserEnrolement_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.App_UserEnrolements))]
        public int Id { get; set; }
        
        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.App_UserEnrolements))]
        public System.Nullable<int> Type { get; set; }
        
        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.App_UserEnrolements))]
        public System.Nullable<int> CreateDate { get; set; }
        
        [Display(Name = "Status", ResourceType = typeof(UT.SL.Model.Resource.App_UserEnrolements))]
        public System.Nullable<int> Status { get; set; }
    }
}
