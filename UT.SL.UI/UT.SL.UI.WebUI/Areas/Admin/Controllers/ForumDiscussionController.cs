using System;
using System.Linq;
using System.Web.Mvc;
using UT.SL.Helper;
using UT.SL.UI.WebUI.Controllers;
using UT.SL.Model;
using UT.SL.Data.LINQ;
using UT.SL.DAL;
using System.Collections.Generic;
using System.Web;
using UT.SL.BLL;
using UT.SL.Model.Models;
using UT.SL.Model.Enumeration;


namespace UT.SL.UI.WebUI.Areas.Admin.Controllers
{


    [Authorize()]
    public class ForumDiscussionController : BaseController
    {
      
        public ActionResult AddDiscussion(int id)
        {
            var model = ForumDAL.Get(id);
            return PartialView(model);
        }

        public ActionResult NewDiscussion(Forum obj)
        {
            var forum = ForumDAL.Get(obj.Id);
            var model = new FormModel<ForumDiscussion, List<SelectListItems>>();
            model.FormObject = new ForumDiscussion
            {
                ForumId = obj.Id
            };
            model.ExtraKnownData = new List<SelectListItems>();
            var tempItems = new List<SelectListItem>();

            var topics = ObjectTopicMapperDAL.GetAll(forum.Id, (int)UT.SL.Model.Enumeration.ObjectType.Forum);

            tempItems.AddRange(topics.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Title
            }).ToList());
            model.ExtraKnownData.Add(new SelectListItems
            {
                Items = tempItems
            });
            //tempItems = new List<SelectListItem>();
            //tempItems.Add(new SelectListItem
            //{
            //    Text = CourseDAL.Get(forum.CourseId).Title,
            //    Value = "2"
            //});
            //tempItems.AddRange(LearningGroupDAL.GetAllByCourse(forum.CourseId).Select(x => new SelectListItem
            //{
            //    Value = x.Id.ToString(),
            //    Text = x.Title
            //}).ToList());
            //model.ExtraKnownData.Add(new SelectListItems
            //{
            //    Items = tempItems,
            //    SelectedIds = new string[] { "2" }
            //});
            ViewBag.UserId = CurrentUser.Id;
            return PartialView(model);
        }

        public ActionResult SaveDiscussionFile(HttpPostedFileBase discussionFile, int id, int? overwite, int? parentId)
        {
            int fileId = 0;
            try
            {
                if (discussionFile != null)
                {
                    if (overwite.HasValue)
                    {
                        var bpr = new BatchProcessResultModel();
                        var forumdisucssion = ForumDiscussionDAL.Get(id);
                        forumdisucssion.FileMime = discussionFile.ContentType;
                        forumdisucssion.FileSize = discussionFile.ContentLength;
                        forumdisucssion.FileTitle = discussionFile.FileName;
                        byte[] tempFile = null;
                        tempFile = new byte[discussionFile.ContentLength];
                        discussionFile.InputStream.Read(tempFile, 0, discussionFile.ContentLength);
                        forumdisucssion.FileContent = tempFile;
                        var drm = (DALReturnModel<ForumDiscussion>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ForumDiscussion),
                                      new Func<ForumDiscussion, BatchProcessResultModel, DALReturnModel<ForumDiscussion>>(ForumDiscussionDAL.UpdateFile), forumdisucssion, bpr);
                        bpr = drm.BPR;    
                    }
                    else
                    {

                        var newFile = new ForumDiscussion
                        {
                            CreateDate = DateTime.Now,
                            FileMime = discussionFile.ContentType,
                            FileSize = discussionFile.ContentLength,
                            FileTitle = discussionFile.FileName,
                            IsValid = false,
                            IsPublishd = false,
                            UserId = CurrentUser.Id,
                            ForumId = id,
                            ParentId = parentId ?? null
                        };

                        byte[] tempFile = null;
                        tempFile = new byte[discussionFile.ContentLength];
                        discussionFile.InputStream.Read(tempFile, 0, discussionFile.ContentLength);
                        newFile.FileContent = tempFile;

                        var bpr = new BatchProcessResultModel();
                        //newFile.Title = "NotYetPublished";
                        var drm = (DALReturnModel<ForumDiscussion>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ForumDiscussion),
                                       new Func<ForumDiscussion, BatchProcessResultModel, DALReturnModel<ForumDiscussion>>(ForumDiscussionDAL.Add), newFile, bpr);
                        bpr = drm.BPR;
                        fileId = drm.ReturnObject.Id;
                        if (bpr.Failed > 0)
                        {
                            return Content("0");
                        }
                    }
                }
            }
            catch
            {
                return Content("0");
            }
            return Content(fileId.ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult RemoveDiscussionFile(int id)
        {
            try
            {
                ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ForumDiscussion),
                                      new Func<ForumDiscussion, DALReturnModel<ForumDiscussion>>(ForumDiscussionDAL.Delete), new ForumDiscussion { Id = id });          
            }
            catch
            {
                return Content("0");
            }
            return Content("2");
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult RemoveDiscussionReplyFile(int id)
        {
            try
            {
                ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ForumDiscussion),
                      new Func<ForumDiscussion, DALReturnModel<ForumDiscussion>>(ForumDiscussionDAL.Delete), new ForumDiscussion { Id = id });  
            }
            catch
            {
                return Content("0");
            }
            return Content("2");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostDiscussion(FormModel<ForumDiscussion, List<SelectListItems>> model, string[] topicIds)
        {
            int discussionId = 0;
            try
            {
                var bpr = new BatchProcessResultModel();
                if (model.FormObject.Id > 0)
                {
                    var obj = ForumDiscussionDAL.Get(model.FormObject.Id);
                    if (obj != null)
                    {
                        obj.Body = model.FormObject.Body.StringNormalizer();
                        obj.Title = model.FormObject.Title.StringNormalizer();
                        obj.IsValid = true;
                        obj.IsPublishd = true;
                        discussionId = obj.Id;
                        var drm = (DALReturnModel<ForumDiscussion>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ForumDiscussion),
                                       new Func<ForumDiscussion, BatchProcessResultModel, DALReturnModel<ForumDiscussion>>(ForumDiscussionDAL.Update), obj, bpr);
                        bpr = drm.BPR; 
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(model.FormObject.Title.StringNormalizer()))
                        {
                            obj = new ForumDiscussion
                            {
                                Body = model.FormObject.Body.StringNormalizer(),
                                Title = model.FormObject.Title.StringNormalizer(),
                                ForumId = model.FormObject.ForumId,
                                CreateDate = DateTime.Now,
                                UserId = CurrentUser.Id,
                                IsPublishd = true,
                                IsValid = true
                            };
                            var drm = (DALReturnModel<ForumDiscussion>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ForumDiscussion),
                                       new Func<ForumDiscussion, BatchProcessResultModel, DALReturnModel<ForumDiscussion>>(ForumDiscussionDAL.Add), obj, bpr);
                            bpr = drm.BPR;
                            discussionId = drm.ReturnObject.Id;
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FormObject.Title.StringNormalizer()))
                    {
                        var obj = new ForumDiscussion
                        {
                            Body = model.FormObject.Body.StringNormalizer(),
                            Title = model.FormObject.Title.StringNormalizer(),
                            ForumId = model.FormObject.ForumId,
                            CreateDate = DateTime.Now,
                            UserId = CurrentUser.Id,
                            IsPublishd = true,
                            IsValid = true
                        };
                        var drm = (DALReturnModel<ForumDiscussion>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ForumDiscussion),
                                       new Func<ForumDiscussion, BatchProcessResultModel, DALReturnModel<ForumDiscussion>>(ForumDiscussionDAL.Add), obj, bpr);
                        bpr = drm.BPR;
                        discussionId = drm.ReturnObject.Id;
                    }
                }
                if (discussionId > 0)
                {
                    if (topicIds != null)
                    {
                        foreach (var item in topicIds)
                        {
                            var topicMapper = new ObjectTopicMapper
                            {
                                ObjectId = discussionId,
                                ObjectType = (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion,
                                TopicId = Int32.Parse(item)
                            };
                            var drm = (DALReturnModel<ObjectTopicMapper>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectTopicMapper),
                                    new Func<ObjectTopicMapper, BatchProcessResultModel, DALReturnModel<ObjectTopicMapper>>(ObjectTopicMapperDAL.Add), topicMapper, bpr);
                            bpr = drm.BPR;
                        }
                    }
                    //update Credit  
                    ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.Create, discussionId, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion, CurrentUser.Id);
                    UserInfoManager.UpdateUserActSummary(CurrentUser.Id, discussionId, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion, (int)UT.SL.Model.Enumeration.ActivityType.Create);

                    //update credit
                }
            }
            catch
            {
                return Content("0");
            }
            return Content(discussionId.ToString());
        }

        public ActionResult EditForumDiscussion(int id)
        {
            var model = new FormModel<ForumDiscussion, List<SelectListItems>>();
            model.FormObject = ForumDiscussionDAL.Get(id);
            model.ExtraKnownData = new List<SelectListItems>();
            var tempItems = new List<SelectListItem>();
            var topics = ObjectTopicMapperDAL.GetAll(model.FormObject.Id, (int)UT.SL.Model.Enumeration.ObjectType.Forum);
            tempItems.AddRange(topics.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Title
            }).ToList());
            model.ExtraKnownData.Add(new SelectListItems
            {
                Items = tempItems,
                SelectedIds = ObjectTopicMapperDAL.GetAll(id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion).Select(x => x.Id.ToString()).ToArray()
            });
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditForumDiscussion(FormModel<ForumDiscussion, List<SelectListItems>> model, string[] topicIds)
        {
            int discussionId = model.FormObject.Id;
            try
            {
                var obj = ForumDiscussionDAL.Get(model.FormObject.Id);
                if (obj != null)
                {
                    var bpr = new BatchProcessResultModel();
                    obj.Body = model.FormObject.Body.StringNormalizer();
                    obj.Title = model.FormObject.Title.StringNormalizer();
                    var drm = (DALReturnModel<ForumDiscussion>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ForumDiscussion),
                                   new Func<ForumDiscussion, BatchProcessResultModel, DALReturnModel<ForumDiscussion>>(ForumDiscussionDAL.Update), obj, bpr);
                    bpr = drm.BPR;
                    var shared = ObjectStreamManager.FindObjectWhichStream(discussionId, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);
                    var tobeMerged = new ToEmergeIds();

                    if (discussionId > 0)
                    {
                        ObjectTopicMapperDAL.DeleteObjectTopics(discussionId, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);

                        if (topicIds != null)
                        {
                            foreach (var item in topicIds)
                            {
                                var topicMapper = new ObjectTopicMapper
                                {
                                    ObjectId = discussionId,
                                    ObjectType = (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion,
                                    TopicId = Int32.Parse(item)
                                };
                                var drm2 = (DALReturnModel<ObjectTopicMapper>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectTopicMapper),
                                      new Func<ObjectTopicMapper, BatchProcessResultModel, DALReturnModel<ObjectTopicMapper>>(ObjectTopicMapperDAL.Add), topicMapper, bpr);
                                bpr = drm2.BPR;
                            }
                        }
                    }
                }
            }
            catch
            {
                return Content("0");
            }
            return Content(discussionId.ToString());
        }

        public ActionResult GetOneDiscussion(int id, DateTime? clickedDate )
        {
            var model = new ForumDiscussionsModel();
            model.Discussion = ForumDiscussionDAL.Get(id);
            model.Posts = model.Discussion.ForumDiscussionPosts.ToList();
            if (clickedDate.HasValue)
                ViewBag.clickedDate = clickedDate.Value;
            return PartialView(model);
        }

        public ActionResult Discussions(int id, DateTime? clickedDate)
        {
            var model = new List<ForumDiscussionsModel>();
            var discussions = ForumDiscussionDAL.GetAllDiscussions(id).OrderBy(x => x.CreateDate);
            foreach (var item in discussions)
            {
                var replies = item.ForumDiscussionPosts.OrderBy(x => x.CreateDate).ToList();
                model.Add(new ForumDiscussionsModel
                {
                    Discussion = item,
                    Posts = replies
                });
            }
            if (clickedDate.HasValue)
                ViewBag.clickedDate = clickedDate.Value;
            return PartialView(model);
        }

        public ActionResult DiscussionRepliesThread(int id, DateTime? clickedDate)
        {
            var model = new List<ForumDiscussionsModel>();
            var discussions = ForumDiscussionDAL.GetAllDiscussions(id).OrderBy(x => x.CreateDate);
            foreach (var item in discussions)
            {
                var replies = item.ForumDiscussionPosts.OrderBy(x => x.CreateDate).ToList();
                model.Add(new ForumDiscussionsModel
                {
                    Discussion = item,
                    Posts = replies
                });
            }
            if (clickedDate.HasValue)
                ViewBag.clickedDate = clickedDate.Value;
            return PartialView(model);
        }



        public ActionResult GetReplyForm(int id)
        {
            var model = ForumDiscussionDAL.Get(id); 
            model.ParentId = model.Id;
            model.Body = string.Empty;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult PostDiscussionReply(ForumDiscussion model)
        {
            int discussionId = 0;
            try
            {
                var bpr = new BatchProcessResultModel();
                if (model.Id > 0)
                {
                    var obj = ForumDiscussionDAL.Get(model.Id);
                    if (obj != null)
                    {
                        obj.Body = model.Body.StringNormalizer();
                        obj.ParentId = model.ParentId;
                        obj.IsValid = true;
                        obj.IsPublishd = true;
                        discussionId = obj.Id;
                        var drm = (DALReturnModel<ForumDiscussion>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ForumDiscussion),
                                      new Func<ForumDiscussion, BatchProcessResultModel, DALReturnModel<ForumDiscussion>>(ForumDiscussionDAL.Update), obj, bpr);
                        bpr = drm.BPR; 
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(model.Body.StringNormalizer()))
                        {
                            obj = new ForumDiscussion
                            {
                                Body = model.Body.StringNormalizer(),
                                ForumId = model.ForumId,
                                ParentId = model.ParentId,
                                CreateDate = DateTime.Now,
                                UserId = CurrentUser.Id,
                                IsPublishd = true,
                                IsValid = true
                            };
                            var drm = (DALReturnModel<ForumDiscussion>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ForumDiscussion),
                                       new Func<ForumDiscussion, BatchProcessResultModel, DALReturnModel<ForumDiscussion>>(ForumDiscussionDAL.Add), obj, bpr);
                            bpr = drm.BPR;
                            discussionId = drm.ReturnObject.Id;
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.Body.StringNormalizer()))
                    {
                        var obj = new ForumDiscussion
                        {
                            Body = model.Body.StringNormalizer(),
                            ForumId = model.ForumId,
                            ParentId = model.ParentId,
                            CreateDate = DateTime.Now,
                            UserId = CurrentUser.Id,
                            IsPublishd = true,
                            IsValid = true
                        };
                        var drm = (DALReturnModel<ForumDiscussion>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ForumDiscussion),
                                       new Func<ForumDiscussion, BatchProcessResultModel, DALReturnModel<ForumDiscussion>>(ForumDiscussionDAL.Add), obj, bpr);
                        bpr = drm.BPR;
                        discussionId = drm.ReturnObject.Id;
                        ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.Create, discussionId, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion, CurrentUser.Id);
                        UserInfoManager.UpdateUserActSummary(CurrentUser.Id, discussionId, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion, (int)UT.SL.Model.Enumeration.ActivityType.Create);
                    }
                }
            }
            catch
            {
                return Content("0");
            }
            return Content(discussionId.ToString());
        }

        public ActionResult GetDiscussionsCount(int id)
        {
            ViewBag.Count = ForumDiscussionDAL.GetAllDiscussions(id).Count - 1;
            return PartialView();
        }

        public ActionResult ViewImageResource(int Id)
        {
            var model = ForumDiscussionDAL.Get(Id);
            return File(model.FileContent.ToArray(), model.FileMime);
        }

        public ActionResult DownloadResource(int Id)
        {
            var model = ForumDiscussionDAL.Get(Id);
            return File(model.FileContent.ToArray(), model.FileMime, model.FileTitle);
        }

        public ActionResult GetLastChanged(int Id)
        {
            var model = ForumDiscussionDAL.GetAllDiscussions(Id);
            var date = DateTime.Now;
            date = model.Where(x => x.CreateDate.HasValue).Max(x => x.CreateDate).Value;
            ViewBag.Date = date;
            return PartialView();
        }

        public ActionResult GradeComponent(int id, int type, int grade = -1)
        {
            var model = new FormModel<ForumDiscussionPostsGrade, Forum>();
            //if (type == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion)
            //{
            //    model.ExtraKnownData = ForumDiscussionDAL.Get(id).Forum;
            //    var gradeObject = ForumDiscussionPostGradeDAL.GetWithDiscussion(id);
            //    if (gradeObject != null)
            //    {
            //        if (grade != -1)
            //        {
            //            if (gradeObject.grade == grade)
            //            {
            //                ForumDiscussionPostGradeDAL.Delete(gradeObject);
            //                //update Credit
            //                ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.Grade, id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion,0, -(gradeObject.grade));
            //                //update credit
            //                model.FormObject = new ForumDiscussionPostsGrade
            //                {
            //                    discussionId = id
            //                };
            //            }
            //            else
            //            {
            //                //update Credit
            //                ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.Grade, id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion, 0, grade-gradeObject.grade);
            //                //update credit
            //                gradeObject.grade = grade;
            //                ForumDiscussionPostGradeDAL.Update(gradeObject);
            //                model.FormObject = gradeObject;
            //            }
            //        }
            //        else
            //        {
            //            model.FormObject = gradeObject;
            //        }
            //    }
            //    else
            //    {
            //        if (grade != -1)
            //        {
            //            var newGrade = new ForumDiscussionPostsGrade
            //            {
            //                discussionId = id,
            //                userId = CurrentUser.Id,
            //                submitDate = DateTime.Now,
            //                grade = grade
            //            };
            //            ForumDiscussionPostGradeDAL.Add(newGrade);
            //            //update Credit
            //            ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.Grade, id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion, 0,grade);
            //            //update credit
            //            model.FormObject = newGrade;
            //        }
            //        else
            //        {
            //            model.FormObject = new ForumDiscussionPostsGrade
            //            {
            //                discussionId = id
            //            };
            //        }
            //    }
            //}
            //else if (type == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussionPost)
            //{
            //    model.ExtraKnownData = ForumDiscussionPostDAL.Get(id).ForumDiscussion.Forum;
            //    var gradeObject = ForumDiscussionPostGradeDAL.GetWithDiscussionReply(id);
            //    if (gradeObject != null)
            //    {
            //        if (grade != -1)
            //        {
            //            if (gradeObject.grade == grade)
            //            {
            //                ForumDiscussionPostGradeDAL.Delete(gradeObject);
            //                //update Credit
            //                ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.Grade, id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussionPost, 0, -(gradeObject.grade));
            //                //update credit
            //                model.FormObject = new ForumDiscussionPostsGrade
            //                {
            //                    postId = id
            //                };
            //            }
            //            else
            //            {
            //                //update Credit
            //                ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.Grade, id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussionPost,0, grade - gradeObject.grade);
            //                //update credit
            //                gradeObject.grade = grade;
            //                ForumDiscussionPostGradeDAL.Update(gradeObject);
            //                model.FormObject = gradeObject;
            //            }
            //        }
            //        else
            //        {
            //            model.FormObject = gradeObject;
            //        }
            //    }
            //    else
            //    {
            //        if (grade != -1)
            //        {
            //            var newGrade = new ForumDiscussionPostsGrade
            //            {
            //                postId = id,
            //                userId = CurrentUser.Id,
            //                submitDate = DateTime.Now,
            //                grade = grade
            //            };
            //            ForumDiscussionPostGradeDAL.Add(newGrade);
            //            //update Credit
            //            ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.Grade, id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussionPost, 0, grade);
            //            //update credit
            //            model.FormObject = newGrade;
            //        }
            //        else
            //        {
            //            model.FormObject = new ForumDiscussionPostsGrade
            //            {
            //                postId = id
            //            };
            //        }
            //    }
            //}
            return PartialView(model);
        }


        [HttpPost]
        public ActionResult DeleteFile(int id)
        {
            ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ForumDiscussion),
                                      new Func<ForumDiscussion, DALReturnModel<ForumDiscussion>>(ForumDiscussionDAL.DeleteFile), new ForumDiscussion { Id = id });                                 
            return Content("true");
        }


        public ActionResult CountNewDiscussions(int Id, int type, DateTime clickedDate)
        {
            var cnt = ForumDiscussionDAL.CountNewDiscussions(Id, type, clickedDate, CurrentUser.Id);
            if (cnt > 0)
                ViewBag.Count = cnt;
            return PartialView();
        }

        public ActionResult CountNewReplies(int Id, int type, DateTime clickedDate)
        {
            var cnt = ForumDiscussionDAL.CountNewReplies(Id, type, clickedDate, CurrentUser.Id);
            if (cnt > 0)
                ViewBag.Count = cnt;
            return PartialView();
        }

        public ActionResult CountNewRepliesForOneDiscussion(int Id, DateTime clickedDate)
        {
            var cnt = ForumDiscussionDAL.CountNewRepliesForOneDiscussion(Id, clickedDate, CurrentUser.Id);
            if (cnt > 0)
                ViewBag.Count = cnt;
            return PartialView();
        }
    }
}
