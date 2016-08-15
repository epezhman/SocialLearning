/*
 * ****************************************************************
 * Filename:        HomeController.cs 
 * version:         
 * Author's name:   Fatemeh Orooji, Pezhman Nasirifard, Elearning lab 
 * Creation date:   
 * Purpose:         containing basic user home actions
 * ****************************************************************
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UT.SL.Data.LINQ;
using UT.SL.DAL;
using System.Data.Linq;
using System.Data;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.BLL;
using UT.SL.Model;
using UT.SL.Data;
using System.Net;
using System.Net.Mail;
using System.Xml;
using UT.SL.Model.Enumeration;

namespace UT.SL.UI.WebUI.Controllers
{

    /// <summary>
    /// contains Actions that are used to generate Layout (user home page) typical Elements
    /// that are used to generate topMenu and sidebars
    /// </summary>
    public class HomeController : BaseController
    {

        public ActionResult Index()
        {
            //App_ActionDAL.Repair();
            //App_ActionDAL.MoveToBookmark();
            //UserInfoManager.Repair(null);
            //ObjectStreamDAL.Repair();
            //ObjectStreamDAL.RepairResources();
            //UserInfoManager.Repair(47);
            //App_UserDAL.UpperCaseNames();
            //UserInfoManager.UpdateGradeOwnerId();
            //App_UserDAL.RepairCreateProfiles();
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Courses");
            }
            else
            {
                return RedirectToAction("About");
            }
        }

        public ActionResult PageNotFound()
        {
            return View();
        }

        public ActionResult GetRightMenu(byte type)
        {
            ViewBag.Type = type;
            return PartialView();
        }

        public ActionResult GetLeftMenu()
        {
            ViewBag.Admin = CurrentUser.IsAdmin;
            return PartialView();
        }
      

        public ActionResult GetLinks(byte type)
        {
            var model = new FormModel<List<Course>, App_User>();
            if (Request.IsAuthenticated)
            {
                model.FormObject = App_UserEnrolementDAL.GetAllUserEnrolemts(CurrentUser.Id).Select(x => x.Course).Distinct().ToList();
                model.ExtraKnownData = CurrentUser;
                model.ExtraData.Add(type);
            }
            return PartialView(model);
        }

        public ActionResult GetTopMenu()
        {
            if (Request.IsAuthenticated)
                return PartialView(CurrentUser);
            else return PartialView();
        }

        public ActionResult TopMenuLinks(int? menuType, int? extra)
        {
            //for test
            System.Diagnostics.Debug.WriteLine(extra + " TopMenuLinks");

            var model = new FormModel<List<Course>, App_User>();
            if (Request.IsAuthenticated)
            {
                model.FormObject = App_UserEnrolementDAL.GetAllUserEnrolemts(CurrentUser.Id).Select(x => x.Course).Distinct().ToList();
                model.ExtraKnownData = CurrentUser;
                if (menuType.HasValue)
                {
                    model.ExtraData.Add(menuType.Value);
                }
                else
                {
                    model.ExtraData.Add(0);
                }
                if (extra.HasValue)
                {
                    model.ExtraData.Add(extra.Value);
                }
                else
                {
                    model.ExtraData.Add(0);
                }
            }
            return PartialView(model);
        }

        public ActionResult TopMenuLinksNotLoggedIn(int? menuType, int? extra)
        {
            
            return PartialView();
        }


        public ActionResult GetCorrespondTitle(int menuType, int? extra)
        {
            var model = new FormModel<Course>();
            model.ExtraData.Add(menuType);
            if (extra.HasValue)
            {
                model.FormObject = CourseDAL.Get(extra.Value);
                model.ExtraData.Add(extra.Value);
            }
            else
            {
                model.ExtraData.Add(0);
            }
            return PartialView(model);
        }

        public ActionResult GetCorrespondLinks(int menuType, int extra, Guid? guid, int id = 0, int rank = 0)
        {
            //for test
            System.Diagnostics.Debug.WriteLine(extra + " GetCorrespondLinks");

            var model = new List<LinkModel>();
            var temp = new LinkModel();
            int linkRank = 1;
            if (menuType == (int)UT.SL.Model.Enumeration.MenuType.MyHome)
            {
                temp = new LinkModel();
                temp.Action = "Index";
                temp.Controller = "MyHome";
                temp.Title = UT.SL.Model.Resource.App_Common.All;
                temp.Rank = linkRank;
                if (linkRank == rank)
                    temp.IsActive = true;
                linkRank++;
                temp.Id = id;
                temp.Filter = string.Empty;
                model.Add(temp);

                temp = new LinkModel();
                temp.Action = "Index";
                temp.Controller = "MyHome";
                temp.Title = UT.SL.Model.Resource.App_Common.AllGroups;
                temp.Rank = linkRank;
                if (linkRank == rank)
                    temp.IsActive = true;
                linkRank++;
                temp.Id = id;
                temp.Filter = "3";
                model.Add(temp);

                var userGroups = CurrentUser.SocialGroups.ToList();
                if (userGroups.Any())
                {
                    if (userGroups.Count() > 4)
                    {
                        foreach (var item in userGroups.Take(4))
                        {
                            temp = new LinkModel();
                            temp.Action = "Index";
                            temp.Controller = "MyHome";
                            temp.Title = item.Title;
                            temp.Rank = linkRank;
                            if (linkRank == rank)
                                temp.IsActive = true;
                            linkRank++;
                            temp.Id = id;
                            temp.Filter = "3_" + item.Id;
                            model.Add(temp);
                        }
                        foreach (var item in userGroups.Skip(4))
                        {
                            temp = new LinkModel();
                            temp.Action = "Index";
                            temp.Controller = "MyHome";
                            temp.Title = item.Title;
                            temp.ExtraLink = true;
                            temp.Rank = linkRank;
                            if (linkRank == rank)
                                temp.IsActive = true;
                            linkRank++;
                            temp.Id = id;
                            temp.Filter = "3_" + item.Id;
                            model.Add(temp);
                        }
                    }
                    else
                    {
                        foreach (var item in userGroups)
                        {
                            temp = new LinkModel();
                            temp.Action = "Index";
                            temp.Controller = "MyHome";
                            temp.Title = item.Title;
                            temp.Rank = linkRank;
                            if (linkRank == rank)
                                temp.IsActive = true;
                            linkRank++;
                            temp.Id = id;
                            temp.Filter = "3_" + item.Id;
                            model.Add(temp);
                        }
                    }
                }
                if (model.All(x => x.IsActive == false))
                    model.First().IsActive = true;
            }
            else if (menuType == (int)UT.SL.Model.Enumeration.MenuType.MyCourses)
            {
                temp = new LinkModel();
                temp.Action = "CourseView";
                temp.Controller = "Course";
                temp.Area = "Admin";
                temp.Title = UT.SL.Model.Resource.App_Common.All;
                temp.Rank = linkRank;
                if (linkRank == rank)
                    temp.IsActive = true;
                linkRank++;
                temp.Id = id;
                temp.Filter = string.Empty;
                temp.IsIcon = true;
                temp.IconTitle = "globe";
                model.Add(temp);


                temp = new LinkModel();
                temp.Action = "CourseView";
                temp.Controller = "Course";
                temp.Area = "Admin";
                temp.Title = UT.SL.Model.Resource.App_Common.Resources;
                temp.Rank = linkRank;
                if (linkRank == rank)
                    temp.IsActive = true;
                linkRank++;
                temp.Id = id;
                temp.Filter = "1";
                temp.IsIcon = true;
                temp.IconTitle = "file";
                model.Add(temp);

                //temp = new LinkModel();
                //temp.Action = "CourseView";
                //temp.Controller = "Course";
                //temp.Area = "Admin";
                //temp.Title = UT.SL.Model.Resource.App_Common.Activities;
                //temp.Rank = linkRank;
                //if (linkRank == rank)
                //    temp.IsActive = true;
                //linkRank++;
                //temp.Id = id;
                //temp.Filter = "4";
                //model.Add(temp);

                temp = new LinkModel();
                temp.Action = "CourseView";
                temp.Controller = "Course";
                temp.Area = "Admin";
                temp.Title = UT.SL.Model.Resource.App_Common.Forumes;
                temp.Rank = linkRank;
                if (linkRank == rank)
                    temp.IsActive = true;
                linkRank++;
                temp.Id = id;
                temp.Filter = "8";
                temp.IsIcon = true;
                temp.IconTitle = "comment";
                model.Add(temp);

                temp = new LinkModel();
                temp.Action = "CourseView";
                temp.Controller = "Course";
                temp.Area = "Admin";
                temp.Title = UT.SL.Model.Resource.App_Common.Assignments;
                temp.Rank = linkRank;
                if (linkRank == rank)
                    temp.IsActive = true;
                linkRank++;
                temp.Id = id;
                temp.Filter = "9";
                temp.IsIcon = true;
                temp.IconTitle = "pencil";
                model.Add(temp);

                //add recommended 
                temp = new LinkModel();
                temp.Action = "CourseView";
                temp.Controller = "Course";
                temp.Area = "Admin";
                temp.Title = UT.SL.Model.Resource.App_Common.Recommended;
                temp.Rank = linkRank;
                if (linkRank == rank)
                    temp.IsActive = true;
                linkRank++;
                temp.Id = id;
                temp.Filter = "6";
                temp.IsIcon = true;
                temp.IconTitle = "gift";
                model.Add(temp);

                temp = new LinkModel();
                temp.Action = "CourseView";
                temp.Controller = "Course";
                temp.Area = "Admin";
                temp.Title = UT.SL.Model.Resource.App_Common.Hot;
                temp.Rank = linkRank;
                if (linkRank == rank)
                    temp.IsActive = true;
                linkRank++;
                temp.Id = id;
                temp.Filter = "7";
                temp.IsIcon = true;
                temp.IconTitle = "fire";
                model.Add(temp);
                //add recommended 


                temp = new LinkModel();
                temp.Action = "Participants";
                temp.Controller = "Course";
                temp.Area = "Admin";
                temp.Title = UT.SL.Model.Resource.App_Common.Participants;
                temp.Rank = linkRank;
                if (linkRank == rank)
                    temp.IsActive = true;
                linkRank++;
                temp.Id = id;
                temp.Filter = "5";
                model.Add(temp);

                temp = new LinkModel();
                temp.Action = "Portfolio";
                temp.Controller = "Course";
                temp.Area = "Admin";
                temp.Title = UT.SL.Model.Resource.App_Common.Portfolio;
                temp.Rank = linkRank;
                if (linkRank == rank)
                    temp.IsActive = true;
                linkRank++;
                temp.Id = id;
                model.Add(temp);

                temp = new LinkModel();
                temp.Action = "CourseView";
                temp.Controller = "Course";
                temp.Area = "Admin";
                temp.Title = UT.SL.Model.Resource.App_Common.AllGroups;
                temp.Rank = linkRank;
                if (linkRank == rank)
                    temp.IsActive = true;
                linkRank++;
                temp.Id = id;
                temp.Filter = "2";
                model.Add(temp);

                //
                //temp = new LinkModel();
                //temp.Action = "CourseView";
                //temp.Controller = "Course";
                //temp.Area = "Admin";
                //temp.Title = UT.SL.Model.Resource.App_Common.Recommended;
                //temp.Rank = linkRank;
                //if (linkRank == rank)
                //    temp.IsActive = true;
                //linkRank++;
                //temp.Id = id;
                //temp.Filter = "6";
                //model.Add(temp);

                //temp = new LinkModel();
                //temp.Action = "CourseView";
                //temp.Controller = "Course";
                //temp.Area = "Admin";
                //temp.Title = UT.SL.Model.Resource.App_Common.Hot;
                //temp.Rank = linkRank;
                //if (linkRank == rank)
                //    temp.IsActive = true;
                //linkRank++;
                //temp.Id = id;
                //temp.Filter = "7";
                //model.Add(temp);
                //

                var groups = CourseDAL.Get(extra).LearningGroups.ToList();

                var roles = App_UserEnrolementDAL.GetAllByCourseAndUser(extra, CurrentUser.Id);

                if (groups.Any())
                {

                    if (roles.Any())
                    {
                        var role = roles.First();
                        if (role.Type == (int)UT.SL.Model.Enumeration.MemebrshipType.Student)
                        {
                            groups = groups.Where(x => x.GroupMembers.Any(u => u.UserId == CurrentUser.Id)).ToList();
                        }
                        if (groups.Count() > 4)
                        {
                            foreach (var item in groups.Take(4))
                            {
                                temp = new LinkModel();
                                temp.Action = "CourseView";
                                temp.Controller = "Course";
                                temp.Area = "Admin";
                                temp.Title = item.Title;
                                temp.Rank = linkRank;
                                if (linkRank == rank)
                                    temp.IsActive = true;
                                linkRank++;
                                temp.Id = id;
                                temp.Filter = "2_" + item.Id;
                                model.Add(temp);
                            }
                            foreach (var item in groups.Skip(4))
                            {
                                temp = new LinkModel();
                                temp.Action = "CourseView";
                                temp.Controller = "Course";
                                temp.Area = "Admin";
                                temp.Title = item.Title;
                                temp.ExtraLink = true;
                                temp.Rank = linkRank;
                                if (linkRank == rank)
                                    temp.IsActive = true;
                                linkRank++;
                                temp.Id = id;
                                temp.Filter = "2_" + item.Id;
                                model.Add(temp);
                            }
                        }
                        else
                        {
                            foreach (var item in groups)
                            {
                                temp = new LinkModel();
                                temp.Action = "CourseView";
                                temp.Controller = "Course";
                                temp.Area = "Admin";
                                temp.Title = item.Title;
                                temp.Rank = linkRank;
                                if (linkRank == rank)
                                    temp.IsActive = true;
                                linkRank++;
                                temp.Id = id;
                                temp.Filter = "2_" + item.Id;
                                model.Add(temp);
                            }
                        }
                    }
                }

                if (roles.Any())
                {
                    var role = roles.First();
                    if (role.Type != (int)UT.SL.Model.Enumeration.MemebrshipType.Student)
                    {
                        temp = new LinkModel();
                        temp.Action = "EditCourseDetails";
                        temp.Controller = "Course";
                        temp.Area = "Admin";
                        temp.Title = UT.SL.Model.Resource.App_Common.Administration;
                        temp.Rank = linkRank;
                        if (linkRank == rank)
                            temp.IsActive = true;
                        linkRank++;
                        temp.Id = id;
                        model.Add(temp);
                    }
                }

                if (model.All(x => x.IsActive == false))
                    model.First().IsActive = true;
            }
            else if (menuType == (int)UT.SL.Model.Enumeration.MenuType.UserProfile)
            {
                //linkRank = 0;
                temp = new LinkModel();
                temp.Action = "Profile";
                temp.Controller = "App_User";
                temp.Area = "Admin";
                temp.Title = UT.SL.Model.Resource.App_Common.About;
                temp.Rank = linkRank;
                if (linkRank == rank)
                    temp.IsActive = true;
                linkRank++;
                temp.Id = extra;
                if (guid.HasValue)
                    temp.guid = guid.Value;
                model.Add(temp);

                //temp = new LinkModel();
                //temp.Action = "Portfolio";
                //temp.Controller = "App_User";
                //temp.Area = "Admin";
                //temp.Title = UT.SL.Model.Resource.App_Common.Portfolio;
                //temp.Rank = linkRank;
                //if (linkRank == rank)
                //    temp.IsActive = true;
                //linkRank++;
                //temp.Id = id;
                //if (guid.HasValue)
                //    temp.guid = guid.Value;
                //model.Add(temp);

                //temp = new LinkModel();
                //temp.Action = "Profile";
                //temp.Controller = "App_User";
                //temp.Area = "Admin";
                //temp.Title = UT.SL.Model.Resource.App_Common.SocialActivity;
                //temp.Rank = linkRank;
                //if (linkRank == rank)
                //    temp.IsActive = true;
                //linkRank++;
                //temp.Id = id;
                //model.Add(temp);
                if (model.All(x => x.IsActive == false))
                    model.First().IsActive = true;
                return PartialView("CorrespondLinksForProfile", model);
            }
            else if (menuType == (int)UT.SL.Model.Enumeration.MenuType.Survey)
            {

                temp = new LinkModel();
                temp.Action = "Harter";
                temp.Controller = "Survey";
                temp.Area = "Admin";
                temp.Title = UT.SL.Model.Resource.App_Common.Harter;
                temp.Rank = linkRank;
                if (linkRank == rank)
                    temp.IsActive = true;
                linkRank++;
                temp.Id = id;
                temp.Filter = string.Empty;
                model.Add(temp);

                temp = new LinkModel();
                temp.Action = "AGQ_R";
                temp.Controller = "Survey";
                temp.Area = "Admin";
                temp.Title = UT.SL.Model.Resource.App_Common.AGQ_R;
                temp.Rank = linkRank;
                if (linkRank == rank)
                    temp.IsActive = true;
                linkRank++;
                temp.Id = id;
                temp.Filter = string.Empty;
                model.Add(temp);

                //temp = new LinkModel();
                //temp.Action = "Hermans";
                //temp.Controller = "Survey";
                //temp.Area = "Admin";
                //temp.Title = UT.SL.Model.Resource.App_Common.Hermans;
                //temp.Rank = linkRank;
                //if (linkRank == rank)
                //    temp.IsActive = true;
                //linkRank++;
                //temp.Id = id;
                //temp.Filter = string.Empty;
                //model.Add(temp);


                temp = new LinkModel();
                temp.Action = "Feedback";
                temp.Controller = "Survey";
                temp.Area = "Admin";
                temp.Title = UT.SL.Model.Resource.App_Common.Feedback;
                temp.Rank = linkRank;
                if (linkRank == rank)
                    temp.IsActive = true;
                linkRank++;
                temp.Id = id;
                temp.Filter = string.Empty;
                model.Add(temp);


                if (model.All(x => x.IsActive == false))
                    model.First().IsActive = true;
                return PartialView("CorrespondLinksForSurvey", model);

            }
            else if (menuType == (int)UT.SL.Model.Enumeration.MenuType.Calendar)
            {
            }
            else if (menuType == (int)UT.SL.Model.Enumeration.MenuType.Messages)
            {
            }
            else if (menuType == (int)UT.SL.Model.Enumeration.MenuType.Notifications)
            {
            }
            else if (menuType == (int)UT.SL.Model.Enumeration.MenuType.User)
            {
            }
            else if (menuType == (int)UT.SL.Model.Enumeration.MenuType.FirstPage)
            {
                //if (Request.IsAuthenticated)
                //{
                //    var userCourses = CourseDAL.GetAll(CurrentUser.Id);
                //    foreach (var item in userCourses.Distinct())
                //    {
                //        temp = new LinkModel();
                //        temp.Action = "CourseView";
                //        temp.Controller = "Course";
                //        temp.Area = "Admin";
                //        temp.Title = item.Title;
                //        temp.Rank = 1;                        
                //        temp.Id = id;
                //        temp.Filter = string.Empty;
                //        model.Add(temp);
                //    }
                //}                

                temp = new LinkModel();
                temp.Action = "Courses";
                temp.Controller = "Home";
                temp.Area = "";
                temp.Title = UT.SL.Model.Resource.App_Common.Courses;
                temp.Rank = linkRank;
                if (linkRank == rank)
                    temp.IsActive = true;
                linkRank++;
                temp.Id = id;
                temp.Filter = string.Empty;
                model.Add(temp);

                temp = new LinkModel();
                temp.Action = "About";
                temp.Controller = "Home";
                temp.Area = "";
                temp.Title = UT.SL.Model.Resource.App_Common.About;
                temp.Rank = linkRank;
                if (linkRank == rank)
                    temp.IsActive = true;
                linkRank++;
                temp.Id = id;
                temp.Filter = string.Empty;
                model.Add(temp);

                temp = new LinkModel();
                temp.Action = "Partners";
                temp.Controller = "Home";
                temp.Area = "";
                temp.Title = UT.SL.Model.Resource.App_Common.Partners;
                temp.Rank = linkRank;
                if (linkRank == rank)
                    temp.IsActive = true;
                linkRank++;
                temp.Id = id;
                temp.Filter = string.Empty;
                model.Add(temp);


                if (model.All(x => x.IsActive == false))
                    model.First().IsActive = true;
                return PartialView("CorrespondLinksForSurvey", model);

            }
            else if (menuType == (int)UT.SL.Model.Enumeration.MenuType.Advrt)
            {
                temp = new LinkModel();
                temp.Action = "About";
                temp.Controller = "Home";
                temp.Area = "";
                temp.Title = UT.SL.Model.Resource.App_Common.About;
                temp.Rank = linkRank;
                if (linkRank == rank)
                    temp.IsActive = true;
                linkRank++;
                temp.Id = id;
                temp.Filter = string.Empty;
                model.Add(temp);

                temp = new LinkModel();
                temp.Action = "Courses";
                temp.Controller = "Home";
                temp.Area = "";
                temp.Title = UT.SL.Model.Resource.App_Common.Courses;
                temp.Rank = linkRank;
                if (linkRank == rank)
                    temp.IsActive = true;
                linkRank++;
                temp.Id = id;
                temp.Filter = string.Empty;
                model.Add(temp);                

                temp = new LinkModel();
                temp.Action = "Partners";
                temp.Controller = "Home";
                temp.Area = "";
                temp.Title = UT.SL.Model.Resource.App_Common.Partners;
                temp.Rank = linkRank;
                if (linkRank == rank)
                    temp.IsActive = true;
                linkRank++;
                temp.Id = id;
                temp.Filter = string.Empty;
                model.Add(temp);

                temp = new LinkModel();
                temp.Action = "LogIn";
                temp.Controller = "App_User";
                temp.Area = "Admin";
                temp.Title = UT.SL.Model.Resource.App_Common.LogIn;
                temp.Rank = linkRank;
                if (linkRank == rank)
                    temp.IsActive = true;
                linkRank++;
                temp.Id = id;
                temp.Filter = string.Empty;
                temp.IsBold = true;
                model.Add(temp);

                temp = new LinkModel();
                temp.Action = "Register";
                temp.Controller = "App_User";
                temp.Area = "Admin";
                temp.Title = UT.SL.Model.Resource.App_Common.Register;
                temp.Rank = linkRank;
                if (linkRank == rank)
                    temp.IsActive = true;
                linkRank++;
                temp.Id = id;
                temp.Filter = string.Empty;
                temp.IsBold = true;
                model.Add(temp);

                if (model.All(x => x.IsActive == false))
                    model.First().IsActive = true;
                return PartialView("CorrespondLinksForSurvey", model);

            }
            else if (menuType == (int)UT.SL.Model.Enumeration.MenuType.Admin)
            {
                if (extra == (int)UT.SL.Model.Enumeration.NavLinks.Category)
                {
                }
                else if (extra == (int)UT.SL.Model.Enumeration.NavLinks.Tag)
                {
                }
                else if (extra == (int)UT.SL.Model.Enumeration.NavLinks.Courses)
                {
                }
                else if (extra == (int)UT.SL.Model.Enumeration.NavLinks.AppActiopn)
                {
                    temp = new LinkModel();
                    temp.Action = "ActionTree";
                    temp.Controller = "AppActions";
                    temp.Area = "Admin";
                    temp.Title = "Master";
                    temp.Rank = linkRank;
                    if (linkRank == rank)
                        temp.IsActive = true;
                    linkRank++;
                    temp.Id = id;
                    temp.Filter = string.Empty;
                    model.Add(temp);

                    temp = new LinkModel();
                    temp.Action = "ActionTree";
                    temp.Controller = "AppActions";
                    temp.Area = "Admin";
                    temp.Title = "Bookmarks";
                    temp.Rank = linkRank;
                    if (linkRank == rank)
                        temp.IsActive = true;
                    linkRank++;
                    temp.Id = id;
                    temp.Filter = string.Empty;
                    model.Add(temp);

                    if (model.All(x => x.IsActive == false))
                        model.First().IsActive = true;
                    return PartialView("CorrespondLinksForSurvey", model);
                }
                else if (extra == (int)UT.SL.Model.Enumeration.NavLinks.Users)
                {
                }
            }

            else
            {
            }
            return PartialView(model);
        }

        public ActionResult AccessDenied()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public ActionResult NotActive()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public ActionResult FirstTimeLogIn()
        {
            if (Request.IsAuthenticated)
                if (CurrentUser.IsFirstTime.HasValue && CurrentUser.IsFirstTime.Value)
                {
                    ViewBag.IsFirstTime = true;
                    App_UserDAL.UpdateLastActivity(CurrentUser);
                    ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.App_User),
                                       new Func<App_User, DALReturnModel<App_User>>(App_UserDAL.UpdateLastActivity), CurrentUser);
                }
            return PartialView();
        }

        public ActionResult ShowCourseNews()
        {
            ObjectStreamSearchModel model = new ObjectStreamSearchModel
            {
                UserId = CurrentUser.Id,
                IsCourse = true,
            };
            model.Area = "Admin";
            return PartialView(SearchResourceFilters(model));
        }

        public PagedList<ObjectStream> SearchResourceFilters(ObjectStreamSearchModel model)
        {
            model.Area = "Admin";
            var qry = ObjectStreamDAL.Find(model);
            model.Update(5, qry.Count());
            var ls = qry.Skip(model.PageSize * model.PageIndex).Take(model.PageSize).ToList();
            var ql = new PagedList<ObjectStream>(ls, model);
            return ql;
        }

        public ActionResult GetObjectTitle(int objectId, int objectType)
        {
            var model = ManageObject.GetSharedObject(objectId, objectType);
            string title = string.Empty;
            if (model.Title != null)
            {
                if (model.Title.Length >= 50)
                {
                    title = model.Title.Substring(0, 50) + "...";
                }
                else
                {
                    title = model.Title;
                }
            }
            return Content(model.Title);
        }

        public ActionResult GetObjectBody(int objectId, int objectType)
        {
            var model = ManageObject.GetSharedObject(objectId, objectType);
            string body = string.Empty;
            if (model.Body != null)
            {
                if (model.Body.Length >= 100)
                {
                    if (model.Title != null)
                    {
                        body = model.Body.Substring(0, 100 - model.Title.Length);
                    }
                    else
                    {
                        body = model.Body.Substring(0, 100) + "...";
                    }
                }
                else
                {
                    //if (model.Title != null)
                    //{
                    //    body = model.Body.Substring(0, model.Body.Length - model.Title.Length);
                    //}
                    //else
                    //{
                    //    body = model.Body;
                    //}
                }
            }
            if (body != string.Empty && model.Title != null)
                body = "-" + body;
            return Content(body);
        }

        public ActionResult GetCourseTitle(int id)
        {
            var model = CourseDAL.Get(id);
            return Content(model.Title);
        }

        public ActionResult ShowSocialNews()
        {
            ObjectStreamSearchModel model = new ObjectStreamSearchModel
            {
                UserId = CurrentUser.Id,
                IsCourse = false,
                ObjectType = (int)UT.SL.Model.Enumeration.ObjectType.Resource
            };
            model.Area = "Admin";
            return PartialView(SearchResourceFilters(model));
        }

        public ActionResult ShowHomeBody()
        {
            if (Request.IsAuthenticated)
            {
                return PartialView("ShowHomeBody", null);
            }
            else
            {
                return PartialView("ShowAdvertise", null);
            }

        }

        public ActionResult Courses(int? rank)
        {
            if (rank.HasValue)
                ViewBag.LinkRank = rank.Value;
            //var model = CourseDAL.GetAllNotForThisUser(CurrentUser.Id);
            var model = CourseDAL.GetAll();
            if (!Request.IsAuthenticated)
            {
                ViewBag.NotLogedIn = true;
            }
            return View(model);
        }

        public ActionResult About(int? rank)
        {
            if (rank.HasValue)
                ViewBag.LinkRank = rank.Value;
            if (!Request.IsAuthenticated)
            {
                ViewBag.NotLogedIn = true;
            }
            return View();
        }

        public ActionResult Partners(int? rank)
        {
            if (rank.HasValue)
                ViewBag.LinkRank = rank.Value;
            if (!Request.IsAuthenticated)
            {
                ViewBag.NotLogedIn = true;
            }
            return View();
        }
    }
}
