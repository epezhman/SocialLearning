using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace UT.SL.Model
{
    public class TagModel
    {
        [Display(Name = "Tags", ResourceType = typeof(UT.SL.Model.Resource.TagMapper))]
        public string Tags { get; set; }
    }
}
