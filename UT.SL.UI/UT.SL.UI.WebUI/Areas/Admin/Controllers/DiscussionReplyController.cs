using System;
using System.Linq;
using System.Web.Mvc;
using UT.SL.Helper;
using UT.SL.UI.WebUI.Controllers;
using UT.SL.Model;
using UT.SL.Data.LINQ;
using UT.SL.DAL;


namespace UT.SL.UI.WebUI.Areas.Admin.Controllers
{


    [Authorize()]
    public class ForumDiscussionPostController : BaseController
    {

        public ActionResult Index(ForumDiscussionPostSearchModel model)
        {
            model.Area = "Admin";
            return View(model);
        }

        public ActionResult ForumDiscussionPostSearchModelView(ForumDiscussionPostSearchModel model)
        {
            model.Area = "Admin";
            return PartialView(model);
        }

        public PagedList<UT.SL.Data.LINQ.ForumDiscussionPost> SearchFilters(ForumDiscussionPostSearchModel model)
        {
            model.Area = "Admin";
            var qry = ForumDiscussionPostDAL.Find(model);
            model.Update(model.PageSize, qry.Count());
            var ls = qry.Skip(model.PageSize * model.PageIndex).Take(model.PageSize).ToList();
            var ql = new PagedList<UT.SL.Data.LINQ.ForumDiscussionPost>(ls, model);
            return ql;
        }

        public ActionResult IX(ForumDiscussionPostSearchModel model)
        {
            model.Area = "Admin";
            return PartialView(SearchFilters(model));
        }

        public ActionResult Create()
        {
            var model = new UT.SL.Data.LINQ.ForumDiscussionPost();
            ViewBag.ForumDiscussionPosts = ForumDiscussionPostDAL.GetAll();
            ViewBag.App_Users = App_UserDAL.GetAll();
            ViewBag.ForumDiscussions = ForumDiscussionDAL.GetAll();
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(UT.SL.Data.LINQ.ForumDiscussionPost model)
        {
            var bpr = new BatchProcessResultModel();
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    ForumDiscussionPostDAL.Add(model);
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

        public ActionResult Edit(int Id)
        {
            var model = ForumDiscussionPostDAL.Get(Id);
            ViewBag.ForumDiscussionPosts = ForumDiscussionPostDAL.GetAll();
            ViewBag.App_Users = App_UserDAL.GetAll();
            ViewBag.ForumDiscussions = ForumDiscussionDAL.GetAll();
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(UT.SL.Data.LINQ.ForumDiscussionPost model)
        {
            var bpr = new BatchProcessResultModel();
            if (ModelState.IsValid)
            {
                try
                {
                    ForumDiscussionPostDAL.Update(model);
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
            var model = ForumDiscussionPostDAL.Get(Id);
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(UT.SL.Data.LINQ.ForumDiscussionPost model)
        {
            var bpr = new BatchProcessResultModel();
            try
            {
                if (ForumDiscussionPostDAL.Delete(model))
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

        public ActionResult GetForumDiscussionPostText(System.Nullable<int> Id)
        {
            if (Id.HasValue)
            {
                try
                {
                    var model = ForumDiscussionPostDAL.Get(Id.Value).Text;
                    return Content(model);
                }
                catch (System.Exception)
                {
                    Content("");
                }
            }
            return Content("");
        }

        public void deleteReply(int id = -1)
        {
            var bpr = new BatchProcessResultModel();
            try
            {
                ForumDiscussionPostDAL.Delete(new ForumDiscussionPost { Id = id });
            }
            catch (Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }

            //return PartialView("ProcessResult", bpr);
        }

        public ActionResult EditForumDiscussionPost(int Id)
        {
            var model = ForumDiscussionPostDAL.Get(Id);
            return PartialView(model);
        }

        public void editReply(int replyId = -1, string reply = "")
        {
            if (replyId != -1)
            {
                UT.SL.Data.LINQ.ForumDiscussionPost dr = ForumDiscussionPostDAL.Get(replyId);
                dr.Text= reply;
                var bpr = new BatchProcessResultModel();
                ForumDiscussionPostDAL.Update(dr, false);
            }
        }
    }
}
