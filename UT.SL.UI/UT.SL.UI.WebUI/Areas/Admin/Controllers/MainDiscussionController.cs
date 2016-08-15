using System;
using System.Linq;
using System.Web.Mvc;
using UT.SL.Helper;
using UT.SL.UI.WebUI.Controllers;
using UT.SL.Model;
using UT.SL.Data.LINQ;
using UT.SL.DAL;
using System.Collections.Generic;


namespace UT.SL.UI.WebUI.Areas.Admin.Controllers
{


    [Authorize()]
    public class ForumDiscussionController : BaseController
    {
        SocialLearningDataContext dc = DBUtility.GetSocialLearningDataContext;
        public ActionResult Index(ForumDiscussionsearchModel model)
        {
            model.Area = "Admin";
            return View(model);
        }

        public ActionResult ForumDiscussionsearchModelView(ForumDiscussionsearchModel model)
        {
            model.Area = "Admin";
            return PartialView(model);
        }

        public PagedList<ForumDiscussion> SearchFilters(ForumDiscussionsearchModel model)
        {
            model.Area = "Admin";
            var qry = ForumDiscussionDAL.Find(model);
            model.Update(model.PageSize, qry.Count());
            var ls = qry.Skip(model.PageSize * model.PageIndex).Take(model.PageSize).ToList();
            var ql = new PagedList<ForumDiscussion>(ls, model);
            return ql;
        }

        public ActionResult IX(ForumDiscussionsearchModel model)
        {
            model.Area = "Admin";
            return PartialView(SearchFilters(model));
        }

