/*
 * ****************************************************************
 * Filename:        ManageObject.cs 
 * version:         
 * Author's name:   Fatemeh orooji, Pezhman Nasirifard, Elearning lab 
 * Creation date:   
 * Purpose:         
 * ****************************************************************
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UT.SL.DAL;
using UT.SL.Data.LINQ;
using UT.SL.Helper;
using UT.SL.Model;
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;
using UT.SL.Model.Enumeration;


namespace UT.SL.BLL
{
    /// <summary>
    /// 
    /// </summary>
    public static class ManageAction
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RDM"></param>
        /// <param name="agentInfo"></param>
        public static void ActionCredit(RequestDetailModel RDM, AgentInfo agentInfo)
        {
            try
            {
                if (RDM.DBObject != null && RDM.DBObject.Id > 0 && RDM.DBObject.Type > 0)
                {
                    if (RDM.DBObject != null && RDM.DBObject.Id > 0)
                    {
                        var userRole = 0;
                        var credit = 0;
                        if (RDM.DBObject.CourseId > 0)
                        {
                            var enrolments = App_UserEnrolementDAL.GetAllByCourseAndUser(RDM.DBObject.CourseId.Value, RDM.CurrentUser.Id);
                            if (enrolments.Any())
                            {
                                userRole = enrolments.First().Type;
                            }
                        }
                        else
                        {
                            var role = App_UserDAL.Get(RDM.CurrentUser.Id).App_UserInRoles.First().App_Role;
                            if (role.Title.ToLower() == "teacher")
                            {
                                userRole = 1;
                            }
                            else if (role.Title.ToLower() == "ta")
                            {
                                userRole = 2;
                            }
                            else if (role.Title.ToLower() == "student")
                            {
                                userRole = 3;
                            }
                        }
                        if (userRole == 1)
                        {
                            credit = RDM.CurrentAction.TeacherCredit ?? 0;
                        }
                        else if (userRole == 2)
                        {
                            credit = RDM.CurrentAction.TACredit ?? 0;
                        }
                        else if (userRole == 3)
                        {
                            credit = RDM.CurrentAction.StudentCredit ?? 0;
                        }
                        var actionEvaluation = new App_ActionEvaulation
                        {
                            ActionId = RDM.CurrentAction.Id,
                            Credit = credit,
                            ObjectId = RDM.DBObject.Id,
                            ObjectType = RDM.DBObject.Type,
                            RoleId = userRole,
                            UserId = RDM.CurrentUser.Id,
                            IP = agentInfo.IP,
                            Browser = agentInfo.Browser,
                            OS = agentInfo.OS,
                            IsMobile = agentInfo.IsMobile,
                            ScreenRes = agentInfo.ScreenRes
                        };
                        var bpr = new BatchProcessResultModel();
                        App_ActionEvaulationDAL.Add(actionEvaluation, bpr);
                    }
                }
                else
                {
                    var actionEvaluation = new App_ActionEvaulation
                    {
                        ActionId = RDM.CurrentAction.Id,
                        UserId = RDM.CurrentUser.Id,
                        IP = agentInfo.IP,
                        Browser = agentInfo.Browser,
                        OS = agentInfo.OS,
                        IsMobile = agentInfo.IsMobile,
                        ScreenRes = agentInfo.ScreenRes
                    };
                    var bpr = new BatchProcessResultModel();
                    App_ActionEvaulationDAL.Add(actionEvaluation, bpr);
                }

            }
            catch
            {

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RDM"></param>
        public static void Notification(RequestDetailModel RDM)
        {
            if (RDM.DBObject != null && RDM.DBObject.Id > 0 && RDM.DBObject.Type > 0)
            {
                var alreadyNotifs = NotificationDAL.IsThereAny(RDM.DBObject.Id, RDM.DBObject.Type);
                if (!alreadyNotifs.Any())
                {
                    if (RDM.DBObject.Type == (int)ObjectType.Vote)
                    {
                        if (RDM.DBObject.CreateUser != null && RDM.DBObject.CameFromUser != null && RDM.DBObject.CreateUser.Id > 0 && RDM.CurrentUser.Id != RDM.DBObject.CameFromUser.Id)
                        {
                            var notification = new Notification
                            {
                                CreateDate = DateTime.Now,
                                ObjectId = RDM.DBObject.Id,
                                ObjectType = RDM.DBObject.Type,
                                Seen = false,
                                Readen = false,
                                UserId = RDM.DBObject.CameFromUser.Id
                            };
                            if (RDM.DBObject.CourseId.HasValue)
                                notification.CourseId = RDM.DBObject.CourseId;
                            NotificationDAL.Add(notification);
                        }

                        if (RDM.DBObject.CourseId > 0)
                        {
                            var teachersTAs = App_UserEnrolementDAL.GetAllCourseTeachersAndTAs(RDM.DBObject.CourseId.Value);
                            foreach (var user in teachersTAs.Where(x => x.Id != RDM.CurrentUser.Id))
                            {
                                var notification = new Notification
                                {
                                    CreateDate = DateTime.Now,
                                    ObjectId = RDM.DBObject.Id,
                                    ObjectType = RDM.DBObject.Type,
                                    Seen = false,
                                    Readen = false,
                                    UserId = user.Id
                                };
                                if (RDM.DBObject.CourseId.HasValue)
                                    notification.CourseId = RDM.DBObject.CourseId;
                                NotificationDAL.Add(notification);
                            }
                        }
                    }
                    else if (RDM.DBObject.Type == (int)ObjectType.Comment)
                    {
                        if (RDM.DBObject.CreateUser != null && RDM.DBObject.CameFromUser != null && RDM.DBObject.CreateUser.Id > 0 && RDM.CurrentUser.Id != RDM.DBObject.CameFromUser.Id)
                        {
                            var notification = new Notification
                            {
                                CreateDate = DateTime.Now,
                                ObjectId = RDM.DBObject.Id,
                                ObjectType = RDM.DBObject.Type,
                                Seen = false,
                                Readen = false,
                                UserId = RDM.DBObject.CameFromUser.Id
                            };
                            if (RDM.DBObject.CourseId.HasValue)
                                notification.CourseId = RDM.DBObject.CourseId;
                            NotificationDAL.Add(notification);
                        }
                        var associatedUser = CommentDAL.GetAllAssociatedUser(RDM.DBObject.CameFromId, RDM.DBObject.CameFromType);
                        foreach (var user in associatedUser.Where(x => x.Id != RDM.CurrentUser.Id))
                        {
                            var notification = new Notification
                            {
                                CreateDate = DateTime.Now,
                                ObjectId = RDM.DBObject.Id,
                                ObjectType = RDM.DBObject.Type,
                                Seen = false,
                                Readen = false,
                                UserId = user.Id
                            };
                            if (RDM.DBObject.CourseId.HasValue)
                                notification.CourseId = RDM.DBObject.CourseId;
                            NotificationDAL.Add(notification);
                        }
                        if (RDM.DBObject.CourseId > 0)
                        {
                            var teachersTAs = App_UserEnrolementDAL.GetAllCourseTeachersAndTAs(RDM.DBObject.CourseId.Value);
                            foreach (var user in teachersTAs.Where(x => x.Id != RDM.CurrentUser.Id))
                            {
                                var notification = new Notification
                                {
                                    CreateDate = DateTime.Now,
                                    ObjectId = RDM.DBObject.Id,
                                    ObjectType = RDM.DBObject.Type,
                                    Seen = false,
                                    Readen = false,
                                    UserId = user.Id
                                };
                                if (RDM.DBObject.CourseId.HasValue)
                                    notification.CourseId = RDM.DBObject.CourseId;
                                NotificationDAL.Add(notification);
                            }
                        }
                    }
                    else if (RDM.DBObject.Type == (int)ObjectType.TagMapper)
                    {
                        if (RDM.DBObject.CreateUser != null && RDM.DBObject.CameFromUser != null && RDM.DBObject.CreateUser.Id > 0 && RDM.CurrentUser.Id != RDM.DBObject.CameFromUser.Id)
                        {
                            var notification = new Notification
                            {
                                CreateDate = DateTime.Now,
                                ObjectId = RDM.DBObject.Id,
                                ObjectType = RDM.DBObject.Type,
                                Seen = false,
                                Readen = false,
                                UserId = RDM.DBObject.CameFromUser.Id
                            };
                            if (RDM.DBObject.CourseId.HasValue)
                                notification.CourseId = RDM.DBObject.CourseId;
                            NotificationDAL.Add(notification);
                        }
                        var associatedUser = TagMapperDAL.GetAllAssociatedUser(RDM.DBObject.CameFromId, RDM.DBObject.CameFromType);
                        foreach (var user in associatedUser.Where(x => x.Id != RDM.CurrentUser.Id))
                        {
                            var notification = new Notification
                            {
                                CreateDate = DateTime.Now,
                                ObjectId = RDM.DBObject.Id,
                                ObjectType = RDM.DBObject.Type,
                                Seen = false,
                                Readen = false,
                                UserId = user.Id
                            };
                            if (RDM.DBObject.CourseId.HasValue)
                                notification.CourseId = RDM.DBObject.CourseId;
                            NotificationDAL.Add(notification);
                        }
                        if (RDM.DBObject.CourseId > 0)
                        {
                            var teachersTAs = App_UserEnrolementDAL.GetAllCourseTeachersAndTAs(RDM.DBObject.CourseId.Value);
                            foreach (var user in teachersTAs.Where(x => x.Id != RDM.CurrentUser.Id))
                            {
                                var notification = new Notification
                                {
                                    CreateDate = DateTime.Now,
                                    ObjectId = RDM.DBObject.Id,
                                    ObjectType = RDM.DBObject.Type,
                                    Seen = false,
                                    Readen = false,
                                    UserId = user.Id
                                };
                                if (RDM.DBObject.CourseId.HasValue)
                                    notification.CourseId = RDM.DBObject.CourseId;
                                NotificationDAL.Add(notification);
                            }
                        }
                    }
                    else if (RDM.DBObject.Type == (int)ObjectType.AssignmentSubmission)
                    {
                        var assignmentSub = AssignmentSubmissionDAL.Get(RDM.DBObject.Id);
                        if (assignmentSub.AssignmentId > 0)
                        {
                            var assignment = assignmentSub.Assignment;
                            if (assignment.CreateUserId != RDM.CurrentUser.Id)
                            {
                                var notification = new Notification
                                {
                                    CreateDate = DateTime.Now,
                                    ObjectId = RDM.DBObject.Id,
                                    ObjectType = RDM.DBObject.Type,
                                    Seen = false,
                                    Readen = false,
                                    UserId = assignment.CreateUserId
                                };
                                if (RDM.DBObject.CourseId.HasValue)
                                    notification.CourseId = RDM.DBObject.CourseId;
                                NotificationDAL.Add(notification);
                            }
                        }
                        if (RDM.DBObject.CourseId > 0)
                        {
                            var teachersTAs = App_UserEnrolementDAL.GetAllCourseTeachersAndTAs(RDM.DBObject.CourseId.Value);
                            foreach (var user in teachersTAs.Where(x => x.Id != RDM.CurrentUser.Id))
                            {
                                var notification = new Notification
                                {
                                    CreateDate = DateTime.Now,
                                    ObjectId = RDM.DBObject.Id,
                                    ObjectType = RDM.DBObject.Type,
                                    Seen = false,
                                    Readen = false,
                                    UserId = user.Id
                                };
                                if (RDM.DBObject.CourseId.HasValue)
                                    notification.CourseId = RDM.DBObject.CourseId;
                                NotificationDAL.Add(notification);
                            }
                        }
                    }
                    else if (RDM.DBObject.Type == (int)ObjectType.Grade)
                    {
                        if (RDM.DBObject.CreateUser != null && RDM.DBObject.CreateUser.Id > 0 && RDM.CurrentUser.Id != RDM.DBObject.CreateUser.Id)
                        {
                            var notification = new Notification
                            {
                                CreateDate = DateTime.Now,
                                ObjectId = RDM.DBObject.Id,
                                ObjectType = RDM.DBObject.Type,
                                Seen = false,
                                Readen = false,
                                UserId = RDM.DBObject.CreateUser.Id
                            };
                            if (RDM.DBObject.CourseId.HasValue)
                                notification.CourseId = RDM.DBObject.CourseId;
                            NotificationDAL.Add(notification);
                        }
                        if (RDM.DBObject.CourseId > 0)
                        {
                            var teachersTAs = App_UserEnrolementDAL.GetAllCourseTeachersAndTAs(RDM.DBObject.CourseId.Value);
                            foreach (var user in teachersTAs.Where(x => x.Id != RDM.CurrentUser.Id))
                            {
                                var notification = new Notification
                                {
                                    CreateDate = DateTime.Now,
                                    ObjectId = RDM.DBObject.Id,
                                    ObjectType = RDM.DBObject.Type,
                                    Seen = false,
                                    Readen = false,
                                    UserId = user.Id
                                };
                                if (RDM.DBObject.CourseId.HasValue)
                                    notification.CourseId = RDM.DBObject.CourseId;
                                NotificationDAL.Add(notification);
                            }
                        }
                    }
                    else if (RDM.DBObject.Type == (int)ObjectType.ForumDiscussion)
                    {
                        var forumDisc = ForumDiscussionDAL.Get(RDM.DBObject.Id);
                        if (!forumDisc.ParentId.HasValue)
                        {
                            var forum = forumDisc.Forum;
                            if (forum.CreateUserId != RDM.CurrentUser.Id)
                            {
                                var notification = new Notification
                                {
                                    CreateDate = DateTime.Now,
                                    ObjectId = RDM.DBObject.Id,
                                    ObjectType = RDM.DBObject.Type,
                                    Seen = false,
                                    Readen = false,
                                    UserId = forum.CreateUserId
                                };
                                if (RDM.DBObject.CourseId.HasValue)
                                    notification.CourseId = RDM.DBObject.CourseId;
                                NotificationDAL.Add(notification);
                            }
                        }
                        var associatedUser = ForumDiscussionDAL.GetAllAssociatedUser(RDM.DBObject.CameFromId, forumDisc.ParentId ?? forumDisc.Id);
                        foreach (var user in associatedUser.Where(x => x.Id != RDM.CurrentUser.Id))
                        {
                            var notification = new Notification
                            {
                                CreateDate = DateTime.Now,
                                ObjectId = RDM.DBObject.Id,
                                ObjectType = RDM.DBObject.Type,
                                Seen = false,
                                Readen = false,
                                UserId = user.Id
                            };
                            if (RDM.DBObject.CourseId.HasValue)
                                notification.CourseId = RDM.DBObject.CourseId;
                            NotificationDAL.Add(notification);
                        }
                        if (RDM.DBObject.CourseId > 0)
                        {
                            var teachersTAs = App_UserEnrolementDAL.GetAllCourseTeachersAndTAs(RDM.DBObject.CourseId.Value);
                            foreach (var user in teachersTAs.Where(x => x.Id != RDM.CurrentUser.Id))
                            {
                                var notification = new Notification
                                {
                                    CreateDate = DateTime.Now,
                                    ObjectId = RDM.DBObject.Id,
                                    ObjectType = RDM.DBObject.Type,
                                    Seen = false,
                                    Readen = false,
                                    UserId = user.Id
                                };
                                if (RDM.DBObject.CourseId.HasValue)
                                    notification.CourseId = RDM.DBObject.CourseId;
                                NotificationDAL.Add(notification);
                            }
                        }
                    }
                    else if (RDM.DBObject.Type == (int)ObjectType.ObjectStream)
                    {
                        if (RDM.DBObject.CreateUser != null && RDM.DBObject.CreateUser.Id > 0 && RDM.CurrentUser.Id != RDM.DBObject.CreateUser.Id)
                        {
                            var notification = new Notification
                            {
                                CreateDate = DateTime.Now,
                                ObjectId = RDM.DBObject.Id,
                                ObjectType = RDM.DBObject.Type,
                                Seen = false,
                                Readen = false,
                                UserId = RDM.DBObject.CreateUser.Id
                            };
                            if (RDM.DBObject.CourseId.HasValue)
                                notification.CourseId = RDM.DBObject.CourseId;
                            NotificationDAL.Add(notification);
                        }
                    }
                    else if (RDM.DBObject.Type == (int)ObjectType.Share)
                    {
                        //I guess nothing goes here
                    }
                    else if (RDM.DBObject.Type == (int)ObjectType.Resource)
                    {
                        //I guess nothing goes here
                    }
                    else if (RDM.DBObject.Type == (int)ObjectType.Assignment)
                    {
                        //I guess nothing goes here
                    }
                    else if (RDM.DBObject.Type == (int)ObjectType.Forum)
                    {
                        //I guess nothing goes here
                    }
                    else if (RDM.DBObject.Type == (int)ObjectType.VoteParent)
                    {
                        //I guess nothing goes here
                    }
                }
                else
                {
                    foreach (var item in alreadyNotifs)
                    {
                        NotificationDAL.UpdateAsNew(item.Id);
                    }
                }

            }
            else if (RDM.ObjectId > 0 && RDM.ObjectType > 0)
            {
                NotificationDAL.DeleteAll(RDM.ObjectId, RDM.ObjectType);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RDM"></param>
        public static void Email(RequestDetailModel RDM)
        {
            if (RDM.DBObject != null && RDM.DBObject.Id > 0 && RDM.DBObject.Type > 0)
            {
                if (RDM.DBObject.CreateUser.App_UserProfiles.Any(x => x.GetNotificationEmails) || RDM.DBObject.CreateUser.App_UserProfiles.Count() == 0)
                {
                    if (RDM.DBObject.Type == (int)ObjectType.Grade)
                    {
                        if (RDM.DBObject.CreateUser != null && RDM.DBObject.CreateUser.Id > 0 && RDM.CurrentUser.Id != RDM.DBObject.CreateUser.Id)
                        {
                            var notificationEmail = new NotificationEmail
                            {
                                CreateDate = DateTime.Now,
                                ObjectId = RDM.DBObject.Id,
                                ObjectType = RDM.DBObject.Type,
                                Sent = false,
                                UserId = RDM.DBObject.CreateUser.Id
                            };
                            if (RDM.DBObject.CourseId.HasValue)
                                notificationEmail.CourseId = RDM.DBObject.CourseId;
                            NotificationEmailDAL.Add(notificationEmail);
                        }
                        if (RDM.DBObject.CreateUser != null && RDM.DBObject.CreateUser.Id > 0 && RDM.CurrentUser.Id != RDM.DBObject.CreateUser.Id)
                        {
                            var notificationEmail = new NotificationEmail
                            {
                                CreateDate = DateTime.Now,
                                ObjectId = RDM.DBObject.Id,
                                ObjectType = RDM.DBObject.Type,
                                Sent = false,
                                UserId = RDM.DBObject.CreateUser.Id
                            };
                            if (RDM.DBObject.CourseId.HasValue)
                                notificationEmail.CourseId = RDM.DBObject.CourseId;
                            NotificationEmailDAL.Add(notificationEmail);
                        }
                        if (RDM.DBObject.CourseId > 0)
                        {
                            var teachersTAs = App_UserEnrolementDAL.GetAllCourseTeachersAndTAs(RDM.DBObject.CourseId.Value);
                            foreach (var user in teachersTAs.Where(x => x.Id != RDM.CurrentUser.Id))
                            {
                                var notificationEmail = new NotificationEmail
                                {
                                    CreateDate = DateTime.Now,
                                    ObjectId = RDM.DBObject.Id,
                                    ObjectType = RDM.DBObject.Type,
                                    Sent = false,
                                    UserId = RDM.DBObject.CreateUser.Id
                                };
                                if (RDM.DBObject.CourseId.HasValue)
                                    notificationEmail.CourseId = RDM.DBObject.CourseId;
                                NotificationEmailDAL.Add(notificationEmail);
                            }
                        }
                    }
                    else if (RDM.DBObject.Type == (int)ObjectType.ObjectStream)
                    {
                        if (RDM.DBObject.CreateUser != null && RDM.DBObject.CreateUser.Id > 0 && RDM.CurrentUser.Id != RDM.DBObject.CreateUser.Id)
                        {
                            var notificationEmail = new NotificationEmail
                            {
                                CreateDate = DateTime.Now,
                                ObjectId = RDM.DBObject.Id,
                                ObjectType = RDM.DBObject.Type,
                                Sent = false,
                                UserId = RDM.DBObject.CreateUser.Id
                            };
                            if (RDM.DBObject.CourseId.HasValue)
                                notificationEmail.CourseId = RDM.DBObject.CourseId;
                            NotificationEmailDAL.Add(notificationEmail);
                        }
                    }
                    else if (RDM.DBObject.Type == (int)ObjectType.ForumDiscussion)
                    {
                        var forumDisc = ForumDiscussionDAL.Get(RDM.DBObject.Id);
                        if (!forumDisc.ParentId.HasValue)
                        {
                            var forum = forumDisc.Forum;
                            if (forum.CreateUserId != RDM.CurrentUser.Id)
                            {
                                var notificationEmail = new NotificationEmail
                                {
                                    CreateDate = DateTime.Now,
                                    ObjectId = RDM.DBObject.Id,
                                    ObjectType = RDM.DBObject.Type,
                                    Sent = false,
                                    UserId = forum.CreateUserId
                                };
                                if (RDM.DBObject.CourseId.HasValue)
                                    notificationEmail.CourseId = RDM.DBObject.CourseId;
                                NotificationEmailDAL.Add(notificationEmail);
                            }
                        }
                        var associatedUser = ForumDiscussionDAL.GetAllAssociatedUser(RDM.DBObject.CameFromId, forumDisc.ParentId ?? forumDisc.Id);
                        foreach (var user in associatedUser.Where(x => x.Id != RDM.CurrentUser.Id))
                        {
                            var notificationEmail = new NotificationEmail
                            {
                                CreateDate = DateTime.Now,
                                ObjectId = RDM.DBObject.Id,
                                ObjectType = RDM.DBObject.Type,
                                Sent = false,
                                UserId = user.Id
                            };
                            if (RDM.DBObject.CourseId.HasValue)
                                notificationEmail.CourseId = RDM.DBObject.CourseId;
                            NotificationEmailDAL.Add(notificationEmail);
                        }
                        if (RDM.DBObject.CourseId > 0)
                        {
                            var teachersTAs = App_UserEnrolementDAL.GetAllCourseTeachersAndTAs(RDM.DBObject.CourseId.Value);
                            foreach (var user in teachersTAs.Where(x => x.Id != RDM.CurrentUser.Id))
                            {
                                var notificationEmail = new NotificationEmail
                                {
                                    CreateDate = DateTime.Now,
                                    ObjectId = RDM.DBObject.Id,
                                    ObjectType = RDM.DBObject.Type,
                                    Sent = false,
                                    UserId = user.Id
                                };
                                if (RDM.DBObject.CourseId.HasValue)
                                    notificationEmail.CourseId = RDM.DBObject.CourseId;
                                NotificationEmailDAL.Add(notificationEmail);
                            }
                        }
                    }
                }
            }
            else if (RDM.ObjectId > 0 && RDM.ObjectType > 0)
            {
                NotificationEmailDAL.DeleteAll(RDM.ObjectId, RDM.ObjectType);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RDM"></param>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object ProxyCall(RequestDetailModel RDM, Delegate method, params object[] args)
        {
            var returnObject = method.DynamicInvoke(args);
            try
            {
                if (returnObject != null && RDM.ObjectType > 0)
                {
                    if (returnObject.GetType().IsGenericType)
                    {
                        RDM.DBObject = ManageObject.CastObjectAndGetIt(returnObject, RDM.ObjectType, true);
                        if (returnObject.GetType().GetProperty("ReturnObject") != null)
                        {
                            var retunVal = returnObject.GetType().GetProperty("ReturnObject").GetValue(returnObject, null);
                            if (retunVal.GetType().GetProperty("Id") != null)
                            {
                                var tryer = 0;
                                Int32.TryParse(retunVal.GetType().GetProperty("Id").GetValue(retunVal, null).ToString(), out tryer);
                                RDM.ObjectId = tryer;
                            }
                        }
                    }
                    else
                    {
                        RDM.DBObject = ManageObject.CastObjectAndGetIt(returnObject, RDM.ObjectType);
                        if (returnObject.GetType().GetProperty("ReturnObject") != null)
                        {
                            var retunVal = returnObject.GetType().GetProperty("ReturnObject").GetValue(returnObject, null);
                            if (retunVal.GetType().GetProperty("Id") != null)
                            {
                                var tryer = 0;
                                Int32.TryParse(retunVal.GetType().GetProperty("Id").GetValue(retunVal, null).ToString(), out tryer);
                                RDM.ObjectId = tryer;
                            }
                        }

                    }
                }
                if (RDM.CurrentAction != null)
                {
                    if (RDM.CurrentUser != null && RDM.CurrentAction.IsCredit)
                    {
                        if (RDM.CurrentAction.CreditOnlyPost)
                        {
                            if (RDM.CurrentRequestContext.HttpContext.Request.RequestType.ToLower() == "post")
                            {
                                var agentInfo = new AgentInfo
                                {
                                    Browser = string.Format("{0} {1}", RDM.CurrentRequestContext.HttpContext.Request.Browser.Browser, RDM.CurrentRequestContext.HttpContext.Request.Browser.MajorVersion),
                                    OS = string.Format("{0}", RDM.CurrentRequestContext.HttpContext.Request.UserAgent),
                                    IP = RDM.CurrentRequestContext.HttpContext.Request.UserHostAddress,
                                    IsMobile = RDM.CurrentRequestContext.HttpContext.Request.Browser.IsMobileDevice,
                                    ScreenRes = string.Format("{0} {1}", RDM.CurrentRequestContext.HttpContext.Request.Browser.ScreenPixelsHeight, RDM.CurrentRequestContext.HttpContext.Request.Browser.ScreenPixelsWidth),
                                };
                                ManageAction.ActionCredit(RDM, agentInfo);
                            }
                        }
                        else
                        {
                            var agentInfo = new AgentInfo
                            {
                                Browser = string.Format("{0} {1}", RDM.CurrentRequestContext.HttpContext.Request.Browser.Browser, RDM.CurrentRequestContext.HttpContext.Request.Browser.MajorVersion),
                                OS = string.Format("{0}", RDM.CurrentRequestContext.HttpContext.Request.UserAgent),
                                IP = RDM.CurrentRequestContext.HttpContext.Request.UserHostAddress,
                                IsMobile = RDM.CurrentRequestContext.HttpContext.Request.Browser.IsMobileDevice,
                                ScreenRes = string.Format("{0} {1}", RDM.CurrentRequestContext.HttpContext.Request.Browser.ScreenPixelsHeight, RDM.CurrentRequestContext.HttpContext.Request.Browser.ScreenPixelsWidth),
                            };
                            ManageAction.ActionCredit(RDM, agentInfo);
                        }

                    }
                    if (RDM.CurrentUser != null && RDM.CurrentAction.InNotification)
                    {
                        ManageAction.Notification(RDM);
                    }
                    if (RDM.CurrentUser != null && RDM.CurrentAction.InEmail)
                    {
                        ManageAction.Email(RDM);
                    }
                }
            }
            catch
            {

            }
            return returnObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CurrentUser"></param>
        /// <param name="filterContext"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static RequestDetailModel RequestDetail(App_User CurrentUser, RequestContext filterContext, int objectType)
        {
            var model = new RequestDetailModel();
            var area = filterContext.RouteData.DataTokens["area"];
            var controllerName = filterContext.RouteData.Values["controller"].ToString();
            string actionName = filterContext.RouteData.Values["action"].ToString();
            string areaName = area != null ? area.ToString() : "";
            model.CurrentAction = App_ActionDAL.Get(areaName, controllerName, actionName);
            model.CurrentUser = CurrentUser ?? new App_User();
            model.CurrentRequestContext = filterContext;
            model.ObjectType = objectType;
            return model;
        }

    }
}
