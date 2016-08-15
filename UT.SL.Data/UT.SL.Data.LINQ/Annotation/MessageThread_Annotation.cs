using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(MessageThread_Annotation))]
	public partial class MessageThread
	{
	}

	 public partial class MessageThread_Annotation {
        
        public MessageThread_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.MessageThread))]
        public int Id { get; set; }
        
        [Display(Name = "FolderId", ResourceType = typeof(UT.SL.Model.Resource.MessageThread))]
        public System.Nullable<int> FolderId { get; set; }
        
        [Display(Name = "MessageCount", ResourceType = typeof(UT.SL.Model.Resource.MessageThread))]
        [Required(ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="FieldIsRequired")]
        public int MessageCount { get; set; }
        
        [Display(Name = "BeginningMessage", ResourceType = typeof(UT.SL.Model.Resource.MessageThread))]
        public System.Nullable<int> BeginningMessage { get; set; }
        
        [Display(Name = "Snippest", ResourceType = typeof(UT.SL.Model.Resource.MessageThread))]
        public string Snippest { get; set; }
        
        [Display(Name = "Subject", ResourceType = typeof(UT.SL.Model.Resource.MessageThread))]
        public string Subject { get; set; }
        
        [Display(Name = "LastUpdate", ResourceType = typeof(UT.SL.Model.Resource.MessageThread))]
        public System.Nullable<System.DateTime> LastUpdate { get; set; }
        
        [Display(Name = "CreatDate", ResourceType = typeof(UT.SL.Model.Resource.MessageThread))]
        public System.Nullable<System.DateTime> CreatDate { get; set; }
        
        [Display(Name = "GuidId", ResourceType = typeof(UT.SL.Model.Resource.MessageThread))]
        [Required(ErrorMessageResourceType= typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName="FieldIsRequired")]
        public System.Guid GuidId { get; set; }
    }
}