        public ActionResult Create()
        {
            var model = new ForumDiscussion();
            ViewBag.Forums = ForumDAL.GetAll();
            ViewBag.App_Users = App_UserDAL.GetAll();
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(ForumDiscussion model)
        {
            var bpr = new BatchProcessResultModel();
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    ForumDiscussionDAL.Add(model);
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
            var model = ForumDiscussionDAL.Get(Id);
            ViewBag.Forums = ForumDAL.GetAll();
            ViewBag.App_Users = App_UserDAL.GetAll();
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(ForumDiscussion model)
        {
            var bpr = new BatchProcessResultModel();
            if (ModelState.IsValid)
            {
                try
                {
                    ForumDiscussionDAL.Update(model, out bpr);
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
            var model = ForumDiscussionDAL.Get(Id);
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(ForumDiscussion model)
        {
            var bpr = new BatchProcessResultModel();
            try
            {
                if (ForumDiscussionDAL.Delete(model))
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

        public ActionResult GetForumDiscussionsubject(System.Nullable<int> Id)
        {
            if (Id.HasValue)
            {
                try
                {
                    var model = ForumDiscussionDAL.Get(Id.Value).Subject;
                    return Content(model);
                }
                catch (System.Exception)
                {
                    Content("");
                }
            }
            return Content("");
        }

        public ActionResult GetForumDiscussionBody(System.Nullable<int> Id)
        {
            if (Id.HasValue)
            {
                try
                {
                    var model = ForumDiscussionDAL.Get(Id.Value).Body;
                    return Content(model);
                }
                catch (System.Exception)
                {
                    Content("");
                }
            }
            return Content("");
        }

        public ActionResult createDiscussion(int forumID = -1)
        {
            ViewBag.forumID = forumID;
            var tempDiscussion = new UT.SL.Data.LINQ.ForumDiscussion
            {
                CreateDate = DateTime.Now,
                UserId = CurrentUser.Id,
                Subject = "",
                Body = "",
                ForumId = forumID
            };
            ViewBag.tempDiscussion = ForumDiscussionDAL.Add(tempDiscussion);

            return View();
        }

        [HttpPost]
        public ActionResult createDiscussion()
        {

            if (Request.Form["forumID"] != "-1" && Request.Form["Subject"] != "" && Request.Form["Body"] != "" && Request.Form["tempDisId"] != "")
            {
                UT.SL.Data.LINQ.ForumDiscussion md = ForumDiscussionDAL.Get(Convert.ToInt32(Request.Form["tempDisId"]));
                md.Subject = Request.Form["Subject"];
                md.Body = Request.Form["Body"];
                var bpr = new BatchProcessResultModel();
                ForumDiscussionDAL.Update(md, out bpr);
                /*var newDiscussion = new UT.SL.Data.LINQ.ForumDiscussion
                {
                    CreateDate = DateTime.Now,
                    UserId = CurrentUser.Id,
                    Subject = Subject,
                    Body = Body,
                    ForumId = forumID
                };
                ForumDiscussionDAL.Add(newDiscussion);*/
            }

            return RedirectToAction("showDiscussions", new { forumId = Convert.ToInt32(Request.Form["forumID"]) });
        }

        public ActionResult getReplies(int discussionID = -1)
        {
            ViewBag.discussionID = discussionID;
            //UT.SL.Data.LINQ.SocialLearningDataContext tdc = new SocialLearningDataContext();
            var discussionobj = dc.ForumDiscussions.Where(j => j.Id == discussionID).SingleOrDefault();
            ViewBag.discussionSubject = discussionobj.Subject;

            var temp = (from d in dc.ForumDiscussionPosts
                        where d.ParentId == discussionID && d.ParentReplyId == null
                        select d);

            List<showForumPosts> lTsprts = new List<showForumPosts>();
            foreach (var t in temp)
            {
                ForumDiscussionPostDAL.setLeveledPosts(ref lTsprts, t.Id, t.ForumDiscussion.Id, 0);
            }
            ViewBag.replies = lTsprts.ToList();

            ViewBag.forumId = discussionobj.ForumId;
            return PartialView();// lTsprts.ToList();
        }

        public ActionResult CommentPage(int ObjectId = 0, int replyID = 0)
        {
            ViewBag.ObjectId = ObjectId;
            ViewBag.replyID = replyID;
            return PartialView();
        }

        public ActionResult replyPage(int discussionID = 0, int replyID = 0)
        {
            ViewBag.discussionID = discussionID;
            ViewBag.replyID = replyID;
            var model = new ForumDiscussionPost();
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult replyPage(FormCollection fmc)
        {
            string comment = Request.Form["Text"];
            string discussionID = Request.Form["discussionID"];
            string replyID = Request.Form["replyID"];
            var bpr2 = new BatchProcessResultModel();
            if (!string.IsNullOrEmpty(comment) && replyID == "0")
            {
                var newPost = new UT.SL.Data.LINQ.ForumDiscussionPost
                {
                    CreateDate = DateTime.Now,
                    UserId = CurrentUser.Id,
                    Text = comment,
                    ParentId = Convert.ToInt32(discussionID)
                };
                ForumDiscussionPostDAL.Add(newPost);
            }
            else if (replyID != "0")
            {
                var newPost = new UT.SL.Data.LINQ.ForumDiscussionPost
                {
                    CreateDate = DateTime.Now,
                    UserId = CurrentUser.Id,
                    Text = comment,
                    ParentId = Convert.ToInt32(discussionID),
                    ParentReplyId = Convert.ToInt32(replyID)
                };
                ForumDiscussionPostDAL.Add(newPost);

            }

            ViewBag.discussionID = discussionID;
            return PartialView();
        }

        public ActionResult showDiscussion(int forumId = -1, int discussionId = -1)
        {
            var drs = dc.ForumDiscussions.Where(j => j.Id == discussionId);
            if (drs.Count() == 1)
            {
                ViewBag.Discussion = drs.SingleOrDefault();
            }
            ViewBag.forumId = forumId;
            ViewBag.discussionID = discussionId;
            return View();

        }

        public ActionResult showDiscussions(int forumId = -1, int delTemp = -1)
        {
            if (delTemp != -1)
            {
                UT.SL.Data.LINQ.ForumDiscussion md = ForumDiscussionDAL.Get(delTemp);
                ForumDiscussionDAL.Delete(md);
            }
            ViewBag.forumId = forumId;
            ViewBag.forum = dc.Forums.Where(j => j.Id == forumId).SingleOrDefault();
            ViewBag.Discussions = dc.ForumDiscussions.Where(j => j.ForumId == forumId).ToList();

            //Last Week Activities

            //
            return View();
        }

        public ActionResult showDiscussionHistory(int discussionId = -1)
        {
            var users = new List<App_User>();
            if (discussionId != -1)
            {
                List<VoteParent> votes = dc.VoteParents.Where(j => j.ObjectId == discussionId && j.ObjectType == 8).ToList();

                foreach (VoteParent vote in votes)
                {
                    //users.Add(dc.App_Users.Where(j => j.Id == ) vote.Votes.Select(e => e.App_User));
                    users.AddRange(vote.Votes.Select(x => x.App_User).ToList());
                }
                ViewBag.discussionId = discussionId;
            }
            ViewBag.users = users;
            return PartialView();
        }

        public void deleteDiscussion(int id = -1)
        {
            if (id != -1)
            {
                UT.SL.Data.LINQ.ForumDiscussion md = ForumDiscussionDAL.Get(id);
                if (ForumDiscussionDAL.getChildren(id).Count() > 0)
                {
                    foreach (UT.SL.Data.LINQ.ForumDiscussionPost dr in md.getChildren())
                    {
                        ForumDiscussionPostDAL.Delete(dr);
                    }
                }
                ForumDiscussionDAL.Delete(md);
            }
        }
        
        public string getDiscussionActivties(int discussionId = -1)
        {
            ForumDiscussion md = dc.ForumDiscussions.Where(j => j.Id == discussionId).SingleOrDefault();
            List<ForumDiscussionPost> allReplies = dc.ForumDiscussionPosts.Where(i => i.ParentId == md.Id).ToList();
            string oldReplies = allReplies.Where(k => (DateTime.Now - (DateTime)k.CreateDate).TotalDays > 6).Count().ToString();
            string sixDaysbefore = allReplies.Where(k => (DateTime.Now - (DateTime)k.CreateDate).TotalDays == 6).Count().ToString();
            string fiveDaysbefore = allReplies.Where(k => (DateTime.Now - (DateTime)k.CreateDate).TotalDays == 5).Count().ToString();
            string fourDaysbefore = allReplies.Where(k => (DateTime.Now - (DateTime)k.CreateDate).TotalDays == 4).Count().ToString();
            string threeDaysbefore = allReplies.Where(k => (DateTime.Now - (DateTime)k.CreateDate).TotalDays == 3).Count().ToString();
            string twoDaysbefore = allReplies.Where(k => (DateTime.Now - (DateTime)k.CreateDate).TotalDays == 2).Count().ToString();
            string oneDaysbefore = allReplies.Where(k => (DateTime.Now - (DateTime)k.CreateDate).TotalDays == 1).Count().ToString();
            string zeroDaysbefore = allReplies.Where(k => (DateTime.Now - (DateTime)k.CreateDate).TotalDays == 0).Count().ToString();


            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", oldReplies, sixDaysbefore, fiveDaysbefore, fourDaysbefore, threeDaysbefore, twoDaysbefore, oneDaysbefore, zeroDaysbefore);
        }

        public ActionResult EditForumDiscussion(int Id)
        {
            var model = ForumDiscussionDAL.Get(Id);
            return PartialView(model);
        }
        public void editDiscussion(int discussionId = -1, string discussion = "")
        {
            if (discussionId != -1)
            {
                UT.SL.Data.LINQ.ForumDiscussion md = ForumDiscussionDAL.Get(discussionId);
                md.Body = discussion;
                var bpr = new BatchProcessResultModel();
                ForumDiscussionDAL.Update(md, out bpr);
            }
        }

        public void setPostGrade(int Id = -1, int grade = -1) 
        {
            UT.SL.Data.LINQ.ForumDiscussionPostsGrade postGrade = null;
            if (dc.ForumDiscussionPostsGrades.Where(j => j.postId == Id).Count() == 1) 
            {
                postGrade = dc.ForumDiscussionPostsGrades.Where(j => j.postId == Id).SingleOrDefault();
            }
            else 
            {
                postGrade = new ForumDiscussionPostsGrade();
                dc.ForumDiscussionPostsGrades.InsertOnSubmit(postGrade);
            }
            postGrade.grade = grade;
            postGrade.postId = Id;
            postGrade.userId = 1;//??
            postGrade.submitDate = DateTime.Now;
            dc.SubmitChanges();
        
        }
    }
}
