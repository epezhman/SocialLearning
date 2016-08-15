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
    public class GradeController : BaseController
    {        
        public ActionResult Edit(int Id)
        {
            var model = GradeDAL.Get(Id);
            return PartialView(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(Grade model)
        {
            var bpr = new BatchProcessResultModel();
            var grade = GradeDAL.Get(model.Id);
            ModelState.Remove("Id");
            if (ModelState.IsValidField(UT.SL.Model.Resource.Grade.GradeValue))
            {
                double gradeChange = 0;
                gradeChange = (double)model.GradeValue - (double)grade.GradeValue;
                if (grade != null)
                {
                    grade.GradeValue = model.GradeValue;
                    grade.CreateDate = DateTime.Now;
                    grade.UserId = CurrentUser.Id;
                }
                try
                {
                    var drm = (DALReturnModel<Grade>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Grade),
                           new Func<Grade, BatchProcessResultModel, bool, DALReturnModel<Grade>>(GradeDAL.Update), grade, bpr, true);
                    bpr = drm.BPR;
                    //update Credit
                    var assignmentSubmission = AssignmentSubmissionDAL.Get(grade.ObjectId.Value);
                    ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.Grade, assignmentSubmission.Id, (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission, 0, gradeChange);
                    //update credit
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
                bpr.AddError(string.Format(UT.SL.Model.Resource.App_Errors.Range0To100, UT.SL.Model.Resource.Grade.GradeValue), true, true);
                //bpr.AddModelStateErrors(ModelState);
                return PartialView("ProcessResult", bpr);
            }
            return PartialView("GetOneGrade", grade);

        }

        public ActionResult GetOneGrade(int Id)
        {
            var model = GradeDAL.Get(Id);
            return PartialView(model);
        }

        public ActionResult GradeComponent(int id, int type, int gradeFrom, int grade = -1)
        {
            ViewBag.GradeFrom = gradeFrom;
            ViewBag.CurrentUserName = CurrentUser.UserName;
            var gradeObject = GradeDAL.GetAll(id, type).LastOrDefault();
            var model = new Grade();
            if (gradeObject != null)
            {
                if (grade != -1)
                {
                    if (gradeObject.GradeValue == grade)
                    {
                        ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Grade),
                                      new Func<Grade, DALReturnModel<Grade>>(GradeDAL.Delete), gradeObject);         
                        //update Credit
                        ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.Grade, id, type, 0, -(gradeObject.GradeValue));
                        //update credit
                        model = new Grade
                        {
                            ObjectId = id,
                            ObjectType = type
                        };
                    }
                    else
                    {
                        //update Credit
                        ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.Grade, id, type, 0, grade - gradeObject.GradeValue);
                        //update credit
                        gradeObject.GradeValue = grade;
                        gradeObject.UserId = CurrentUser.Id;
                        var bpr = new BatchProcessResultModel();
                        var drm = (DALReturnModel<Grade>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Grade),
                          new Func<Grade, BatchProcessResultModel, bool, DALReturnModel<Grade>>(GradeDAL.Update), gradeObject, bpr, true);
                        bpr = drm.BPR;
                        model = gradeObject;
                    }
                }
                else
                {
                    model = gradeObject;
                }
            }
            else
            {
                if (grade != -1)
                {
                    var sharedObject = ManageObject.GetSharedObject(id, type);
                    var newGrade = new Grade
                       {
                           CreateDate = DateTime.Now,
                           ObjectId = id,
                           ObjectType = type,
                           UserId = CurrentUser.Id,
                           GradeValue = grade
                       };
                    if (sharedObject.CourseId.HasValue)
                        newGrade.CourseId = sharedObject.CourseId.Value;
                    newGrade.ParentObjectId = sharedObject.CameFromId;
                    newGrade.ParentObjectType = sharedObject.CameFromType;
                    if (sharedObject != null)
                    {
                        newGrade.CourseId = newGrade.CourseId > 0 ? newGrade.CourseId : sharedObject.CourseId.Value;
                        if (sharedObject.CreateUser.Id > 0)
                            newGrade.GradeOwnerId = sharedObject.CreateUser.Id;
                    }
                    var bpr = new BatchProcessResultModel();
                    var drm = (DALReturnModel<Grade>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Grade),
                         new Func<Grade, BatchProcessResultModel, DALReturnModel<Grade>>(GradeDAL.Add), newGrade, bpr);
                    bpr = drm.BPR;
                    //update Credit
                    ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.Grade, id, type, 0, grade);
                    //update credit
                    model = newGrade;
                }
                else
                {
                    model = new Grade
                    {
                        ObjectId = id,
                        ObjectType = type
                    };
                }
            }
            return PartialView(model);
        }

        public ActionResult GetGradeChart(Guid userId, int courseId)
        {
            var grades = GradeDAL.GetAllUserAndCourse(courseId, App_UserDAL.Get(userId).Id);
            var userIntId = App_UserDAL.Get(userId).Id;
            var allCoursegrades = GradeDAL.GetAllCourse(courseId);
            var courseForums = CourseDAL.Get(courseId).Forums.Where(x => x.IsPublishd && x.IsValid).ToList();
            var courseAssignments = CourseDAL.Get(courseId).Assignments.Where(x => x.IsPublished && x.IsValid).ToList();
            var model = new FormModel<List<GradeTypeModel>, Course>();
            var tempGrades = new List<GradeTypeModel>();
            //int i = 0;
            //foreach (var item in grades)
            //{
            //    i++;
            //    var objectModel = ManageObject.GetSharedObject(item.ObjectId.Value, item.ObjectType.Value);
            //    var tempGrade = new GradeTypeModel
            //    {
            //        CreateDate = item.CreateDate,
            //        MaxGrade = allCoursegrades.Where(x => x.ParentObjectId == item.ParentObjectId && x.ParentObjectType == item.ParentObjectType).Max(x => x.GradeValue),
            //        MinGrade = allCoursegrades.Where(x => x.ParentObjectId == item.ParentObjectId && x.ParentObjectType == item.ParentObjectType).Min(x => x.GradeValue),
            //        MyGrade = item.GradeValue,
            //        Title = ((item.ObjectType == 8) ? UT.SL.Model.Resource.App_Common.FD + i : UT.SL.Model.Resource.App_Common.Assignment + i)
            //    };
            //    if (objectModel != null)
            //    {
            //        var title = objectModel.CameFromTitle;
            //        var titleLong = title;
            //        if (title.Length >= 4)
            //            title = title.Substring(0, 4) + "...";
            //        if (titleLong.Length >= 20)
            //            titleLong = titleLong.Substring(0, 19) + "...";
            //        tempGrade.Title = title;
            //        tempGrade.FullTitle = titleLong;
            //    }
            //    tempGrades.Add(tempGrade);
            //}
            foreach (var forum in courseForums.Where(x => x.ForumDiscussions.Any(p => p.UserId == userIntId)))
            {
                if (grades.Any(x => x.ParentObjectId == forum.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum))
                {
                    foreach (var item in grades.Where(x => x.ParentObjectId == forum.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum).GroupBy(x => x.ParentObjectId))
                    {
                        var tempGrade = new GradeTypeModel
                        {
                            CreateDate = item.First().CreateDate,
                            MyGradeDouble = allCoursegrades.Where(x => x.ParentObjectId == item.First().ParentObjectId && x.ParentObjectType == item.First().ParentObjectType && x.GradeOwnerId == userIntId).Sum(x => x.GradeValue),
                        };

                        var inUser = forum.ForumDiscussions.Select(x => x.App_User).ToList();
                        var sumUsers = new Dictionary<int, double?>();
                        foreach (var user in inUser.Distinct())
                        {
                            var sum = allCoursegrades.Where(x => x.ParentObjectId == item.First().ParentObjectId && x.ParentObjectType == item.First().ParentObjectType && x.GradeOwnerId == user.Id).Sum(x => x.GradeValue);
                            //var cnt = allCoursegrades.Count(x => x.ParentObjectId == item.First().ParentObjectId && x.ParentObjectType == item.First().ParentObjectType && x.GradeOwnerId == user.Id);
                            if (sum > 0 /*&& cnt > 0*/)
                                sumUsers.Add(user.Id, sum /*/ cnt*/);
                        }
                        var minVal = sumUsers.Min(x => x.Value) ?? 0;
                        tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                        var maxVal = sumUsers.Max(x => x.Value) ?? 0;
                        tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;

                        var title = forum.Title;
                        if (string.IsNullOrEmpty(title))
                            title = forum.Body;
                        if (string.IsNullOrEmpty(title))
                            title = forum.FileTitle;
                        var titleLong = title;
                        if (title.Length >= 4)
                            title = title.Substring(0, 4) + "...";
                        if (titleLong.Length >= 20)
                            titleLong = titleLong.Substring(0, 19) + "...";
                        tempGrade.Title = title;
                        tempGrade.FullTitle = titleLong;
                        tempGrade.GradedObjectType = (int)UT.SL.Model.Enumeration.ObjectType.Forum;
                        tempGrades.Add(tempGrade);
                    }
                }
                else
                {
                    var tempGrade = new GradeTypeModel
                    {
                        MyGradeDouble = 0
                    };
                    if (allCoursegrades.Any(x => x.ParentObjectId == forum.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum))
                    {
                        var inUser = forum.ForumDiscussions.Select(x => x.App_User).ToList();
                        var sumUsers = new Dictionary<int, double?>();
                        foreach (var user in inUser.Distinct())
                        {
                            var sum = allCoursegrades.Where(x => x.ParentObjectId == forum.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum && x.GradeOwnerId == user.Id).Sum(x => x.GradeValue);
                            //var cnt = allCoursegrades.Count(x => x.ParentObjectId == forum.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum && x.GradeOwnerId == user.Id);
                            if (sum > 0 /*&& cnt > 0*/)
                                sumUsers.Add(user.Id, sum /*/ cnt*/);
                        }
                        var minVal = sumUsers.Min(x => x.Value) ?? 0;
                        tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                        var maxVal = sumUsers.Max(x => x.Value) ?? 0;
                        tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                    }
                    else
                    {
                        tempGrade.MinGrade = 0;
                        tempGrade.MaxGrade = 0;
                    }
                    var title = forum.Title;
                    if (string.IsNullOrEmpty(title))
                        title = forum.Body;
                    if (string.IsNullOrEmpty(title))
                        title = forum.FileTitle;
                    var titleLong = title;
                    if (title.Length >= 4)
                        title = title.Substring(0, 4) + "...";
                    if (titleLong.Length >= 20)
                        titleLong = titleLong.Substring(0, 19) + "...";
                    tempGrade.Title = title;
                    tempGrade.FullTitle = titleLong;
                    tempGrade.GradedObjectType = (int)UT.SL.Model.Enumeration.ObjectType.Forum;
                    tempGrades.Add(tempGrade);
                }
            }
            foreach (var assignment in courseAssignments.Where(x => x.AssignmentSubmissions.Any(p => p.UserId == userIntId)))
            {
                if (grades.Any(x => x.ParentObjectId == assignment.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment))
                {
                    foreach (var item in grades.Where(x => x.ParentObjectId == assignment.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment).GroupBy(x => x.ParentObjectId))
                    {
                        var tempGrade = new GradeTypeModel
                        {
                            CreateDate = item.First().CreateDate,
                            MyGradeDouble = allCoursegrades.Where(x => x.ParentObjectId == item.First().ParentObjectId && x.ParentObjectType == item.First().ParentObjectType && x.GradeOwnerId == userIntId).Sum(x => x.GradeValue),
                        };
                        var minVal = allCoursegrades.Where(x => x.ParentObjectId == item.First().ParentObjectId && x.ParentObjectType == item.First().ParentObjectType).Min(x => x.GradeValue);
                        tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                        var maxVal = allCoursegrades.Where(x => x.ParentObjectId == item.First().ParentObjectId && x.ParentObjectType == item.First().ParentObjectType).Max(x => x.GradeValue);
                        tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                        var title = assignment.Title;
                        if (string.IsNullOrEmpty(title))
                            title = assignment.Body;
                        if (string.IsNullOrEmpty(title))
                            title = assignment.FileTitle;
                        var titleLong = title;
                        if (title.Length >= 4)
                            title = title.Substring(0, 4) + "...";
                        if (titleLong.Length >= 20)
                            titleLong = titleLong.Substring(0, 19) + "...";
                        tempGrade.Title = title;
                        tempGrade.FullTitle = titleLong;
                        tempGrade.GradedObjectType = (int)UT.SL.Model.Enumeration.ObjectType.Assignment;
                        tempGrades.Add(tempGrade);
                    }
                }
                else
                {
                    var tempGrade = new GradeTypeModel
                    {
                        MyGradeDouble = 0
                    };
                    if (allCoursegrades.Any(x => x.ParentObjectId == assignment.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment))
                    {
                        var minVal = allCoursegrades.Where(x => x.ParentObjectId == assignment.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment).Min(x => x.GradeValue);
                        tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                        var maxVal = allCoursegrades.Where(x => x.ParentObjectId == assignment.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment).Max(x => x.GradeValue);
                        tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                    }
                    else
                    {
                        tempGrade.MinGrade = 0;
                        tempGrade.MaxGrade = 0;
                    }
                    var title = assignment.Title;
                    if (string.IsNullOrEmpty(title))
                        title = assignment.Body;
                    if (string.IsNullOrEmpty(title))
                        title = assignment.FileTitle;
                    var titleLong = title;
                    if (title.Length >= 4)
                        title = title.Substring(0, 4) + "...";
                    if (titleLong.Length >= 20)
                        titleLong = titleLong.Substring(0, 19) + "...";
                    tempGrade.Title = title;
                    tempGrade.FullTitle = titleLong;
                    tempGrade.GradedObjectType = (int)UT.SL.Model.Enumeration.ObjectType.Assignment;
                    tempGrades.Add(tempGrade);
                }
            }

            model.FormObject = tempGrades.OrderBy(x => x.CreateDate).ThenBy(x => x.GradedObjectType).ToList();
            model.ExtraKnownData = CourseDAL.Get(courseId);
            ViewBag.UserId = userId;
            ViewBag.CourseId = courseId;
            return PartialView(model);
        }

        public ActionResult GetGradeDetails(Guid userId, int courseId, int position)
        {
            var grades = GradeDAL.GetAllUserAndCourse(courseId, App_UserDAL.Get(userId).Id);
            var userIntId = App_UserDAL.Get(userId).Id;
            var allCoursegrades = GradeDAL.GetAllCourse(courseId);
            var courseForums = CourseDAL.Get(courseId).Forums.Where(x => x.IsPublishd && x.IsValid).ToList();
            var courseAssignments = CourseDAL.Get(courseId).Assignments.Where(x => x.IsPublished && x.IsValid).ToList();
            var model = new FormModel<List<GradeTypeModel>, Course>();
            var tempGrades = new List<GradeTypeModel>();
            foreach (var forum in courseForums.Where(x => x.ForumDiscussions.Any(p => p.UserId == userIntId)))
            {
                if (grades.Any(x => x.ParentObjectId == forum.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum))
                {
                    foreach (var item in grades.Where(x => x.ParentObjectId == forum.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum).GroupBy(x => x.ParentObjectId))
                    {
                        var tempGrade = new GradeTypeModel
                        {
                            CreateDate = item.First().CreateDate,
                            MyGradeDouble = allCoursegrades.Where(x => x.ParentObjectId == item.First().ParentObjectId && x.ParentObjectType == item.First().ParentObjectType && x.GradeOwnerId == userIntId).Sum(x => x.GradeValue),
                        };

                        foreach (var grade in item)
                        {
                            tempGrade.GradedObjects.Add(new GradeTypeModel
                            {
                                MyGrade = grade.GradeValue,
                                GradeGiver = grade.App_User,
                                CreateDate = grade.CreateDate,
                                GradedObject = ManageObject.GetSharedObject(grade.ObjectId.Value, grade.ObjectType.Value)
                            });
                        }

                        var inUser = forum.ForumDiscussions.Select(x => x.App_User).ToList();
                        var sumUsers = new Dictionary<int, double?>();
                        foreach (var user in inUser.Distinct())
                        {
                            var sum = allCoursegrades.Where(x => x.ParentObjectId == item.First().ParentObjectId && x.ParentObjectType == item.First().ParentObjectType && x.GradeOwnerId == user.Id).Sum(x => x.GradeValue);
                            //var cnt = allCoursegrades.Count(x => x.ParentObjectId == item.First().ParentObjectId && x.ParentObjectType == item.First().ParentObjectType && x.GradeOwnerId == user.Id);
                            if (sum > 0 /*&& cnt > 0*/)
                                sumUsers.Add(user.Id, sum /*/ cnt*/);
                        }
                        var minVal = sumUsers.Min(x => x.Value) ?? 0;
                        tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                        var maxVal = sumUsers.Max(x => x.Value) ?? 0;
                        tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                        if (maxVal > 0 && sumUsers.Any(x => x.Value == maxVal))
                        {
                            foreach (var user in sumUsers.Where(x => x.Value == maxVal))
                            {
                                tempGrade.MaxGardeUsers.Add(App_UserDAL.Get(user.Key));
                            }
                        }

                        tempGrade.GradedObject = ManageObject.GetSharedObject(forum.Id, (int)UT.SL.Model.Enumeration.ObjectType.Forum);

                        var title = forum.Title;
                        if (string.IsNullOrEmpty(title))
                            title = forum.Body;
                        if (string.IsNullOrEmpty(title))
                            title = forum.FileTitle;
                        var titleLong = title;
                        if (title.Length >= 4)
                            title = title.Substring(0, 4) + "...";
                        if (titleLong.Length >= 20)
                            titleLong = titleLong.Substring(0, 19) + "...";
                        tempGrade.Title = title;
                        tempGrade.FullTitle = titleLong;
                        tempGrade.GradedObjectType = (int)UT.SL.Model.Enumeration.ObjectType.Forum;
                        tempGrades.Add(tempGrade);
                    }
                }
                else
                {
                    var tempGrade = new GradeTypeModel
                    {
                        MyGradeDouble = 0
                    };
                    if (allCoursegrades.Any(x => x.ParentObjectId == forum.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum))
                    {
                        var inUser = forum.ForumDiscussions.Select(x => x.App_User).ToList();
                        var sumUsers = new Dictionary<int, double?>();
                        foreach (var user in inUser.Distinct())
                        {
                            var sum = allCoursegrades.Where(x => x.ParentObjectId == forum.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum && x.GradeOwnerId == user.Id).Sum(x => x.GradeValue);
                            //var cnt = allCoursegrades.Count(x => x.ParentObjectId == forum.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum && x.GradeOwnerId == user.Id);
                            if (sum > 0 /*&& cnt > 0*/)
                                sumUsers.Add(user.Id, sum /*/ cnt*/);
                        }
                        var minVal = sumUsers.Min(x => x.Value) ?? 0;
                        tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                        var maxVal = sumUsers.Max(x => x.Value) ?? 0;
                        tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                        if (maxVal > 0 && sumUsers.Any(x => x.Value == maxVal))
                        {
                            foreach (var user in sumUsers.Where(x => x.Value == maxVal))
                            {
                                tempGrade.MaxGardeUsers.Add(App_UserDAL.Get(user.Key));
                            }
                        }
                    }
                    else
                    {
                        tempGrade.MinGrade = 0;
                        tempGrade.MaxGrade = 0;
                    }

                    tempGrade.GradedObject = ManageObject.GetSharedObject(forum.Id, (int)UT.SL.Model.Enumeration.ObjectType.Forum);

                    var title = forum.Title;
                    if (string.IsNullOrEmpty(title))
                        title = forum.Body;
                    if (string.IsNullOrEmpty(title))
                        title = forum.FileTitle;
                    var titleLong = title;
                    if (title.Length >= 4)
                        title = title.Substring(0, 4) + "...";
                    if (titleLong.Length >= 20)
                        titleLong = titleLong.Substring(0, 19) + "...";
                    tempGrade.Title = title;
                    tempGrade.FullTitle = titleLong;
                    tempGrade.GradedObjectType = (int)UT.SL.Model.Enumeration.ObjectType.Forum;
                    tempGrades.Add(tempGrade);
                }
            }
            foreach (var assignment in courseAssignments.Where(x => x.AssignmentSubmissions.Any(p => p.UserId == userIntId)))
            {
                if (grades.Any(x => x.ParentObjectId == assignment.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment))
                {
                    foreach (var item in grades.Where(x => x.ParentObjectId == assignment.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment).GroupBy(x => x.ParentObjectId))
                    {
                        var tempGrade = new GradeTypeModel
                        {
                            CreateDate = item.First().CreateDate,
                            MyGradeDouble = allCoursegrades.Where(x => x.ParentObjectId == item.First().ParentObjectId && x.ParentObjectType == item.First().ParentObjectType && x.GradeOwnerId == userIntId).Sum(x => x.GradeValue),
                        };
                        var minVal = allCoursegrades.Where(x => x.ParentObjectId == item.First().ParentObjectId && x.ParentObjectType == item.First().ParentObjectType).Min(x => x.GradeValue);
                        tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                        var maxVal = allCoursegrades.Where(x => x.ParentObjectId == item.First().ParentObjectId && x.ParentObjectType == item.First().ParentObjectType).Max(x => x.GradeValue);
                        tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                        if (maxVal > 0)
                        {
                            tempGrade.MaxGardeUsers = allCoursegrades.Where(x => x.ParentObjectId == item.First().ParentObjectId && x.ParentObjectType == item.First().ParentObjectType && x.GradeValue == maxVal).Select(x => x.App_User1).ToList();
                        }
                        tempGrade.GradedObject = ManageObject.GetSharedObject(item.First().ParentObjectId.Value, item.First().ParentObjectType.Value);
                        var title = assignment.Title;
                        if (string.IsNullOrEmpty(title))
                            title = assignment.Body;
                        if (string.IsNullOrEmpty(title))
                            title = assignment.FileTitle;
                        var titleLong = title;
                        if (title.Length >= 4)
                            title = title.Substring(0, 4) + "...";
                        if (titleLong.Length >= 20)
                            titleLong = titleLong.Substring(0, 19) + "...";
                        tempGrade.Title = title;
                        tempGrade.FullTitle = titleLong;
                        tempGrade.GradedObjectType = (int)UT.SL.Model.Enumeration.ObjectType.Assignment;
                        tempGrade.GradeGiver = item.First().App_User;
                        tempGrades.Add(tempGrade);
                    }
                }
                else
                {
                    var tempGrade = new GradeTypeModel
                    {
                        MyGradeDouble = 0
                    };
                    if (allCoursegrades.Any(x => x.ParentObjectId == assignment.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment))
                    {
                        var minVal = allCoursegrades.Where(x => x.ParentObjectId == assignment.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment).Min(x => x.GradeValue);
                        tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                        var maxVal = allCoursegrades.Where(x => x.ParentObjectId == assignment.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment).Max(x => x.GradeValue);
                        tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                        if (maxVal > 0)
                        {
                            tempGrade.MaxGardeUsers = allCoursegrades.Where(x => x.ParentObjectId == assignment.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment && x.GradeValue == maxVal).Select(x => x.App_User1).ToList();
                        }
                    }
                    else
                    {
                        tempGrade.MinGrade = 0;
                        tempGrade.MaxGrade = 0;
                    }
                    tempGrade.GradedObject = ManageObject.GetSharedObject(assignment.Id, (int)UT.SL.Model.Enumeration.ObjectType.Assignment);
                    var title = assignment.Title;
                    if (string.IsNullOrEmpty(title))
                        title = assignment.Body;
                    if (string.IsNullOrEmpty(title))
                        title = assignment.FileTitle;
                    var titleLong = title;
                    if (title.Length >= 4)
                        title = title.Substring(0, 4) + "...";
                    if (titleLong.Length >= 20)
                        titleLong = titleLong.Substring(0, 19) + "...";
                    tempGrade.Title = title;
                    tempGrade.FullTitle = titleLong;
                    tempGrade.GradedObjectType = (int)UT.SL.Model.Enumeration.ObjectType.Assignment;
                    tempGrades.Add(tempGrade);
                }
            }
            model.FormObject = tempGrades.OrderBy(x => x.CreateDate).ThenBy(x => x.GradedObjectType).ToList();
            var tempModel = model.FormObject[position];

            ViewBag.FullName = App_UserDAL.Get(userId).FirstName + " " + App_UserDAL.Get(userId).LastName;
            
            return PartialView(tempModel);
            //return PartialView("GetGradeTableDetails", tempModel);

        }

        public ActionResult GetGradeTableDetails(Guid userId, int courseId)
        {
            var grades = GradeDAL.GetAllUserAndCourse(courseId, App_UserDAL.Get(userId).Id);
            var userIntId = App_UserDAL.Get(userId).Id;
            var allCoursegrades = GradeDAL.GetAllCourse(courseId);
            var courseForums = CourseDAL.Get(courseId).Forums.Where(x => x.IsPublishd && x.IsValid).ToList();
            var courseAssignments = CourseDAL.Get(courseId).Assignments.Where(x => x.IsPublished && x.IsValid).ToList();
            var model = new FormModel<List<GradeTypeModel>, Course>();
            var tempGrades = new List<GradeTypeModel>();
            foreach (var forum in courseForums.Where(x => x.ForumDiscussions.Any(p => p.UserId == userIntId)))
            {
                if (grades.Any(x => x.ParentObjectId == forum.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum))
                {
                    foreach (var item in grades.Where(x => x.ParentObjectId == forum.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum).GroupBy(x => x.ParentObjectId))
                    {
                        var tempGrade = new GradeTypeModel
                        {
                            CreateDate = item.First().CreateDate,
                            MyGradeDouble = allCoursegrades.Where(x => x.ParentObjectId == item.First().ParentObjectId && x.ParentObjectType == item.First().ParentObjectType && x.GradeOwnerId == userIntId).Sum(x => x.GradeValue),
                        };

                        foreach (var grade in item)
                        {
                            tempGrade.GradedObjects.Add(new GradeTypeModel
                            {
                                MyGrade = grade.GradeValue,
                                GradeGiver = grade.App_User,
                                CreateDate = grade.CreateDate,
                                GradedObject = ManageObject.GetSharedObject(grade.ObjectId.Value, grade.ObjectType.Value)
                            });
                        }

                        var inUser = forum.ForumDiscussions.Select(x => x.App_User).ToList();
                        var sumUsers = new Dictionary<int, double?>();
                        foreach (var user in inUser.Distinct())
                        {
                            var sum = allCoursegrades.Where(x => x.ParentObjectId == item.First().ParentObjectId && x.ParentObjectType == item.First().ParentObjectType && x.GradeOwnerId == user.Id).Sum(x => x.GradeValue);
                            //var cnt = allCoursegrades.Count(x => x.ParentObjectId == item.First().ParentObjectId && x.ParentObjectType == item.First().ParentObjectType && x.GradeOwnerId == user.Id);
                            if (sum > 0 /*&& cnt > 0*/)
                                sumUsers.Add(user.Id, sum /*/ cnt*/);
                        }
                        var minVal = sumUsers.Min(x => x.Value) ?? 0;
                        tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                        var maxVal = sumUsers.Max(x => x.Value) ?? 0;
                        tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                        if (maxVal > 0 && sumUsers.Any(x => x.Value == maxVal))
                        {
                            foreach (var user in sumUsers.Where(x => x.Value == maxVal))
                            {
                                tempGrade.MaxGardeUsers.Add(App_UserDAL.Get(user.Key));
                            }
                        }

                        tempGrade.GradedObject = ManageObject.GetSharedObject(forum.Id, (int)UT.SL.Model.Enumeration.ObjectType.Forum);

                        var title = forum.Title;
                        if (string.IsNullOrEmpty(title))
                            title = forum.Body;
                        if (string.IsNullOrEmpty(title))
                            title = forum.FileTitle;
                        var titleLong = title;
                        if (title.Length >= 4)
                            title = title.Substring(0, 4) + "...";
                        if (titleLong.Length >= 20)
                            titleLong = titleLong.Substring(0, 19) + "...";
                        tempGrade.Title = title;
                        tempGrade.FullTitle = titleLong;
                        tempGrade.GradedObjectType = (int)UT.SL.Model.Enumeration.ObjectType.Forum;
                        tempGrades.Add(tempGrade);
                    }
                }
                else
                {
                    var tempGrade = new GradeTypeModel
                    {
                        MyGradeDouble = 0
                    };
                    if (allCoursegrades.Any(x => x.ParentObjectId == forum.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum))
                    {
                        var inUser = forum.ForumDiscussions.Select(x => x.App_User).ToList();
                        var sumUsers = new Dictionary<int, double?>();
                        foreach (var user in inUser.Distinct())
                        {
                            var sum = allCoursegrades.Where(x => x.ParentObjectId == forum.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum && x.GradeOwnerId == user.Id).Sum(x => x.GradeValue);
                            //var cnt = allCoursegrades.Count(x => x.ParentObjectId == forum.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum && x.GradeOwnerId == user.Id);
                            if (sum > 0 /*&& cnt > 0*/)
                                sumUsers.Add(user.Id, sum /*/ cnt*/);
                        }
                        var minVal = sumUsers.Min(x => x.Value) ?? 0;
                        tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                        var maxVal = sumUsers.Max(x => x.Value) ?? 0;
                        tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                        if (maxVal > 0 && sumUsers.Any(x => x.Value == maxVal))
                        {
                            foreach (var user in sumUsers.Where(x => x.Value == maxVal))
                            {
                                tempGrade.MaxGardeUsers.Add(App_UserDAL.Get(user.Key));
                            }
                        }
                    }
                    else
                    {
                        tempGrade.MinGrade = 0;
                        tempGrade.MaxGrade = 0;
                    }

                    tempGrade.GradedObject = ManageObject.GetSharedObject(forum.Id, (int)UT.SL.Model.Enumeration.ObjectType.Forum);

                    var title = forum.Title;
                    if (string.IsNullOrEmpty(title))
                        title = forum.Body;
                    if (string.IsNullOrEmpty(title))
                        title = forum.FileTitle;
                    var titleLong = title;
                    if (title.Length >= 4)
                        title = title.Substring(0, 4) + "...";
                    if (titleLong.Length >= 20)
                        titleLong = titleLong.Substring(0, 19) + "...";
                    tempGrade.Title = title;
                    tempGrade.FullTitle = titleLong;
                    tempGrade.GradedObjectType = (int)UT.SL.Model.Enumeration.ObjectType.Forum;
                    tempGrades.Add(tempGrade);
                }
            }
            foreach (var assignment in courseAssignments.Where(x => x.AssignmentSubmissions.Any(p => p.UserId == userIntId)))
            {
                if (grades.Any(x => x.ParentObjectId == assignment.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment))
                {
                    foreach (var item in grades.Where(x => x.ParentObjectId == assignment.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment).GroupBy(x => x.ParentObjectId))
                    {
                        var tempGrade = new GradeTypeModel
                        {
                            CreateDate = item.First().CreateDate,
                            MyGradeDouble = allCoursegrades.Where(x => x.ParentObjectId == item.First().ParentObjectId && x.ParentObjectType == item.First().ParentObjectType && x.GradeOwnerId == userIntId).Sum(x => x.GradeValue),
                        };
                        var minVal = allCoursegrades.Where(x => x.ParentObjectId == item.First().ParentObjectId && x.ParentObjectType == item.First().ParentObjectType).Min(x => x.GradeValue);
                        tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                        var maxVal = allCoursegrades.Where(x => x.ParentObjectId == item.First().ParentObjectId && x.ParentObjectType == item.First().ParentObjectType).Max(x => x.GradeValue);
                        tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                        if (maxVal > 0)
                        {
                            tempGrade.MaxGardeUsers = allCoursegrades.Where(x => x.ParentObjectId == item.First().ParentObjectId && x.ParentObjectType == item.First().ParentObjectType && x.GradeValue == maxVal).Select(x => x.App_User1).ToList();
                        }
                        tempGrade.GradedObject = ManageObject.GetSharedObject(item.First().ParentObjectId.Value, item.First().ParentObjectType.Value);
                        var title = assignment.Title;
                        if (string.IsNullOrEmpty(title))
                            title = assignment.Body;
                        if (string.IsNullOrEmpty(title))
                            title = assignment.FileTitle;
                        var titleLong = title;
                        if (title.Length >= 4)
                            title = title.Substring(0, 4) + "...";
                        if (titleLong.Length >= 20)
                            titleLong = titleLong.Substring(0, 19) + "...";
                        tempGrade.Title = title;
                        tempGrade.FullTitle = titleLong;
                        tempGrade.GradedObjectType = (int)UT.SL.Model.Enumeration.ObjectType.Assignment;
                        tempGrade.GradeGiver = item.First().App_User;
                        tempGrades.Add(tempGrade);
                    }
                }
                else
                {
                    var tempGrade = new GradeTypeModel
                    {
                        MyGradeDouble = 0
                    };
                    if (allCoursegrades.Any(x => x.ParentObjectId == assignment.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment))
                    {
                        var minVal = allCoursegrades.Where(x => x.ParentObjectId == assignment.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment).Min(x => x.GradeValue);
                        tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                        var maxVal = allCoursegrades.Where(x => x.ParentObjectId == assignment.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment).Max(x => x.GradeValue);
                        tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                        if (maxVal > 0)
                        {
                            tempGrade.MaxGardeUsers = allCoursegrades.Where(x => x.ParentObjectId == assignment.Id && x.ParentObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment && x.GradeValue == maxVal).Select(x => x.App_User1).ToList();
                        }
                    }
                    else
                    {
                        tempGrade.MinGrade = 0;
                        tempGrade.MaxGrade = 0;
                    }
                    tempGrade.GradedObject = ManageObject.GetSharedObject(assignment.Id, (int)UT.SL.Model.Enumeration.ObjectType.Assignment);
                    var title = assignment.Title;
                    if (string.IsNullOrEmpty(title))
                        title = assignment.Body;
                    if (string.IsNullOrEmpty(title))
                        title = assignment.FileTitle;
                    var titleLong = title;
                    if (title.Length >= 4)
                        title = title.Substring(0, 4) + "...";
                    if (titleLong.Length >= 20)
                        titleLong = titleLong.Substring(0, 19) + "...";
                    tempGrade.Title = title;
                    tempGrade.FullTitle = titleLong;
                    tempGrade.GradedObjectType = (int)UT.SL.Model.Enumeration.ObjectType.Assignment;
                    tempGrades.Add(tempGrade);
                }
            }
            model.FormObject = tempGrades.OrderBy(x => x.CreateDate).ThenBy(x => x.GradedObjectType).ToList();

            ViewBag.FullName = App_UserDAL.Get(userId).FirstName + " " + App_UserDAL.Get(userId).LastName;

            //return PartialView(tempModel);
            return PartialView( model);

        }

        public ActionResult GetUserSoActProfileChart(Guid userId, int courseId)
        {
            var grades = App_UserDAL.GetAllUserAndCourseUserSoActProfile(courseId, App_UserDAL.Get(userId).Id);
            var allCoursegrades = App_UserDAL.GetAllCourseUserSoActProfile(courseId);
            var courseTopics = CourseDAL.Get(courseId).CourseAbstract.CourseTopcMappers.Select(x => x.Topic).ToList();
            var model = new FormModel<List<GradeTypeModel>, Course>();
            var tempGrades = new List<GradeTypeModel>();
            foreach (var topic in courseTopics)
            {
                if (grades.Any(x => x.TopicId == topic.Id))
                {
                    foreach (var item in grades.Where(x => x.TopicId == topic.Id))
                    {
                        var tempGrade = new GradeTypeModel
                        {
                            MyGradeDouble = item.SoActValue < 0 ? 0 : item.SoActValue
                        };
                        var minVal = allCoursegrades.Where(x => x.TopicId == item.TopicId).Min(x => x.SoActValue);
                        tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                        var maxVal = allCoursegrades.Where(x => x.TopicId == item.TopicId).Max(x => x.SoActValue);
                        tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                        tempGrade.Topic = topic;
                        var title = topic.Title;
                        var titleLong = title;
                        if (title.Length >= 4)
                            title = title.Substring(0, 4) + "...";
                        tempGrade.Title = title;
                        tempGrade.FullTitle = titleLong;
                        tempGrades.Add(tempGrade);
                    }
                }
                else
                {
                    var tempGrade = new GradeTypeModel
                    {
                        MyGradeDouble = 0
                    };
                    if (allCoursegrades.Any(x => x.TopicId == topic.Id))
                    {
                        var minVal = allCoursegrades.Where(x => x.TopicId == topic.Id).Min(x => x.SoActValue);
                        tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                        var maxVal = allCoursegrades.Where(x => x.TopicId == topic.Id).Max(x => x.SoActValue);
                        tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                    }
                    else
                    {
                        tempGrade.MinGrade = 0;
                        tempGrade.MaxGrade = 0;
                    }
                    tempGrade.Topic = topic;
                    var title = topic.Title;
                    var titleLong = title;
                    if (title.Length >= 4)
                        title = title.Substring(0, 4) + "...";
                    tempGrade.Title = title;
                    tempGrade.FullTitle = titleLong;
                    tempGrades.Add(tempGrade);
                }
            }
            tempGrades = tempGrades.OrderBy(x => x.Title).ToList();
            foreach (var item in grades.Where(x => !x.TopicId.HasValue))
            {
                var tempGrade = new GradeTypeModel
                {
                    MyGradeDouble = item.SoActValue < 0 ? 0 : item.SoActValue
                };
                var minVal = allCoursegrades.Where(x => !x.TopicId.HasValue).Min(x => x.SoActValue);
                tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                var maxVal = allCoursegrades.Where(x => !x.TopicId.HasValue).Max(x => x.SoActValue);
                tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                var title = UT.SL.Model.Resource.App_Common.NoTopic;
                var titleLong = title;
                if (title.Length >= 4)
                    title = title.Substring(0, 4) + "...";
                tempGrade.Title = title;
                tempGrade.FullTitle = titleLong;
                tempGrades.Add(tempGrade);
            }
            model.FormObject = tempGrades.ToList();
            ViewBag.UserId = userId;
            ViewBag.CourseId = courseId;
            model.ExtraKnownData = CourseDAL.Get(courseId);
            return PartialView(model);
        }

        public ActionResult GetSoActDetails(Guid userId, int courseId, int position)
        {
            var grades = App_UserDAL.GetAllUserAndCourseUserSoActProfile(courseId, App_UserDAL.Get(userId).Id);
            var allCoursegrades = App_UserDAL.GetAllCourseUserSoActProfile(courseId);
            var courseTopics = CourseDAL.Get(courseId).CourseAbstract.CourseTopcMappers.Select(x => x.Topic).ToList();
            var tempGrades = new List<GradeTypeModel>();
            foreach (var topic in courseTopics)
            {
                if (grades.Any(x => x.TopicId == topic.Id))
                {
                    foreach (var item in grades.Where(x => x.TopicId == topic.Id))
                    {
                        var tempGrade = new GradeTypeModel
                        {
                            MyGradeDouble = item.SoActValue < 0 ? 0 : item.SoActValue
                        };
                        var minVal = allCoursegrades.Where(x => x.TopicId == item.TopicId).Min(x => x.SoActValue);
                        tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                        var maxVal = allCoursegrades.Where(x => x.TopicId == item.TopicId).Max(x => x.SoActValue);
                        tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                        tempGrade.MaxGardeUsers = allCoursegrades.Where(x => x.TopicId == item.TopicId && x.SoActValue == maxVal).Select(x => x.App_User).ToList();
                        tempGrade.Topic = topic;
                        var title = topic.Title;
                        var titleLong = title;
                        if (title.Length >= 4)
                            title = title.Substring(0, 4) + "...";
                        tempGrade.Title = title;
                        tempGrade.FullTitle = titleLong;
                        tempGrades.Add(tempGrade);
                    }
                }
                else
                {
                    var tempGrade = new GradeTypeModel
                    {
                        MyGradeDouble = 0
                    };
                    if (allCoursegrades.Any(x => x.TopicId == topic.Id))
                    {
                        var minVal = allCoursegrades.Where(x => x.TopicId == topic.Id).Min(x => x.SoActValue);
                        tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                        var maxVal = allCoursegrades.Where(x => x.TopicId == topic.Id).Max(x => x.SoActValue);
                        tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                        tempGrade.MaxGardeUsers = allCoursegrades.Where(x => x.TopicId == topic.Id && x.SoActValue == maxVal).Select(x => x.App_User).ToList();
                    }
                    else
                    {
                        tempGrade.MinGrade = 0;
                        tempGrade.MaxGrade = 0;
                    }
                    tempGrade.Topic = topic;
                    var title = topic.Title;
                    var titleLong = title;
                    if (title.Length >= 4)
                        title = title.Substring(0, 4) + "...";
                    tempGrade.Title = title;
                    tempGrade.FullTitle = titleLong;
                    tempGrades.Add(tempGrade);
                }
            }
            tempGrades = tempGrades.OrderBy(x => x.Title).ToList();
            foreach (var item in grades.Where(x => !x.TopicId.HasValue))
            {
                var tempGrade = new GradeTypeModel
                {
                    MyGradeDouble = item.SoActValue < 0 ? 0 : item.SoActValue
                };
                var minVal = allCoursegrades.Where(x => !x.TopicId.HasValue).Min(x => x.SoActValue);
                tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                var maxVal = allCoursegrades.Where(x => !x.TopicId.HasValue).Max(x => x.SoActValue);
                tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                tempGrade.MaxGardeUsers = allCoursegrades.Where(x => !x.TopicId.HasValue && x.SoActValue == maxVal).Select(x => x.App_User).ToList();
                var title = UT.SL.Model.Resource.App_Common.NoTopic;
                var titleLong = title;
                if (title.Length >= 4)
                    title = title.Substring(0, 4) + "...";
                tempGrade.Title = title;
                tempGrade.FullTitle = titleLong;
                tempGrades.Add(tempGrade);
            }
            var model = tempGrades.ToList()[position];
            ViewBag.FullName = App_UserDAL.Get(userId).FirstName + " " + App_UserDAL.Get(userId).LastName;
            return PartialView(model);
        }

        public ActionResult GetSoActTableDetails(Guid userId, int courseId)
        {
            var grades = App_UserDAL.GetAllUserAndCourseUserSoActProfile(courseId, App_UserDAL.Get(userId).Id);
            var allCoursegrades = App_UserDAL.GetAllCourseUserSoActProfile(courseId);
            var courseTopics = CourseDAL.Get(courseId).CourseAbstract.CourseTopcMappers.Select(x => x.Topic).ToList();
            var tempGrades = new List<GradeTypeModel>();
            foreach (var topic in courseTopics)
            {
                if (grades.Any(x => x.TopicId == topic.Id))
                {
                    foreach (var item in grades.Where(x => x.TopicId == topic.Id))
                    {
                        var tempGrade = new GradeTypeModel
                        {
                            MyGradeDouble = item.SoActValue < 0 ? 0 : item.SoActValue
                        };
                        var minVal = allCoursegrades.Where(x => x.TopicId == item.TopicId).Min(x => x.SoActValue);
                        tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                        var maxVal = allCoursegrades.Where(x => x.TopicId == item.TopicId).Max(x => x.SoActValue);
                        tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                        tempGrade.MaxGardeUsers = allCoursegrades.Where(x => x.TopicId == item.TopicId && x.SoActValue == maxVal).Select(x => x.App_User).ToList();
                        tempGrade.Topic = topic;
                        var title = topic.Title;
                        var titleLong = title;
                        if (title.Length >= 4)
                            title = title.Substring(0, 4) + "...";
                        tempGrade.Title = title;
                        tempGrade.FullTitle = titleLong;
                        tempGrades.Add(tempGrade);
                    }
                }
                else
                {
                    var tempGrade = new GradeTypeModel
                    {
                        MyGradeDouble = 0
                    };
                    if (allCoursegrades.Any(x => x.TopicId == topic.Id))
                    {
                        var minVal = allCoursegrades.Where(x => x.TopicId == topic.Id).Min(x => x.SoActValue);
                        tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                        var maxVal = allCoursegrades.Where(x => x.TopicId == topic.Id).Max(x => x.SoActValue);
                        tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                        tempGrade.MaxGardeUsers = allCoursegrades.Where(x => x.TopicId == topic.Id && x.SoActValue == maxVal).Select(x => x.App_User).ToList();
                    }
                    else
                    {
                        tempGrade.MinGrade = 0;
                        tempGrade.MaxGrade = 0;
                    }
                    tempGrade.Topic = topic;
                    var title = topic.Title;
                    var titleLong = title;
                    if (title.Length >= 4)
                        title = title.Substring(0, 4) + "...";
                    tempGrade.Title = title;
                    tempGrade.FullTitle = titleLong;
                    tempGrades.Add(tempGrade);
                }
            }
            tempGrades = tempGrades.OrderBy(x => x.Title).ToList();
            foreach (var item in grades.Where(x => !x.TopicId.HasValue))
            {
                var tempGrade = new GradeTypeModel
                {
                    MyGradeDouble = item.SoActValue < 0 ? 0 : item.SoActValue
                };
                var minVal = allCoursegrades.Where(x => !x.TopicId.HasValue).Min(x => x.SoActValue);
                tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                var maxVal = allCoursegrades.Where(x => !x.TopicId.HasValue).Max(x => x.SoActValue);
                tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                tempGrade.MaxGardeUsers = allCoursegrades.Where(x => !x.TopicId.HasValue && x.SoActValue == maxVal).Select(x => x.App_User).ToList();
                var title = UT.SL.Model.Resource.App_Common.NoTopic;
                var titleLong = title;
                if (title.Length >= 4)
                    title = title.Substring(0, 4) + "...";
                tempGrade.Title = title;
                tempGrade.FullTitle = titleLong;
                tempGrades.Add(tempGrade);
            }
            var model = tempGrades.ToList();
            ViewBag.FullName = App_UserDAL.Get(userId).FirstName + " " + App_UserDAL.Get(userId).LastName;
            return PartialView(model);
        }

        public ActionResult GetUserKnowledgeProfileChart(Guid userId, int courseId)
        {
            var grades = App_UserDAL.GetAllUserAndCourseUserKnowledgeProfile(courseId, App_UserDAL.Get(userId).Id);
            var allCoursegrades = App_UserDAL.GetAllCourseUserKnowledgeProfile(courseId);
            var courseTopics = CourseDAL.Get(courseId).CourseAbstract.CourseTopcMappers.Select(x => x.Topic).ToList();
            var model = new FormModel<List<GradeTypeModel>, Course>();
            var tempGrades = new List<GradeTypeModel>();
            foreach (var topic in courseTopics)
            {
                if (grades.Any(x => x.TopicId == topic.Id))
                {
                    foreach (var item in grades.Where(x => x.TopicId == topic.Id))
                    {
                        var tempGrade = new GradeTypeModel
                        {
                            MyGradeDouble = item.Knowledge < 0 ? 0 : item.Knowledge
                        };
                        var minVal = allCoursegrades.Where(x => x.TopicId == item.TopicId).Min(x => x.Knowledge);
                        tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                        var maxVal = allCoursegrades.Where(x => x.TopicId == item.TopicId).Max(x => x.Knowledge);
                        tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                        tempGrade.Topic = topic;
                        var title = topic.Title;
                        var titleLong = title;
                        if (title.Length >= 4)
                            title = title.Substring(0, 4) + "...";
                        tempGrade.Title = title;
                        tempGrade.FullTitle = titleLong;
                        tempGrades.Add(tempGrade);
                    }
                }
                else
                {
                    var tempGrade = new GradeTypeModel
                    {
                        MyGradeDouble = 0
                    };
                    if (allCoursegrades.Any(x => x.TopicId == topic.Id))
                    {
                        var minVal = allCoursegrades.Where(x => x.TopicId == topic.Id).Min(x => x.Knowledge);
                        tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                        var maxVal = allCoursegrades.Where(x => x.TopicId == topic.Id).Max(x => x.Knowledge);
                        tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                    }
                    else
                    {
                        tempGrade.MinGrade = 0;
                        tempGrade.MaxGrade = 0;
                    }
                    tempGrade.Topic = topic;
                    var title = topic.Title;
                    var titleLong = title;
                    if (title.Length >= 4)
                        title = title.Substring(0, 4) + "...";
                    tempGrade.Title = title;
                    tempGrade.FullTitle = titleLong;
                    tempGrades.Add(tempGrade);
                }
            }
            tempGrades = tempGrades.OrderBy(x => x.Title).ToList();
            foreach (var item in grades.Where(x => !x.TopicId.HasValue))
            {
                var tempGrade = new GradeTypeModel
                {
                    MyGradeDouble = item.Knowledge < 0 ? 0 : item.Knowledge
                };
                var minVal = allCoursegrades.Where(x => !x.TopicId.HasValue).Min(x => x.Knowledge);
                tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                var maxVal = allCoursegrades.Where(x => !x.TopicId.HasValue).Max(x => x.Knowledge);
                tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                var title = UT.SL.Model.Resource.App_Common.NoTopic;
                var titleLong = title;
                if (title.Length >= 4)
                    title = title.Substring(0, 4) + "...";
                tempGrade.Title = title;
                tempGrade.FullTitle = titleLong;
                tempGrades.Add(tempGrade);
            }
            model.FormObject = tempGrades.ToList();
            model.ExtraKnownData = CourseDAL.Get(courseId);
            ViewBag.UserId = userId;
            ViewBag.CourseId = courseId;
            return PartialView(model);
        }

        public ActionResult KnowledgeDetails(Guid userId, int courseId, int position)
        {
            var grades = App_UserDAL.GetAllUserAndCourseUserKnowledgeProfile(courseId, App_UserDAL.Get(userId).Id);
            var allCoursegrades = App_UserDAL.GetAllCourseUserKnowledgeProfile(courseId);
            var courseTopics = CourseDAL.Get(courseId).CourseAbstract.CourseTopcMappers.Select(x => x.Topic).ToList();
            var tempGrades = new List<GradeTypeModel>();
            foreach (var topic in courseTopics)
            {
                if (grades.Any(x => x.TopicId == topic.Id))
                {
                    foreach (var item in grades.Where(x => x.TopicId == topic.Id))
                    {
                        var tempGrade = new GradeTypeModel
                        {
                            MyGradeDouble = item.Knowledge < 0 ? 0 : item.Knowledge
                        };
                        var minVal = allCoursegrades.Where(x => x.TopicId == item.TopicId).Min(x => x.Knowledge);
                        tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                        var maxVal = allCoursegrades.Where(x => x.TopicId == item.TopicId).Max(x => x.Knowledge);
                        tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                        tempGrade.MaxGardeUsers = allCoursegrades.Where(x => x.TopicId == item.TopicId && x.Knowledge == maxVal).Select(x => x.App_User).ToList();
                        tempGrade.Topic = topic;
                        var title = topic.Title;
                        var titleLong = title;
                        if (title.Length >= 4)
                            title = title.Substring(0, 4) + "...";
                        tempGrade.Title = title;
                        tempGrade.FullTitle = titleLong;
                        tempGrades.Add(tempGrade);
                    }
                }
                else
                {
                    var tempGrade = new GradeTypeModel
                    {
                        MyGradeDouble = 0
                    };
                    if (allCoursegrades.Any(x => x.TopicId == topic.Id))
                    {
                        var minVal = allCoursegrades.Where(x => x.TopicId == topic.Id).Min(x => x.Knowledge);
                        tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                        var maxVal = allCoursegrades.Where(x => x.TopicId == topic.Id).Max(x => x.Knowledge);
                        tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                        tempGrade.MaxGardeUsers = allCoursegrades.Where(x => x.TopicId == topic.Id && x.Knowledge == maxVal).Select(x => x.App_User).ToList();
                    }
                    else
                    {
                        tempGrade.MinGrade = 0;
                        tempGrade.MaxGrade = 0;
                    }
                    tempGrade.Topic = topic;
                    var title = topic.Title;
                    var titleLong = title;
                    if (title.Length >= 4)
                        title = title.Substring(0, 4) + "...";
                    tempGrade.Title = title;
                    tempGrade.FullTitle = titleLong;
                    tempGrades.Add(tempGrade);
                }
            }
            tempGrades = tempGrades.OrderBy(x => x.Title).ToList();
            foreach (var item in grades.Where(x => !x.TopicId.HasValue))
            {
                var tempGrade = new GradeTypeModel
                {
                    MyGradeDouble = item.Knowledge < 0 ? 0 : item.Knowledge
                };
                var minVal = allCoursegrades.Where(x => !x.TopicId.HasValue).Min(x => x.Knowledge);
                tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                var maxVal = allCoursegrades.Where(x => !x.TopicId.HasValue).Max(x => x.Knowledge);
                tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                tempGrade.MaxGardeUsers = allCoursegrades.Where(x => !x.TopicId.HasValue && x.Knowledge == maxVal).Select(x => x.App_User).ToList();
                var title = UT.SL.Model.Resource.App_Common.NoTopic;
                var titleLong = title;
                if (title.Length >= 4)
                    title = title.Substring(0, 4) + "...";
                tempGrade.Title = title;
                tempGrade.FullTitle = titleLong;
                tempGrades.Add(tempGrade);
            }
            var model = tempGrades.ToList()[position];
            ViewBag.FullName = App_UserDAL.Get(userId).FirstName + " " + App_UserDAL.Get(userId).LastName;
            return PartialView(model);
        }

        public ActionResult KnowledgetTableDetails(Guid userId, int courseId)
        {
            var grades = App_UserDAL.GetAllUserAndCourseUserKnowledgeProfile(courseId, App_UserDAL.Get(userId).Id);
            var allCoursegrades = App_UserDAL.GetAllCourseUserKnowledgeProfile(courseId);
            var courseTopics = CourseDAL.Get(courseId).CourseAbstract.CourseTopcMappers.Select(x => x.Topic).ToList();
            var tempGrades = new List<GradeTypeModel>();
            foreach (var topic in courseTopics)
            {
                if (grades.Any(x => x.TopicId == topic.Id))
                {
                    foreach (var item in grades.Where(x => x.TopicId == topic.Id))
                    {
                        var tempGrade = new GradeTypeModel
                        {
                            MyGradeDouble = item.Knowledge < 0 ? 0 : item.Knowledge
                        };
                        var minVal = allCoursegrades.Where(x => x.TopicId == item.TopicId).Min(x => x.Knowledge);
                        tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                        var maxVal = allCoursegrades.Where(x => x.TopicId == item.TopicId).Max(x => x.Knowledge);
                        tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                        tempGrade.MaxGardeUsers = allCoursegrades.Where(x => x.TopicId == item.TopicId && x.Knowledge == maxVal).Select(x => x.App_User).ToList();
                        tempGrade.Topic = topic;
                        var title = topic.Title;
                        var titleLong = title;
                        if (title.Length >= 4)
                            title = title.Substring(0, 4) + "...";
                        tempGrade.Title = title;
                        tempGrade.FullTitle = titleLong;
                        tempGrades.Add(tempGrade);
                    }
                }
                else
                {
                    var tempGrade = new GradeTypeModel
                    {
                        MyGradeDouble = 0
                    };
                    if (allCoursegrades.Any(x => x.TopicId == topic.Id))
                    {
                        var minVal = allCoursegrades.Where(x => x.TopicId == topic.Id).Min(x => x.Knowledge);
                        tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                        var maxVal = allCoursegrades.Where(x => x.TopicId == topic.Id).Max(x => x.Knowledge);
                        tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                        tempGrade.MaxGardeUsers = allCoursegrades.Where(x => x.TopicId == topic.Id && x.Knowledge == maxVal).Select(x => x.App_User).ToList();
                    }
                    else
                    {
                        tempGrade.MinGrade = 0;
                        tempGrade.MaxGrade = 0;
                    }
                    tempGrade.Topic = topic;
                    var title = topic.Title;
                    var titleLong = title;
                    if (title.Length >= 4)
                        title = title.Substring(0, 4) + "...";
                    tempGrade.Title = title;
                    tempGrade.FullTitle = titleLong;
                    tempGrades.Add(tempGrade);
                }
            }
            tempGrades = tempGrades.OrderBy(x => x.Title).ToList();
            foreach (var item in grades.Where(x => !x.TopicId.HasValue))
            {
                var tempGrade = new GradeTypeModel
                {
                    MyGradeDouble = item.Knowledge < 0 ? 0 : item.Knowledge
                };
                var minVal = allCoursegrades.Where(x => !x.TopicId.HasValue).Min(x => x.Knowledge);
                tempGrade.MinGrade = minVal < 0 ? 0 : minVal;
                var maxVal = allCoursegrades.Where(x => !x.TopicId.HasValue).Max(x => x.Knowledge);
                tempGrade.MaxGrade = maxVal < 0 ? 0 : maxVal;
                tempGrade.MaxGardeUsers = allCoursegrades.Where(x => !x.TopicId.HasValue && x.Knowledge == maxVal).Select(x => x.App_User).ToList();
                var title = UT.SL.Model.Resource.App_Common.NoTopic;
                var titleLong = title;
                if (title.Length >= 4)
                    title = title.Substring(0, 4) + "...";
                tempGrade.Title = title;
                tempGrade.FullTitle = titleLong;
                tempGrades.Add(tempGrade);
            }
            var model = tempGrades.ToList();
            ViewBag.FullName = App_UserDAL.Get(userId).FirstName + " " + App_UserDAL.Get(userId).LastName;
            return PartialView(model);
        }

        public ActionResult GetProgressChart(Guid userid, int courseId)
        {
            var model = new FormModel<List<ProgressModel>, Course>();
            var tempProgress = new List<ProgressModel>();
            tempProgress.Add(new ProgressModel
            {
                MyValue = 3,
                Rank = 1,
                Title = "Topic1",
                TopicValue = 5
            });
            tempProgress.Add(new ProgressModel
            {
                MyValue = 6,
                Rank = 2,
                Title = "Topic2",
                TopicValue = 3
            });
            tempProgress.Add(new ProgressModel
            {
                MyValue = 5,
                Rank = 3,
                Title = "Topic3",
                TopicValue = 5
            });
            tempProgress.Add(new ProgressModel
            {
                MyValue = 7,
                Rank = 4,
                Title = "Topic4",
                TopicValue = 10
            });
            tempProgress.Add(new ProgressModel
            {
                MyValue = 2,
                Rank = 5,
                Title = "Topic5",
                TopicValue = 3
            });
            model.FormObject = tempProgress.OrderBy(x => x.Rank).ToList();
            model.ExtraKnownData = CourseDAL.Get(courseId);
            return PartialView(model);
        }

    }
}
