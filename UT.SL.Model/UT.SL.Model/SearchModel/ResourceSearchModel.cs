using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class ResourceSearchModel : PagingItems {
        
        [Display(Name = "Title", ResourceType = typeof(UT.SL.Model.Resource.Resource))]
        public string SearchTitle { get; set; }
        
        [Display(Name = "Body", ResourceType = typeof(UT.SL.Model.Resource.Resource))]
        public string SearchBody { get; set; }
        
        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.Resource))]
        public System.Nullable<System.Int32> SearchType { get; set; }
        
        [Display(Name = "FileContent", ResourceType = typeof(UT.SL.Model.Resource.Resource))]
        public System.Byte[] SearchFileContent { get; set; }

        public int? CourseId { get; set; }
    }
}
