using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class GroupMemberSearchModel : PagingItems {
        
        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.GroupMember))]
        public System.Nullable<System.Int32> SearchType { get; set; }
        
        [Display(Name = "IsCircleAdmin", ResourceType = typeof(UT.SL.Model.Resource.GroupMember))]
        public System.Nullable<System.Int32> SearchIsCircleAdmin { get; set; }
    }
}
