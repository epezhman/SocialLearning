using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {
    
    
    public class TagMapperSearchModel : PagingItems {
        
        [Display(Name = "ObjectType", ResourceType = typeof(UT.SL.Model.Resource.TagMapper))]
        public System.Nullable<System.Int32> SearchObjectType { get; set; }
        
        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.TagMapper))]
        public System.Nullable<System.DateTime> SearchCreateDate { get; set; }
    }
}
