/*
 * ****************************************************************
 * Filename:        MyHomeController.cs 
 * version:         
 * Author's name:   Fatemeh Orooji, Pezhman Nasirifardfard, Elearning lab 
 * Creation date:   
 * Purpose:         containig Actions used in user homepage body
 * ****************************************************************
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UT.SL.BLL;
using UT.SL.DAL;
using UT.SL.Data.LINQ;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Model.Enumeration;
using UT.SL.UI.WebUI.Areas.Admin.Controllers;

namespace UT.SL.UI.WebUI.Controllers
{
    /// <summary>
    /// contains actions used to provide user home page body elements such as post
    /// </summary>
    [Authorize]
    public class MyHomeController : BaseController
    {

        public ActionResult Index(int? rank, string filter)
        {
            if (rank.HasValue)
                ViewBag.LinkRank = rank.Value;
            if (!String.IsNullOrEmpty(filter))
            {
                ViewBag.Filter = filter;
            }
            return View();
        }

        public ActionResult GetPanel(string filter)
        {
            ViewBag.Filter = filter;
            return PartialView();
        }

        public ActionResult PostResource()
        {
            var model = new FormModel<Resource, List<SelectListItems>>();
            model.FormObject = new Resource();
            model.ExtraKnownData = new List<SelectListItems>();
            var tempItems = new List<SelectListItem>();
            tempItems = new List<SelectListItem>();
            tempItems.Add(new SelectListItem
            {
                Text = UT.SL.Model.Resource.App_Common.All,
                Value = "D_1"
            });
            tempItems.AddRange(CurrentUser.SocialGroups.Where(x => x.GroupMembers.Any(p => p.UserId == CurrentUser.Id && p.IsCircleAdmin.Value == 1)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Title
            }).ToList());
            model.ExtraKnownData.Add(new SelectListItems
            {
                Items = tempItems,
                SelectedIds = new string[] { "D_1" }
            });
            ViewBag.UserId = CurrentUser.Id;
            return PartialView(model);
        }

        public ActionResult SaveResource(HttpPostedFileBase resourceFile, int? id, int? overwite)
        {
            int fileId = 0;
            try
            {
                if (resourceFile != null)
                {
                    if (overwite.HasValue && id.HasValue)
                    {
                        var bpr = new BatchProcessResultModel();
                        var resource = ResourceDAL.Get(id.Value);
                        resource.FileMime = resourceFile.ContentType;
                        resource.FileSize = resourceFile.ContentLength;
                        resource.FileTitle = resourceFile.FileName;
                        byte[] tempFile = null;
                        tempFile = new byte[resourceFile.ContentLength];
                        resourceFile.InputStream.Read(tempFile, 0, resourceFile.ContentLength);
                        resource.FileContent = tempFile;
                        var drm = (DALReturnModel<Resource>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Resource),
                                       new Func<Resource, BatchProcessResultModel, DALReturnModel<Resource>>(ResourceDAL.UpdateFile), resource, bpr);
                        bpr = drm.BPR;
                    }
                    else
                    {
                        var newFile = new Resource
                        {
                            CreateDate = DateTime.Now,
                            FileMime = resourceFile.ContentType,
                            FileSize = resourceFile.ContentLength,
                            FileTitle = resourceFile.FileName,
                            GuidId = Guid.NewGuid(),
                            IsValid = false,
                            IsPublishd = false,
                            CreateUserId = CurrentUser.Id,
                            CourseId = null
                        };

                        byte[] tempFile = null;
                        tempFile = new byte[resourceFile.ContentLength];
                        resourceFile.InputStream.Read(tempFile, 0, resourceFile.ContentLength);
                        newFile.FileContent = tempFile;

                        var bpr = new BatchProcessResultModel();
                        var drm = (DALReturnModel<Resource>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Resource),
                            new Func<Resource, BatchProcessResultModel, DALReturnModel<Resource>>(ResourceDAL.Add), newFile, bpr);
                        bpr = drm.BPR;
                        fileId = drm.ReturnObject.Id;
                        if (bpr.Failed > 0)
                        {
                            return Content("0");
                        }
                    }
                }
            }
            catch
            {
                return Content("0");
            }
            return Content(fileId.ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult RemoveResource(int id)
        {
            try
            {
                ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Resource),
                                       new Func<Resource, DALReturnModel<Resource>>(ResourceDAL.Delete), new Resource { Id = id });
            }
            catch
            {
                return Content("0");
            }
            return Content("2");
        }

        public ActionResult DeleteResourceFile(int id)
        {
            try
            {
                ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Resource),
                                      new Func<Resource, DALReturnModel<Resource>>(ResourceDAL.DeleteFile), new Resource { Id = id });
            }
            catch
            {
                return Content("0");
            }
            return Content("2");
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult PostResource(FormModel<Resource, List<SelectListItems>> model, string[] groupIds)
        {
            int resourceId = 0;
            try
            {
                if (groupIds == null)
                {
                    return Content("-1");
                }
                var bpr = new BatchProcessResultModel();
                if (model.FormObject.Id > 0)
                {
                    var obj = ResourceDAL.Get(model.FormObject.Id);
                    if (obj != null)
                    {
                        obj.Body = model.FormObject.Body.StringNormalizer();
                        obj.IsValid = true;
                        obj.IsPublishd = true;
                        resourceId = obj.Id;
                        var drm = (DALReturnModel<Resource>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Resource),
                                       new Func<Resource, BatchProcessResultModel, DALReturnModel<Resource>>(ResourceDAL.Update), obj, bpr);
                        bpr = drm.BPR;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(model.FormObject.Body.StringNormalizer()))
                        {
                            obj = new Resource
                            {
                                Body = model.FormObject.Body.StringNormalizer(),
                                CourseId = null,
                                CreateDate = DateTime.Now,
                                CreateUserId = CurrentUser.Id,
                                GuidId = Guid.NewGuid(),
                                IsPublishd = true,
                                IsValid = true
                            };
                            var drm = ((DALReturnModel<Resource>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Resource), new Func<Resource, BatchProcessResultModel, DALReturnModel<Resource>>(ResourceDAL.Add), obj, bpr));
                            resourceId = drm.ReturnObject.Id;
                            bpr = drm.BPR;
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FormObject.Body.StringNormalizer()))
                    {
                        var obj = new Resource
                        {
                            Body = model.FormObject.Body.StringNormalizer(),
                            CourseId = null,
                            CreateDate = DateTime.Now,
                            CreateUserId = CurrentUser.Id,
                            GuidId = Guid.NewGuid(),
                            IsPublishd = true,
                            IsValid = true
                        };
                        var drm = ((DALReturnModel<Resource>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Resource), new Func<Resource, BatchProcessResultModel, DALReturnModel<Resource>>(ResourceDAL.Add), obj, bpr));
                        resourceId = drm.ReturnObject.Id;
                        bpr = drm.BPR;
                    }
                }
                if (resourceId > 0)
                {
                    if (groupIds != null && !groupIds.Any(x => x.StartsWith("D_")))
                        ObjectStreamManager.ObjectResourceToStream(resourceId, (int)UT.SL.Model.Enumeration.ObjectType.Resource, new ToEmergeIds { SocialGroups = groupIds.ToList() }, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
                    else
                        ObjectStreamManager.ObjectResourceToStream(resourceId, (int)UT.SL.Model.Enumeration.ObjectType.Resource, new ToEmergeIds(), ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream), CurrentUser.Id);
                    //updaet credit
                    ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.Create, resourceId, (int)UT.SL.Model.Enumeration.ObjectType.Resource, CurrentUser.Id);
                    //updaet credit
                }
            }
            catch
            {
                return Content("0");
            }
            return Content(resourceId.ToString());
        }

        public ActionResult GetObjects(int? page, DateTime? date, string resourceSelections)
        {
            HttpCookie screenCookie = Request.Cookies["_screen"];
            if (screenCookie != null)
            {
                var widthHeight = screenCookie.Value.Split('_');
                var width = 0;
                if (Int32.TryParse(widthHeight[0], out width))
                {
                    if (width <= 768)
                    {
                        return PartialView("ResourcesCompact", ResourceFinder.GetUserHome(page, date, CurrentUser.Id, resourceSelections, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream)));
                    }
                }
                return PartialView("Resources", ResourceFinder.GetUserHome(page, date, CurrentUser.Id, resourceSelections, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream)));
            }
            else
            {
                return PartialView("Resources", ResourceFinder.GetUserHome(page, date, CurrentUser.Id, resourceSelections, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream)));
            }
        }

        public ActionResult GetNewObjects(DateTime date, string resourceSelections)
        {
            var resources = ResourceFinder.GetNewUserHome(date, CurrentUser.Id, resourceSelections, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
            ViewBag.LastCheckDate = DateTime.Now;
            HttpCookie screenCookie = Request.Cookies["_screen"];
            if (screenCookie != null)
            {
                var widthHeight = screenCookie.Value.Split('_');
                var width = 0;
                if (Int32.TryParse(widthHeight[0], out width))
                {
                    if (width <= 768)
                    {
                        return PartialView("ResourcesCompact", resources);
                    }
                }
                return PartialView("Resources", resources);
            }
            else
            {
                return PartialView("Resources", resources);
            }
        }

        public ActionResult GetOneResource(int Id)
        {
            var models = new List<ObjectViewModelList>();
            var tempObjectViewModelList = new ObjectViewModelList();
            var item = ManageObject.GetSharedObject(Id, (int)UT.SL.Model.Enumeration.ObjectType.Resource);
            tempObjectViewModelList.ObjectViewModels.Add(item);
            if (item.IsWide)
            {
                tempObjectViewModelList.IsWide = true;
            }
            models.Add(tempObjectViewModelList);
            ViewBag.Show = true;
            HttpCookie screenCookie = Request.Cookies["_screen"];
            if (screenCookie != null)
            {
                var widthHeight = screenCookie.Value.Split('_');
                var width = 0;
                if (Int32.TryParse(widthHeight[0], out width))
                {
                    if (width <= 768)
                    {
                        return PartialView("ResourcesCompact", models);
                    }
                }
                return PartialView("Resources", models);
            }
            else
            {
                return PartialView("Resources", models);
            }
        }

        public ActionResult GetOneObjectAfterEdit(int Id, int type)
        {
            var model = ManageObject.GetSharedObject(Id, type);
            return PartialView("ObjectViewer", model);
        }

        [HttpPost]
        public ActionResult DeleteObject(int ObjectId, int ObjecType)
        {
            ManageObject.DeleteObject(ObjectId, ObjecType, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
            return Content("true");
        }

        public ActionResult EditResource(int ObjectId, int ObjecType)
        {
            var modelObject = ManageObject.GetSharedObject(ObjectId, ObjecType);
            if (ObjecType == (int)UT.SL.Model.Enumeration.ObjectType.Resource)
            {

                var model = new FormModel<Resource, List<SelectListItems>>();
                model.FormObject = ResourceDAL.Get(ObjectId);
                model.ExtraKnownData = new List<SelectListItems>();
                var tempItems = new List<SelectListItem>();
                tempItems.Add(new SelectListItem
                {
                    Text = UT.SL.Model.Resource.App_Common.All,
                    Value = "D_1"
                });
                var shared = ObjectStreamManager.FindObjectWhichStream(ObjectId, ObjecType);
                var sharedIds = new List<string>();
                foreach (var item in shared.CourseIds)
                {
                    sharedIds.Add("D_" + item);
                }
                foreach (var item in shared.SocialGroups)
                {
                    sharedIds.Add(item.ToString());
                }
                tempItems.AddRange(CurrentUser.SocialGroups.Where(x => x.GroupMembers.Any(p => p.UserId == CurrentUser.Id && p.IsCircleAdmin.Value == 1)).Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Title
                }).ToList());
                model.ExtraKnownData.Add(new SelectListItems
                {
                    Items = tempItems,
                    SelectedIds = sharedIds.ToArray()
                });
                ViewBag.UserId = CurrentUser.Id;
                return PartialView(model);
            }
            return PartialView(modelObject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditResource(FormModel<Resource, List<SelectListItems>> model, string[] groupIds)
        {
            int resourceId = model.FormObject.Id;
            try
            {
                var obj = ResourceDAL.Get(model.FormObject.Id);
                if (obj != null)
                {
                    var bpr = new BatchProcessResultModel();
                    obj.Body = model.FormObject.Body;
                    var drm = (DALReturnModel<Resource>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Resource),
                                       new Func<Resource, BatchProcessResultModel, DALReturnModel<Resource>>(ResourceDAL.Update), obj, bpr);
                    bpr = drm.BPR;
                    var shared = ObjectStreamManager.FindObjectWhichStream(resourceId, (int)UT.SL.Model.Enumeration.ObjectType.Resource);
                    var tobeMerged = new ToEmergeIds();

                    //if (groupIds != null && !groupIds.Any(x => x.StartsWith("D_")))
                    //{
                    //    ObjectStreamManager.ObjectResourceToStream(resourceId,
                    //                                               (int)UT.SL.Model.Enumeration.ObjectType.Resource,
                    //                                                new ToEmergeIds { LearningGroups = groupIds.ToList() });
                    //}
                    //else
                    //{
                    //    ObjectStreamManager.ObjectResourceToStream(resourceId, (int)UT.SL.Model.Enumeration.ObjectType.Resource, new ToEmergeIds());
                    //}                   
                }
            }
            catch
            {
                return Content("0");
            }
            return Content(resourceId.ToString());
        }

        public ActionResult FilterPosts()
        {
            var model = SocialGroupDAL.GetAllByUserId(CurrentUser.Id).ToList();
            return PartialView(model);
        }

    }
}
