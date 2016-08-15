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
    public class App_UserLogController : BaseController
    {

        public ActionResult Index(App_UserLogSearchModel model)
        {
            model.Area = "Admin";
            return View(model);
        }

        public ActionResult UsersLogSearchModelView(App_UserLogSearchModel model)
        {
            model.Area = "Admin";
            return PartialView(model);
        }

        public PagedList<App_ActionEvaulation> SearchFilters(App_UserLogSearchModel model)
        {
            model.Area = "Admin";
            var qry = App_UserLogDAL.Find(model);
            model.Update(model.PageSize, qry.Count());
            var ls = qry.Skip(model.PageSize * model.PageIndex).Take(model.PageSize).ToList();
            var ql = new PagedList<App_ActionEvaulation>(ls, model);
            return ql;
        }

        public ActionResult IX(App_UserLogSearchModel model)
        {
            model.Area = "Admin";
            return PartialView(SearchFilters(model));
        }

        public ActionResult View(int Id)
        {
            var model = App_ActionEvaulationDAL.Get(Id);
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }


        public ActionResult Delete(int Id)
        {
            var model = App_ActionEvaulationDAL.Get(Id);
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(App_ActionEvaulation model)
        {
            var bpr = new BatchProcessResultModel();
            try
            {
                var drm = (DALReturnModel<App_ActionEvaulation>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_ActionEvaulation),
                                       new Func<App_ActionEvaulation, DALReturnModel<App_ActionEvaulation>>(App_ActionEvaulationDAL.Delete), model);
                if (drm.ReturnObject.Id > 0)
                {
                    bpr.SuccessClientScript = "$('#searchForm').submit();";
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true);
                }
                else
                {
                    bpr.AddError(UT.SL.Model.Resource.App_Errors.BprMainUnknownError, true, true);
                }
            }
            catch (System.Exception)
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.BprMainUnknownError, true, true);
            }
            return PartialView("ProcessResult", bpr);
        }

    }
}
