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
using UT.SL.Model.Enumeration;


namespace UT.SL.UI.WebUI.Areas.Admin.Controllers
{


    [Authorize()]
    public class ForumController : BaseController
    {

        public ActionResult ForumPost(Course obj)
        {
            var model = new FormModel<Forum, List<SelectListItems>>();
            model.FormObject = new Forum
            {
                CourseId = obj.Id,
                GetLockedAfterExpiration = false,
                GradeFrom = 0
            };
            model.ExtraKnownData = new List<SelectListItems>();
            var tempItems = new List<SelectListItem>();
            tempItems.AddRange(CourseDAL.Get(obj.Id).CourseAbstract.CourseTopcMappers.Select(x => new SelectListItem
            {
                Value = x.TopicId.ToString(),
                Text = x.Topic.Title
            }).ToList());
            model.ExtraKnownData.Add(new SelectListItems
            {
                Items = tempItems
            });
            tempItems = new List<SelectListItem>();
            tempItems.Add(new SelectListItem
            {
                Text = CourseDAL.Get(obj.Id).Title,
                Value = "D_" + obj.Id
            });
            tempItems.AddRange(LearningGroupDAL.GetAllByCourse(obj.Id).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Title
            }).ToList());
            model.ExtraKnownData.Add(new SelectListItems
            {
                Items = tempItems,
                SelectedIds = new string[] { "D_" + obj.Id }
            });
            ViewBag.UserId = CurrentUser.Id;
            return PartialView(model);
        }

        public ActionResult SaveForumFile(HttpPostedFileBase forumFile, int id, int? overwite)
        {
            int fileId = 0;
            try
            {
                if (forumFile != null)
                {
                    if (overwite.HasValue)
                    {
                        var bpr = new BatchProcessResultModel();
                        var forum = ForumDAL.Get(id);
                        forum.FileMime = forumFile.ContentType;
                        forum.FileSize = forumFile.ContentLength;
                        forum.FileTitle = forumFile.FileName;
                        byte[] tempFile = null;
                        tempFile = new byte[forumFile.ContentLength];
                        forumFile.InputStream.Read(tempFile, 0, forumFile.ContentLength);
                        forum.FileContent = tempFile;
                        var drm = (DALReturnModel<Forum>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Forum),
                                       new Func<Forum, BatchProcessResultModel, DALReturnModel<Forum>>(ForumDAL.UpdateFile), forum, bpr);
                        bpr = drm.BPR;                       
                    }
                    else
                    {
                        var newFile = new Forum
                        {
                            CreateDate = DateTime.Now,
                            FileMime = forumFile.ContentType,
                            FileSize = forumFile.ContentLength,
                            FileTitle = forumFile.FileName,
                            IsValid = false,
                            IsPublishd = false,
                            CreateUserId = CurrentUser.Id,
                            CourseId = id
                        };

                        byte[] tempFile = null;
                        tempFile = new byte[forumFile.ContentLength];
                        forumFile.InputStream.Read(tempFile, 0, forumFile.ContentLength);
                        newFile.FileContent = tempFile;

                        var bpr = new BatchProcessResultModel();
                        newFile.Title = "NotYetPublished";
                        var drm = (DALReturnModel<Forum>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Forum),
                                       new Func<Forum, BatchProcessResultModel, DALReturnModel<Forum>>(ForumDAL.Add), newFile, bpr);
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
        public ActionResult RemoveForumFile(int id)
        {
            try
            {
                ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Forum),
                                       new Func<Forum, DALReturnModel<Forum>>(ForumDAL.Delete), new Forum { Id = id });
            }
            catch
            {
                return Content("0");
            }
            return Content("2");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostForum(FormModel<Forum, List<SelectListItems>> model, string[] topicIds, string[] groupIds)
        {
            int forumId = 0;
            try
            {
                var bpr = new BatchProcessResultModel();
                if (model.FormObject.Id > 0)
                {
                    var obj = ForumDAL.Get(model.FormObject.Id);
                    if (obj != null)
                    {
                        obj.Body = model.FormObject.Body.StringNormalizer();
                        obj.Title = model.FormObject.Title.StringNormalizer();
                        obj.IsValid = true;
                        obj.IsPublishd = true;
                        obj.GradeFrom = model.FormObject.GradeFrom;
                        obj.GetLockedAfterExpiration = model.FormObject.GetLockedAfterExpiration;
                        obj.DueDate = DateUtils.WesternizeDateTime(model.FormObject.DueDate);
                        forumId = obj.Id;
                        var drm = (DALReturnModel<Forum>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Forum),
                                       new Func<Forum, BatchProcessResultModel, DALReturnModel<Forum>>(ForumDAL.Update), obj, bpr);
                        bpr = drm.BPR;  
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(model.FormObject.Title.StringNormalizer()))
                        {
                            obj = new Forum
                            {
                                Body = model.FormObject.Body.StringNormalizer(),
                                Title = model.FormObject.Title.StringNormalizer(),
                                CourseId = model.FormObject.CourseId,
                                CreateDate = DateTime.Now,
                                CreateUserId = CurrentUser.Id,
                                IsPublishd = true,
                                IsValid = true,
                                GradeFrom = model.FormObject.GradeFrom,
                                GetLockedAfterExpiration = model.FormObject.GetLockedAfterExpiration,
                                DueDate = DateUtils.WesternizeDateTime(model.FormObject.DueDate)
                            };
                            var drm = (DALReturnModel<Forum>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Forum),
                                      new Func<Forum, BatchProcessResultModel, DALReturnModel<Forum>>(ForumDAL.Add), obj, bpr);
                            bpr = drm.BPR;
                            forumId = drm.ReturnObject.Id;
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FormObject.Title.StringNormalizer()))
                    {
                        var obj = new Forum
                        {
                            Body = model.FormObject.Body.StringNormalizer(),
                            Title = model.FormObject.Title.StringNormalizer(),
                            CourseId = model.FormObject.CourseId,
                            CreateDate = DateTime.Now,
                            CreateUserId = CurrentUser.Id,
                            IsPublishd = true,
                            IsValid = true,
                            GradeFrom = model.FormObject.GradeFrom,
                            GetLockedAfterExpiration = model.FormObject.GetLockedAfterExpiration,
                            DueDate = DateUtils.WesternizeDateTime(model.FormObject.DueDate)
                        };
                        var drm = (DALReturnModel<Forum>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Forum),
                                      new Func<Forum, BatchProcessResultModel, DALReturnModel<Forum>>(ForumDAL.Add), obj, bpr);
                        bpr = drm.BPR;
                        forumId = drm.ReturnObject.Id;
                    }
                }
                if (forumId > 0)
                {
                    if (groupIds != null && !groupIds.Any(x => x.StartsWith("D_")))
                        ObjectStreamManager.ObjectResourceToStream(forumId, (int)UT.SL.Model.Enumeration.ObjectType.Forum, new ToEmergeIds { LearningGroups = groupIds.ToList() }, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
                    else
                        ObjectStreamManager.ObjectResourceToStream(forumId, (int)UT.SL.Model.Enumeration.ObjectType.Forum, new ToEmergeIds(), ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
                    if (topicIds != null)
                    {
                        foreach (var item in topicIds)
                        {
                            var topicMapper = new ObjectTopicMapper
                            {
                                ObjectId = forumId,
                                ObjectType = (int)(int)UT.SL.Model.Enumeration.ObjectType.Forum,
                                TopicId = Int32.Parse(item)
                            };
                            var drm = (DALReturnModel<ObjectTopicMapper>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectTopicMapper),
                                      new Func<ObjectTopicMapper, BatchProcessResultModel, DALReturnModel<ObjectTopicMapper>>(ObjectTopicMapperDAL.Add), topicMapper, bpr);
                            bpr = drm.BPR;
                        }
                    }
                    //update Credit
                    ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.Create, forumId, (int)UT.SL.Model.Enumeration.ObjectType.Forum, CurrentUser.Id);
                    UserInfoManager.UpdateUserActSummary(CurrentUser.Id, forumId, (int)UT.SL.Model.Enumeration.ObjectType.Forum, (int)UT.SL.Model.Enumeration.ActivityType.Create);
                    //update credit
                }
            }
            catch
            {
                return Content("0");
            }
            return Content(forumId.ToString());
        }

        public ActionResult EditForum(int ObjectId, int ObjecType)
        {
            var model = new FormModel<Forum, List<SelectListItems>>();
            model.FormObject = ForumDAL.Get(ObjectId);
            model.ExtraKnownData = new List<SelectListItems>();
            var tempItems = new List<SelectListItem>();
            tempItems.AddRange(model.FormObject.Course.CourseAbstract.CourseTopcMappers.Select(x => new SelectListItem
            {
                Value = x.TopicId.ToString(),
                Text = x.Topic.Title
            }).ToList());
            model.ExtraKnownData.Add(new SelectListItems
            {
                Items = tempItems,
                SelectedIds = ObjectTopicMapperDAL.GetAll(ObjectId, ObjecType).Select(x => x.Id.ToString()).ToArray()
            });
            tempItems = new List<SelectListItem>();
            tempItems.Add(new SelectListItem
            {
                Text = model.FormObject.Course.Title,
                Value = "D_" + model.FormObject.Course.Id
            });
            var shared = ObjectStreamManager.FindObjectWhichStream(ObjectId, ObjecType);
            var sharedIds = new List<string>();
            foreach (var item in shared.CourseIds)
            {
                sharedIds.Add("D_" + item);
            }
            foreach (var item in shared.LearningGroups)
            {
                sharedIds.Add(item.ToString());
            }
            tempItems.AddRange(LearningGroupDAL.GetAllByCourse(model.FormObject.Course.Id).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Title
            }).ToList());
            model.ExtraKnownData.Add(new SelectListItems
            {
                Items = tempItems,
                SelectedIds = sharedIds.ToArray()
            });
            ViewBag.UserId = CurrentUser.Id;
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditForum(FormModel<Forum, List<SelectListItems>> model, string[] topicIds, string[] groupIds)
        {
            int forumId = model.FormObject.Id;
            try
            {
                var obj = ForumDAL.Get(model.FormObject.Id);
                if (obj != null)
                {
                    var bpr = new BatchProcessResultModel();
                    obj.Body = model.FormObject.Body;
                    obj.Title = model.FormObject.Title;
                    obj.GradeFrom = model.FormObject.GradeFrom;
                    obj.GetLockedAfterExpiration = model.FormObject.GetLockedAfterExpiration;
                    obj.DueDate = DateUtils.WesternizeDateTime(model.FormObject.DueDate);
                    var drm = (DALReturnModel<Forum>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Forum),
                                     new Func<Forum, BatchProcessResultModel, DALReturnModel<Forum>>(ForumDAL.Update), obj, bpr);
                    bpr = drm.BPR;
                    var shared = ObjectStreamManager.FindObjectWhichStream(forumId, (int)UT.SL.Model.Enumeration.ObjectType.Forum);
                    ObjectStreamManager.UpdateEditedObjectStream(forumId, (int)UT.SL.Model.Enumeration.ObjectType.Forum, CurrentUser.Id);

                    var tobeMerged = new ToEmergeIds();

                    if (forumId > 0)
                    {
                        //if (groupIds != null && !groupIds.Any(x => x.StartsWith("D_")))
                        //{
                        //    ObjectStreamManager.ObjectResourceToStream(forumId, (int)UT.SL.Model.Enumeration.ObjectType.Forum, new ToEmergeIds { LearningGroups = groupIds.ToList() });
                        //}
                        //else
                        //{
                        //    ObjectStreamManager.ObjectResourceToStream(forumId, (int)UT.SL.Model.Enumeration.ObjectType.Forum, new ToEmergeIds());
                        //}
                        ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectTopicMapper),
                                       new Func<int, int, DALReturnModel<ObjectTopicMapper>>(ObjectTopicMapperDAL.DeleteObjectTopics), forumId, (int)ObjectType.Forum);
           
                        if (topicIds != null)
                        {
                            foreach (var item in topicIds)
                            {
                                var topicMapper = new ObjectTopicMapper
                                {
                                    ObjectId = forumId,
                                    ObjectType = (int)(int)UT.SL.Model.Enumeration.ObjectType.Forum,
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
            return Content(forumId.ToString());
        }

        public ActionResult ForumViewer(int id, DateTime? clickedDate, bool? isEdited)
        {
            var model = ForumDAL.Get(id);
            if (clickedDate.HasValue)
                ViewBag.clickedDate = clickedDate.Value;
            if (isEdited.HasValue && isEdited.Value)
                ViewBag.IsEdited = true;
            return PartialView(model);
        }

        public ActionResult DiscussionThread(int id, DateTime? clickedDate)
        {
            var forum = ForumDAL.Get(id);
            var model = forum.ForumDiscussions.Where(x => !x.ParentId.HasValue).ToList();
            ViewBag.Id = id;
            if (clickedDate.HasValue)
                ViewBag.clickedDate = clickedDate.Value;
            return PartialView(model);
        }

        public ActionResult DiscussionThreadItems(int id, DateTime? clickedDate)
        {
            var forum = ForumDAL.Get(id);
            var model = forum.ForumDiscussions.Where(x => !x.ParentId.HasValue).ToList();
            ViewBag.Id = id;
            if (clickedDate.HasValue)
                ViewBag.clickedDate = clickedDate.Value;
            return PartialView(model);
        }

        public ActionResult GetOneForumDiscussion(int id, DateTime? clickedDate)
        {
            var model = ForumDiscussionDAL.Get(id); ;
            if (clickedDate.HasValue)
                ViewBag.clickedDate = clickedDate.Value;
            return PartialView(model);
        }

        public ActionResult GetUserPosters(int id)
        {
            //var forumDiscussion = ForumDAL.GetDiscussion(id);
            var model = new DiscussionPosterModel();
            var replies = ForumDiscussionDAL.GetDiscussionReplies(id);

            if (replies != null && replies.Any())
            {
                model.TotalPosts = replies.Count();
                if (replies.Any(x => x.App_User.UserPic != null))
                {
                    model.Users = replies.OrderBy(x => x.CreateDate)
                                                                      .Where(x => x.App_User.UserPic != null)
                                                                      .Select(x => x.App_User).Distinct().Take(2).ToList();
                    var posters = replies.Select(x => x.App_User).Distinct().Count();
                    model.TheRemainingUserCount = posters - model.Users.Count();
                }
                else
                {
                    model.Users = null;
                    model.TheRemainingUserCount = 0;
                }
            }
            else
            {
                model.TotalPosts = 0;
                model.Users = null;
                model.TheRemainingUserCount = 0;
            }

            return PartialView(model);
        }

        public ActionResult ViewImageResource(int Id)
        {
            var model = ForumDAL.Get(Id);
            return File(model.FileContent.ToArray(), model.FileMime);
        }

        public ActionResult DownloadResource(int Id)
        {
            var model = ForumDAL.Get(Id);
            return File(model.FileContent.ToArray(), model.FileMime, model.FileTitle);
        }

        [HttpPost]
        public ActionResult DeleteForum(int ObjectId, int ObjecType)
        {
            ManageObject.DeleteObject(ObjectId, ObjecType, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
            return Content("true");
        }

        [HttpPost]
        public ActionResult DeleteFile(int id)
        {
            ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Forum),
                                       new Func<Forum, DALReturnModel<Forum>>(ForumDAL.DeleteFile), new Forum { Id = id });           
            return Content("true");
        }

        public ActionResult ForumPostPanel(Course obj)
        {
            return PartialView(obj);
        }

        public ActionResult ForumPostButton()
        {
            return PartialView();
        }

    }
}
