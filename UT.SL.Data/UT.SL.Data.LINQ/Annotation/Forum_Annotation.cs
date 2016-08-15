using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UT.SL.Data.LINQ
{


    [MetadataType(typeof(Forum_Annotation))]
    public partial class Forum
    {
    }

    public partial class Forum_Annotation
    {

        public Forum_Annotation() { }

        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.Forum  ))]
        public int Id { get; set; }

        [Display(Name = "Title", ResourceType = typeof(UT.SL.Model.Resource.Forum))]
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public string Title { get; set; }

        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.Forum))]
        public System.Nullable<int> Type { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.Forum))]
        public System.Nullable<System.DateTime> CreateDate { get; set; }

        [Display(Name = "CourseId", ResourceType = typeof(UT.SL.Model.Resource.Forum))]
        public int CourseId { get; set; }

        [Display(Name = "Body", ResourceType = typeof(UT.SL.Model.Resource.Forum))]
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public string Body { get; set; }

        [Display(Name = "DueDate", ResourceType = typeof(UT.SL.Model.Resource.Forum))]
        public System.Nullable<System.DateTime> DueDate { get; set; }

        [Display(Name = "GetLockedAfterExpiration", ResourceType = typeof(UT.SL.Model.Resource.Forum))]
        public System.Nullable<System.Boolean> GetLockedAfterExpiration { get; set; }

        [Display(Name = "GradeFrom", ResourceType = typeof(UT.SL.Model.Resource.Forum))]
        public System.Nullable<int> GradeFrom { get; set; }
    }
}
