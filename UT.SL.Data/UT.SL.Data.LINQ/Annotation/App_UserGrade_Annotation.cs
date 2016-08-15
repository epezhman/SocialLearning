using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ
{


    [MetadataType(typeof(App_UserGrade_Annotation))]
    public partial class App_UserGrade
    {
    }

    public partial class App_UserGrade_Annotation
    {

        public App_UserGrade_Annotation() { }

        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.App_UserGrade))]
        public int Id { get; set; }

        [Display(Name = "Grade", ResourceType = typeof(UT.SL.Model.Resource.App_UserGrade))]
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public int Grade { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.App_UserGrade))]
        public System.Nullable<System.DateTime> CreateDate { get; set; }

        [Display(Name = "GuidId", ResourceType = typeof(UT.SL.Model.Resource.App_UserGrade))]
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public System.Guid GuidId { get; set; }
    }
}
