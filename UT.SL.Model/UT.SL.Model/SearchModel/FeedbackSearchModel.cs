using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {


    public class FeedbackSearchModel : PagingItems
    {

        [Display(Name = "Title", ResourceType = typeof(UT.SL.Model.Resource.Feedback))]
        public string SearchTitle { get; set; }

        [Display(Name = "CreateDate", ResourceType = typeof(UT.SL.Model.Resource.Feedback))]
        public DateTime CreateDate { get; set; }
       
    }
}
