using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;


namespace UT.SL.Model {


    public class EmailSearchModel : PagingItems
    {
        
        [Display(Name = "Email", ResourceType = typeof(UT.SL.Model.Resource.App_Common))]
        public string SearchTitle { get; set; }
        
    }
}
