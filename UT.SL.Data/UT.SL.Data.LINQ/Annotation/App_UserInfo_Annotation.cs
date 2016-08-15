using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(App_UserInfo_Annotation))]
	public partial class App_UserInfo
	{
	}

	 public partial class App_UserInfo_Annotation {
        
        public App_UserInfo_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.App_UserInfo))]
        public int Id { get; set; }             
        
        [Display(Name = "NationalId", ResourceType = typeof(UT.SL.Model.Resource.App_UserInfo))]
        [StringLength(10, ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="MaxLength10")]
        public string NationalId { get; set; }
        
        [Display(Name = "BirsthDate", ResourceType = typeof(UT.SL.Model.Resource.App_UserInfo))]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public System.Nullable<System.DateTime> BirsthDate { get; set; }

        [Display(Name = "About", ResourceType = typeof(UT.SL.Model.Resource.App_UserInfo))]
        public int About { get; set; }

        [Display(Name = "BSin", ResourceType = typeof(UT.SL.Model.Resource.App_UserInfo))]
        public int BSin { get; set; }

        [Display(Name = "MSin", ResourceType = typeof(UT.SL.Model.Resource.App_UserInfo))]
        public int MSin { get; set; }

        [Display(Name = "PHDin", ResourceType = typeof(UT.SL.Model.Resource.App_UserInfo))]
        public int PHDin { get; set; }

        [Display(Name = "Occupation", ResourceType = typeof(UT.SL.Model.Resource.App_UserInfo))]
        public int Occupation { get; set; }

        [Display(Name = "Title", ResourceType = typeof(UT.SL.Model.Resource.App_UserInfo))]
        public int Title { get; set; }
    }
}
