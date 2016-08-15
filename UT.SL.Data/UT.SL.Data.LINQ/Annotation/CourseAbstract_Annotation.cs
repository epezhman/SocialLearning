using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ
{


    [MetadataType(typeof(CourseAbstract_Annotation))]
    public partial class CourseAbstract
    {
    }

    public partial class CourseAbstract_Annotation
    {

        public CourseAbstract_Annotation() { }

        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.Course))]
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        [Display(Name = "Title", ResourceType = typeof(UT.SL.Model.Resource.Course))]
        [StringLength(500, ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "MaxLength500")]
        public string Title { get; set; }

        [Display(Name = "About", ResourceType = typeof(UT.SL.Model.Resource.Course))]
        public string About { get; set; }
        
    }
}
