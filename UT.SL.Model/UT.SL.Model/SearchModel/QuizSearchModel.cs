using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class QuizSearchModel : PagingItems {
        
        [Display(Name = "DueDate", ResourceType = typeof(UT.SL.Model.Resource.Quiz))]
        public System.Nullable<System.DateTime> SearchDueDate { get; set; }
        
        [Display(Name = "Deadline", ResourceType = typeof(UT.SL.Model.Resource.Quiz))]
        public System.Nullable<System.DateTime> SearchDeadline { get; set; }
        
        [Display(Name = "Duration", ResourceType = typeof(UT.SL.Model.Resource.Quiz))]
        public System.Nullable<System.Int32> SearchDuration { get; set; }
        
        [Display(Name = "EffectiveScore", ResourceType = typeof(UT.SL.Model.Resource.Quiz))]
        public System.Nullable<System.Boolean> SearchEffectiveScore { get; set; }
        
        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.Quiz))]
        public System.Nullable<System.Int32> SearchType { get; set; }
    }
}
