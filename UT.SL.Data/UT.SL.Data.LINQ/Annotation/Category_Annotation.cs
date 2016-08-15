using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ
{


    [MetadataType(typeof(Category_Annotation))]
    public partial class Category
    {
    }

    public partial class Category_Annotation
    {

        public Category_Annotation() { }

        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.Category))]
        public int Id { get; set; }

        [Display(Name = "Title", ResourceType = typeof(UT.SL.Model.Resource.Category))]
        [StringLength(500, ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "MaxLength500")]
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public string Title { get; set; }

        [Display(Name = "ParentId", ResourceType = typeof(UT.SL.Model.Resource.Category))]
        public int? ParentId { get; set; }
    }
}
