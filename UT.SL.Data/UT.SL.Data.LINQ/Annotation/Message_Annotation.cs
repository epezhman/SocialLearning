using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(Message_Annotation))]
	public partial class Message
	{
	}

	 public partial class Message_Annotation {
        
        public Message_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.Message))]
        public int Id { get; set; }
        
        [Display(Name = "Subject", ResourceType = typeof(UT.SL.Model.Resource.Message))]
        public string Subject { get; set; }
        
        [Display(Name = "Body", ResourceType = typeof(UT.SL.Model.Resource.Message))]
        public string Body { get; set; }
        
        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.Message))]
        public System.Nullable<System.DateTime> CreateDate { get; set; }
        
        [Display(Name = "GuidId", ResourceType = typeof(UT.SL.Model.Resource.Message))]
        [Required(ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="FieldIsRequired")]
        public System.Guid GuidId { get; set; }
    }
}
