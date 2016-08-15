using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Data.Linq;
using UT.SL.Helper;
using UT.SL.Data.LINQ;
using UT.SL.DAL;
using System.Web;

namespace UT.SL.Security
{

    public class UTMembershipProvider : MembershipProvider
    {
        string cnnString = "";
        string appName = "";

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            cnnString = string.Empty;
            appName = config["applicationName"];
            base.Initialize(name, config);
        }

        private MembershipUser GetMembershipUserFromUser(App_User usr)
        {
            MembershipUser u = null;
            u = new MembershipUser(this.Name, usr.UserName, usr.Id
                , usr.Email, string.Empty, string.Empty, true, true, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
            return u;
        }

        public override string ApplicationName
        {
            get
            {
                return appName;
            }
            set
            {
                appName = value;
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var objUser = App_UserDAL.Get(username);
            if (objUser != null)
            {
                if (!string.IsNullOrEmpty(newPassword) && PasswordUtils.EncodeString(oldPassword.StringNormalizer()) == objUser.Password)
                {
                    objUser.Password = PasswordUtils.EncodeString(newPassword.StringNormalizer());
                    var bpr = new BatchProcessResultModel();
                    bpr = App_UserDAL.Update(objUser, bpr).BPR;
                    if (bpr.Failed == 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer,
            bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            try
            {
                if (App_UserDAL.CheckEmail(email))
                {
                    status = MembershipCreateStatus.DuplicateEmail;
                    return null;
                }
                if (App_UserDAL.CheckUsername(username))
                {
                    status = MembershipCreateStatus.DuplicateUserName;
                    return null;
                }
                var user = new App_User
                {
                    UserName = username.StringNormalizer(),
                    Password = password.StringNormalizer(),
                    Email = email.StringNormalizer(),
                    FirstName = passwordQuestion.StringNormalizer(),
                    LastName = passwordAnswer.StringNormalizer(),
                    IsAdmin = false,
                    IsActive = false,
                    IsDeleted = false
                };
                var bpr = new BatchProcessResultModel();
                var role = App_RoleDAL.GetRole("student");
                var newUser = new App_User();
                if (role != null)
                {
                    var returnObject = App_UserDAL.Add(user, bpr);
                    newUser = returnObject.ReturnObject;
                    bpr = returnObject.BPR;
                }

                if (bpr.Failed == 0 && role != null)
                {
                    status = MembershipCreateStatus.Success;
                    var userInRole = new App_UserInRole
                    {
                        UserId = newUser.Id,
                        RoleId = role.Id
                    };
                    bpr = App_UserInRoleDAL.Add(userInRole, bpr).BPR;
                    return GetMembershipUserFromUser(user);
                }
                else
                {
                    status = MembershipCreateStatus.ProviderError;
                    return null;
                }
            }
            catch
            {
                status = MembershipCreateStatus.ProviderError;
                return null;
            }
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            var user = App_UserDAL.Get(username);
            if (user != null)
                return GetMembershipUserFromUser(user);
            else return null;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 3; }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        public bool ResetUserPassword(string UserName, string Email, string FullNAme, string NewPassword)
        {
            throw new NotImplementedException();
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            var existUser = App_UserDAL.ValidateUser(username, password);
            if (existUser != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }

    public class UTRoleProvider : RoleProvider
    {
        string cnnString = "";
        string appName = "";

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            cnnString = string.Empty;
            appName = config["applicationName"];
            base.Initialize(name, config);
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                return appName;
            }
            set
            {
                appName = value;
            }
        }

        public override void CreateRole(string roleName)
        {
            if (!App_RoleDAL.Get(roleName))
            {
                var ojr = new App_Role() { Title = roleName.StringNormalizer().ToLower(), Description = roleName.StringNormalizer().ToLower() };
                var bpr = new BatchProcessResultModel();
                bpr = App_RoleDAL.Add(ojr, bpr).BPR;
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            return App_RoleDAL.IsUserInRole(username, roleName);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public static bool IsUserAdmin(string name)
        {
            throw new NotImplementedException();
        }

        public static short CheckPermissions(HttpContextBase httpContext)
        {
            //    var dc = new APS.SIS.DAL.NHDbContext();
            //    var rd = httpContext.Request.RequestContext.RouteData;
            //    string currentAction = rd.GetRequiredString("action");
            //    string currentController = rd.GetRequiredString("controller");
            //    string currentArea = rd.Values["area"] as string;
            //    var permissions = dc.Db<App_Permission>().Where(x => x.App_Action.ActionName == currentAction && x.App_Action.ControllerName == currentController && x.App_Action.AreaName == currentArea);
            //    if (permissions.Any())
            //    {
            //        var personId = dc.Db<App_User>().Single(x => x.UserName == httpContext.User.Identity.Name).PersonId;
            //        foreach (var item in permissions)
            //        {
            //            if (item.App_Role != null)
            //            {
            //                if (dc.Db<App_UserRole>().Any(x => x.UserId_App_User.UserName == httpContext.User.Identity.Name && x.App_Role.Id == item.App_Role.Id))
            //                    return 1;
            //            }
            //            if (item.OrganizationPost != null)
            //            {
            //                if (dc.Db<Signification>().Any(x => x.OrganizationPost.Id == item.OrganizationPost.Id && x.Employee.Person.Id == personId && x.EduYear.Title == DateTime.Now.GetStringThisEduYear()))
            //                    return 1;
            //            }
            //        }
            //        return 0;
            //    }
            //    return -1;
            return 0;
        }


    }

}