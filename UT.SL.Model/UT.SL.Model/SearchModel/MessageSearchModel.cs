using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class MessageSearchModel : PagingItems {
        
        [Display(Name = "Subject", ResourceType = typeof(UT.SL.Model.Resource.Message))]
        public string SearchSubject { get; set; }
        
        [Display(Name = "Body", ResourceType = typeof(UT.SL.Model.Resource.Message))]
        public string SearchBody { get; set; }
        
        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.Message))]
        public System.Nullable<System.DateTime> SearchCreateDate { get; set; }
        
        [Display(Name = "Attachement", ResourceType = typeof(UT.SL.Model.Resource.Message))]
        public System.Byte[] SearchAttachement { get; set; }
    }
}
