using System;
using System.Linq;
using System.Web.Mvc;
using UT.SL.Helper;
using UT.SL.UI.WebUI.Controllers;
using UT.SL.Model;
using UT.SL.Data.LINQ;
using UT.SL.DAL;
using UT.SL.BLL;
using System.Collections.Generic;
using UT.SL.Model.Enumeration;


namespace UT.SL.UI.WebUI.Areas.Admin.Controllers
{


    [Authorize()]
    public class SocialGroupController : BaseController
    {

        public ActionResult Index(SocialGroupSearchModel model)
        {
            model.Area = "Admin";
            return View(model);
        }

        public ActionResult SocialGroupSearchModelView(SocialGroupSearchModel model)
        {
            model.Area = "Admin";
            return PartialView(model);
        }

        public PagedList<SocialGroup> SearchFilters(SocialGroupSearchModel model)
        {
            model.Area = "Admin";
            model.userId = CurrentUser.Id;
            var qry = SocialGroupDAL.Find(model);
            model.Update(model.PageSize, qry.Count());
            var ls = qry.ToList();
            var ql = new PagedList<SocialGroup>(ls, model);
            return ql;
        }

        public ActionResult IX(SocialGroupSearchModel model)
        {
            model.Area = "Admin";
            return PartialView(SearchFilters(model));
        }

