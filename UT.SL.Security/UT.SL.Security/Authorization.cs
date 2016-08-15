using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UT.SL.BLL;
using UT.SL.DAL;
using UT.SL.Data.LINQ;
using UT.SL.Model;

namespace UT.SL.Security
{
    public static class Authorization
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        /// <returns></returns>
        public static bool IsAuthorized(string userName, int? objectId, int? objectType, string actionName, string controllerName, object routeValues)
        {
            if (string.IsNullOrEmpty(userName))
                userName = string.Empty;
            var user = App_UserDAL.Get(userName);
            if (user == null)
                return false;
            if (string.IsNullOrEmpty(controllerName))
                controllerName = string.Empty;
            var objectDB = new ObjectViewModel();
            if (objectId.HasValue && objectType.HasValue)
                objectDB = ManageObject.GetSharedObject(objectId.Value, objectType.Value);
            var areaName = string.Empty;
            if (routeValues != null && routeValues.GetType().GetProperties().Any(x => x.Name.ToLower() == "area"))
            {
                areaName = routeValues.GetType().GetProperties().First(x => x.Name.ToLower() == "area").GetValue(routeValues, null).ToString();
            }
            if (string.IsNullOrEmpty(actionName))
                actionName = "Index";
            var actionAuthorizationInfo = App_ActionDAL.Get(areaName, controllerName, actionName);
            if (actionAuthorizationInfo == null || !actionAuthorizationInfo.App_Permissions.Any() || (actionAuthorizationInfo != null && !actionAuthorizationInfo.RequireAuthorization))
                return true;
            if (objectDB != null && objectDB.Id > 0 && objectDB.CreateUser != null && objectDB.CreateUser.Id == user.Id && !actionAuthorizationInfo.IgnoreOwner)
            {
                return true;
            }
            if (objectDB != null && objectDB.Id > 0 && objectDB.CourseId > 0)
            {
                var enrolment = App_UserEnrolementDAL.IsAMember(objectDB.CourseId.Value, userName);
                if (enrolment != null && enrolment.Any() && actionAuthorizationInfo != null && actionAuthorizationInfo.App_Permissions.Any())
                {
                    if (actionAuthorizationInfo.App_Permissions.Any(x => x.App_Role.Title.ToLower() == "teacher"))
                    {
                        if (enrolment.Any(x => x.Type == (int)UT.SL.Model.Enumeration.MemebrshipType.Teacher))
                        {
                            return true;
                        }
                    }
                    if (actionAuthorizationInfo.App_Permissions.Any(x => x.App_Role.Title.ToLower() == "ta"))
                    {
                        if (enrolment.Any(x => x.Type == (int)UT.SL.Model.Enumeration.MemebrshipType.TA))
                        {
                            return true;
                        }
                    }
                    if (actionAuthorizationInfo.App_Permissions.Any(x => x.App_Role.Title.ToLower() == "student"))
                    {
                        if (enrolment.Any(x => x.Type == (int)UT.SL.Model.Enumeration.MemebrshipType.Student))
                        {
                            return true;
                        }
                    }

                    if (!user.IsAdmin)
                        return false;
                    return true;
                }
                else
                {
                    if (!user.IsAdmin)
                        return false;
                    return true;
                }
            }
            else
            {
                if (actionAuthorizationInfo != null && actionAuthorizationInfo.App_Permissions.Any())
                {
                    if (actionAuthorizationInfo.App_Permissions.Any(x => x.App_Role.Title.ToLower() == "admin"))
                    {
                        if (user.App_UserInRoles.Any(x => x.App_Role.Title.ToLower() == "admin"))
                        {
                            return true;
                        }
                    }
                    if (actionAuthorizationInfo.App_Permissions.Any(x => x.App_Role.Title.ToLower() == "teacher"))
                    {
                        if (user.App_UserInRoles.Any(x => x.App_Role.Title.ToLower() == "teacher"))
                        {
                            return true;
                        }
                    }
                    if (actionAuthorizationInfo.App_Permissions.Any(x => x.App_Role.Title.ToLower() == "ta"))
                    {
                        if (user.App_UserInRoles.Any(x => x.App_Role.Title.ToLower() == "ta"))
                        {
                            return true;
                        }
                    }
                    if (actionAuthorizationInfo.App_Permissions.Any(x => x.App_Role.Title.ToLower() == "student"))
                    {
                        if (user.App_UserInRoles.Any(x => x.App_Role.Title.ToLower() == "student"))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (!user.IsAdmin)
                            return false;
                        return true;
                    }
                }
                if (!user.IsAdmin)
                    return false;
                return true;
            }
        }


        public static bool IsActive(App_Action actionAuthorizationInfo)
        {
            if (actionAuthorizationInfo == null)
                return true;
            return actionAuthorizationInfo.IsActive;
        }

        public static bool IsAuthenticated(App_Action actionAuthorizationInfo)
        {
            if (actionAuthorizationInfo == null)
                return false;
            return actionAuthorizationInfo.RequireAuthentication;
        }

        public static bool IsAuthorized(App_Action actionAuthorizationInfo, string userName)
        {
            if (actionAuthorizationInfo == null)
                return true;
            if (actionAuthorizationInfo.RequireAuthorization && actionAuthorizationInfo.App_Permissions.Any())
            {
                foreach (var item in actionAuthorizationInfo.App_Permissions)
                {
                    if (App_UserInRoleDAL.IsInRole(userName, item.App_Role.Title) != null)
                        return true;
                    if (item.App_Role.Title.ToLower() == "student")
                    {
                        if (App_UserEnrolementDAL.HaveThisMembership(userName, (int)UT.SL.Model.Enumeration.MemebrshipType.Student) != null)
                            return true;
                    }
                    if (item.App_Role.Title.ToLower() == "ta")
                    {
                        if (App_UserEnrolementDAL.HaveThisMembership(userName, (int)UT.SL.Model.Enumeration.MemebrshipType.TA) != null)
                            return true;
                    }
                    if (item.App_Role.Title.ToLower() == "teacher")
                    {
                        if (App_UserEnrolementDAL.HaveThisMembership(userName, (int)UT.SL.Model.Enumeration.MemebrshipType.Teacher) != null)
                            return true;
                    }
                }
                return false;
            }
            if (actionAuthorizationInfo.RequireAuthorization && !actionAuthorizationInfo.App_Permissions.Any())
            {
                return true;
            }
            return !actionAuthorizationInfo.RequireAuthorization;
        }

        public static bool IsStudentHere(int courseId, int userId)
        {
            var roles = App_UserEnrolementDAL.GetAllByCourseAndUser(courseId, userId);
            var role = roles.First();
            if (role.Type == (int)UT.SL.Model.Enumeration.MemebrshipType.Student)
            {
                return true;
            }
            return false;
        }

        public static bool IsTAHere(int courseId, int userId)
        {
            var roles = App_UserEnrolementDAL.GetAllByCourseAndUser(courseId, userId);
            var role = roles.First();
            if (role.Type == (int)UT.SL.Model.Enumeration.MemebrshipType.TA)
            {
                return true;
            }
            return false;
        }

        public static bool IsTeacherHere(int courseId, int userId)
        {
            var roles = App_UserEnrolementDAL.GetAllByCourseAndUser(courseId, userId);
            var role = roles.First();
            if (role.Type == (int)UT.SL.Model.Enumeration.MemebrshipType.Teacher)
            {
                return true;
            }
            return false;
        }



    }
}
