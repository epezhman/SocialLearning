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
    public class LearningGroupController : BaseController
    {

        public ActionResult Create(int id)
        {
            var model = new FormModel<LearningGroup, SelectListItems>();
            model.FormObject = new LearningGroup { CourseId = id };
            //model.ExtraKnownData = new SelectListItems
            //{
            //    Items = App_UserEnrolementDAL.GetAllByCourse(id).Select(x => new SelectListItem { Value = x.App_User.Id.ToString(), Text = string.Format("{0} {1} - {2}", x.App_User.FirstName, x.App_User.LastName, x.Type) }).Distinct()
            //};
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        public ActionResult CreateForPanel(int id)
        {
            var model = new FormModel<LearningGroup, SelectListItems>();
            model.FormObject = new LearningGroup { CourseId = id };
            ViewBag.Panel = true;
            //model.ExtraKnownData = new SelectListItems
            //{
            //    Items = App_UserEnrolementDAL.GetAllByCourse(id).Select(x => new SelectListItem { Value = x.App_User.Id.ToString(), Text = string.Format("{0} {1} - {2}", x.App_User.FirstName, x.App_User.LastName, x.Type) }).Distinct()
            //};
            if (Request.IsAjaxRequest())
            {
                return PartialView("Create", model);
            }
            return View("Create", model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(FormModel<LearningGroup, SelectListItems> model, byte panel = 0)
        {
            var bpr = new BatchProcessResultModel();
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    model.FormObject.CreateUserId = CurrentUser.Id;
                    var drm = (DALReturnModel<LearningGroup>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.LearningGroup),
                            new Func<LearningGroup, BatchProcessResultModel, DALReturnModel<LearningGroup>>(LearningGroupDAL.Add), model.FormObject, bpr);
                    bpr = drm.BPR;
                    var groupId = drm.ReturnObject.Id;
                    if (bpr.Failed > 0)
                    {
                        return PartialView("ProcessResult", bpr);
                    }
                    else
                    {
                        //if (model.ExtraKnownData.SelectedIds.Any())
                        //    foreach (var item in model.ExtraKnownData.SelectedIds)
                        //    {
                        //        var groupMember = new GroupMember
                        //        {
                        //            UserId = Int32.Parse(item),
                        //            LearningGroupId = groupId
                        //        };
                        //        GroupMemberDAL.Add(groupMember, ref bpr);
                        //    }
                        if (panel == 0)
                            bpr.SuccessClientScript = "$('#submitLearningForm').submit();";
                        else
                            bpr.SuccessClientScript = "UpdatePanel('" + Url.Action("ManageLearningGroupsPanel", "LearningGroup", new { Id = drm.ReturnObject.CourseId }) + "')";
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
            var model = new FormModel<LearningGroup, SelectListItems>();
            model.FormObject = LearningGroupDAL.Get(Id);
            //model.ExtraKnownData = new SelectListItems
            //{
            //    SelectedIds = model.FormObject.GroupMembers.Select(x => x.UserId.ToString()).ToArray(),
            //    Items = App_UserEnrolementDAL.GetAllByCourse(model.FormObject.CourseId).Select(x => new SelectListItem { Value = x.App_User.Id.ToString(), Text = string.Format("{0} {1} - {2}", x.App_User.FirstName, x.App_User.LastName, x.Type) }).Distinct()
            //};
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        public ActionResult EditGroupForPanel(int Id)
        {
            var model = new FormModel<LearningGroup, SelectListItems>();
            model.FormObject = LearningGroupDAL.Get(Id);
            ViewBag.Panel = true;
            //model.ExtraKnownData = new SelectListItems
            //{
            //    SelectedIds = model.FormObject.GroupMembers.Select(x => x.UserId.ToString()).ToArray(),
            //    Items = App_UserEnrolementDAL.GetAllByCourse(model.FormObject.CourseId).Select(x => new SelectListItem { Value = x.App_User.Id.ToString(), Text = string.Format("{0} {1} - {2}", x.App_User.FirstName, x.App_User.LastName, x.Type) }).Distinct()
            //};
            if (Request.IsAjaxRequest())
            {
                return PartialView("Edit", model);
            }
            return View("Edit", model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(FormModel<LearningGroup, SelectListItems> model, byte panel = 0)
        {
            var bpr = new BatchProcessResultModel();
            if (ModelState.IsValid)
            {
                try
                {
                    var drm = (DALReturnModel<LearningGroup>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.LearningGroup),
                            new Func<LearningGroup, BatchProcessResultModel, DALReturnModel<LearningGroup>>(LearningGroupDAL.Update), model.FormObject, bpr);
                    bpr = drm.BPR;
                    if (bpr.Failed > 0)
                    {
                        return PartialView("searchFormGroups", bpr);
                    }
                    else
                    {
                        //GroupMemberDAL.DeleteAll(model.FormObject.Id);
                        //if (model.ExtraKnownData.SelectedIds.Any())
                        //    foreach (var item in model.ExtraKnownData.SelectedIds)
                        //    {
                        //        var groupMember = new GroupMember
                        //        {
                        //            UserId = Int32.Parse(item),
                        //            LearningGroupId = model.FormObject.Id
                        //        };
                        //        GroupMemberDAL.Add(groupMember, ref bpr);
                        //    }
                        if (panel == 0)
                            bpr.SuccessClientScript = "$('#submitLearningForm').submit();";
                        else
                            bpr.SuccessClientScript = "UpdatePanel('" + Url.Action("ManageLearningGroupsPanel", "LearningGroup", new { Id = drm.ReturnObject.CourseId }) + "')";
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
            var model = LearningGroupDAL.Get(Id);
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        public ActionResult DeleteForPanel(int Id)
        {
            var model = LearningGroupDAL.Get(Id);
            ViewBag.Panel = true;
            if (Request.IsAjaxRequest())
            {
                return PartialView("Delete", model);
            }
            return View("Delete", model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(LearningGroup model, byte panel = 0)
        {
            var bpr = new BatchProcessResultModel();
            try
            {
                var courseId = LearningGroupDAL.Get(model.Id).CourseId;
                GroupMemberDAL.DeleteAll(model.Id);
                var drm = (DALReturnModel<LearningGroup>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.LearningGroup),
                                      new Func<LearningGroup, DALReturnModel<LearningGroup>>(LearningGroupDAL.Delete), model);
                if (drm.ReturnObject.Id > 0)
                {
                    if (panel == 0)
                        bpr.SuccessClientScript = "$('#submitLearningForm').submit();";
                    else
                        bpr.SuccessClientScript = "UpdatePanel('" + Url.Action("ManageLearningGroupsPanel", "LearningGroup", new { Id = courseId }) + "')"; bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true);
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

        public ActionResult CourseLearnignGroups(int Id)
        {
            ViewBag.CourseId = Id;
            var model = LearningGroupDAL.GetAllByCourse(Id);
            return PartialView(model);
        }

        public ActionResult MamangeMmber(int Id)
        {
            var model = LearningGroupDAL.Get(Id);
            return PartialView(model);
        }

        public ActionResult RemoveMember(int Id)
        {
            var gId = 0;
            try
            {
                var thisOne = GroupMemberDAL.Get(Id);
                gId = thisOne.LearningGroupId.Value;
                ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.GroupMember),
                                      new Func<GroupMember, DALReturnModel<GroupMember>>(GroupMemberDAL.Delete), new GroupMember { Id = Id });
                ObjectStreamManager.ObjectResourceToStreamForRemoveLearningGroupMemeber(gId, thisOne.UserId, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
            }
            catch (System.Exception)
            {
            }
            return PartialView("GetMembersLargePic", GroupMemberDAL.GetAll(gId).ToList());
        }

        [HttpPost]
        public ActionResult RemoveMemberByPanel(int gId, Guid userId)
        {
            try
            {
                GroupMemberDAL.DeleteLearningGroupMember(gId, userId);
                ObjectStreamManager.ObjectResourceToStreamForRemoveLearningGroupMemeber(gId, App_UserDAL.Get(userId).Id, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
            }
            catch (System.Exception)
            {
            }
            return Content("OK");
        }

        public ActionResult AddMember(int Id)
        {
            var model = new AddLearningGroupMemberModel { GroupeId = Id };
            return PartialView(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult AddMember(AddLearningGroupMemberModel model)
        {
            var bpr = new BatchProcessResultModel();
            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(model.Member))
                    {
                        if (model.Member.StringNormalizer().Split('-').Count() == 2 && !string.IsNullOrEmpty(model.Member.StringNormalizer().Split('-').Last()))
                        {
                            model.Member = model.Member.StringNormalizer().Split('-').Last();
                            var drm = (DALReturnModel<GroupMember>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.GroupMember),
                                       new Func<AddLearningGroupMemberModel, BatchProcessResultModel, DALReturnModel<GroupMember>>(GroupMemberDAL.AddMember), model, bpr);
                            bpr = drm.BPR;
                            var gmember = drm.ReturnObject;
                            if (gmember != null)
                                ObjectStreamManager.ObjectResourceToStreamForNewLearningGroupMemeber(model.GroupeId, gmember.UserId, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
                        }
                    }
                }
                catch (System.Exception)
                {
                }
            }
            else
            {
                bpr.AddModelStateErrors(ModelState);
            }
            var members = GroupMemberDAL.GetAll(model.GroupeId).ToList();
            return PartialView("GetMembersLargePic", members);
        }

        public ActionResult GetMembersLargePic(int Id)
        {
            var model = GroupMemberDAL.GetAll(Id).ToList();
            return PartialView(model);
        }

        public ActionResult GetTopMembershipMembers(int Id)
        {
            var model = GroupMemberDAL.GetAll(Id).ToList();
            return PartialView(model);
        }

        public ActionResult GetUsers(string title, int groupId)
        {
            var user = title.Split(new char[] { '،', ',', ' ' }).Last().StringNormalizer().Trim();
            var courseId = LearningGroupDAL.Get(groupId).CourseId;
            var model = App_UserEnrolementDAL.GetAll(user, courseId).Take(10).Select(x => new { id = x.GuidId, title = string.Format("{0} {1} - {2}", x.FirstName, x.LastName, x.UserName) }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManageLearningGroupsPanel(Course course)
        {
            ViewBag.CourseId = course.Id;
            var model = LearningGroupDAL.GetAllByCourse(course.Id);
            return PartialView(model);
        }

        public ActionResult GetTopMembersForPanel(int Id)
        {
            var model = LearningGroupDAL.Get(Id).GroupMembers.Where(x => x.IsCircleAdmin != 1).ToList();
            return PartialView(model);
        }

        public ActionResult GetMembersForManageForPanel(int Id)
        {
            var model = LearningGroupDAL.Get(Id).GroupMembers.Where(x => x.IsCircleAdmin != 1).ToList();
            return PartialView(model);
        }

        public ActionResult EditForPanel(int Id)
        {
            var model = LearningGroupDAL.Get(Id);
            return PartialView(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult AddMemberPanel(AddLearningGroupMemberModel model)
        {
            var bpr = new BatchProcessResultModel();
            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(model.Member))
                    {
                        if (model.Member.StringNormalizer().Split('-').Count() == 2 && !string.IsNullOrEmpty(model.Member.StringNormalizer().Split('-').Last()))
                        {
                            model.Member = model.Member.StringNormalizer().Split('-').Last();
                            var drm = (DALReturnModel<GroupMember>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.GroupMember),
                                       new Func<AddLearningGroupMemberModel, BatchProcessResultModel, DALReturnModel<GroupMember>>(GroupMemberDAL.AddMember), model, bpr);
                            bpr = drm.BPR;
                            var gmember = drm.ReturnObject;
                            if (gmember != null)
                                ObjectStreamManager.ObjectResourceToStreamForNewLearningGroupMemeber(model.GroupeId, gmember.UserId, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
                        }
                    }
                }
                catch (System.Exception)
                {
                }
            }
            else
            {
                bpr.AddModelStateErrors(ModelState);
            }

            return Content("OK");
        }

        public ActionResult ManageOneLearningGroupsPanel(string id)
        {
            var ids = id.Split('_');
            if (ids.Count() == 2)
            {
                var model = LearningGroupDAL.Get(Int32.Parse(ids.Last()));
                if (model != null)
                {
                    return PartialView(model);
                }
            }
            return Content("");
        }

    }
}
