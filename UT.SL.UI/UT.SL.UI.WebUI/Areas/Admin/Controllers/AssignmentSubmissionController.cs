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
//using UT.SL.UI.WebUI.ViewModels.AssignmentSubmission;
using ViewModels.AssignmentSubmission;

namespace UT.SL.UI.WebUI.Areas.Admin.Controllers
{
    [Authorize()]
    public class AssignmentSubmissionController : BaseController
    {
        //public ActionResult AddSubmission(int id)
        //{
        //    var model = AssignmentDAL.Get(id);
        //    var submissions = model.AssignmentSubmissions.Where(u => u.UserId == CurrentUser.Id);

        //    ViewBag.submissionsCount = submissions.Count();
        //    return PartialView(model);
        //}

        //public ActionResult ShowSubmissionThread(int id)
        //{
        //    var model = AssignmentDAL.Get(id);
        //    return PartialView(model);
        //}

        [HttpGet]
        public ActionResult PostSubmission(Assignment obj)
        {
            var assignment = AssignmentDAL.Get(obj.Id);
            var oModel = new ViewModels.AssignmentSubmission.NewSubmissionViewModel();
            AssignmentSubmission oAssignmentSubmission = new AssignmentSubmission
            {
                AssignmentId = obj.Id
            };
            Comment oComment = new Comment();

            oModel.AssignmentSubmission = oAssignmentSubmission;
            oModel.Comment = oComment;
            ViewBag.UserId = CurrentUser.Id;
            return PartialView(oModel);
        }

