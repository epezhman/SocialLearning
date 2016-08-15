using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class VoteSearchModel : PagingItems {
        
        [Display(Name = "Updown", ResourceType = typeof(UT.SL.Model.Resource.Vote))]
        public System.Nullable<System.Boolean> SearchUpdown { get; set; }
        
        [Display(Name = "CreatDate", ResourceType = typeof(UT.SL.Model.Resource.Vote))]
        public System.Nullable<System.DateTime> SearchCreateDate { get; set; }
    }
}
