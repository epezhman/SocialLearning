using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace UT.SL.Data.LINQ.Annotation
{
    [MetadataType(typeof(GradeType_Annotation))]
    public partial class GradeTypeAnnotation
    {
    }
    public partial class GradeType_Annotation
    {

        public GradeType_Annotation() { }

        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.GradeType))]
        public int Id { get; set; }

        [Display(Name = "CourseId", ResourceType = typeof(UT.SL.Model.Resource.GradeType))]
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public int CourseId { get; set; }

        [Display(Name = "MinGrade", ResourceType = typeof(UT.SL.Model.Resource.GradeType))]
        public System.Nullable<double> MinGrade { get; set; }

        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.GradeType))]
        public System.Nullable<int> Type { get; set; }

        [Display(Name = "MaxGrade", ResourceType = typeof(UT.SL.Model.Resource.GradeType))]
        public System.Nullable<double> MaxGrade { get; set; }
    }
}
