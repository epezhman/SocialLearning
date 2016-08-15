using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class MessageThreadSearchModel : PagingItems {
        
        [Display(Name = "MessageCount", ResourceType = typeof(UT.SL.Model.Resource.MessageThread))]
        public System.Nullable<System.Int32> SearchMessageCount { get; set; }
        
        [Display(Name = "BeginningMessage", ResourceType = typeof(UT.SL.Model.Resource.MessageThread))]
        public System.Nullable<System.Int32> SearchBeginningMessage { get; set; }
        
        [Display(Name = "ParentMessageThread", ResourceType = typeof(UT.SL.Model.Resource.MessageThread))]
        public System.Nullable<System.Int32> SearchParentMessageThread { get; set; }
        
        [Display(Name = "Recipnients", ResourceType = typeof(UT.SL.Model.Resource.MessageThread))]
        public System.Nullable<System.Int32> SearchRecipnients { get; set; }
        
        [Display(Name = "Snippest", ResourceType = typeof(UT.SL.Model.Resource.MessageThread))]
        public string SearchSnippest { get; set; }
        
        [Display(Name = "Subject", ResourceType = typeof(UT.SL.Model.Resource.MessageThread))]
        public string SearchSubject { get; set; }
        
        [Display(Name = "LastUpdate", ResourceType = typeof(UT.SL.Model.Resource.MessageThread))]
        public System.Nullable<System.DateTime> SearchLastUpdate { get; set; }
        
        [Display(Name = "CreatDate", ResourceType = typeof(UT.SL.Model.Resource.MessageThread))]
        public System.Nullable<System.DateTime> SearchCreateDate { get; set; }
    }
}
