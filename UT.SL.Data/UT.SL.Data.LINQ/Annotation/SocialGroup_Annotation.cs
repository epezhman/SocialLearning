using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ
{


    [MetadataType(typeof(SocialGroup_Annotation))]
    public partial class SocialGroup
    {
    }

    public partial class SocialGroup_Annotation
    {

        public SocialGroup_Annotation() { }

        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.SocialGroup))]
        public int Id { get; set; }

        [Display(Name = "Title", ResourceType = typeof(UT.SL.Model.Resource.SocialGroup))]
        [StringLength(500, ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "MaxLength500")]
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public string Title { get; set; }

        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.SocialGroup))]
        public System.Nullable<int> Type { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.SocialGroup))]
        public System.Nullable<System.DateTime> CreateDate { get; set; }

        [Display(Name = "About", ResourceType = typeof(UT.SL.Model.Resource.SocialGroup))]
        public string About { get; set; }
    }
}
