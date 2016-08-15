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
using UT.SL.Model.Enumeration;

namespace UT.SL.BLL
{
    /// <summary>
    /// 
    /// </summary>
    public class EmailManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mailServerType"></param>
        /// <param name="receiverEmail"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="RDM"></param>
        public static void SendMail(int mailServerType, string receiverEmail, string subject, string body, RequestDetailModel RDM)
        {
            var sent = false;
            if (mailServerType == (int)MailServerType.DoosMooc)
            {
                sent = SendByDoosMoocAccount(receiverEmail, subject, body);
            }
            else if (mailServerType == (int)MailServerType.Gmail)
            {
                sent = SendByGmailAccount(receiverEmail, subject, body);
            }
            if (sent)
            {
                var email = new Email
                {
                    Body = body,
                    CreateDate = DateTime.Now,
                    ReceiverEmail = receiverEmail,
                    Subject = subject
                };
                var user = App_UserDAL.GetEmail(receiverEmail);
                if (user != null)
                    email.UserId = user.Id;
                var bpr = new BatchProcessResultModel();
                ManageAction.ProxyCall(RDM, new Func<Email, BatchProcessResultModel, DALReturnModel<Email>>(EmailDAL.Add), email, bpr);
                var emailRecord = new EmailRecord
                {
                    CreateDate = DateTime.Now,
                    MailServer = mailServerType,
                    EmailType = (int)SentEmailType.Confirmation
                };
                if (user != null)
                    emailRecord.UserId = user.Id;
                EmailRecordDAL.Add(emailRecord);

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mailServerType"></param>
        /// <param name="receiverEmail"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="emailId"></param>
        /// <param name="mailType"></param>
        public static void SendMail(int mailServerType, string receiverEmail, string subject, string body, int emailId, int mailType)
        {
            var sent = false;
            if (mailServerType == (int)MailServerType.DoosMooc)
            {
                sent = SendByDoosMoocAccount(receiverEmail, subject, body);
            }
            else if (mailServerType == (int)MailServerType.Gmail)
            {
                sent = SendByGmailAccount(receiverEmail, subject, body);
            }
            if (sent)
            {
                var rec = EmailDAL.UpdateSent(emailId);
                var emailRecord = new EmailRecord
                {
                    CreateDate = DateTime.Now,
                    MailServer = mailServerType,
                    EmailType = mailType
                };
                if (rec.ReturnObject.UserId != null)
                    emailRecord.UserId = rec.ReturnObject.UserId;
                EmailRecordDAL.Add(emailRecord);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mailServerType"></param>
        /// <param name="receiverEmail"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="emailIds"></param>
        /// <param name="userId"></param>
        public static void SendNotificationMail(int mailServerType, string receiverEmail, string subject, string body, List<int> emailIds, int userId)
        {
            var sent = false;
            if (mailServerType == (int)MailServerType.DoosMooc)
            {
                sent = SendByDoosMoocAccount(receiverEmail, subject, body);
            }
            else if (mailServerType == (int)MailServerType.Gmail)
            {
                sent = SendByGmailAccount(receiverEmail, subject, body);
            }
            if (sent)
            {
                foreach (var item in emailIds)
                {
                    NotificationEmailDAL.UpdateSeenDate(item);
                }

                var emailRecord = new EmailRecord
                {
                    CreateDate = DateTime.Now,
                    MailServer = mailServerType,
                    EmailType = (int)SentEmailType.Notification,
                    UserId = userId
                };
                EmailRecordDAL.Add(emailRecord);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mailServerType"></param>
        /// <param name="receiverEmail"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="emailIds"></param>
        /// <param name="userId"></param>
        public static void SendUnreadenMessageEmails(int mailServerType, string receiverEmail, string subject, string body, List<int> emailIds, int userId)
        {
            var sent = false;
            if (mailServerType == (int)MailServerType.DoosMooc)
            {
                sent = SendByDoosMoocAccount(receiverEmail, subject, body);
            }
            else if (mailServerType == (int)MailServerType.Gmail)
            {
                sent = SendByGmailAccount(receiverEmail, subject, body);
            }
            if (sent)
            {
                foreach (var item in emailIds)
                {
                    var mail = App_UserProfileDAL.UpdateSentEmailDate(item);

                    var emailRecord = new EmailRecord
                    {
                        CreateDate = DateTime.Now,
                        MailServer = mailServerType,
                        EmailType = (int)SentEmailType.UnreadMessages,
                        UserId = userId
                    };
                    EmailRecordDAL.Add(emailRecord);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiverEmail"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public static bool SendByGmailAccount(string receiverEmail, string subject, string body)
        {
            return MailUtils.SendMailbyGmail(receiverEmail, subject, body);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiverEmail"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public static bool SendByDoosMoocAccount(string receiverEmail, string subject, string body)
        {
            return MailUtils.SendMailByDoosMooc(receiverEmail, subject, body);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool CehckDoosMoocLimit()
        {
            var dayLimit = EmailRecordDAL.CheckDoosMoocDailyLimit((int)MailServerType.DoosMooc);
            var hourLimit = EmailRecordDAL.CheckDoosMoocHourlyLimit((int)MailServerType.DoosMooc);
            if (hourLimit < 97 && dayLimit < 497)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool CheckUserLimitForNotification(int userId)
        {
            var dayLimit = EmailRecordDAL.CheckUserDailyLimit((int)SentEmailType.Notification, userId);
            if (dayLimit < 2)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool CheckUserLimitForUnreadMessages(int userId)
        {
            var dayLimit = EmailRecordDAL.CheckUserDailyLimit((int)SentEmailType.UncategorizedEmail, userId);
            if (dayLimit < 2)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mailServerType"></param>
        /// <param name="userName"></param>
        /// <param name="userId"></param>
        /// <param name="email"></param>
        /// <param name="RDM"></param>
        public static void SendEmailComfirmationMail(int mailServerType, string userName, Guid userId, string email, RequestDetailModel RDM)
        {
            string subject = "Please confirm your email";
            string body = string.Format("<div style='font-family: Verdana,Arial,sans-serif; font-size: 1em;'>" +
                                        "<div style='background-color: #AFC7DB; color: #F25614; width: 600px; height: 40px; padding: 5px 10px 3px 10px; vertical-align:central'><a style='color: #FFFFFF;display: block;float: left;font-size: 20px;font-weight: 200;margin-left: -20px;padding: 10px 20px;text-shadow: 0 1px 0 #94B5D2;' href='http://doosmooc.com/'><img style='width:58px; height:26px;' src='http://doosmooc.com/Images/Logos/logoDoosmoocPNG.png' alt='DoosMooc'></a></div>" +
                                        "<div style='width: 600px; height: 250px; padding: 15px 20px 13px 20px; '><p style='font-weight:bold'> Dear {0}, <br /> <br /> to activate your DoosMooc account, please click below to confirm your email address. <br /> <br /> <a style='background-color: #006DCC; background-image: linear-gradient(to bottom, #0088CC, #0044CC); background-repeat: repeat-x; border-color: rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.25); color: #FFFFFF; text-shadow: 0 -1px 0 rgba(0, 0, 0, 0.25); border-image: none; border-radius: 4px; border-style: solid; border-width: 1px; box-shadow: 0 1px 0 rgba(255, 255, 255, 0.2) inset, 0 1px 2px rgba(0, 0, 0, 0.05); line-height: 20px; padding: 4px 12px; text-align: center; text-decoration:none;' href='http://doosmooc.com/Admin/App_User/Confirm/{1}' target='_blank'> Confirm </a> <br /><br /><br /><div style='font-size:small;'> If the button above doesn't work, please copy and paste this URL into your browser's address bar: <br /> http://doosmooc.com/Admin/App_User/Confirm/{1} </div></p></div>" +
                                        "<div style='background-color: #F5F5F5; width: 600px; height: 20px; padding: 5px 10px 3px 10px; vertical-align: central;text-align:center;'><span style='font-size:small;'>© {2} DoosMooc</span></div>" +
                                        "</div>", userName, userId, DateTime.Now.Year);

            if (mailServerType == (int)MailServerType.DoosMooc && EmailManager.CehckDoosMoocLimit())
            {
                EmailManager.SendMail(mailServerType, email, subject, body, RDM);
            }
            else
            {
                EmailManager.SendMail((int)MailServerType.Gmail, email, subject, body, RDM);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mailServerType"></param>
        public static void SendUpdatesEmais(int mailServerType)
        {
            var mails = EmailDAL.GetNotSent().Take(10);
            foreach (var item in mails)
            {
                string subject = item.Subject;
                string body = item.Body;
                if (mailServerType == (int)MailServerType.DoosMooc && EmailManager.CehckDoosMoocLimit())
                {
                    EmailManager.SendMail(mailServerType, item.ReceiverEmail, subject, body, item.Id, (int)SentEmailType.UncategorizedEmail);
                }
                else if (mailServerType == (int)MailServerType.Gmail)
                {
                    EmailManager.SendMail(mailServerType, item.ReceiverEmail, subject, body, item.Id, (int)SentEmailType.UncategorizedEmail);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mailServerType"></param>
        public static void SendUpdatesNotificationEmails(int mailServerType)
        {
            string subject = "Updates of DoosMooc";

            var template = "<div style='font-family: Verdana,Arial,sans-serif; font-size: 1em;'>" +
                             "<div style='background-color: #AFC7DB; color: #F25614; width: 600px; height: 40px; padding: 5px 10px 3px 10px; vertical-align:central'><a style='color: #FFFFFF;display: block;float: left;font-size: 20px;font-weight: 200;margin-left: -20px;padding: 10px 20px;text-shadow: 0 1px 0 #94B5D2;' href='http://doosmooc.com/'><img style='width:58px; height:26px;' src='http://doosmooc.com/Images/Logos/logoDoosmoocPNG.png' alt='DoosMooc'></a></div><div style='width: 600px; height: 250px; padding: 15px 20px 13px 20px; '>" +
                                    "<p style='font-weight:bold'>Dear {0}, <br /> <br /> Here are some updates of DoosMooc. <br /> <br /><br /></p>" +
                                    "{1}" +
                                    "<br /> <br /><p style='font-weight:bold'><a style='background-color: #006DCC; background-image: linear-gradient(to bottom, #0088CC, #0044CC); background-repeat: repeat-x; border-color: rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.25); color: #FFFFFF; text-shadow: 0 -1px 0 rgba(0, 0, 0, 0.25); border-image: none; border-radius: 4px; border-style: solid; border-width: 1px; box-shadow: 0 1px 0 rgba(255, 255, 255, 0.2) inset, 0 1px 2px rgba(0, 0, 0, 0.05);  padding: 4px 12px; text-align: center; text-decoration:none;' href='http://doosmooc.com'>Go to DoosMooc</a></p>" +
                                    "<br /> <br /><br /><p style='color:gray'>If are not interested in receiving any emails, <a href='http://doosmooc.com/Admin/App_User/AccountIndex'>change account setting.</a> <br /><br /></p>" +
                               "</div><div style='background-color: #F5F5F5; width: 600px; height: 20px; padding: 5px 10px 3px 10px; vertical-align: central;text-align:center;'><span style='font-size:small;'>© {2} DoosMooc</span></div>" +
                        "</div>";

            //var picTemplate = "<div style='display: inline-block;'><a href='http://doosmooc.com/Admin/App_User/Profile/{0}' title='{1}' style='background: none repeat scroll 0 0 transparent; text-decoration: none; color: #428bca'>" +
            //                    "<img style='border: 2px solid white; border-radius: 50%; align-self: center; height: 28px; margin: 5px; width: 28px; vertical-align: middle; ' alt='' src='http://doosmooc.com/Admin/App_User/ViewUserPic/{0}'></a></div>";

            var picInforTemplate = "<div style='display: inline-block;'>" +
                                       "<div style='display: inline-block;'><div style='display: inline-block;'><a href='http://doosmooc.com/Admin/App_User/Profile/{0}' title='{1}' style='background: none repeat scroll 0 0 transparent; text-decoration: none; color: #428bca'>" +
                                       "<img style='border: 2px solid white; border-radius: 50%; align-self: center; height: 28px; margin: 5px; width: 28px; vertical-align: middle; ' alt='' src='http://doosmooc.com/Admin/App_User/ViewUserPic/{0}'></a></div> </div>" +
                                       "<div style='display: inline-block;'>   <a href='http://doosmooc.com/Admin/App_User/Profile/{0}' style='background: none repeat scroll 0 0 transparent; text-decoration: none; color: #428bca'> {1} </a> </div>" +
                                       "<div style='display: inline-block;font-size: smaller; color:gray'>{2} </div>" +
                                       "<div style='display: inline-block;font-size: smaller; color:gray'>{3} </div>" +
                                       " </div><br />";

            var titleTemplate = "<div style='display: inline-block;'>{0} </div>";

            //var dateTemplate = "<div style='display: inline-block; font-size: smaller; color:gray'>{0} </div>";

            var objectInfoTitleTemplate = "<span style='font-weight:bold; padding-left:35px'>{0} </span><br />";

            var objectInfoBodyTemplate = "<span style='padding-left:35px'>{0} </span><br />";

            var fileTemplate = "<span style='padding-left:35px'>   <div style='display: inline-block;'> <img style='width:15px; height:15px;' src='http://doosmooc.com/Content/Icons/glyphicons_062_paperclip.png' alt='Has Attachment'>  &nbsp;  </div><div style='display: inline-block;'>{0} </div> </span>";

            var courseTitle = "<div style='margin-bottom: 0px !important;border-top: 1px solid #dfedf6;background-color: #f5f5f5;margin-bottom: 6%;padding-bottom: 3px;padding-top: 3px;'> <a href='http://doosmooc.com/Admin/Course/CourseView/{0}'>{1}</a> </div>";

            var itemTemplate = "<div style=' background: none repeat scroll 0 0 #eceff5;border-bottom: 1px solid #e3e3e3;border-top: 1px solid #e3e3e3;box-shadow: 0 1px 1px rgba(0, 0, 0, 0.05) inset;margin-top: 3px;overflow: hidden;padding: 10px 5px 7px;'>{0} </div><br />";

            var mails = NotificationEmailDAL.GetNotSent().GroupBy(x => x.App_User).Take(10);

            foreach (var userMails in mails)
            {
                if (userMails.Key.App_UserProfiles.Any(x => x.GetNotificationEmails))
                {
                    var sentMails = new List<int>();

                    var fill = string.Empty;

                    var cnt = 0;

                    foreach (var mailCourses in userMails.GroupBy(x => x.CourseId))
                    {
                        if (mailCourses.Key.HasValue)
                        {
                            fill += string.Format(courseTitle, mailCourses.Key.Value, CourseDAL.Get(mailCourses.Key.Value).Title) + "<br />";
                        }
                        else
                        {
                            fill += string.Format(courseTitle, "Other") + "<br />";
                        }

                        foreach (var item in mailCourses)
                        {
                            cnt++;
                            var oneItem = string.Empty;
                            sentMails.Add(item.Id);
                            var objectInfo = EmailManager.GetInfo(item);
                            //oneItem += string.Format(titleTemplate, cnt + "- ");
                            var objectType = string.Empty;
                            if (objectInfo.NotificationOnObject.CameFromType == (int)ObjectType.Resource)
                            {
                                objectType += string.Format(titleTemplate, string.Format(UT.SL.Model.Resource.App_Common.CreatedThing, UT.SL.Model.Resource.App_Common.Resource + "&nbsp; "));
                            }
                            else if (objectInfo.NotificationOnObject.CameFromType == (int)ObjectType.Assignment)
                            {
                                objectType += string.Format(titleTemplate, string.Format(UT.SL.Model.Resource.App_Common.CreatedThing, UT.SL.Model.Resource.App_Common.Assignment + "&nbsp; "));
                            }
                            else if (objectInfo.NotificationOnObject.CameFromType == (int)ObjectType.Forum)
                            {
                                objectType += string.Format(titleTemplate, string.Format(UT.SL.Model.Resource.App_Common.CreatedThing, UT.SL.Model.Resource.App_Common.Forum + "&nbsp; "));
                            }
                            oneItem += string.Format(picInforTemplate, objectInfo.NotificationOnObject.CameFromUser.GuidId, objectInfo.NotificationOnObject.CameFromUser.FirstName + " " + objectInfo.NotificationOnObject.CameFromUser.LastName + "&nbsp; ", objectType, string.Format(UT.SL.Model.Resource.App_Common.AtTime, objectInfo.CreateDate));


                            var dbObject = ManageObject.GetSharedObject(objectInfo.NotificationOnObject.CameFromId, objectInfo.NotificationOnObject.CameFromType);
                            if (!string.IsNullOrEmpty(dbObject.Title))
                            {
                                //    oneItem += string.Format(objectInfoTemplate, StringUtils.ShortenStringForTitle(objectInfo.NotificationOnObject.CameFromTitle, string.Empty, string.Empty),
                                //        StringUtils.ShortenStringForPreview(objectInfo.NotificationOnObject.CameFromTitle, string.Empty, string.Empty) + "&nbsp; ");
                                oneItem += string.Format(objectInfoTitleTemplate, dbObject.Title);
                            }
                            if (!string.IsNullOrEmpty(dbObject.Body))
                            {
                                oneItem += string.Format(objectInfoBodyTemplate, dbObject.Body);
                            }
                            if (dbObject.FileContent != null)
                            {
                                oneItem += string.Format(fileTemplate, dbObject.FileTitle);
                            }
                            //if (objectInfo.NotificationOnObject.CameFromUser != null)
                            //{
                            //    oneItem += string.Format(titleTemplate, UT.SL.Model.Resource.App_Common.Of + " " + objectInfo.NotificationOnObject.CameFromUser.FirstName + " " + objectInfo.NotificationOnObject.CameFromUser.LastName + "&nbsp; ");
                            //}
                            //if (!string.IsNullOrEmpty(objectInfo.NotificationOnObject.CourseTitle))
                            //{
                            //    oneItem += string.Format(titleTemplate, string.Format(UT.SL.Model.Resource.App_Common.FromCourse, objectInfo.NotificationOnObject.CourseTitle) + "&nbsp; ");
                            //}
                            //oneItem += string.Format(dateTemplate, string.Format(UT.SL.Model.Resource.App_Common.AtTime, objectInfo.CreateDate));

                            fill += string.Format(itemTemplate, oneItem);
                        }
                    }

                    string body = string.Format(template, userMails.Key.FirstName, fill, DateTime.Now.Year);

                    if (mailServerType == (int)MailServerType.DoosMooc && EmailManager.CehckDoosMoocLimit() && EmailManager.CheckUserLimitForNotification(userMails.Key.Id))
                    {
                        EmailManager.SendNotificationMail(mailServerType, userMails.Key.Email, subject, body, sentMails, userMails.Key.Id);
                    }
                    else if (mailServerType == (int)MailServerType.Gmail && EmailManager.CheckUserLimitForUnreadMessages(userMails.Key.Id))
                    {
                        EmailManager.SendNotificationMail(mailServerType, userMails.Key.Email, subject, body, sentMails, userMails.Key.Id);
                    }

                    //if (userMails.Key.Email == "epezhman@yahoo.com")
                    //    EmailManager.SendNotificationMail(mailServerType, userMails.Key.Email, subject, body, sentMails, userMails.Key.Id);
                    //else
                    //{
                    //    foreach (var item in sentMails)
                    //    {
                    //        NotificationEmailDAL.UpdateSeenDate(item);
                    //    }
                    //}
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mailServerType"></param>
        public static void SendLongUnreadenMessages(int mailServerType)
        {
            string subject = "Messages from DoosMooc";

            var template = "<div style='font-family: Verdana,Arial,sans-serif; font-size: 1em;'>" +
                             "<div style='background-color: #AFC7DB; color: #F25614; width: 600px; height: 40px; padding: 5px 10px 3px 10px; vertical-align:central'><a style='color: #FFFFFF;display: block;float: left;font-size: 20px;font-weight: 200;margin-left: -20px;padding: 10px 20px;text-shadow: 0 1px 0 #94B5D2;' href='http://doosmooc.com/'><img style='width:58px; height:26px;' src='http://doosmooc.com/Images/Logos/logoDoosmoocPNG.png' alt='DoosMooc'></a></div><div style='width: 600px; height: 250px; padding: 15px 20px 13px 20px; '>" +
                                    "<p style='font-weight:bold'>Dear {0}, <br /> <br /> Here are some messages you haven't seen in DoosMooc. <br /> <br /><br /></p>" +
                                    "{1}" +
                                    "<br /> <br /><p style='font-weight:bold'><a style='background-color: #006DCC; background-image: linear-gradient(to bottom, #0088CC, #0044CC); background-repeat: repeat-x; border-color: rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.25); color: #FFFFFF; text-shadow: 0 -1px 0 rgba(0, 0, 0, 0.25); border-image: none; border-radius: 4px; border-style: solid; border-width: 1px; box-shadow: 0 1px 0 rgba(255, 255, 255, 0.2) inset, 0 1px 2px rgba(0, 0, 0, 0.05);  padding: 4px 12px; text-align: center; text-decoration:none;' href='http://doosmooc.com'>Go to DoosMooc</a></p>" +
                                    "<br /> <br /><br /><p style='color:gray'>If are not interested in receiving any emails, <a href='http://doosmooc.com/Admin/App_User/AccountIndex'>change account setting.</a> <br /><br /></p>" +
                               "</div><div style='background-color: #F5F5F5; width: 600px; height: 20px; padding: 5px 10px 3px 10px; vertical-align: central;text-align:center;'><span style='font-size:small;'>© {2} DoosMooc</span></div>" +
                        "</div>";

            var picTemplate = "<div style='display: inline-block;'><a href='http://doosmooc.com/Admin/App_User/Profile/{0}' title='{1}' style='background: none repeat scroll 0 0 transparent; text-decoration: none; color: #428bca'>" +
                                "<img style='border: 2px solid white; border-radius: 50%; align-self: center; height: 28px; margin: 5px; width: 28px; vertical-align: middle; ' alt='' src='http://doosmooc.com/Admin/App_User/ViewUserPic/{0}'></a></div>";

            var titleTemplate = "<div style='display: inline-block;'>{0} </div>";

            var dateTemplate = "<div style='display: inline-block; font-size: smaller; color:gray'>{0} </div>";

            var fileTemplate = "<span style='color: Green;'>   <div style='display: inline-block;'> <img style='width:15px; height:15px;' src='http://doosmooc.com/Content/Icons/glyphicons_062_paperclip.png' alt='Has Attachment'>  &nbsp;  </div><div style='display: inline-block;'>{0} </div> </span>";

            var personTitle = "<div style='margin-bottom: 0px !important;border-top: 1px solid #dfedf6;background-color: #f5f5f5;margin-bottom: 6%;padding-bottom: 3px;padding-top: 3px;'><div style='display: inline-block;'><div style='display: inline-block;'>{0} </div><div style='display: inline-block;'>{1} </div> </div></div>";

            var mails = MessageDAL.GetLongUnreadenMessages().GroupBy(x => x.App_User1).Take(10);

            foreach (var userMails in mails)
            {
                var sentUserIds = new List<int>();

                var fill = string.Empty;

                var cnt = 0;

                sentUserIds.Add(userMails.Key.Id);

                foreach (var mailAuthors in userMails.GroupBy(x => x.AutherId))
                {
                    if (mailAuthors.Key > 0)
                    {
                        var user = App_UserDAL.Get(mailAuthors.Key);
                        fill += string.Format(personTitle, string.Format(picTemplate, user.GuidId, user.FirstName + " " + user.LastName + "&nbsp; "), user.FirstName + " " + user.LastName) + "<br />";
                    }
                    else
                    {
                        fill += string.Format(personTitle, "Other") + "<br />";
                    }

                    foreach (var item in mailAuthors)
                    {
                        cnt++;
                        var oneItem = string.Empty;
                        //oneItem += string.Format(titleTemplate, cnt + "- ");

                        if (!string.IsNullOrEmpty(item.Body))
                        {
                            oneItem += string.Format(titleTemplate, item.Body + "&nbsp; ");
                        }

                        if (item.FileContent != null)
                        {
                            oneItem += string.Format(fileTemplate, item.FileTitle + "&nbsp; ");
                        }

                        oneItem += string.Format(dateTemplate, string.Format(UT.SL.Model.Resource.App_Common.AtTime, item.CreateDate));

                        fill += string.Format(titleTemplate, oneItem) + "<br /><br />";
                    }
                }

                string body = string.Format(template, userMails.Key.FirstName, fill, DateTime.Now.Year);

                if (mailServerType == (int)MailServerType.DoosMooc && EmailManager.CehckDoosMoocLimit() && EmailManager.CheckUserLimitForUnreadMessages(userMails.Key.Id))
                {
                    EmailManager.SendUnreadenMessageEmails(mailServerType, userMails.Key.Email, subject, body, sentUserIds, userMails.Key.Id);
                }
                else if (mailServerType == (int)MailServerType.Gmail && EmailManager.CheckUserLimitForUnreadMessages(userMails.Key.Id))
                {
                    EmailManager.SendUnreadenMessageEmails(mailServerType, userMails.Key.Email, subject, body, sentUserIds, userMails.Key.Id);
                }

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static NotificationEmailViewModel GetInfo(NotificationEmail model)
        {
            var returnModel = new NotificationEmailViewModel();
            returnModel.CreateDate = model.CreateDate;
            returnModel.OwnerUser = model.App_User;

            var dbmodel = ManageObject.GetSharedObject(model.ObjectId, model.ObjectType);
            returnModel.NotificationOnObject = dbmodel;
            returnModel.CreateUser = dbmodel.CreateUser;

            returnModel.ObjectId = model.ObjectId;
            returnModel.ObjectType = model.ObjectType;

            returnModel.Notification = model;

            return returnModel;
        }

    }
}
