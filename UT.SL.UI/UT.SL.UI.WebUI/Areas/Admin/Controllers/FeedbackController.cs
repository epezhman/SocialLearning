using System;
using System.Linq;
using System.Web.Mvc;
using UT.SL.Helper;
using UT.SL.UI.WebUI.Controllers;
using UT.SL.Model;
using UT.SL.Data.LINQ;
using UT.SL.DAL;
using UT.SL.BLL;
using UT.SL.Model.Enumeration;


namespace UT.SL.UI.WebUI.Areas.Admin.Controllers
{

    public class FeedbackController : BaseController
    {

        public ActionResult Index(FeedbackSearchModel model)
        {
            model.Area = "Admin";
            return View(model);
        }

        public ActionResult FeedbackSearchModelView(FeedbackSearchModel model)
        {
            model.Area = "Admin";
            return PartialView(model);
        }

        public PagedList<Feedback> SearchFilters(FeedbackSearchModel model)
        {
            model.Area = "Admin";
            var qry = FeedbackDAL.Find(model);
            model.Update(model.PageSize, qry.Count());
            var ls = qry.Skip(model.PageSize * model.PageIndex).Take(model.PageSize).ToList();
            var ql = new PagedList<Feedback>(ls, model);
            return ql;
        }

        public ActionResult IX(FeedbackSearchModel model)
        {
            model.Area = "Admin";
            return PartialView(SearchFilters(model));
        }

        public ActionResult View(System.Int32 Id)
        {
            var model = FeedbackDAL.Get(Id);
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new Feedback();
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Feedback model)
        {
            var bpr = new BatchProcessResultModel();
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    if (Request.IsAuthenticated)
                        model.UserId = CurrentUser.Id;
                    model.CreateDate = DateTime.Now;
                    var drm = (DALReturnModel<Feedback>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Feedback),
                            new Func<Feedback, BatchProcessResultModel, DALReturnModel<Feedback>>(FeedbackDAL.Add), model, bpr);
                    bpr = drm.BPR;
                    if (bpr.Failed > 0)
                    {
                        return PartialView("ProcessResult", bpr);
                    }
                    else
                    {
                        bpr.SuccessClientScript = "$('#searchForm').submit();";
                    }
                }
                catch (System.Exception)
                {
                    bpr.AddError(UT.SL.Model.Resource.App_Errors.BprMainUnknownError, true, true);
                }
            }
            else
            {
                bpr.AddModelStateErrors(ModelState);
            }
            return PartialView("ProcessResult", bpr);
        }

        public ActionResult Edit(System.Int32 Id)
        {
            var model = FeedbackDAL.Get(Id);
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(Feedback model)
        {
            var bpr = new BatchProcessResultModel();
            if (ModelState.IsValid)
            {
                try
                {
                    var drm = (DALReturnModel<Feedback>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Feedback),
                           new Func<Feedback, BatchProcessResultModel, DALReturnModel<Feedback>>(FeedbackDAL.Update), model, bpr);
                    bpr = drm.BPR;
                    if (bpr.Failed > 0)
                    {
                        return PartialView("ProcessResult", bpr);
                    }
                    else
                    {
                        bpr.SuccessClientScript = "$('#searchForm').submit();";
                    }
                }
                catch (System.Exception)
                {
                    bpr.AddError(UT.SL.Model.Resource.App_Errors.BprMainUnknownError, true, true);
                }
            }
            else
            {
                bpr.AddModelStateErrors(ModelState);
            }
            return PartialView("ProcessResult", bpr);
        }

        public ActionResult Delete(System.Int32 Id)
        {
            var model = FeedbackDAL.Get(Id);
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(Feedback model)
        {
            var bpr = new BatchProcessResultModel();
            try
            {
                var drm = (DALReturnModel<Feedback>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Feedback),
                           new Func<Feedback, DALReturnModel<Feedback>>(FeedbackDAL.Delete), model);
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
