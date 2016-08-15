using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UT.SL.BLL;
using UT.SL.DAL;
using UT.SL.Data.LINQ;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Model.Enumeration;
using UT.SL.UI.WebUI.Controllers;
using ViewModels.AssignmentSubmission;

namespace UT.SL.UI.WebUI.Areas.Admin.Controllers
{

    [Authorize()]
    public class AssignmentController : BaseController
    {
        public ActionResult AssignmentSearchModelView(AssignmentSearchModel model)
        {
            model.Area = "Admin";
            return PartialView(model);
        }

        public ActionResult AssignmentPost(Course obj)
        {
            //var model = new FormModel<Assignment, List<SelectListItems>>();
            //model.FormObject = new Assignment
            //{
            //    CourseId = obj.Id,
            //    GetLockedAfterExpiration = false,
            //    GradeFrom = 0
            //};
            //model.ExtraKnownData = new List<SelectListItems>();
            //var tempItems = new List<SelectListItem>();
            //tempItems.AddRange(CourseDAL.Get(obj.Id).CourseAbstract.CourseTopcMappers.Select(x => new SelectListItem
            //{
            //    Value = x.TopicId.ToString(),
            //    Text = x.Topic.Title
            //}).ToList());
            //model.ExtraKnownData.Add(new SelectListItems
            //{
            //    Items = tempItems
            //});

            //tempItems = new List<SelectListItem>();

            //tempItems.AddRange(LearningGroupDAL.GetAllByCourse(obj.Id).Select(x => new SelectListItem
            //{
            //    Value = x.Id.ToString(),
            //    Text = x.Title
            //}).ToList());
            //model.ExtraKnownData.Add(new SelectListItems
            //{
            //    Items = tempItems

            //});
            //ViewBag.UserId = CurrentUser.Id;

            //return PartialView(model);

            var model = new FormModel<Assignment, List<SelectListItems>>();
            model.FormObject = new Assignment
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostAssignment(FormModel<Assignment, List<SelectListItems>> model, string[] topicIds, string[] groupIds)
        {
            int assignmentId = 0;
            try
            {
                var bpr = new BatchProcessResultModel();
                if (model.FormObject.Id > 0)
                {
                    var obj = AssignmentDAL.Get(model.FormObject.Id);
                    if (obj != null)
                    {
                        obj.Body = model.FormObject.Body.StringNormalizer();
                        obj.Title = model.FormObject.Title.StringNormalizer();
                        obj.IsValid = true;
                        obj.IsPublished = true;
                        obj.GradeFrom = model.FormObject.GradeFrom;
                        obj.GetLockedAfterExpiration = model.FormObject.GetLockedAfterExpiration;
                        obj.DueDate = model.FormObject.DueDate;
                        obj.GroupSubmissionIsDemanded = model.FormObject.GroupSubmissionIsDemanded;
                        assignmentId = obj.Id;
                        var drm = (DALReturnModel<Assignment>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Assignment),
                                      new Func<Assignment, BatchProcessResultModel, DALReturnModel<Assignment>>(AssignmentDAL.Update), obj, bpr);
                        bpr = drm.BPR;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(model.FormObject.Title.StringNormalizer()))
                        {
                            obj = new Assignment
                            {
                                Body = model.FormObject.Body.StringNormalizer(),
                                Title = model.FormObject.Title.StringNormalizer(),
                                CourseId = model.FormObject.CourseId,
                                CreateDate = DateTime.Now,
                                CreateUserId = CurrentUser.Id,
                                IsPublished = true,
                                IsValid = true,
                                GradeFrom = model.FormObject.GradeFrom,
                                GetLockedAfterExpiration = model.FormObject.GetLockedAfterExpiration,
                                GroupSubmissionIsDemanded = model.FormObject.GroupSubmissionIsDemanded,
                                DueDate = model.FormObject.DueDate,
                                Status = (int)UT.SL.Model.Enumeration.ActivityStatusType.Open
                            };
                            var drm = (DALReturnModel<Assignment>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Assignment),
                                      new Func<Assignment, BatchProcessResultModel, DALReturnModel<Assignment>>(AssignmentDAL.Add), obj, bpr);
                            bpr = drm.BPR;
                            assignmentId = drm.ReturnObject.Id;

                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FormObject.Title.StringNormalizer()))
                    {
                        var obj = new Assignment
                        {
                            Body = model.FormObject.Body.StringNormalizer(),
                            Title = model.FormObject.Title.StringNormalizer(),
                            CourseId = model.FormObject.CourseId,
                            CreateDate = DateTime.Now,
                            CreateUserId = CurrentUser.Id,
                            IsPublished = true,
                            IsValid = true,
                            GradeFrom = model.FormObject.GradeFrom,
                            GetLockedAfterExpiration = model.FormObject.GetLockedAfterExpiration,
                            GroupSubmissionIsDemanded = model.FormObject.GroupSubmissionIsDemanded,
                            DueDate = model.FormObject.DueDate,
                            Status = (int)UT.SL.Model.Enumeration.ActivityStatusType.Open
                        };
                        var drm = (DALReturnModel<Assignment>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Assignment),
                                      new Func<Assignment, BatchProcessResultModel, DALReturnModel<Assignment>>(AssignmentDAL.Add), obj, bpr);
                        bpr = drm.BPR;
                        assignmentId = drm.ReturnObject.Id;
                    }
                }

