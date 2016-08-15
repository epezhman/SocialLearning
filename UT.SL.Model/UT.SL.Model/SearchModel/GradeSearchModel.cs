using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {


    public class GradeTypeSearchModel : PagingItems
    {
        
        [Display(Name = "MinGrade", ResourceType = typeof(UT.SL.Model.Resource.GradeType))]
        public System.Nullable<System.Double> SearchMinGrade { get; set; }
        
        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.GradeType))]
        public System.Nullable<System.Int32> SearchType { get; set; }
        
        [Display(Name = "MaxGrade", ResourceType = typeof(UT.SL.Model.Resource.GradeType))]
        public System.Nullable<System.Double> SearchMaxGrade { get; set; }
    }
}
