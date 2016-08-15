using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using System;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UT.SL.Security;

namespace UT.SL.UI
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class UTAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly object _typeId = new object();

        private string _roles;
        private string[] _rolesSplit = new string[0];
        private string _users;
        private string[] _usersSplit = new string[0];

        new public string Roles
        {
            get { return _roles ?? String.Empty; }
            set
            {
                _roles = value;
                _rolesSplit = SplitString(value);
            }
        }

        public override object TypeId
        {
            get { return _typeId; }
        }

        new public string Users
        {
            get { return _users ?? String.Empty; }
            set
            {
                _users = value;
                _usersSplit = SplitString(value);
            }
        }

        public int ProjectId { get; set; }

        public UTAuthorizeAttribute()
        {

        }

        public UTAuthorizeAttribute(string users = "", string roles = "")
        {
            Users = users;
            Roles = roles;
        }

        public UTAuthorizeAttribute(string users = "", string roles = "", int ProjectId = 0)
        {
            Users = users;
            Roles = roles;
            if (ProjectId != 0)
                this.ProjectId = ProjectId;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            IList<bool> flag = new List<bool>();

            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            IPrincipal user = httpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                return false;
            }

            if (_usersSplit.Length > 0 && !_usersSplit.Contains(user.Identity.Name, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            var checkPermissions = UTRoleProvider.CheckPermissions(httpContext);
            if (checkPermissions > 0)
            {
                if (checkPermissions == 1)
                    flag.Add(true);
                else
                    flag.Add(false);
            }

            if (_rolesSplit.Any())
            {
                foreach (var role in _rolesSplit)
                {
                    if (role[0] != '!')
                    {
                        switch (role.ToLower())
                        {
                            case "admin":
                                flag.Add(UTRoleProvider.IsUserAdmin(httpContext.User.Identity.Name));
                                break;
                            default:
                                return false;
                        }
                    }
                    else
                    {
                        switch (role.Substring(1).ToLower())
                        {
                            case "enduser":
                                break;
                            default:
                                return false;
                        }
                    }
                }
            }
            if (flag.Count > 0)
            {
                foreach (var f in flag)
                {
                    if (f)
                        return true;
                }
                return false;
            }
            return true;
        }

        private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {

            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (AuthorizeCore(filterContext.HttpContext))
            {
                HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
                cachePolicy.SetProxyMaxAge(new TimeSpan(0));
                cachePolicy.AddValidationCallback(CacheValidateHandler, null /* data */);
            }
            else
            {
                filterContext.Result = filterContext.Result = new RedirectResult("~/Account/Unauthorized");

            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }

        protected override HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }
            bool isAuthorized = AuthorizeCore(httpContext);
            return (isAuthorized) ? HttpValidationStatus.Valid : HttpValidationStatus.IgnoreThisRequest;
        }

        internal static string[] SplitString(string original)
        {
            if (String.IsNullOrEmpty(original))
            {
                return new string[0];
            }
            char[] delimiterChars = { ' ', ',', '.', '\r', '\t', '\n', '"' };
            var split = from piece in original.Split(delimiterChars)
                        let trimmed = piece.Trim()
                        where !String.IsNullOrEmpty(trimmed)
                        select trimmed;
            return split.ToArray();
        }
    }
}