using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using UT.SL.Data.LINQ;

namespace UT.SL.Model
{
    public class RequestDetailModel
    {
        public App_User CurrentUser { get; set; }
        public App_Action CurrentAction { get; set; }
        public RequestContext CurrentRequestContext { get; set; }
        public int ObjectType { get; set; }
        public int ObjectId { get; set; }
        public ObjectViewModel DBObject { get; set; }
    }
}
