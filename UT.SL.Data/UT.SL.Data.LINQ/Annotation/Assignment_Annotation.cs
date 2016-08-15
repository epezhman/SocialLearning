using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace UT.SL.Data.LINQ
{

    [MetadataType(typeof(Assignment_Annotation))]
    public partial class Assignment
    {
    }

    public partial class Assignment_Annotation
    {

        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.Assignment ))]
        public int Id { get; set; }

        [Display(Name = "Title", ResourceType = typeof(UT.SL.Model.Resource.Assignment))]
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public string Title { get; set; }

        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.Assignment))]
        public System.Nullable<int> Type { get; set; }

        [Display(Name = "CourseId", ResourceType = typeof(UT.SL.Model.Resource.Assignment))]
        public int CourseId { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.Assignment))]
        public System.Nullable<System.DateTime> CreateDate { get; set; }

        [Display(Name = "DueDate", ResourceType = typeof(UT.SL.Model.Resource.Assignment))]
        public System.Nullable<System.DateTime> DueDate { get; set; }

        [Display(Name = "Status", ResourceType = typeof(UT.SL.Model.Resource.Assignment))]
        public int  Status { get; set; }

        [Display(Name = "GradeFrom", ResourceType = typeof(UT.SL.Model.Resource.Assignment))]
        [Range(0, 100, ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "Range0To100")]
        public System.Nullable<int> GradeFrom { get; set; }

        [Display(Name = "GetLockedAfterExpiration", ResourceType = typeof(UT.SL.Model.Resource.Assignment))]
        public System.Nullable<System.Boolean> GetLockedAfterExpiration { get; set; }

        [Display(Name = "GroupSubmissionIsDemanded", ResourceType = typeof(UT.SL.Model.Resource.Assignment))]
        public System.Nullable<System.Boolean> GroupSubmissionIsDemanded { get; set; }

        [Display(Name = "Body", ResourceType = typeof(UT.SL.Model.Resource.Assignment))]
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public string Body { get; set; }



    }
}
