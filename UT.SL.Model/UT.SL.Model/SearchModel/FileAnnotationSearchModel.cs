using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class FileAnnotationSearchModel : PagingItems {
        
        [Display(Name = "Text", ResourceType = typeof(UT.SL.Model.Resource.FileAnnotation))]
        public string SearchText { get; set; }
        
        [Display(Name = "ObjectType", ResourceType = typeof(UT.SL.Model.Resource.FileAnnotation))]
        public System.Nullable<System.Int32> SearchObjectType { get; set; }
        
        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.FileAnnotation))]
        public System.Nullable<System.DateTime> SearchCreateDate { get; set; }
        
        [Display(Name = "FromPage", ResourceType = typeof(UT.SL.Model.Resource.FileAnnotation))]
        public System.Nullable<System.Int32> SearchFromPage { get; set; }
        
        [Display(Name = "FromeLine", ResourceType = typeof(UT.SL.Model.Resource.FileAnnotation))]
        public System.Nullable<System.Int32> SearchFromeLine { get; set; }
        
        [Display(Name = "FromLetter", ResourceType = typeof(UT.SL.Model.Resource.FileAnnotation))]
        public System.Nullable<System.Int32> SearchFromLetter { get; set; }
        
        [Display(Name = "ToPage", ResourceType = typeof(UT.SL.Model.Resource.FileAnnotation))]
        public System.Nullable<System.Int32> SearchToPage { get; set; }
        
        [Display(Name = "ToLine", ResourceType = typeof(UT.SL.Model.Resource.FileAnnotation))]
        public System.Nullable<System.Int32> SearchToLine { get; set; }
        
        [Display(Name = "ToLetter", ResourceType = typeof(UT.SL.Model.Resource.FileAnnotation))]
        public System.Nullable<System.Int32> SearchToLetter { get; set; }
    }
}
