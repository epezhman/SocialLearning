using System;
using System.Linq;
using System.Web.Mvc;
using UT.SL.Helper;
using UT.SL.UI.WebUI.Controllers;
using UT.SL.Model;
using UT.SL.Data.LINQ;
using UT.SL.DAL;
using UT.SL.Model.Enumeration;
using UT.SL.BLL;

namespace UT.SL.UI.WebUI.Areas.Admin.Controllers
{
    [Authorize()]
    public class ObjectStreamController : BaseController
    {
        public void UpdateReadFlag(int objectId, int objecType)
        {
            var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream),
                                       new Func<int, int, int, DALReturnModel<ObjectStream>>(ObjectStreamDAL.UpdateReadFlag), objectId, objecType, CurrentUser.Id);                      
        }

    }
}
