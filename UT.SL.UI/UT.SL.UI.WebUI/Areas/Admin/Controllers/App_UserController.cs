using System;
using System.Linq;
using System.Web.Mvc;
using UT.SL.Helper;
using UT.SL.UI.WebUI.Controllers;
using UT.SL.Model;
using UT.SL.Data.LINQ;
using UT.SL.DAL;
using System.Web;
using System.Collections.Generic;
using UT.SL.BLL;
using System.Web.Security;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using UT.SL.Model.Enumeration;
//using DotNetOpenAuth.AspNet;
//using Microsoft.Web.WebPages.OAuth;

namespace UT.SL.UI.WebUI.Areas.Admin.Controllers
{

    [Authorize()]
    public class App_UserController : BaseController
    {
        public ActionResult AccountIndex()
        {
            return View(CurrentUser);
        }

        public ActionResult ViewAccount(Guid Id)
        {
            var user = App_UserDAL.Get(Id);
            if (CurrentUser.UserName == user.UserName)
            {
                ViewBag.ShowAccountOptions = true;
            }
            return PartialView(user);
        }

        public ActionResult EditEmailSetting(Guid Id)
        {
            var user = App_UserDAL.Get(Id);
            var db = (DALReturnModel<App_UserProfile>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_UserProfile),
                                      new Func<int, DALReturnModel<App_UserProfile>>(App_UserProfileDAL.GetOrAddIfNotExist), user.Id);
            var model = db.ReturnObject;
            if (CurrentUser.UserName != user.UserName)
            {
                model = new App_UserProfile();
            }
            return PartialView(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult EditEmailSetting(App_UserProfile model)
        {
            var bpr = new BatchProcessResultModel();
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    var drm = ((DALReturnModel<App_UserProfile>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_UserProfile),
                         new Func<App_UserProfile, BatchProcessResultModel, DALReturnModel<App_UserProfile>>(App_UserProfileDAL.Update), model, bpr));
                    bpr = drm.BPR;
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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        //[AllowAnonymous]
        //public ActionResult ExternalLoginCallback(string returnUrl)
        //{
        //    AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        //    if (!result.IsSuccessful)
        //    {
        //        return RedirectToAction("ExternalLoginFailure");
        //    }

        //    if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
        //    {
        //        return RedirectToLocal(returnUrl);
        //    }

        //    if (User.Identity.IsAuthenticated)
        //    {
        //        // If the current user is logged in add the new account
        //        OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
        //        return RedirectToLocal(returnUrl);
        //    }
        //    else
        //    {
        //        // User is new, ask for their desired membership name
        //        string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
        //        ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
        //        ViewBag.ReturnUrl = returnUrl;
        //        return RedirectToLocal(returnUrl);
        //    }
        //}

        //internal class ExternalLoginResult : ActionResult
        //{
        //    public ExternalLoginResult(string provider, string returnUrl)
        //    {
        //        Provider = provider;
        //        ReturnUrl = returnUrl;
        //    }

        //    public string Provider { get; private set; }
        //    public string ReturnUrl { get; private set; }

        //    public override void ExecuteResult(ControllerContext context)
        //    {
        //        OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
        //    }
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        //}

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        //[AllowAnonymous]
        //public ActionResult ExternalLoginsList(string returnUrl)
        //{
        //    ViewBag.ReturnUrl = returnUrl;
        //    return PartialView( OAuthWebSecurity.RegisteredClientData);
        //}

        [AllowAnonymous]
        public ActionResult LogIn(string returnUrl, int? rank)
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home", new { area = "" });
            if (rank.HasValue)
                ViewBag.LinkRank = rank.Value;
            return View(new LogInModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LogIn(LogInModel model)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    var user = App_UserDAL.Get(model.UserName);
                    if (user.IsDeleted)
                    {
                        ModelState.AddModelError("", UT.SL.Model.Resource.App_Errors.DeletedAccount);
                        return View(model);
                    }
                    if (!user.IsActive)
                    {
                        //ModelState.AddModelError("", UT.SL.Model.Resource.App_Errors.UnactiveAccount);
                        //return View(model);
                        return RedirectToAction("Confirm", "App_User", new { area = "Admin", id = user.Email.Replace('@', '_').Replace('.', '-') });
                    }
                    FormsAuthentication.SetAuthCookie(model.UserName.StringNormalizer(), model.RememberMe);
                    if (Url.IsLocalUrl(model.ReturnUrl) && model.ReturnUrl.Length > 1 && model.ReturnUrl.StartsWith("/")
                        && !model.ReturnUrl.StartsWith("//") && !model.ReturnUrl.StartsWith("/\\"))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                }
                else
                {
                    ModelState.AddModelError("", UT.SL.Model.Resource.App_Errors.InvalidUserPassword);
                }
            }
            model.Password = string.Empty;
            ViewBag.Reset = true;
            return View();
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [AllowAnonymous]
        public ActionResult Confirm(string id, bool retry = false)
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home", new { area = "" });
            if (!string.IsNullOrEmpty(id))
            {
                Guid userId;
                if (id.ToLower() == "done")
                {
                    ViewBag.Code = 1;
                    return View();
                }
                if (Guid.TryParse(id, out userId))
                {
                    var user = App_UserDAL.Get(userId);
                    if (user != null)
                    {
                        user.IsActive = true;
                        ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_User),
                                      new Func<App_User, DALReturnModel<App_User>>(App_UserDAL.UpdateActivity), user);
                        FormsAuthentication.SetAuthCookie(user.UserName.StringNormalizer(), false);
                        ViewBag.Code = 2;
                        return View();
                    }
                    else
                    {
                        ViewBag.Code = 3;
                        return View();
                    }
                }
                id = id.Replace('_', '@').Replace('-', '.');
                var userByEmail = App_UserDAL.GetEmail(id);
                if (userByEmail != null)
                {
                    if (!retry)
                    {
                        ViewBag.Email = userByEmail.Email.Replace('@', '_').Replace('.', '-');
                        ViewBag.Code = 4;
                        return View();
                    }
                    else
                    {
                        var sentMail = EmailDAL.GetLastByUserId(userByEmail.Id);
                        if (sentMail == null)
                        {
                            EmailManager.SendEmailComfirmationMail((int)UT.SL.Model.Enumeration.MailServerType.DoosMooc, userByEmail.FirstName, userByEmail.GuidId, userByEmail.Email, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Email));
                            ViewBag.Code = 6;
                            return View();
                        }
                        if (sentMail.CreateDate.Value.AddMinutes(30) >= DateTime.Now)
                        {
                            ViewBag.Code = 5;
                            return View();
                        }
                        else
                        {
                            EmailManager.SendEmailComfirmationMail((int)UT.SL.Model.Enumeration.MailServerType.DoosMooc, userByEmail.FirstName, userByEmail.GuidId, userByEmail.Email, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Email));
                            ViewBag.Code = 6;
                            return View();
                        }
                    }
                }
            }
            ViewBag.Code = 7;
            return View();
        }

        [AllowAnonymous]
        public ActionResult Register(int? rank)
        {
            if (rank.HasValue)
                ViewBag.LinkRank = rank.Value;
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home", new { area = "" });
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, model.FirstName, model.LastName, true, null, out createStatus);
                if (createStatus == MembershipCreateStatus.Success)
                {
                    //FormsAuthentication.SetAuthCookie(model.UserName.StringNormalizer(), false);
                    var user = App_UserDAL.GetEmail(model.Email);
                    EmailManager.SendEmailComfirmationMail((int)UT.SL.Model.Enumeration.MailServerType.DoosMooc, user.FirstName, user.GuidId, model.Email, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Email));
                    return RedirectToAction("Confirm", "App_User", new { area = "Admin", id = "Done" });
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }
            ViewBag.Reset = true;
            return View(model);
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return PartialView(new ChangePasswordModel { Guid = CurrentUser.GuidId });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            var bpr = new BatchProcessResultModel();
            if (ModelState.IsValid)
            {
                try
                {
                    var drm = (DALReturnModel<App_User>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_User),
                                       new Func<ChangePasswordModel, BatchProcessResultModel, DALReturnModel<App_User>>(App_UserDAL.UpdatePassword), model, bpr);
                    bpr = drm.BPR;
                    if (bpr.Failed > 0)
                    {
                        return PartialView("ProcessResult", bpr);
                    }
                    else
                    {
                        //bpr.SuccessClientScript = "$('#searchForm').submit(); closeCreditsinfo();";
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

        [Authorize]
        public ActionResult ChangeEmail()
        {
            return PartialView(new ChangeEmailModel { Guid = CurrentUser.GuidId });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeEmail(ChangeEmailModel model)
        {
            var bpr = new BatchProcessResultModel();
            if (ModelState.IsValid)
            {
                try
                {
                    var drm = ((DALReturnModel<App_User>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_User),
                        new Func<ChangeEmailModel, BatchProcessResultModel, DALReturnModel<App_User>>(App_UserDAL.UpdateEmail), model, bpr));
                    bpr = drm.BPR;
                    var user = drm.ReturnObject;
                    if (bpr.Failed > 0)
                    {
                        return PartialView("ProcessResult", bpr);
                    }
                    else
                    {
                        EmailManager.SendEmailComfirmationMail((int)UT.SL.Model.Enumeration.MailServerType.DoosMooc, user.FirstName, user.GuidId, user.Email, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Email));
                        //bpr.SuccessClientScript = "$('#searchForm').submit(); closeCreditsinfo();";
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


        [Authorize]
        public ActionResult DeleteAccount()
        {
            return PartialView(new DeleteAccountModel { Guid = CurrentUser.GuidId });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAccount(DeleteAccountModel model)
        {
            var bpr = new BatchProcessResultModel();
            if (ModelState.IsValid)
            {
                try
                {
                    var drm = (DALReturnModel<App_User>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_User),
                                       new Func<DeleteAccountModel, BatchProcessResultModel, DALReturnModel<App_User>>(App_UserDAL.UpdateDeleteUser), model, bpr);
                    bpr = drm.BPR;
                    if (bpr.Failed > 0)
                    {
                        return PartialView("ProcessResult", bpr);
                    }
                    else
                    {
                        bpr.SuccessClientScript = "location.reload();";
                    }
                }
                catch (System.Exception)
                {
                    bpr.AddError(UT.SL.Model.Resource.App_Errors.BprMainUnknownError, true, true);
                    return PartialView("ProcessResult", bpr);
                }
            }
            else
            {
                bpr.AddModelStateErrors(ModelState);
                return PartialView("ProcessResult", bpr);
            }
            FormsAuthentication.SignOut();
            return PartialView("ProcessResult", bpr);
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

        public ActionResult Index(App_UserSearchModel model)
        {
            model.Area = "Admin";
            return View(model);
        }

        public ActionResult App_UserSearchModelView(App_UserSearchModel model)
        {
            model.Area = "Admin";
            return PartialView(model);
        }

        public PagedList<App_User> SearchFilters(App_UserSearchModel model)
        {
            model.Area = "Admin";
            var qry = App_UserDAL.Find(model);
            model.Update(model.PageSize, qry.Count());
            var ls = qry.Skip(model.PageSize * model.PageIndex).Take(model.PageSize).ToList();
            var ql = new PagedList<App_User>(ls, model);
            return ql;
        }

        public ActionResult IX(App_UserSearchModel model)
        {
            model.Area = "Admin";
            return PartialView(SearchFilters(model));
        }

        public ActionResult Create()
        {
            var model = new FormModel<UT.SL.Data.LINQ.App_User, UT.SL.Data.LINQ.App_Role>();
            model.FormObject = new App_User(); ;
            model.ExtraKnownData = new App_Role
            {
                Id = App_RoleDAL.GetAll().Single(x => x.Title.ToLower() == "student").Id,
                Title = App_RoleDAL.GetAll().Single(x => x.Title.ToLower() == "student").Title
            };
            ViewBag.Roles = App_RoleDAL.GetAll();
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(FormModel<UT.SL.Data.LINQ.App_User, UT.SL.Data.LINQ.App_Role> model)
        {
            var bpr = new BatchProcessResultModel();
            ModelState.Remove("FormObject.Id");
            ModelState.Remove("ExtraKnownData.Title");
            if (ModelState.IsValid)
            {
                try
                {
                    model.FormObject.IsAdmin = false;
                    model.FormObject.IsActive = true;
                    model.FormObject.IsDeleted = false;
                    var drm = ((DALReturnModel<App_User>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_User),
                         new Func<App_User, BatchProcessResultModel, DALReturnModel<App_User>>(App_UserDAL.Add), model.FormObject, bpr));
                    bpr = drm.BPR;
                    var user = drm.ReturnObject;
                    if (bpr.Failed > 0)
                    {
                        return PartialView("ProcessResult", bpr);
                    }
                    else
                    {
                        if (user != null)
                        {
                            bpr = new BatchProcessResultModel();
                            var drm2 = (DALReturnModel<App_UserInRole>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_UserInRole),
                                       new Func<int, int, BatchProcessResultModel, DALReturnModel<App_UserInRole>>(App_UserInRoleDAL.AddRole), user.Id, model.ExtraKnownData.Id, bpr);
                            bpr = drm2.BPR;
                        }
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

        public ActionResult Edit(System.Guid Id)
        {
            var model = new FormModel<UT.SL.Data.LINQ.App_User, UT.SL.Data.LINQ.App_Role>();
            var user = App_UserDAL.Get(Id);
            user.Password = string.Empty;
            model.FormObject = user;
            if (user.App_UserInRoles.Any())
            {
                model.ExtraKnownData = new App_Role
                {
                    Id = user.App_UserInRoles.First().App_Role.Id,
                    Title = user.App_UserInRoles.First().App_Role.Title
                };
            }
            else
            {
                model.ExtraKnownData = new App_Role
                {
                    Id = App_RoleDAL.GetAll().Single(x => x.Title.ToLower() == "student").Id,
                    Title = App_RoleDAL.GetAll().Single(x => x.Title.ToLower() == "student").Title
                };
            }
            ViewBag.Roles = App_RoleDAL.GetAll();
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(FormModel<UT.SL.Data.LINQ.App_User, UT.SL.Data.LINQ.App_Role> model)
        {
            var bpr = new BatchProcessResultModel();
            if (string.IsNullOrEmpty(model.FormObject.Password))
            {
                ModelState.Remove("FormObject.Password");
            }
            ModelState.Remove("ExtraKnownData.Title");
            if (ModelState.IsValid)
            {
                try
                {
                    var drm = ((DALReturnModel<App_User>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_User),
                         new Func<App_User, BatchProcessResultModel, DALReturnModel<App_User>>(App_UserDAL.Update), model.FormObject, bpr));
                    bpr = drm.BPR;
                    var userId = drm.ReturnObject.Id;
                    if (bpr.Failed > 0)
                    {
                        return PartialView("ProcessResult", bpr);
                    }
                    else
                    {
                        bpr = new BatchProcessResultModel();
                        var drm2 = (DALReturnModel<App_UserInRole>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_UserInRole),
                                       new Func<int, int, BatchProcessResultModel, DALReturnModel<App_UserInRole>>(App_UserInRoleDAL.AddRole), userId, model.ExtraKnownData.Id, bpr);
                        bpr = drm2.BPR;
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

        public ActionResult EditAccount(System.Guid Id)
        {
            var model = new FormModel<App_User, ChangePasswordModel>();
            model.FormObject = App_UserDAL.Get(Id);
            return PartialView(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult EditAccount(FormModel<App_User, ChangePasswordModel> model)
        {
            var bpr = new BatchProcessResultModel();
            ModelState.Remove("FormObject.Id");
            ModelState.Remove("FormObject.UserName");
            ModelState.Remove("FormObject.Password");
            if (ModelState.IsValid)
            {
                try
                {
                    //App_UserDAL.UpdatePassword(model.ExtraKnownData, out bpr);
                    //if (bpr.Success > 0)
                    //{
                    var drm = ((DALReturnModel<App_User>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_User),
                         new Func<App_User, BatchProcessResultModel, DALReturnModel<App_User>>(App_UserDAL.UpdateAccount), model.FormObject, bpr));
                    bpr = drm.BPR;
                    var guid = drm.ReturnObject.GuidId;
                    if (bpr.Failed == 0)
                    {
                        bpr.SuccessClientScript = "$('#searchForm').submit(); updateAccountView('" + guid + "');";
                    }
                    //}
                    //if (bpr.Failed > 0)
                    //{
                    //    return PartialView("ProcessResult", bpr);
                    //}
                    //else
                    //{
                    //    bpr.SuccessClientScript = "$('#searchForm').submit(); closeCreditsinfo();";
                    //}
                    //var guid = App_UserDAL.UpdateAccount(model.FormObject, ref bpr).GuidId;
                    //if (bpr.Failed > 0)
                    //{
                    //    return PartialView("ProcessResult", bpr);
                    //}
                    //else
                    //{
                    //    bpr.SuccessClientScript = "$('#searchForm').submit(); updateAccountView('" + guid + "');";
                    //}
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

        public ActionResult Delete(System.Guid Id)
        {
            var model = App_UserDAL.Get(Id);
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(App_User model)
        {
            var bpr = new BatchProcessResultModel();
            try
            {
                var drm = ((DALReturnModel<App_User>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_User),
                        new Func<App_User, DALReturnModel<App_User>>(App_UserDAL.Delete), model));
                bpr = drm.BPR;
                if (drm.ReturnObject.Id > 0)
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

        public ActionResult EditProfile(System.Guid Id)
        {
            var model = App_UserDAL.Get(Id).App_UserInfos.FirstOrDefault();
            if (model == null)
            {
                model = new App_UserInfo();
                model.App_User = new App_User { GuidId = Id };
            }
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(App_UserInfo model)
        {
            var userGuid = model.App_User.GuidId;
            var bpr = new BatchProcessResultModel();
            ModelState.Remove("Id");
            ModelState.Remove("App_User.UserName");
            ModelState.Remove("App_User.Password");
            if (ModelState.IsValid)
            {
                try
                {
                    var drm = ((DALReturnModel<App_UserInfo>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_UserInfo),
                                 new Func<App_UserInfo, BatchProcessResultModel, DALReturnModel<App_UserInfo>>(App_UserInfoDAL.Update), model, bpr));
                    bpr = drm.BPR;
                    bpr.SuccessClientScript = "location.reload();";                    
                }
                catch
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

        public ActionResult ProfileDetails(System.Guid Id)
        {
            var user = App_UserDAL.Get(Id);
            var model = user.App_UserInfos.FirstOrDefault();
            if (model == null)
            {
                model = new App_UserInfo();
                model.App_User = user;
            }
            return PartialView(model);
        }

        public ActionResult GetApp_UserUserName(System.Nullable<System.Guid> Id)
        {
            if (Id.HasValue)
            {
                try
                {
                    var model = App_UserDAL.Get(Id.Value).UserName;
                    return Content(model);
                }
                catch (System.Exception)
                {
                    Content("");
                }
            }
            return Content("");
        }

        public ActionResult GetApp_UserFirstName(System.Nullable<System.Guid> Id)
        {
            if (Id.HasValue)
            {
                try
                {
                    var model = App_UserDAL.Get(Id.Value).FirstName;
                    return Content(model);
                }
                catch (System.Exception)
                {
                    Content("");
                }
            }
            return Content("");
        }

        public ActionResult GetApp_UserLastName(System.Nullable<System.Guid> Id)
        {
            if (Id.HasValue)
            {
                try
                {
                    var model = App_UserDAL.Get(Id.Value).LastName;
                    return Content(model);
                }
                catch (System.Exception)
                {
                    Content("");
                }
            }
            return Content("");
        }

        public ActionResult GetApp_UserPassword(System.Nullable<System.Guid> Id)
        {
            if (Id.HasValue)
            {
                try
                {
                    var model = App_UserDAL.Get(Id.Value).Password;
                    return Content(model);
                }
                catch (System.Exception)
                {
                    Content("");
                }
            }
            return Content("");
        }

        public ActionResult GetApp_UserEmail(System.Nullable<System.Guid> Id)
        {
            if (Id.HasValue)
            {
                try
                {
                    var model = App_UserDAL.Get(Id.Value).Email;
                    return Content(model);
                }
                catch (System.Exception)
                {
                    Content("");
                }
            }
            return Content("");
        }

        public ActionResult SaveUserPic(HttpPostedFileBase userPic, Guid id)
        {
            try
            {
                if (userPic != null && (userPic.ContentType == "image/jpeg" || userPic.ContentType == "image/pjpeg" || userPic.ContentType == "image/png" || userPic.ContentType == "image/gif"))
                {
                    byte[] tempFile = null;
                    var resizeImage = new Bitmap(Image.FromStream(userPic.InputStream, true, true), new Size(90, 90));
                    ImageConverter converter = new ImageConverter();
                    tempFile = (byte[])converter.ConvertTo(resizeImage, typeof(byte[]));
                    var user = App_UserDAL.Get(id);
                    //tempFile = new byte[userPic.ContentLength];
                    //userPic.InputStream.Read(tempFile, 0, userPic.ContentLength);
                    user.UserPic = tempFile;
                    user.UserPicMime = userPic.ContentType;
                    ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_User),
                                      new Func<App_User, DALReturnModel<App_User>>(App_UserDAL.UpdateUserPic), user);
                }
            }
            catch
            {
                return Content("0");
            }
            return Content("1");
        }

        public ActionResult RemoveUserPic(Guid id)
        {
            try
            {
                var user = App_UserDAL.Get(id);
                user.UserPic = null;
                user.UserPicMime = null;
                ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_User),
                                     new Func<App_User, DALReturnModel<App_User>>(App_UserDAL.UpdateUserPic), user);
            }
            catch
            {
                return Content("0");
            }
            return Content("2");
        }

        [AllowAnonymous]
        public ActionResult ViewUserPic(Guid Id)
        {
            var model = App_UserDAL.Get(Id);
            return File(model.UserPic.ToArray(), model.UserPicMime);
        }

        [AllowAnonymous]
        public ActionResult GetPic(Guid Id)
        {
            var model = App_UserDAL.Get(Id);
            if (model.UserPic != null)
                return File(model.UserPic.ToArray(), model.UserPicMime);
            return File("~/Images/content/default_avatar.png", "image/png");
        }

        public ActionResult GetUserPic(int Id, byte type, bool draggable = false)
        {
            var model = App_UserDAL.Get(Id);
            ViewBag.Type = type;
            ViewBag.Drag = draggable;
            return PartialView(model);
        }

        public ActionResult GetUserPicWithoutLink(int Id, byte type)
        {
            var model = App_UserDAL.Get(Id);
            ViewBag.Type = type;
            return PartialView(model);
        }

        [AllowAnonymous]
        public ActionResult GetUserProfileLink(int Id, byte type)
        {
            var model = App_UserDAL.Get(Id);
            ViewBag.Type = type;
            if (Request.IsAuthenticated)
            {
                ViewBag.UserId = CurrentUser.Id;
            }
            return PartialView(model);
        }

        new public ActionResult Profile(Guid Id)
        {
            var model = App_UserDAL.Get(Id);
            if (CurrentUser.UserName == model.UserName)
            {
                ViewBag.ShowAccountOptions = true;
            }
            return View(model);
        }

        public ActionResult ViewGrades(int Id)
        {
            var model = App_UserDAL.Get(Id);
            return PartialView(model);
        }

        public ActionResult ViewPrgress(int Id)
        {
            var model = App_UserDAL.Get(Id);
            return PartialView(model);
        }

        public ActionResult GetUsers(string title)
        {
            var user = title.Split(new char[] { '،', ',', ' ' }).Last().StringNormalizer().Trim();
            var model = App_UserDAL.GetAll(user).Take(10).Select(x => new { id = x.GuidId, title = string.Format("{0} {1} - {2}", x.FirstName, x.LastName, x.UserName) }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUsersButCurrent(string title)
        {
            var user = title.Split(new char[] { '،', ',', ' ' }).Last().StringNormalizer().Trim();
            var model = App_UserDAL.GetAllButCurrent(user, CurrentUser.Id).Take(10).Select(x => new { id = x.GuidId, title = string.Format("{0} {1} - {2}", x.FirstName, x.LastName, x.UserName) }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUserNamesButCurrent(string title)
        {
            var user = title.Split(new char[] { '،', ',', ' ' }).Last().StringNormalizer().Trim();
            var model = App_UserDAL.GetAllButCurrent(user, CurrentUser.Id).Take(10).Select(x => new { id = x.GuidId, title = string.Format("{0} {1} - {2}", x.FirstName, x.LastName, x.UserName), username = x.UserName }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPanel(App_User user, string filter)
        {
            ViewBag.Filter = filter;
            return PartialView(user);
        }

    }
}