        public ActionResult Create()
        {
            var model = new SocialGroup();
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        public ActionResult CreateForPanel()
        {
            var model = new SocialGroup();
            ViewBag.Panel = true;
            if (Request.IsAjaxRequest())
            {
                return PartialView("Create", model);
            }
            return View("Create", model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(SocialGroup model, byte panel = 0)
        {
            var bpr = new BatchProcessResultModel();
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    model.CreateUserId = CurrentUser.Id;                    
                    var drm = (DALReturnModel<SocialGroup>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.SocialGroup),
                                       new Func<SocialGroup, BatchProcessResultModel, DALReturnModel<SocialGroup>>(SocialGroupDAL.Add), model, bpr);
                    bpr = drm.BPR;
                    var groupId = drm.ReturnObject.Id;
                    if (bpr.Failed > 0)
                    {
                        return PartialView("ProcessResult", bpr);
                    }
                    else
                    {
                        var groupMember = new AddGroupMemberModel
                        {
                            GroupeId = groupId,
                            IsAdmin = 1,
                            Member = CurrentUser.GuidId
                        };
                        bpr = new BatchProcessResultModel();
                        var drm2 = (DALReturnModel<GroupMember>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.GroupMember),
                                       new Func<AddGroupMemberModel, BatchProcessResultModel, DALReturnModel<GroupMember>>(SocialGroupDAL.AddGroupMember), groupMember, bpr);
                        bpr = drm2.BPR;
                        if (panel == 0)
                            bpr.SuccessClientScript = "$('#searchForm').submit();";
                        else
                            bpr.SuccessClientScript = "UpdatePanel('" + Url.Action("ManageSocialGroupsPanel", "SocialGroup") + "')";
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
            var model = SocialGroupDAL.Get(Id);
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        public ActionResult EditGroupForPanel(int Id)
        {
            var model = SocialGroupDAL.Get(Id);
            ViewBag.Panel = true;
            if (Request.IsAjaxRequest())
            {
                return PartialView("Edit", model);
            }
            return View("Edit", model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(SocialGroup model, byte panel = 0)
        {
            var bpr = new BatchProcessResultModel();
            if (ModelState.IsValid)
            {
                try
                {
                    var drm = (DALReturnModel<SocialGroup>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.SocialGroup),
                                       new Func<SocialGroup, BatchProcessResultModel, DALReturnModel<SocialGroup>>(SocialGroupDAL.Update), model, bpr);
                    bpr = drm.BPR;
                    if (bpr.Failed > 0)
                    {
                        return PartialView("ProcessResult", bpr);
                    }
                    else
                    {
                        if (panel == 0)
                            bpr.SuccessClientScript = "$('#searchForm').submit();";
                        else
                            bpr.SuccessClientScript = "UpdatePanel('" + Url.Action("ManageSocialGroupsPanel", "SocialGroup") + "')";
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
            var model = SocialGroupDAL.Get(Id);
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        public ActionResult DeleteForPanel(int Id)
        {
            var model = SocialGroupDAL.Get(Id);
            ViewBag.Panel = true;
            if (Request.IsAjaxRequest())
            {
                return PartialView("Delete", model);
            }
            return View("Delete", model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(SocialGroup model, byte panel = 0)
        {
            var bpr = new BatchProcessResultModel();
            try
            {
                var drm = (DALReturnModel<SocialGroup>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.SocialGroup),
                                       new Func<SocialGroup, DALReturnModel<SocialGroup>>(SocialGroupDAL.Delete), model);
                   
                if (drm.ReturnObject.Id > 0)
                {
                    if (panel == 0)
                        bpr.SuccessClientScript = "$('#searchForm').submit();";
                    else
                        bpr.SuccessClientScript = "UpdatePanel('" + Url.Action("ManageSocialGroupsPanel", "SocialGroup") + "')"; bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true);
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

        public ActionResult MamangeMmber(int Id)
        {
            var model = SocialGroupDAL.Get(Id);
            return PartialView(model);
        }

        public ActionResult RemoveMmber(int Id)
        {
            var bpr = new BatchProcessResultModel();
            try
            {
                var member = SocialGroupDAL.GetMembership(Id);
                ObjectStreamManager.ObjectResourceToStreamForRemoveSocialGroupMemeber(member.SocialGroupId.Value, member.UserId, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
                var drm = (DALReturnModel<GroupMember>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.GroupMember),
                                      new Func<int, DALReturnModel<GroupMember>>(SocialGroupDAL.DeleteMember), Id);
               
                if (drm.ReturnObject.Id > 0)
                {
                    bpr.SuccessClientScript = "$('#searchMemberForm').submit();";
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

        public ActionResult RemoveMember(int Id, byte largepic = 0)
        {
            var groupId = 0;
            try
            {
                var member = SocialGroupDAL.GetMembership(Id);
                ObjectStreamManager.ObjectResourceToStreamForRemoveSocialGroupMemeber(member.SocialGroupId.Value, member.UserId, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
                var drm = (DALReturnModel<SocialGroup>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.SocialGroup),
                                     new Func<int, DALReturnModel<SocialGroup>>(SocialGroupDAL.DeleteSocialMember), Id);
              
                groupId = drm.ReturnObject.Id;
            }
            catch (System.Exception)
            {
            }
            if (largepic == 1)
                return PartialView("GetMembersLargePic", SocialGroupDAL.GetAllMemebrs(groupId));
            return PartialView("GetMembers", SocialGroupDAL.GetAllMemebrs(groupId));
        }

        [HttpPost]
        public ActionResult RemoveMemberByPanel(int gId, Guid userId)
        {
            try
            {
                GroupMemberDAL.DeleteSocialGroupMember(gId, userId);
                ObjectStreamManager.ObjectResourceToStreamForRemoveSocialGroupMemeber(gId, App_UserDAL.Get(userId).Id, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
            }
            catch (System.Exception)
            {
            }
            return Content("OK");
        }

        public ActionResult AddMember(int Id, byte largepic = 0)
        {
            var model = new AddSocialGroupMemberModel { GroupeId = Id };
            ViewBag.Largepic = largepic;
            return PartialView(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult AddMember(AddSocialGroupMemberModel model, byte largepic = 0)
        {
            var bpr = new BatchProcessResultModel();
            if (ModelState.IsValid)
            {
                try
                {
                    model.IsAdmin = 0;
                    if (!string.IsNullOrEmpty(model.Member))
                    {
                        if (model.Member.StringNormalizer().Split('-').Count() == 2 && !string.IsNullOrEmpty(model.Member.StringNormalizer().Split('-').Last()))
                        {
                            model.Member = model.Member.StringNormalizer().Split('-').Last();
                            var drm = (DALReturnModel<GroupMember>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.GroupMember),
                                       new Func<AddSocialGroupMemberModel, BatchProcessResultModel, DALReturnModel<GroupMember>>(SocialGroupDAL.AddSocialGroupMember), model, bpr);
                            bpr = drm.BPR;
                            var gmember = drm.ReturnObject;
                            if (gmember != null)
                                ObjectStreamManager.ObjectResourceToStreamForNewSocialGroupMemeber(model.GroupeId, gmember.UserId, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
                        }
                    }
                    if (bpr.Failed > 0)
                    {
                        //return PartialView("ProcessResult", bpr);
                    }
                    else
                    {
                        bpr.SuccessClientScript = "$('#searchMemberForm').submit();";
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
            //return PartialView("ProcessResult", bpr);
            var members = SocialGroupDAL.Get(model.GroupeId).GroupMembers.ToList();
            if (largepic == 1)
                return PartialView("GetMembersLargePic", members);
            return PartialView("GetMembers", members);
        }

        public ActionResult GetMembers(int Id)
        {
            var model = SocialGroupDAL.Get(Id).GroupMembers.ToList();
            return PartialView(model);
        }

        public ActionResult GetMembersLargePic(int Id)
        {
            var model = SocialGroupDAL.Get(Id).GroupMembers.ToList();
            return PartialView(model);
        }

        public ActionResult GetTopMembers(int Id)
        {
            var model = SocialGroupDAL.Get(Id).GroupMembers.Where(x => x.IsCircleAdmin != 1).ToList();
            return PartialView(model);
        }

        public ActionResult GetUsers(string title)
        {
            var user = title.Split(new char[] { '،', ',', ' ' }).Last().StringNormalizer().Trim();
            var model = App_UserDAL.GetAll().Where(x => x.UserName.ToLower().Contains(user) || x.FirstName.ToLower().Contains(user) || x.LastName.ToLower().Contains(user) || x.Email.ToLower().Contains(user)).Select(x => new { id = x.GuidId, title = string.Format("{0} {1} - {2}", x.FirstName, x.LastName, x.UserName) }).Take(10).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManageSocialGroupsPanel()
        {
            var model = SocialGroupDAL.GetAllCreatedByUserId(CurrentUser.Id);
            return PartialView(model);
        }

        public ActionResult GetTopMembersForPanel(int Id)
        {
            var model = SocialGroupDAL.Get(Id).GroupMembers.Where(x => x.IsCircleAdmin != 1).ToList();
            return PartialView(model);
        }

        public ActionResult GetMembersForManageForPanel(int Id)
        {
            var model = SocialGroupDAL.Get(Id).GroupMembers.Where(x => x.IsCircleAdmin != 1).ToList();
            return PartialView(model);
        }

        public ActionResult EditForPanel(int Id)
        {
            var model = SocialGroupDAL.Get(Id);
            return PartialView(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult AddMemberPanel(AddSocialGroupMemberModel model)
        {
            var bpr = new BatchProcessResultModel();
            if (ModelState.IsValid)
            {
                try
                {
                    model.IsAdmin = 0;
                    if (!string.IsNullOrEmpty(model.Member))
                    {
                        if (model.Member.StringNormalizer().Split('-').Count() == 2 && !string.IsNullOrEmpty(model.Member.StringNormalizer().Split('-').Last()))
                        {
                            model.Member = model.Member.StringNormalizer().Split('-').Last();
                            var drm = (DALReturnModel<GroupMember>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.GroupMember),
                                      new Func<AddSocialGroupMemberModel, BatchProcessResultModel, DALReturnModel<GroupMember>>(SocialGroupDAL.AddSocialGroupMember), model, bpr);
                            bpr = drm.BPR;
                            var gmember = drm.ReturnObject;
                            if (gmember != null)
                                ObjectStreamManager.ObjectResourceToStreamForNewSocialGroupMemeber(model.GroupeId, gmember.UserId, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
                        }
                    }
                    if (bpr.Failed > 0)
                    {
                        //return PartialView("ProcessResult", bpr);
                    }
                    else
                    {
                        bpr.SuccessClientScript = "$('#searchMemberForm').submit();";
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
            return Content("OK");
        }

        public ActionResult ManageOneSocialGroupsPanel(string id)
        {
            var ids = id.Split('_');
            if (ids.Count() == 2)
            {
                var model = SocialGroupDAL.Get(Int32.Parse(ids.Last()));
                if (model != null)
                {
                    return PartialView(model);
                }
            }
            return Content("");
        }

        public ActionResult SocialGroupsReviewPanel()
        {
            var model = new FormModel<List<SocialGroup>, List<App_User>>();
            model.FormObject = SocialGroupDAL.GetAllByUserId(CurrentUser.Id);
            model.ExtraKnownData = SocialGroupDAL.GetAllUserUveThisUIserInGoups(CurrentUser.Id);
            return PartialView(model);
        }

    }
}