                if (assignmentId > 0)
                {
                    if (groupIds != null && !groupIds.Any(x => x.StartsWith("D_")))
                        ObjectStreamManager.ObjectResourceToStream(assignmentId, (int)UT.SL.Model.Enumeration.ObjectType.Assignment, new ToEmergeIds { LearningGroups = groupIds.ToList() }, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
                    else
                        ObjectStreamManager.ObjectResourceToStream(assignmentId, (int)UT.SL.Model.Enumeration.ObjectType.Assignment, new ToEmergeIds(), ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
                    if (topicIds != null)
                    {
                        foreach (var item in topicIds)
                        {
                            var topicMapper = new ObjectTopicMapper
                            {
                                ObjectId = assignmentId,
                                ObjectType = (int)(int)UT.SL.Model.Enumeration.ObjectType.Assignment,
                                TopicId = Int32.Parse(item)
                            };
                            var drm = (DALReturnModel<ObjectTopicMapper>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectTopicMapper),
                                     new Func<ObjectTopicMapper, BatchProcessResultModel, DALReturnModel<ObjectTopicMapper>>(ObjectTopicMapperDAL.Add), topicMapper, bpr);
                            bpr = drm.BPR;
                        }
                    }
                    //update Credit    
                    ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.Create, assignmentId, (int)UT.SL.Model.Enumeration.ObjectType.Assignment, CurrentUser.Id);
                    //update credit
                }
            }
            catch
            {
                return Content("0");
            }
            return Content(assignmentId.ToString());
        }

        public ActionResult SaveAssignmentFile(HttpPostedFileBase assignmentFile, int id, int? overwite)
        {
            int fileId = 0;
            try
            {
                if (assignmentFile != null)
                {
                    if (overwite.HasValue)
                    {
                        var bpr = new BatchProcessResultModel();
                        var assignment = AssignmentDAL.Get(id);
                        assignment.FileMime = assignmentFile.ContentType;
                        assignment.FileSize = assignmentFile.ContentLength;
                        assignment.FileTitle = assignmentFile.FileName;
                        byte[] tempFile = null;
                        tempFile = new byte[assignmentFile.ContentLength];
                        assignmentFile.InputStream.Read(tempFile, 0, assignmentFile.ContentLength);
                        assignment.FileContent = tempFile;
                        var drm = (DALReturnModel<Assignment>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Assignment),
                                      new Func<Assignment, BatchProcessResultModel, DALReturnModel<Assignment>>(AssignmentDAL.UpdateFile), assignment, bpr);
                        bpr = drm.BPR;
                    }
                    else
                    {
                        var newFile = new Assignment
                        {
                            CreateDate = DateTime.Now,
                            FileMime = assignmentFile.ContentType,
                            FileSize = assignmentFile.ContentLength,
                            FileTitle = assignmentFile.FileName,
                            IsValid = false,
                            IsPublished = false,
                            CreateUserId = CurrentUser.Id,
                            CourseId = id
                        };

                        byte[] tempFile = null;
                        tempFile = new byte[assignmentFile.ContentLength];
                        assignmentFile.InputStream.Read(tempFile, 0, assignmentFile.ContentLength);
                        newFile.FileContent = tempFile;

                        var bpr = new BatchProcessResultModel();
                        newFile.Title = "NotYetPublished";
                        var drm = (DALReturnModel<Assignment>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Assignment),
                                      new Func<Assignment, BatchProcessResultModel, DALReturnModel<Assignment>>(AssignmentDAL.Add), newFile, bpr);
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
        public ActionResult RemoveAssignmentFile(int id)
        {
            try
            {
                ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Assignment),
                                      new Func<Assignment, DALReturnModel<Assignment>>(AssignmentDAL.Delete), new Assignment { Id = id });
            }
            catch
            {
                return Content("0");
            }
            return Content("0");
        }

        public ActionResult AssignmentViewer(int id, DateTime? clickedDate, bool? isEdited)
        {
            var model = AssignmentDAL.Get(id);
            if (clickedDate.HasValue)
                ViewBag.clickedDate = clickedDate.Value;
            if (isEdited.HasValue && isEdited.Value)
                ViewBag.IsEdited = true;
            return PartialView(model);
        }

        public ActionResult ViewImageResource(int Id)
        {
            var model = AssignmentDAL.Get(Id);
            return File(model.FileContent.ToArray(), model.FileMime);
        }

        public ActionResult DownloadResource(int Id)
        {
            var model = AssignmentDAL.Get(Id);
            return File(model.FileContent.ToArray(), model.FileMime, model.FileTitle);
        }

        public ActionResult GetSpecificPanel(int id)
        {
            var model = AssignmentDAL.Get(id);

            var submissions = AssignmentSubmissionDAL.GetAllSubmissions(id, CurrentUser.Id);
            ViewBag.submissionsCount = submissions.Count();

            return PartialView(model);
        }

        public ActionResult ViewAllSubmissions(int id)
        {
            var assignment = AssignmentDAL.Get(id);
            var tempAssignmentSubmissions = assignment.AssignmentSubmissions.ToList();

            List<NewSubmissionViewModel> AssignmentSubmissions = new List<NewSubmissionViewModel>();
            foreach (var item in tempAssignmentSubmissions)
            {

                var comments = CommentDAL.GetAll(item.Id, (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission);

                var AssessParent = AssessParentDAL.GetAll(item.Id, (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission);

                var grade = GradeDAL.GetAll(item.Id, (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission);

                var assesses = new List<Assess>();
                if (AssessParent.Count != 0)
                    assesses = AssessDAL.GetListIfExist(AssessParent.FirstOrDefault().Id);

                NewSubmissionViewModel oNewSubmissionViewModel = new NewSubmissionViewModel();
                oNewSubmissionViewModel.AssignmentSubmission = item;
                if (comments.Count != 0)
                    oNewSubmissionViewModel.Comment = comments.FirstOrDefault();
                if (assesses.Count != 0)
                {
                    oNewSubmissionViewModel.SelfAssess = assesses.Where(a => a.AssessType == (int)UT.SL.Model.Enumeration.AssessType.self).LastOrDefault();
                    oNewSubmissionViewModel.ExpertAssess = assesses.Where(a => a.AssessType == (int)UT.SL.Model.Enumeration.AssessType.expert).LastOrDefault();
                }
                if (grade.Count != 0)
                    oNewSubmissionViewModel.Grade = grade.FirstOrDefault();
                AssignmentSubmissions.Add(oNewSubmissionViewModel);

            }

            ViewBag.Id = id;
            return PartialView(AssignmentSubmissions);
        }

        public ActionResult EditAssignment(int ObjectId, int ObjecType)
        {
            var model = new FormModel<Assignment, List<SelectListItems>>();
            model.FormObject = AssignmentDAL.Get(ObjectId);
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
        public ActionResult EditAssignment(FormModel<Assignment, List<SelectListItems>> model, string[] topicIds, string[] groupIds)
        {
            int assignmentId = model.FormObject.Id;
            try
            {
                var obj = AssignmentDAL.Get(model.FormObject.Id);
                if (obj != null)
                {
                    var bpr = new BatchProcessResultModel();
                    obj.Body = model.FormObject.Body;
                    obj.Title = model.FormObject.Title;
                    obj.GradeFrom = model.FormObject.GradeFrom;
                    obj.GetLockedAfterExpiration = model.FormObject.GetLockedAfterExpiration;
                    obj.DueDate = DateUtils.WesternizeDateTime(model.FormObject.DueDate);
                    obj.GroupSubmissionIsDemanded = model.FormObject.GroupSubmissionIsDemanded;
                    var drm = (DALReturnModel<Assignment>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Assignment),
                                      new Func<Assignment, BatchProcessResultModel, DALReturnModel<Assignment>>(AssignmentDAL.Update), obj, bpr);
                    bpr = drm.BPR;
                    var shared = ObjectStreamManager.FindObjectWhichStream(assignmentId, (int)UT.SL.Model.Enumeration.ObjectType.Assignment);
                    ObjectStreamManager.UpdateEditedObjectStream(assignmentId, (int)UT.SL.Model.Enumeration.ObjectType.Assignment, CurrentUser.Id);

                    var tobeMerged = new ToEmergeIds();

                    if (assignmentId > 0)
                    {
                        //if (groupIds != null && !groupIds.Any(x => x.StartsWith("D_")))
                        //{
                        //    ObjectStreamManager.ObjectResourceToStream(assignmentId, (int)UT.SL.Model.Enumeration.ObjectType.Assignment, new ToEmergeIds { LearningGroups = groupIds.ToList() });
                        //}
                        //else
                        //{
                        //    ObjectStreamManager.ObjectResourceToStream(assignmentId, (int)UT.SL.Model.Enumeration.ObjectType.Assignment, new ToEmergeIds());
                        //}
                        ObjectTopicMapperDAL.DeleteObjectTopics(assignmentId, (int)ObjectType.Assignment);
                        ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectTopicMapper),
                                      new Func<int, int, DALReturnModel<ObjectTopicMapper>>(ObjectTopicMapperDAL.DeleteObjectTopics), assignmentId, (int)ObjectType.Assignment);
                        if (topicIds != null)
                        {
                            foreach (var item in topicIds)
                            {
                                var topicMapper = new ObjectTopicMapper
                                {
                                    ObjectId = assignmentId,
                                    ObjectType = (int)(int)UT.SL.Model.Enumeration.ObjectType.Assignment,
                                    TopicId = Int32.Parse(item)
                                };
                                var drm2 = ((DALReturnModel<ObjectTopicMapper>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectTopicMapper),
                                             new Func<ObjectTopicMapper, BatchProcessResultModel, DALReturnModel<ObjectTopicMapper>>(ObjectTopicMapperDAL.Add), topicMapper, bpr));
                            }
                        }
                    }
                }
            }
            catch
            {
                return Content("0");
            }
            return Content(assignmentId.ToString());
        }

        public ActionResult OverDuedAssignment()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult DeleteFile(int id)
        {
            ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Assignment),
                                      new Func<Assignment, DALReturnModel<Assignment>>(AssignmentDAL.DeleteFile), new Assignment { Id = id });
            return Content("true");
        }

        public ActionResult AssignmentPostPanel(Course obj)
        {
            return PartialView(obj);
        }

        public ActionResult AssignmentPostButton()
        {
            return PartialView();
        }


    }
}
