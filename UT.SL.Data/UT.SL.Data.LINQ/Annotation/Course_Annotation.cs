using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ
{


    [MetadataType(typeof(Course_Annotation))]
    public partial class Course
    {
    }

    public partial class Course_Annotation
    {

        public Course_Annotation() { }

        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.Course))]
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        [Display(Name = "Title", ResourceType = typeof(UT.SL.Model.Resource.Course))]
        [StringLength(500, ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "MaxLength500")]
        public string Title { get; set; }

        [Display(Name = "About", ResourceType = typeof(UT.SL.Model.Resource.Course))]
        public string About { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "StartDate", ResourceType = typeof(UT.SL.Model.Resource.Course))]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "EndDate", ResourceType = typeof(UT.SL.Model.Resource.Course))]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }
}
