using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UT.SL.Helper
{
    public class Authorization
    {
        public bool IsLink { get; set; }
        public bool IsAjaxLink { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string AreaName { get; set; }
        public int PassedId { get; set; }
        public Guid PassedGuid { get; set; }
        public string IconClass { get; set; }
        public string TooltipTitle { get; set; }
    }
}
