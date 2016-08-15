using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class App_UserGradeSearchModel : PagingItems {
        
        [Display(Name = "Grade", ResourceType = typeof(UT.SL.Model.Resource.App_UserGrade))]
        public System.Nullable<System.Int32> SearchGrade { get; set; }
        
        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.App_UserGrade))]
        public System.Nullable<System.DateTime> SearchCreateDate { get; set; }
    }
}
