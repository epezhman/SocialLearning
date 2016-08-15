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
    public class ForumController : BaseController
    {
        SocialLearningDataContext dc = DBUtility.GetSocialLearningDataContext;
        public ActionResult Index(ForumsearchModel model)
        {
            model.Area = "Admin";
            return View(model);
        }

        public ActionResult ForumsearchModelView(ForumsearchModel model)
        {
            model.Area = "Admin";
            return PartialView(model);
        }

        public PagedList<Forum> SearchFilters(ForumsearchModel model)
        {
            model.Area = "Admin";
            var qry = ForumDAL.Find(model);
            model.Update(model.PageSize, qry.Count());
            var ls = qry.Skip(model.PageSize * model.PageIndex).Take(model.PageSize).ToList();
            var ql = new PagedList<Forum>(ls, model);
            return ql;
        }

        public ActionResult IX(ForumsearchModel model)
        {
            model.Area = "Admin";
            return PartialView(SearchFilters(model));
        }

        public ActionResult Create()
        {
            var model = new Forum();
            ViewBag.Courses = CourseDAL.GetAll();
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Forum model)
        {
            var bpr = new BatchProcessResultModel();
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    ForumDAL.Add(model);
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
            var model = ForumDAL.Get(Id);
            ViewBag.Courses = CourseDAL.GetAll();
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(Forum model)
        {
            var bpr = new BatchProcessResultModel();
            if (ModelState.IsValid)
            {
                try
                {
                    ForumDAL.Update(model, out bpr);
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
            var model = ForumDAL.Get(Id);
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(Forum model)
        {
            var bpr = new BatchProcessResultModel();
            try
            {
                if (ForumDAL.Delete(model))
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

        public ActionResult GetForumTitle(System.Nullable<int> Id)
        {
            if (Id.HasValue)
            {
                try
                {
                    var model = ForumDAL.Get(Id.Value).Title;
                    return Content(model);
                }
                catch (System.Exception)
                {
                    Content("");
                }
            }
            return Content("");
        }

        //Controller action:
        public JsonResult GetData()
        {
            Dictionary<string, double> data = new Dictionary<string, double>();//this.repository.GetData(id);
            /*for (int i = 1; i < 4; i++)
            {
                data.Add(i, i/10);
            }*/
            data.Add("A", 4);
            data.Add("B", 6);
            data.Add("C", 3);
            return Json(data.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult test()
        {
            List<int> numOfPosts = new List<int>() { 1, 2, 3, 4, 5 };
            ViewBag.numOfPosts = numOfPosts;
            return View();
        }

        //

        //public ActionResult moodleData(object sender, System.EventArgs e)
        //{
        //    string connStr = String.Format("server={0};uid={1};pwd={2};database={3}",
        //        "192.168.8.1", "epezhman", "master68", "elabmoodle");
        //    conn = new MySqlConnection(connStr);
        //    try
        //    {
        //        conn.Open();

        //        string sql = "SELECT * FROM mdl_assign";
        //        cmd = new MySqlCommand(sql, conn);
        //        //cmd.ExecuteNonQuery();
        //        MySqlDataReader myReader = cmd.ExecuteReader();
        //        if (myReader.HasRows)
        //        {
        //            //int i = 0;
        //            // Always call Read before accessing data.
        //            while (myReader.Read())
        //            {
        //                if (myReader["course"].ToString() != "")
        //                {
        //                    UT.SL.Data.LINQ.ForumDiscussion md = new ForumDiscussion();
        //                    md.Subject = myReader["name"].ToString();
        //                    md.CreateDate = DateTime.Now;//???
        //                    md.Body = myReader["intro"].ToString();
        //                    md.ViewCount = 0;//???
        //                    md.UserId = 1;//???

        //                    dc.ForumDiscussions.InsertOnSubmit(md);
        //                    dc.SubmitChanges();
        //                    //strInsertSQL = "Insert Into tblProduct_temp (Productid) Values('this istest') ";
        //                    //MySqlCommand cmdInserttblProductFrance = new MySqlCommand(strInsertSQL, myConnection);
        //                    //cmdInserttblProductFrance.ExecuteNonQuery(); //<=====THIS LINE THROWS "C# mySQL There is already an open DataReader associated with this Connection which must be closed first."
        //                }
        //            }
        //        }

        //        /*sql = "DROP PROCEDURE IF EXISTS AsyncSample;" +
        //            "CREATE PROCEDURE AsyncSample() BEGIN " +
        //            "set @x=0; repeat set @x=@x+1; until @x > 5000000 end repeat; " +
        //            "INSERT INTO AsyncSampleTable VALUES (1); end;";
        //        cmd.CommandText = sql;
        //        cmd.ExecuteNonQuery();*/

        //        cmd.CommandText = "AsyncSample";
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        asyncResult = cmd.BeginExecuteNonQuery();
        //        nextTime = 5;
        //        start = DateTime.Now;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw (ex);
        //    }
        //    return View();
        //}

        public ActionResult More()
        {
            return PartialView();
        }

        public ActionResult createForum(int courseID = -1, string Title = "")
        {
            ViewBag.courseID = courseID;
            if (courseID != -1 && Title != "")
            {
                var newForum = new UT.SL.Data.LINQ.Forum
                {
                    CreateDate = DateTime.Now,
                    Type = 1,
                    Title = Title,
                    CourseId = courseID
                };
                ForumDAL.Add(newForum);
            }
            return View();
        }

        public ActionResult newForum(int forumId = -1, int discussionId = -1)
        {
            ViewBag.Discussion = dc.ForumDiscussions.First(j => j.Id == discussionId);
            //if (drs.Count() == 1)
            //{
            //    ViewBag.Discussion = drs.SingleOrDefault();
            //}
            ViewBag.forumId = forumId;
            return View();

        }

        public ActionResult showForums(int courseId = -1)
        {
            ViewBag.courseId = courseId;
            ViewBag.Forums = dc.Forums.Where(j => j.CourseId == courseId).ToList();
            //ViewBag.Forums = dc.App_Roles.ToList();
            return View();
        }
    }
}
