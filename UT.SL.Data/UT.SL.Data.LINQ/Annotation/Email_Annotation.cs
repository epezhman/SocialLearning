using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(Eamil_Annotation))]
	public partial class Email
	{
	}

	 public partial class Eamil_Annotation {

         public Eamil_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.Email))]
        public int Id { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.Email))]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "Statuse", ResourceType = typeof(UT.SL.Model.Resource.Email))]
        public byte? Statuse { get; set; }

        [Display(Name = "Subject", ResourceType = typeof(UT.SL.Model.Resource.Email))]
        [StringLength(500, ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "MaxLength500")]
        public string Subject { get; set; }

        [Display(Name = "Body", ResourceType = typeof(UT.SL.Model.Resource.Email))]
        public string Body { get; set; }

        [Display(Name = "SenderEmail", ResourceType = typeof(UT.SL.Model.Resource.Email))]
        [StringLength(500, ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "MaxLength500")]
        public string SenderEmail { get; set; }

        [Display(Name = "ReceiverEmail", ResourceType = typeof(UT.SL.Model.Resource.Email))]
        [StringLength(500, ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "MaxLength500")]
        public string ReceiverEmail { get; set; }

        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.Email))]
        public byte? Type { get; set; }
       
    }
}
