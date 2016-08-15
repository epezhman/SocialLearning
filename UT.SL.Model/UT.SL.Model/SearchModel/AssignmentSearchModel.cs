using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using UT.SL.Helper;

namespace UT.SL.Model
{


    public class AssignmentSearchModel : PagingItems
    {

        [Display(Name = "Title", ResourceType = typeof(UT.SL.Model.Resource.Assignment ))]
        public string SearchTitle { get; set; }

        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.Assignment))]
        public System.Nullable<System.Int32> SearchType { get; set; }

        [Display(Name = "CreatDate", ResourceType = typeof(UT.SL.Model.Resource.Assignment))]
        public System.Nullable<System.DateTime> SearchCreateDate { get; set; }

        [Display(Name = "Body", ResourceType = typeof(UT.SL.Model.Resource.Assignment))]
        public System.Nullable<System.DateTime> SearchBody { get; set; }

        [Display(Name = "FileContent", ResourceType = typeof(UT.SL.Model.Resource.Assignment ))]
        public System.Byte[] SearchFileContent { get; set; }

        public int? CourseId { get; set; }
    }
}
