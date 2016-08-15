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


    [Authorize()]
    public class ForumDiscussionPostController : BaseController
    {

        public ActionResult Edit(int Id)
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
        public ActionResult Edit(ForumDiscussionPost model)
        {
            var bpr = new BatchProcessResultModel();
            if (ModelState.IsValid)
            {
                try
                {
                    var drm = (DALReturnModel<ForumDiscussionPost>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ForumDiscussionPost),
                            new Func<ForumDiscussionPost, DALReturnModel<ForumDiscussionPost>>(ForumDiscussionPostDAL.Update), model);
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

        public void DeleteReply(int id = -1)
        {
            var bpr = new BatchProcessResultModel();
            try
            {
                ForumDiscussionPost fdp = ForumDiscussionPostDAL.Get(id);
                ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ForumDiscussionPost),
                                     new Func<ForumDiscussionPost, DALReturnModel<ForumDiscussionPost>>(ForumDiscussionPostDAL.Delete), fdp);
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
                dr.Text = reply;
                var bpr = new BatchProcessResultModel();
                var drm = (DALReturnModel<ForumDiscussionPost>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ForumDiscussionPost),
                                     new Func<ForumDiscussionPost, DALReturnModel<ForumDiscussionPost>>(ForumDiscussionPostDAL.Update), dr);
                bpr = drm.BPR;
            }
        }

        public ActionResult DiscussionPosts(int id)
        {
            var model = ForumDiscussionDAL.Get(id).ForumDiscussionPosts.ToList();
            ViewBag.Id = id;
            return PartialView(model);
        }

        public ActionResult GetDiscussionPostForm(int id)
        {
            var model = new ForumDiscussionPost
            {
                ParentId = id,
                UserId = CurrentUser.Id
            };
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult PostDiscussionPostReply(ForumDiscussionPost model)
        {
            int discussionPostId = 0;
            try
            {
                if (!string.IsNullOrEmpty(model.Text.StringNormalizer()))
                {
                    var obj = new ForumDiscussionPost
                    {
                        Text = model.Text.StringNormalizer(),
                        ParentId = model.ParentId,
                        CreateDate = DateTime.Now,
                        UserId = CurrentUser.Id
                    };
                    var drm = (DALReturnModel<ForumDiscussionPost>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ForumDiscussionPost),
                            new Func<ForumDiscussionPost, DALReturnModel<ForumDiscussionPost>>(ForumDiscussionPostDAL.Add), obj);
                    discussionPostId = drm.ReturnObject.Id;
                    ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.Create, discussionPostId, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussionPost, CurrentUser.Id);
                    UserInfoManager.UpdateUserActSummary(CurrentUser.Id, discussionPostId, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussionPost, (int)UT.SL.Model.Enumeration.ActivityType.Create);

                }
            }
            catch
            {
                return Content("0");
            }
            return Content(discussionPostId.ToString());
        }

        public ActionResult DiscussionPostsCount(int id)
        {
            var model = ForumDiscussionDAL.Get(id);
            return PartialView(model);
        }

        public ActionResult GetOneDiscussionPost(int id)
        {
            var model = ForumDiscussionPostDAL.Get(id);
            return PartialView(model);
        }

    }
}
