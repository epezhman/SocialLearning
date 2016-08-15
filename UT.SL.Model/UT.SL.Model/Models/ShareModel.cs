using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using UT.SL.Helper;

namespace UT.SL.Model
{
    public class ShareModel
    {
        [Display(Name = "LearningGroup", ResourceType = typeof(UT.SL.Model.Resource.Share))]
        public SelectListItems LearningGroup { get; set; }

        [Display(Name = "SocialGroup", ResourceType = typeof(UT.SL.Model.Resource.Share))]
        public SelectListItems SocialGroup { get; set; }

        [Display(Name = "ShareUserIds", ResourceType = typeof(UT.SL.Model.Resource.Share))]
        public string ShareUserIds { get; set; }

        [Display(Name = "HiddentShareUserIds", ResourceType = typeof(UT.SL.Model.Resource.Share))]
        public string HiddentShareUserIds { get; set; }
    }
}
