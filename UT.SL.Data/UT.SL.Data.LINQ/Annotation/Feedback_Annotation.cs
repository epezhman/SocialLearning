using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {


    [MetadataType(typeof(Feedback_Annotation))]
    public partial class Feedback
	{
	}

	 public partial class Feedback_Annotation {

         public Feedback_Annotation() { }

        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.Feedback))]
        public int Id { get; set; }

        [Display(Name = "PageURL", ResourceType = typeof(UT.SL.Model.Resource.Feedback))]
        [StringLength(500, ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "MaxLength500")]
        public string PageURL { get; set; }

        [Display(Name = "Feedback1", ResourceType = typeof(UT.SL.Model.Resource.Feedback))]
        public string Feedback1 { get; set; }
        
    }
}
