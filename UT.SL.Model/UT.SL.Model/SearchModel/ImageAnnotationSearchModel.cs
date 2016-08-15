using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {


    public class ImageAnnotationSearchModel : PagingItems
    {

        [Display(Name = "TopCoord", ResourceType = typeof(UT.SL.Model.Resource.ImageAnnotation))]
        public System.Nullable<int> SearchTopCoord { get; set; }

        [Display(Name = "LeftCoord", ResourceType = typeof(UT.SL.Model.Resource.ImageAnnotation))]
        public System.Nullable<int> SearchLeftCoord { get; set; }

        [Display(Name = "Width", ResourceType = typeof(UT.SL.Model.Resource.ImageAnnotation))]
        public System.Nullable<int> SearchWidth { get; set; }

        [Display(Name = "Height", ResourceType = typeof(UT.SL.Model.Resource.ImageAnnotation))]
        public System.Nullable<int> SearchHeight { get; set; }
    }
}
