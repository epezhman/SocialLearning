using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace UT.SL.Data.LINQ
{
    [MetadataType(typeof(Grade_Annotation))]
    public partial class Grade
    {
    }
    public partial class Grade_Annotation
    {

        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.Grade))]
        public int Id { get; set; }

        [Display(Name = "Body", ResourceType = typeof(UT.SL.Model.Resource.Grade))]
        public string Body { get; set; }

        [Display(Name = "ObjectId", ResourceType = typeof(UT.SL.Model.Resource.Grade))]
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public int ObjectId { get; set; }

        [Display(Name = "ObjectType", ResourceType = typeof(UT.SL.Model.Resource.Grade))]
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public int ObjectType { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.Grade))]
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "UserId", ResourceType = typeof(UT.SL.Model.Resource.Grade))]
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public int UserId { get; set; }

        [Display(Name = "GradeValue", ResourceType = typeof(UT.SL.Model.Resource.Grade))]
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        //[Range( 0, 100, ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "Range0To100")]
        [Range(typeof(double), "0", "100", ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "Range0To100")]
        public float GradeValue { get; set; }

    }
}
