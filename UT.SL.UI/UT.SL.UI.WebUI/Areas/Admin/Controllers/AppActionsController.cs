using System;
using System.Linq;
using System.Web.Mvc;
using UT.SL.Helper;
using UT.SL.UI.WebUI.Controllers;
using UT.SL.Model;
using UT.SL.Data.LINQ;
using UT.SL.DAL;
using System.Collections.Generic;
using UT.SL.BLL;
using UT.SL.Model.Enumeration;


namespace UT.SL.UI.WebUI.Areas.Admin.Controllers
{
    [Authorize]
    public class AppActionsController : BaseController
    {

        #region Action Tree

        public ActionResult GetActions(string Id = "-1", string operation = "")
        {
            var tn = new treeNode();
            if (Id == "-1" || Id == "0")
            {
                var qry = App_ActionDAL.GetAllAreas();
                tn = new treeNode() { attr = new NodeAttribute() { id = "0", title = UT.SL.Model.Resource.App_Common.Program }, data = UT.SL.Model.Resource.App_Common.Program, state = "open" };
                foreach (var q in qry.OrderBy(u => u))
                {
                    tn.children.Add(new treeNode() { attr = new NodeAttribute() { id = "ar_" + q, title = q, selected = false }, data = q, state = "closed" });
                }
                return Json(tn, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var tnl = new List<treeNode>();
                var s2 = Id.Split('_');
                switch (s2[0])
                {
                    case "ar":
                        var qar = App_ActionDAL.GetAllAreaControllers(Id.Substring(3, Id.Length - 3));
                        foreach (var q in qar)
                        {
                            tnl.Add(new treeNode() { attr = new NodeAttribute() { id = "cn_" + q, title = q }, data = q, state = "closed" });
                        }
                        break;
                    case "cn":
                        var qcn = App_ActionDAL.GetAllControllers(Id.Substring(3, Id.Length - 3));
                        foreach (var q in qcn)
                        {
                            tnl.Add(new treeNode() { attr = new NodeAttribute() { id = "ac_" + q.Id, title = q.ActionName }, data = q.ActionName, state = "closed" });
                        }
                        break;
                    case "ac":
                        break;
                }
                return Json(tnl, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DrawBookmarksTree()
        {
            return PartialView(App_ActionDAL.GetAllBookmarks());
        }

        public ActionResult DrawTree()
        {
            return PartialView(App_ActionDAL.GetAll());
        }

        public ActionResult ActionTree(int? rank)
        {
            if (rank.HasValue)
                ViewBag.LinkRank = rank.Value;
            return View();
        }

        public ActionResult ViewArea(int Id)
        {
            var obj = App_ActionDAL.Get(Id);
            if (obj != null)
                return PartialView(obj);
            return Content("");
        }

        public ActionResult ViewBoomarkArea(int Id)
        {
            var obj = App_ActionDAL.Get(Id);
            if (obj != null)
                return PartialView(obj);
            return Content("");
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(int id, App_Action obj)
        {
            var bpr = new BatchProcessResultModel();
            if (ModelState.IsValid)
            {
                if (id == obj.Id)
                {
                    var actionObj = App_ActionDAL.Get(id);
                    if (actionObj != null)
                    {
                        try
                        {
                            actionObj.Title = obj.Title;
                            actionObj.IsActive = obj.IsActive;
                            actionObj.RequireAuthorization = obj.RequireAuthorization;
                            actionObj.RequireAuthentication = obj.RequireAuthentication;
                            actionObj.IgnoreOwner = obj.IgnoreOwner;
                            actionObj.ReturnBlankIfNotAllowed = obj.ReturnBlankIfNotAllowed;
                            actionObj.Bookmark = obj.Bookmark;
                            var drm = (DALReturnModel<App_Action>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_Action),
                                       new Func<App_Action, BatchProcessResultModel, DALReturnModel<App_Action>>(App_ActionDAL.Update), actionObj, bpr);
                            bpr = drm.BPR;

                        }
                        catch (Exception ex)
                        {
                            bpr.AddError(ex.Message, true, true);
                        }
                    }
                    else
                        bpr.AddError(UT.SL.Model.Resource.App_Errors.BprActionNotFound, true, true);
                }
                else
                    bpr.AddError(UT.SL.Model.Resource.App_Errors.BprWrongActionCode, true, true);
            }
            else
                bpr.AddModelStateErrors(ModelState, true, true);
            //bpr.CoverMessageWithUL();
            return PartialView("ProcessResult", bpr);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult EditBookmarked(int id, App_Action obj)
        {
            var bpr = new BatchProcessResultModel();
            if (ModelState.IsValid)
            {
                if (id == obj.Id)
                {
                    var actionObj = App_ActionDAL.Get(id);
                    if (actionObj != null)
                    {
                        try
                        {
                            actionObj.Title = obj.Title;
                            actionObj.IsActive = obj.IsActive;
                            actionObj.RequireAuthorization = obj.RequireAuthorization;
                            actionObj.RequireAuthentication = obj.RequireAuthentication;
                            actionObj.IgnoreOwner = obj.IgnoreOwner;
                            actionObj.ReturnBlankIfNotAllowed = obj.ReturnBlankIfNotAllowed;
                            actionObj.Bookmark = obj.Bookmark;
                            actionObj.InNotification = obj.InNotification;
                            actionObj.InEmail = obj.InEmail;
                            actionObj.DoesNotHaveDAL = obj.DoesNotHaveDAL;
                            //actionObj.AssociatedObjectId = obj.AssociatedObjectId; 
                            if (obj.IsCredit)
                            {
                                actionObj.IsCredit = true;
                                actionObj.CreditOnlyPost = obj.CreditOnlyPost;
                                actionObj.StudentCredit = obj.StudentCredit;
                                actionObj.TACredit = obj.TACredit;
                                actionObj.TeacherCredit = obj.TeacherCredit;
                            }
                            else
                            {
                                actionObj.IsCredit = false;
                                actionObj.CreditOnlyPost = false;
                                actionObj.StudentCredit = 0;
                                actionObj.TACredit = 0;
                                actionObj.TeacherCredit = 0;
                            }
                            var drm = (DALReturnModel<App_Action>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_Action),
                                       new Func<App_Action, BatchProcessResultModel, DALReturnModel<App_Action>>(App_ActionDAL.UpdateBookmarked), actionObj, bpr);
                            bpr = drm.BPR;

                        }
                        catch (Exception ex)
                        {
                            bpr.AddError(ex.Message, true, true);
                        }
                    }
                    else
                        bpr.AddError(UT.SL.Model.Resource.App_Errors.BprActionNotFound, true, true);
                }
                else
                    bpr.AddError(UT.SL.Model.Resource.App_Errors.BprWrongActionCode, true, true);
            }
            else
                bpr.AddModelStateErrors(ModelState, true, true);
            //bpr.CoverMessageWithUL();
            return PartialView("ProcessResult", bpr);
        }

        public ActionResult Delete(int id)
        {
            var bpr = new BatchProcessResultModel();
            try
            {
                ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_Action),
                                       new Func<App_Action, DALReturnModel<App_Action>>(App_ActionDAL.Delete), new App_Action { Id = id });
            }
            catch (Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }

            return PartialView("ProcessResult", bpr);
        }

        #endregion

        public ActionResult Index()
        {
            return RedirectToAction("ActionTree");
        }

        public ActionResult ActionRoles(int id)
        {
            ViewBag.actionId = id;
            var qry = App_RoleDAL.GetAll();
            var rs = new List<ViewModel>();
            foreach (var p in qry)
            {
                var roleVal = App_PermissionDAL.GetByRole(id, p.Id);
                var role = new ViewModel() { Id = p.Id.ToString(), Tag = p.Description, Title = p.Title, Value = roleVal };
                rs.Add(role);
            }
            return PartialView(rs);
        }

        [HttpPost]
        public ActionResult SaveActionRoles(int id, FormCollection frm)
        {
            var bpr = new BatchProcessResultModel();
            try
            {
                ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_Permission),
                       new Func<int, DALReturnModel<App_Permission>>(App_PermissionDAL.DeleteAll), id);
                foreach (var k in frm.AllKeys.Where(u => u.StartsWith("chk_")))
                {
                    short roleId = short.Parse(k.Split('_')[1]);
                    var perm = new App_Permission() { ActionId = id, RoleId = roleId, CreateDate = DateTime.Now };
                    var drm = (DALReturnModel<App_Permission>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_Permission),
                                      new Func<App_Permission, BatchProcessResultModel, DALReturnModel<App_Permission>>(App_PermissionDAL.Add), perm, bpr);
                    bpr = drm.BPR;
                }
                bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
            }
            catch (Exception ex)
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.BprMainUnknownError, true, true);
                bpr.AddError(ex.Message, true, true);
            }

            bpr.CoverMessageWithUL();
            return PartialView("ProcessResult", bpr);
        }

    }
}
