using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class AnswerSearchModel : PagingItems {
        
        [Display(Name = "Title", ResourceType = typeof(UT.SL.Model.Resource.Answer))]
        public string SearchTitle { get; set; }
        
        [Display(Name = "CorrectAnsver", ResourceType = typeof(UT.SL.Model.Resource.Answer))]
        public string SearchCorrectAnsver { get; set; }
        
        [Display(Name = "IsEffective", ResourceType = typeof(UT.SL.Model.Resource.Answer))]
        public System.Nullable<System.Boolean> SearchIsEffective { get; set; }
    }
}
