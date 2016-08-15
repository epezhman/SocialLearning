using System;
using System.Linq;
using System.Web.Mvc;
using UT.SL.Helper;
using UT.SL.UI.WebUI.Controllers;
using UT.SL.Model;
using UT.SL.Data.LINQ;
using UT.SL.DAL;
using System.Collections.Generic;
using UT.SL.BLL;
using UT.SL.Model.Enumeration;


namespace UT.SL.UI.WebUI.Areas.Admin.Controllers
{


    [Authorize()]
    public class TagController : BaseController
    {

        public ActionResult Index(TagSearchModel model)
        {
            model.Area = "Admin";
            return View(model);
        }

        public ActionResult TagSearchModelView(TagSearchModel model)
        {
            model.Area = "Admin";
            return PartialView(model);
        }

        public PagedList<Tag> SearchFilters(TagSearchModel model)
        {
            model.Area = "Admin";
            var qry = TagDAL.Find(model);
            model.Update(model.PageSize, qry.Count());
            var ls = qry.Skip(model.PageSize * model.PageIndex).Take(model.PageSize).ToList();
            var ql = new PagedList<Tag>(ls, model);
            return ql;
        }

        public ActionResult IX(TagSearchModel model)
        {
            model.Area = "Admin";
            return PartialView(SearchFilters(model));
        }

        public ActionResult Create()
        {
            var model = new Tag();
            ViewBag.Categories = CategoryDAL.GetAll().Where(x => x.Category1 == null);
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Tag model, int? CategoryTag, int? SubjectTag)
        {
            var bpr = new BatchProcessResultModel();
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    if (SubjectTag.HasValue && SubjectTag.Value > 0)
                        model.CategoryId = SubjectTag;
                    else if (CategoryTag.HasValue && CategoryTag.Value > 0)
                        model.CategoryId = CategoryTag;
                    model.IsValid = true;
                    model.UserId = CurrentUser.Id;
                    //TagDAL.Add(model);
                    var drm = (DALReturnModel<Tag>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Tag),
                                       new Func<Tag, DALReturnModel<Tag>>(TagDAL.Add), model);
                    bpr = drm.BPR;
                    if (bpr.Failed > 0)
                    {
                        return PartialView("ProcessResult", bpr);
                    }
                    else
                    {
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
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

        public ActionResult Edit(int Id)
        {
            var model = TagDAL.Get(Id);
            ViewBag.Categories = CategoryDAL.GetAll().Where(x => x.Category1 == null);
            if (model.CategoryId.HasValue && model.Category.ParentId.HasValue)
                ViewBag.Subjects = CategoryDAL.GetAll().Where(x => x.ParentId == model.Category.ParentId);
            else
                ViewBag.Subjects = new List<Category>();

            if (model.CategoryId.HasValue && model.Category.ParentId.HasValue)
                ViewBag.Tags = TagDAL.GetAll(model.Category.ParentId.Value);
            else if (model.CategoryId.HasValue)
                ViewBag.Tags = TagDAL.GetAll(model.CategoryId.Value);
            else
                ViewBag.Tags = new List<Tag>();

            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(Tag model, int? CategoryTag, int? SubjectTag)
        {
            var bpr = new BatchProcessResultModel();
            if (ModelState.IsValid)
            {
                try
                {
                    if (SubjectTag.HasValue && SubjectTag.Value > 0)
                        model.CategoryId = SubjectTag;
                    else if (CategoryTag.HasValue && CategoryTag.Value > 0)
                        model.CategoryId = CategoryTag;
                    else
                        model.CategoryId = null;
                    model.IsValid = true;
                    var drm = (DALReturnModel<Tag>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Tag),
                                       new Func<Tag, BatchProcessResultModel, DALReturnModel<Tag>>(TagDAL.Update), model, bpr);
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

        public ActionResult Delete(int Id)
        {
            var model = TagDAL.Get(Id);
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(Tag model)
        {
            var bpr = new BatchProcessResultModel();
            try
            {
                var drm = (DALReturnModel<Tag>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Tag),
                                       new Func<Tag, DALReturnModel<Tag>>(TagDAL.Delete), model);
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

        public ActionResult GetTagTitile(System.Nullable<int> Id)
        {
            if (Id.HasValue)
            {
                try
                {
                    var model = TagDAL.Get(Id.Value).Title;
                    return Content(model);
                }
                catch (System.Exception)
                {
                    Content("");
                }
            }
            return Content("");
        }

        public ActionResult GetSubjects(int Id)
        {
            var model = CategoryDAL.GetAll(Id).Select(x => new { id = x.Id, title = x.Title }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTagsCategory(int Id)
        {
            var model = TagDAL.GetAll(Id).Select(x => new { id = x.Id, title = x.Title }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Confirm(int Id)
        {
            var model = TagDAL.Get(Id);
            model.IsValid = true;
            var bpr = new BatchProcessResultModel();
            var drm = (DALReturnModel<Tag>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Tag),
                                       new Func<Tag, BatchProcessResultModel, DALReturnModel<Tag>>(TagDAL.Update), model, bpr);
            bpr = drm.BPR;
            return PartialView(model);
        }
    }
}