        public ActionResult EditAssignmentSubmission(int id)
        {
            var model = new ViewModels.AssignmentSubmission.NewSubmissionViewModel();
            model.AssignmentSubmission = AssignmentSubmissionDAL.Get(id);
            var comments = CommentDAL.GetAll(model.AssignmentSubmission.Id, (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission);
            var AssessParent = AssessParentDAL.GetAll(model.AssignmentSubmission.Id, (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission);
            var assesses = new List<Assess>();
            if (AssessParent.Count != 0)
                assesses = AssessDAL.GetListIfExist(AssessParent.FirstOrDefault().Id);
            if (comments.Count != 0)
                model.Comment = comments.FirstOrDefault();
            if (assesses.Count != 0)
            {
                model.SelfAssess = assesses.Where(a => a.AssessType == (int)UT.SL.Model.Enumeration.AssessType.self).LastOrDefault();
            }
            return PartialView(model);
        }

        public ActionResult EditGradeSubmission(int id, int gradeid)
        {
            var model = new ViewModels.AssignmentSubmission.NewSubmissionViewModel();
            model.AssignmentSubmission = AssignmentSubmissionDAL.Get(id);
            model.Grade = GradeDAL.Get(gradeid);
            var AssessParent = AssessParentDAL.GetAll(model.AssignmentSubmission.Id, (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission);
            var assesses = new List<Assess>();
            if (AssessParent.Count != 0)
                assesses = AssessDAL.GetListIfExist(AssessParent.FirstOrDefault().Id);
            if (assesses.Count != 0)
            {
                model.ExpertAssess = assesses.Where(a => a.AssessType == (int)UT.SL.Model.Enumeration.AssessType.expert).LastOrDefault();
            }
            return PartialView(model);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditAssignmentSubmission(FormModel<ForumDiscussion, List<SelectListItems>> model, string[] topicIds)
        //{
        //    int discussionId = model.FormObject.Id;
        //    try
        //    {
        //        var obj = ForumDiscussionDAL.Get(model.FormObject.Id);
        //        if (obj != null)
        //        {
        //            var bpr = new BatchProcessResultModel();
        //            obj.Body = model.FormObject.Body.StringNormalizer();
        //            obj.Title = model.FormObject.Title.StringNormalizer();
        //            ForumDiscussionDAL.Update(obj, out bpr);
        //            var shared = ObjectStreamManager.FindObjectWhichStream(discussionId, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);
        //            var tobeMerged = new ToEmergeIds();

        //            if (discussionId > 0)
        //            {
        //                ObjectTopicMapperDAL.DeleteObjectTopics(discussionId, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);

        //                if (topicIds != null)
        //                {
        //                    foreach (var item in topicIds)
        //                    {
        //                        var topicMapper = new ObjectTopicMapper
        //                        {
        //                            ObjectId = discussionId,
        //                            ObjectType = (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion,
        //                            TopicId = Int32.Parse(item)
        //                        };
        //                        ObjectTopicMapperDAL.Add(topicMapper, out bpr);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return Content("0");
        //    }
        //    return Content(discussionId.ToString());
        //}

        [HttpPost]
        public ActionResult PostSubmissionOld(FormModel<AssignmentSubmission, List<SelectListItems>> model, string[] voteIds)
        {
            int SubmissionId = 0;
            try
            {
                var bpr = new BatchProcessResultModel();
                if (model.FormObject.Id > 0)
                {
                    var obj = AssignmentSubmissionDAL.Get(model.FormObject.Id);
                    if (obj != null)
                    {
                        obj.Body = model.FormObject.Body.StringNormalizer();
                        //obj.Title = model.FormObject.Title.StringNormalizer();
                        obj.IsValid = true;
                        obj.IsPublishd = true;
                        SubmissionId = obj.Id;
                        var drm = ((DALReturnModel<AssignmentSubmission>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.AssignmentSubmission),
                           new Func<AssignmentSubmission, BatchProcessResultModel, DALReturnModel<AssignmentSubmission>>(AssignmentSubmissionDAL.Update), obj, bpr));
                        bpr = drm.BPR;
                    }
                    else
                    {
                        //if (!string.IsNullOrEmpty(model.FormObject.Title.StringNormalizer()))
                        //{
                        obj = new AssignmentSubmission
                            {
                                Body = model.FormObject.Body.StringNormalizer(),
                                //Title = model.FormObject.Title.StringNormalizer(),
                                AssignmentId = model.FormObject.AssignmentId,
                                CreateDate = DateTime.Now,
                                UserId = CurrentUser.Id,
                                IsPublishd = true,
                                IsValid = true
                            };
                        var drm = ((DALReturnModel<AssignmentSubmission>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.AssignmentSubmission),
                            new Func<AssignmentSubmission, BatchProcessResultModel, DALReturnModel<AssignmentSubmission>>(AssignmentSubmissionDAL.Add), obj, bpr));
                        SubmissionId = drm.ReturnObject.Id;
                        bpr = drm.BPR;
                        //}
                    }
                }
                else
                {
                    //if (!string.IsNullOrEmpty(model.FormObject.Title.StringNormalizer()))
                    //{
                    var obj = new AssignmentSubmission
                        {
                            Body = model.FormObject.Body.StringNormalizer(),
                            // Title = model.FormObject.Title.StringNormalizer(),
                            AssignmentId = model.FormObject.AssignmentId,
                            CreateDate = DateTime.Now,
                            UserId = CurrentUser.Id,
                            IsPublishd = true,
                            IsValid = true
                        };
                    var drm = ((DALReturnModel<AssignmentSubmission>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.AssignmentSubmission),
                            new Func<AssignmentSubmission, BatchProcessResultModel, DALReturnModel<AssignmentSubmission>>(AssignmentSubmissionDAL.Add), obj, bpr));
                    SubmissionId = drm.ReturnObject.Id;
                    bpr = drm.BPR;
                    //}
                }
                if (SubmissionId > 0)
                {
                    if (voteIds != null)
                    {
                        foreach (var item in voteIds)
                        {
                            var newComment = new Comment
                            {
                                CreateDate = DateTime.Now,
                                ObjectId = Int32.Parse(item.ToString()),
                                Type = (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission,
                                OwnerId = CurrentUser.Id,
                                Title = item.ToString()
                            };
                            ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Comment),
                             new Func<Comment, DALReturnModel<Comment>>(CommentDAL.Add), newComment);
                        }
                    }
                }
            }
            catch
            {
                return Content("0");
            }
            return Content(SubmissionId.ToString());
        }

