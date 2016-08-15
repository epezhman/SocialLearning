using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UT.SL.UI.WebUI.Areas.Sandbox.Models
{
    public class BadgetModel
    {
        public String BadgetName { get; set; }
        public UT.SL.Data.LINQ.Resource Resource { get; set; }
    }
}