using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class CourseSearchModel : PagingItems {
        
        [Display(Name = "Title", ResourceType = typeof(UT.SL.Model.Resource.Course))]
        public string SearchTitle { get; set; }
        
        [Display(Name = "About", ResourceType = typeof(UT.SL.Model.Resource.Course))]
        public string SearchAbout { get; set; }

        public int? CourseId { get; set; }
    }
}