        //tempSubmissionId for get submissionid from file upload
        [HttpPost]
        public ActionResult PostSubmission(ViewModels.AssignmentSubmission.NewSubmissionViewModel model, int id, int tempSubmissionId = 0)
        {
            int AssignmentSubmissionId = 0;
            int AssignmentId = 0;
            try
            {
                var bpr = new BatchProcessResultModel();
                AssignmentId = id;
                if (AssignmentId > 0)
                {
                    var obj = AssignmentSubmissionDAL.Get(tempSubmissionId);
                    if (obj != null)
                    {
                        obj.IsValid = true;
                        obj.IsPublishd = true;
                        AssignmentSubmissionId = obj.Id;
                        obj.Body = model.AssignmentSubmission.Body;
                        var drm = ((DALReturnModel<AssignmentSubmission>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.AssignmentSubmission),
                            new Func<AssignmentSubmission, BatchProcessResultModel, DALReturnModel<AssignmentSubmission>>(AssignmentSubmissionDAL.Update), obj, bpr));
                        bpr = drm.BPR;
                    }
                    else
                    {
                        obj = new AssignmentSubmission
                        {
                            AssignmentId = AssignmentId,
                            CreateDate = DateTime.Now,
                            UserId = CurrentUser.Id,
                            IsPublishd = true,
                            IsValid = true,
                            Body = model.AssignmentSubmission.Body
                        };
                        var drm = ((DALReturnModel<AssignmentSubmission>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.AssignmentSubmission),
                            new Func<AssignmentSubmission, BatchProcessResultModel, DALReturnModel<AssignmentSubmission>>(AssignmentSubmissionDAL.Add), obj, bpr));
                        bpr = drm.BPR;
                        AssignmentSubmissionId = drm.ReturnObject.Id;
                    }
                }
                else
                {
                    var obj = new AssignmentSubmission
                    {
                        AssignmentId = AssignmentId,
                        CreateDate = DateTime.Now,
                        UserId = CurrentUser.Id,
                        IsPublishd = true,
                        IsValid = true,
                        Body = model.AssignmentSubmission.Body
                    };
                    var drm = ((DALReturnModel<AssignmentSubmission>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.AssignmentSubmission),
                            new Func<AssignmentSubmission, BatchProcessResultModel, DALReturnModel<AssignmentSubmission>>(AssignmentSubmissionDAL.Add), obj, bpr));
                    bpr = drm.BPR;
                    AssignmentSubmissionId = drm.ReturnObject.Id;
                }
                if (AssignmentSubmissionId > 0)
                {
                    //string strComment = model.Comment.Title;
                    //if (strComment != null)
                    //{
                    //    var newComment = new Comment
                    //    {
                    //        CreateDate = DateTime.Now,
                    //        ObjectId = AssignmentSubmissionId,
                    //        Type = (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission,
                    //        OwnerId = CurrentUser.Id,
                    //        Title = strComment
                    //    };
                    //    CommentDAL.Add(newComment);
                    //}

                    int? AssessId = model.SelfAssess.AssessValue;
                    if (AssessId != 0)
                    {
                        AssessController assessController = new Controllers.AssessController();
                        assessController.Assess(AssessId.Value, CurrentUser.Id, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.AssessParent), AssignmentSubmissionId,
                                               (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission,
                                               (int)UT.SL.Model.Enumeration.AssessType.self);
                    }
                    //update Credit
                    ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.Create, AssignmentSubmissionId, (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission, CurrentUser.Id);
                    //update credit
                }
            }
            catch
            {
                return Content("0");
            }
            return Content(AssignmentSubmissionId.ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAssignmentSubmission(ViewModels.AssignmentSubmission.NewSubmissionViewModel model, int Id)
        {
            int AssignmentSubmissionId = Id;

            try
            {
                var bpr = new BatchProcessResultModel();

                if (AssignmentSubmissionId > 0)
                {
                    var obj = AssignmentSubmissionDAL.Get(Id);
                    if (obj != null)
                    {
                        obj.IsValid = true;
                        obj.IsPublishd = true;
                        obj.CreateDate = DateTime.Now;
                        AssignmentSubmissionId = obj.Id;
                        obj.Body = model.AssignmentSubmission.Body;
                        var drm = ((DALReturnModel<AssignmentSubmission>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.AssignmentSubmission),
                            new Func<AssignmentSubmission, BatchProcessResultModel, DALReturnModel<AssignmentSubmission>>(AssignmentSubmissionDAL.Update), obj, bpr));
                        bpr = drm.BPR;
                    }
                    else
                    {
                        obj = new AssignmentSubmission
                        {
                            AssignmentId = model.AssignmentSubmission.AssignmentId,
                            CreateDate = DateTime.Now,
                            UserId = CurrentUser.Id,
                            IsPublishd = true,
                            IsValid = true,
                            Body = model.AssignmentSubmission.Body
                        };
                        var drm = ((DALReturnModel<AssignmentSubmission>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.AssignmentSubmission),
                           new Func<AssignmentSubmission, BatchProcessResultModel, DALReturnModel<AssignmentSubmission>>(AssignmentSubmissionDAL.Add), obj, bpr));
                        bpr = drm.BPR;
                        AssignmentSubmissionId = drm.ReturnObject.Id;
                    }
                }
                else
                {
                    var obj = new AssignmentSubmission
                    {
                        AssignmentId = model.AssignmentSubmission.AssignmentId,
                        CreateDate = DateTime.Now,
                        UserId = CurrentUser.Id,
                        IsPublishd = true,
                        IsValid = true,
                        Body = model.AssignmentSubmission.Body
                    };
                    var drm = ((DALReturnModel<AssignmentSubmission>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.AssignmentSubmission),
                           new Func<AssignmentSubmission, BatchProcessResultModel, DALReturnModel<AssignmentSubmission>>(AssignmentSubmissionDAL.Add), obj, bpr));
                    bpr = drm.BPR;
                    AssignmentSubmissionId = drm.ReturnObject.Id;
                }
                if (AssignmentSubmissionId > 0)
                {
                    int? AssessId = model.SelfAssess.AssessValue;
                    if (AssessId != 0)
                    {
                        AssessController assessController = new Controllers.AssessController();
                        assessController.Assess(AssessId.Value, CurrentUser.Id, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.AssessParent), AssignmentSubmissionId,
                                               (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission,
                                               (int)UT.SL.Model.Enumeration.AssessType.self);
                    }
                }
            }
            catch
            {
                return Content("0");
            }
            return Content(AssignmentSubmissionId.ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGradeSubmission(ViewModels.AssignmentSubmission.NewSubmissionViewModel model, int Id, int GradeId)
        {
            int assignmentSubmissionId = Id;
            int gradeId = GradeId;

            try
            {
                var bpr = new BatchProcessResultModel();
                if (gradeId > 0)
                {
                    var obj = GradeDAL.Get(gradeId);
                    if (obj != null)
                    {
                        obj.CreateDate = DateTime.Now;
                        obj.GradeValue = model.Grade.GradeValue;
                        obj.Body = model.Grade.Body;
                        obj.UserId = CurrentUser.Id;
                        var drm = ((DALReturnModel<Grade>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Grade),
                           new Func<Grade, BatchProcessResultModel, bool, DALReturnModel<Grade>>(GradeDAL.Update), obj, bpr, false));
                        bpr = drm.BPR;
                    }
                    else
                    {
                        //obj = new Grade
                        //{
                        //    CreateDate = DateTime.Now,
                        //    GradeValue = model.Grade.GradeValue,
                        //    Body = model.Grade.Body,
                        //    UserId = CurrentUser.Id,
                        //};
                        //var drm = ((DALReturnModel<Grade>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Grade),
                        //   new Func<Grade, BatchProcessResultModel, DALReturnModel<Grade>>(GradeDAL.Add), obj, bpr));
                        //bpr = drm.BPR;
                        //gradeId = drm.ReturnObject.Id;
                    }
                }
                else
                {
                    //var obj = new Grade
                    //{
                    //    CreateDate = DateTime.Now,
                    //    GradeValue = model.Grade.GradeValue,
                    //    Body = model.Grade.Body,
                    //    UserId = CurrentUser.Id,
                    //};
                    //var drm = ((DALReturnModel<Grade>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Grade),
                    //       new Func<Grade, BatchProcessResultModel, bool, DALReturnModel<Grade>>(GradeDAL.Update), obj, bpr, false));
                    //bpr = drm.BPR;
                    //gradeId = drm.ReturnObject.Id;
                }
                int? AssessId = model.ExpertAssess.AssessValue;
                if (AssessId != 0)
                {
                    AssessController assessController = new Controllers.AssessController();
                    assessController.Assess(AssessId.Value, CurrentUser.Id, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.AssessParent), assignmentSubmissionId,
                                           (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission,
                                           (int)UT.SL.Model.Enumeration.AssessType.expert);
                }
            }
            catch
            {
                return Content("0");
            }
            return Content(gradeId.ToString());
        }
        //public ActionResult SaveSubmissionFile(HttpPostedFileBase submissionFile, int id)
        //{
        //    int fileId = 0;
        //    try
        //    {
        //        if (submissionFile != null)
        //        {
        //            var newFile = new AssignmentSubmission
        //            {
        //                CreateDate = DateTime.Now,
        //                FileMime = submissionFile.ContentType,
        //                FileSize = submissionFile.ContentLength,
        //                FileTitle = submissionFile.FileName,
        //                IsValid = false,
        //                IsPublishd = false,
        //                UserId = CurrentUser.Id,
        //                AssignmentId = id
        //            };

        //            byte[] tempFile = null;
        //            tempFile = new byte[submissionFile.ContentLength];
        //            submissionFile.InputStream.Read(tempFile, 0, submissionFile.ContentLength);
        //            newFile.FileContent = tempFile;

        //            var bpr = new BatchProcessResultModel();
        //            //newFile.Title = "NotYetPublished";
        //            fileId = AssignmentSubmissionDAL.Add(newFile, out bpr).Id;
        //            if (bpr.Failed > 0)
        //            {
        //                return Content("0");
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return Content("0");
        //    }
        //    return Content(fileId.ToString());
        //}

        public ActionResult SaveSubmissionFile(HttpPostedFileBase submissionFile, int id, int? overwite)
        {
            int fileId = 0;
            try
            {
                if (submissionFile != null)
                {
                    if (overwite.HasValue)
                    {
                        var bpr = new BatchProcessResultModel();
                        var Submission = AssignmentSubmissionDAL.Get(id);
                        Submission.FileMime = submissionFile.ContentType;
                        Submission.FileSize = submissionFile.ContentLength;
                        Submission.FileTitle = submissionFile.FileName;
                        byte[] tempFile = null;
                        tempFile = new byte[submissionFile.ContentLength];
                        submissionFile.InputStream.Read(tempFile, 0, submissionFile.ContentLength);
                        Submission.FileContent = tempFile;
                        var drm = ((DALReturnModel<AssignmentSubmission>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.AssignmentSubmission),
                           new Func<AssignmentSubmission, BatchProcessResultModel, DALReturnModel<AssignmentSubmission>>(AssignmentSubmissionDAL.UpdateFile), Submission, bpr));
                        bpr = drm.BPR;
                    }
                    else
                    {
                        var newFile = new AssignmentSubmission
                        {
                            CreateDate = DateTime.Now,
                            FileMime = submissionFile.ContentType,
                            FileSize = submissionFile.ContentLength,
                            FileTitle = submissionFile.FileName,
                            IsValid = false,
                            IsPublishd = false,
                            UserId = CurrentUser.Id,
                            AssignmentId = id
                        };

                        byte[] tempFile = null;
                        tempFile = new byte[submissionFile.ContentLength];
                        submissionFile.InputStream.Read(tempFile, 0, submissionFile.ContentLength);
                        newFile.FileContent = tempFile;

                        var bpr = new BatchProcessResultModel();
                        //newFile.Title = "NotYetPublished";
                        var drm = ((DALReturnModel<AssignmentSubmission>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.AssignmentSubmission),
                           new Func<AssignmentSubmission, BatchProcessResultModel, DALReturnModel<AssignmentSubmission>>(AssignmentSubmissionDAL.Add), newFile, bpr));
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
        public ActionResult RemoveSubmissionFile(int id)
        {
            try
            {
                ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.AssignmentSubmission),
                                       new Func<AssignmentSubmission, DALReturnModel<AssignmentSubmission>>(AssignmentSubmissionDAL.Delete), new AssignmentSubmission { Id = id });
            }
            catch
            {
                return Content("0");
            }
            return Content("0");
        }

        public ActionResult ViewSubmission(int id, int? assignmentSubmissionId)
        {
            AssignmentSubmission tempAssignmentSubmission = new AssignmentSubmission();
            if (assignmentSubmissionId == null)
            {
                var assignment = AssignmentDAL.Get(id);
                tempAssignmentSubmission = AssignmentSubmissionDAL.Get(id, CurrentUser.Id);
            }
            else
            {
                tempAssignmentSubmission = AssignmentSubmissionDAL.Get(assignmentSubmissionId.Value);
            }
            NewSubmissionViewModel oNewSubmissionViewModel = new NewSubmissionViewModel();
            if (tempAssignmentSubmission != null)
            {
                var comments = CommentDAL.GetAll(tempAssignmentSubmission.Id, (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission);
                var AssessParent = AssessParentDAL.GetAll(tempAssignmentSubmission.Id, (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission);
                var grade = GradeDAL.GetAll(tempAssignmentSubmission.Id, (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission);

                var assesses = new List<Assess>();
                if (AssessParent.Count != 0)
                    assesses = AssessDAL.GetListIfExist(AssessParent.FirstOrDefault().Id);

                oNewSubmissionViewModel.AssignmentSubmission = tempAssignmentSubmission;
                if (comments.Count != 0)
                    oNewSubmissionViewModel.Comment = comments.FirstOrDefault();
                if (grade.Count != 0)
                    oNewSubmissionViewModel.Grade = grade.FirstOrDefault();
                if (assesses.Count != 0)
                {
                    oNewSubmissionViewModel.SelfAssess = assesses.Where(a => a.AssessType == (int)UT.SL.Model.Enumeration.AssessType.self).LastOrDefault();
                    oNewSubmissionViewModel.ExpertAssess = assesses.Where(a => a.AssessType == (int)UT.SL.Model.Enumeration.AssessType.expert).LastOrDefault();
                }
            }
            if (oNewSubmissionViewModel.Grade == null)
                return PartialView(oNewSubmissionViewModel);
            else
                return PartialView("ViewGradedSubmission", oNewSubmissionViewModel);
        }

        public ActionResult ViewGradedSubmission(int id)
        {
            AssignmentSubmission tempAssignmentSubmission = new AssignmentSubmission();
            tempAssignmentSubmission = AssignmentSubmissionDAL.Get(id);

            NewSubmissionViewModel oNewSubmissionViewModel = new NewSubmissionViewModel();
            if (tempAssignmentSubmission != null)
            {

                var AssessParent = AssessParentDAL.GetAll(tempAssignmentSubmission.Id, (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission);
                var grade = GradeDAL.GetAll(tempAssignmentSubmission.Id, (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission);

                var assesses = new List<Assess>();
                if (AssessParent.Count != 0)
                    assesses = AssessDAL.GetListIfExist(AssessParent.FirstOrDefault().Id);

                oNewSubmissionViewModel.AssignmentSubmission = tempAssignmentSubmission;

                if (grade.Count != 0)
                    oNewSubmissionViewModel.Grade = grade.Last();
                if (assesses.Count != 0)
                {
                    oNewSubmissionViewModel.SelfAssess = assesses.Where(a => a.AssessType == (int)UT.SL.Model.Enumeration.AssessType.self).LastOrDefault();
                    oNewSubmissionViewModel.ExpertAssess = assesses.Where(a => a.AssessType == (int)UT.SL.Model.Enumeration.AssessType.expert).LastOrDefault();
                }
            }
            return PartialView(oNewSubmissionViewModel);
        }

        public ActionResult ViewAllSubmission(NewSubmissionViewModel model)
        {
            if (model.Grade != null)
            {
                return PartialView("ViewGradedSubmission", model);
            }
            else
            {
                return PartialView("ViewSubmission", model);
            }
        }

        [HttpGet]
        public ActionResult GradeSubmission(int id)
        {
            var assignmentSubmission = AssignmentSubmissionDAL.Get(id);
            ViewModels.AssignmentSubmission.NewSubmissionViewModel oModel = new NewSubmissionViewModel
            {
                AssignmentSubmission = assignmentSubmission
            };
            ViewBag.UserId = CurrentUser.Id;
            return PartialView(oModel);
        }

        [HttpPost]
        public ActionResult GradeSubmission(NewSubmissionViewModel model, int Id)
        {
            var obj = AssignmentSubmissionDAL.Get(Id);
            var sharedObject = ManageObject.GetSharedObject(Id, (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission);
            var newGrade = new Grade
            {
                CreateDate = DateTime.Now,
                ObjectId = obj.Id,
                ObjectType = (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission,
                UserId = CurrentUser.Id,
                Body = model.Grade.Body,
                GradeValue = model.Grade.GradeValue
            };
            var bpr = new BatchProcessResultModel();
            if (sharedObject.CourseId.HasValue)
                newGrade.CourseId = sharedObject.CourseId.Value;
            newGrade.ParentObjectId = sharedObject.CameFromId;
            newGrade.ParentObjectType = sharedObject.CameFromType;
            if (obj != null)
            {
                //newGrade.CourseId = objectModel.CourseId;
                if (obj.UserId > 0)
                    newGrade.GradeOwnerId = obj.UserId;
            }
            var drm = (DALReturnModel<Grade>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Grade),
                                       new Func<Grade, BatchProcessResultModel, DALReturnModel<Grade>>(GradeDAL.Add), newGrade, bpr);
            bpr = drm.BPR;
            //update Credit
            ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.Grade, obj.Id, (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission, 0, newGrade.GradeValue);
            //update credit
            //if (model.Comment != null)
            //{
            //    var newComment = new Comment
            //    {
            //        CreateDate = DateTime.Now,
            //        ObjectId = obj.Id,
            //        Type = (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission,
            //        OwnerId = CurrentUser.Id,
            //        Title = model.Comment.Title
            //    };
            //    CommentDAL.Add(newComment);
            //}
            //if (model.ExpertAssess.AssessValue != null)
            {
                AssessController assessController = new Controllers.AssessController();
                assessController.Assess(model.ExpertAssess.AssessValue, CurrentUser.Id, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.AssessParent), obj.Id,
                                       (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission,
                                       (int)UT.SL.Model.Enumeration.AssessType.expert);
            }
            return Content("0");
        }

        public ActionResult GetSpecificPanel(int id)
        {
            var model = AssignmentSubmissionDAL.Get(id);
            NewSubmissionViewModel oNewSubmissionViewModel = new NewSubmissionViewModel();
            if (model != null)
            {
                oNewSubmissionViewModel.AssignmentSubmission = model;
            }
            return PartialView(oNewSubmissionViewModel);
        }

        public ActionResult Grade(int id)
        {
            var assignmentSubmission = AssignmentSubmissionDAL.Get(id);
            var grade = GradeDAL.GetAll(id, (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission);

            var oModel = new NewSubmissionViewModel
            {
                AssignmentSubmission = assignmentSubmission,
                Grade = grade.LastOrDefault()
            };

            return PartialView(oModel);
        }

        [HttpPost]
        public ActionResult DeleteFile(int id)
        {
            ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.AssignmentSubmission),
                                      new Func<AssignmentSubmission, DALReturnModel<AssignmentSubmission>>(AssignmentSubmissionDAL.DeleteFile), new AssignmentSubmission { Id = id });
            return Content("true");
        }

        //public ActionResult GetLastChanged(int Id)
        //{
        //    var model =AssignmentSubmissionDAL.GetAllSubmissions (Id);
        //    var date = DateTime.Now;
        //    date = model.Where(x => x.CreateDate).Max(x => x.CreateDate).Value;
        //    ViewBag.Date = date;
        //    return PartialView();
        //}
    }
}
