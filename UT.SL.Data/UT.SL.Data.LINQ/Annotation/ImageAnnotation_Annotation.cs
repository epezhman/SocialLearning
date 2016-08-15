using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(ImageAnnotation_Annotation))]
    public partial class ImageAnnotation
	{
	}

	 public partial class ImageAnnotation_Annotation {

         public ImageAnnotation_Annotation() { }

        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.ImageAnnotation))]
        public int Id { get; set; }

        [Display(Name = "TopCoord", ResourceType = typeof(UT.SL.Model.Resource.ImageAnnotation))]
        public System.Nullable<int> TopCoord { get; set; }

        [Display(Name = "LeftCoord", ResourceType = typeof(UT.SL.Model.Resource.ImageAnnotation))]
        public System.Nullable<int> LeftCoord { get; set; }

        [Display(Name = "Width", ResourceType = typeof(UT.SL.Model.Resource.ImageAnnotation))]
        public System.Nullable<int> Width { get; set; }

        [Display(Name = "Height", ResourceType = typeof(UT.SL.Model.Resource.ImageAnnotation))]
        public System.Nullable<int> Height { get; set; }
    }
}
