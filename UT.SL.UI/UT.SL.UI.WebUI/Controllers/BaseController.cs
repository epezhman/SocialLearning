/*
 * ****************************************************************
 * Filename:        BaseController.cs 
 * version:         
 * Author's name:   Fatemeh Orooji, Pezhman Nasirifard, Elearning lab 
 * Creation date:   
 * Purpose:         providing base needs of controllers
 * ****************************************************************
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using UT.SL.BLL;
using UT.SL.DAL;
using UT.SL.Data.LINQ;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Model.Enumeration;


namespace UT.SL.UI.WebUI.Controllers
{
    /// <summary>
    /// a controller that every other controller inherited from it
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// OnActionExecuting is a method in Controller class that executes before execution of each action
        /// it overrode here so it will be executed before each action that be inherited from this class (BaseController)
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var area = filterContext.RouteData.DataTokens["area"];
            var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string actionName = filterContext.ActionDescriptor.ActionName;
            string areaName = area != null ? area.ToString() : "";
            //if (actionName == "LogIn" || actionName == "LogOff")
            //{
            //    System.Console.Write("LogIn");
            //}

            ///inserting action to database in App-actions if it not exist
            if (!App_ActionDAL.CheckAvailability(areaName, controllerName, actionName))
            {
                var action = new App_Action
                {
                    ActionName = actionName,
                    ControllerName = controllerName,
                    AreaName = areaName,
                    CreateDate = DateTime.Now,
                    Title = areaName + "/" + controllerName + "/" + actionName,
                    IsActive = true,
                    RequireAuthentication = true,
                    RequireAuthorization = false
                };
                var bpr = new BatchProcessResultModel();
                ManageAction.ProxyCall(ManageAction.RequestDetail(null, Request.RequestContext, (int)ObjectType.App_Action),
                    new Func<App_Action, BatchProcessResultModel, DALReturnModel<App_Action>>(App_ActionDAL.Add), action, bpr);
            }

            ///cheking cookie to find language
            string cultureName = null;
            HttpCookie cultureCookie = Request.Cookies["_culture"];
            if (cultureCookie != null)
                cultureName = cultureCookie.Value;
            else
                cultureName = "en-US";
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureName);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(cultureName);

            ///check access permission
            var actionToCeheck = App_ActionDAL.Get(areaName, controllerName, actionName);
            if (!UT.SL.Security.Authorization.IsActive(actionToCeheck))
            {
                if (actionToCeheck.ReturnBlankIfNotAllowed)
                    filterContext.Result = Content("");
                else
                    filterContext.Result = new RedirectResult(Url.Action("NotActive", "Home", new { area = "" }));
            }
            else if (UT.SL.Security.Authorization.IsAuthenticated(actionToCeheck) && !Request.IsAuthenticated)
            {
                if (actionToCeheck.ReturnBlankIfNotAllowed)
                    filterContext.Result = Content("");
                else
                    filterContext.Result = new RedirectResult(Url.Action("LogIn", "App_User", new { area = "Admin" }));
            }
            else if (!UT.SL.Security.Authorization.IsAuthorized(actionToCeheck, User.Identity.Name))
            {
                if (actionToCeheck.ReturnBlankIfNotAllowed)
                    filterContext.Result = Content("");
                else
                    filterContext.Result = new RedirectResult(Url.Action("AccessDenied", "Home", new { area = "" }));
            }
            else
            {
                if (actionToCeheck.IsCredit && Request.IsAuthenticated && actionToCeheck.DoesNotHaveDAL)
                {
                    if (actionToCeheck.CreditOnlyPost)
                    {
                        if (Request.RequestType.ToLower() == "post")
                        {
                            var agentInfo = new AgentInfo
                            {
                                Browser = string.Format("{0} {1}", Request.Browser.Browser, Request.Browser.MajorVersion),
                                OS = string.Format("{0}", Request.UserAgent),
                                IP = Request.UserHostAddress,
                                IsMobile = Request.Browser.IsMobileDevice,
                                ScreenRes = string.Format("{0} {1}", Request.Browser.ScreenPixelsHeight, Request.Browser.ScreenPixelsWidth),
                            };
                            ManageAction.ActionCredit(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, 0), agentInfo);
                        }
                    }
                    else
                    {
                        var agentInfo = new AgentInfo
                        {
                            Browser = string.Format("{0} {1}", Request.Browser.Browser, Request.Browser.MajorVersion),
                            OS = string.Format("{0}", Request.UserAgent),
                            IP = Request.UserHostAddress,
                            IsMobile = Request.Browser.IsMobileDevice,
                            ScreenRes = string.Format("{0} {1}", Request.Browser.ScreenPixelsHeight, Request.Browser.ScreenPixelsWidth),
                        };
                        ManageAction.ActionCredit(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, 0), agentInfo);
                    }

                }

            }

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// get current user 
        /// it is usefull in other controllers when we want to know who is using the website now
        /// </summary>
        /// <returns>the user that is using website now</returns>
        protected App_User GetcurrentUser()
        {
            var curUser = App_UserDAL.Get(HttpContext.User.Identity.Name);
            return curUser;
        }

        /// <summary>
        /// it is working such as GetCurrentUser()
        /// </summary>
        protected App_User CurrentUser
        {
            get
            {
                return this.GetcurrentUser();
            }
        }
    }
}
