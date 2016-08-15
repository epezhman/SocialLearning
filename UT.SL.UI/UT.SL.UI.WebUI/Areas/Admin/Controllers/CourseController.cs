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
using UT.SL.Model.Enumeration;


namespace UT.SL.UI.WebUI.Areas.Admin.Controllers
{
    [Authorize()]
    public class CourseController : BaseController
    {

        public ActionResult Index(CourseSearchModel model)
        {
            model.Area = "Admin";
            return View(model);
        }

        public ActionResult CourseSearchModelView(CourseSearchModel model)
        {
            model.Area = "Admin";
            return PartialView(model);
        }

        public PagedList<Course> SearchFilters(CourseSearchModel model)
        {
            model.Area = "Admin";
            var qry = CourseDAL.Find(model);
            model.Update(model.PageSize, qry.Count());
            var ls = qry.Skip(model.PageSize * model.PageIndex).Take(model.PageSize).ToList();
            var ql = new PagedList<Course>(ls, model);
            return ql;
        }

        public ActionResult IX(CourseSearchModel model)
        {
            model.Area = "Admin";
            return PartialView(SearchFilters(model));
        }

        public ActionResult CourseView(int Id, int? rank, string filter)
        {
            var model = CourseDAL.Get(Id);
            if (rank.HasValue)
                ViewBag.LinkRank = rank.Value;
            if (!String.IsNullOrEmpty(filter))
            {
                ViewBag.Filter = filter;
            }
            return View(model);
        }

        public ActionResult FilterPosts(Course obj)
        {
            var model = LearningGroupDAL.GetAllByUserId(CurrentUser.Id, obj.Id).ToList();
            return PartialView(model);
        }

        public ActionResult CoursePost(Course obj)
        {
            var model = new FormModel<Resource, List<SelectListItems>>();
            model.FormObject = new Resource
            {
                CourseId = obj.Id
            };
            model.ExtraKnownData = new List<SelectListItems>();
            var tempItems = new List<SelectListItem>();
            tempItems.AddRange(CourseDAL.Get(obj.Id).CourseAbstract.CourseTopcMappers.Select(x => new SelectListItem
            {
                Value = x.TopicId.ToString(),
                Text = x.Topic.Title
            }).ToList());
            model.ExtraKnownData.Add(new SelectListItems
            {
                Items = tempItems
            });
            tempItems = new List<SelectListItem>();
            tempItems.Add(new SelectListItem
            {
                Text = CourseDAL.Get(obj.Id).Title,
                Value = "D_" + obj.Id
            });
            tempItems.AddRange(LearningGroupDAL.GetAllByCourse(obj.Id).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Title
            }).ToList());
            model.ExtraKnownData.Add(new SelectListItems
            {
                Items = tempItems,
                SelectedIds = new string[] { "D_" + obj.Id }
            });
            ViewBag.UserId = CurrentUser.Id;
            return PartialView(model);
        }

        public ActionResult PostToCourse(Course obj)
        {
            var model = obj;
            ViewBag.UserId = CurrentUser.Id;
            return PartialView(model);
        }

        public ActionResult Create()
        {
            var formObject = new Course();
            var listItems = new SelectListItems
            {
                Items = CategoryDAL.GetAll().Select(x => new SelectListItem
                 {
                     Value = x.Id.ToString(),
                     Text = (x.ParentId == null ? x.Title : x.Title + "-" + x.Category1.Title)
                 })
            };
            var model = new FormModel<Course, SelectListItems>
            {
                FormObject = formObject,
                ExtraKnownData = listItems
            };
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        public ActionResult CreateWithAbstract(int Id)
        {
            var formObject = new Course { CourseAbstractId = Id };
            var listItems = new SelectListItems
            {
                Items = TopicDAL.GetAllSelectedTopics(Id).Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Title
                })
            };
            var courseAbstract = CourseAbstractDAL.Get(Id);
            var category = String.Format("{0} - {1}", courseAbstract.Category.Category1.Title, courseAbstract.Category.Title);
            var model = new FormModel<Course, SelectListItems>
            {
                FormObject = formObject,
                ExtraKnownData = listItems
            };
            model.ExtraData = new List<object>();
            model.ExtraData.Add(category);
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        [ValidateInput(false)]
        public ActionResult Create(FormModel<Course, SelectListItems> model)
        {
            var bpr = new BatchProcessResultModel();
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    var drm = ((DALReturnModel<Course>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Course),
                            new Func<Course, BatchProcessResultModel, DALReturnModel<Course>>(CourseDAL.Add), model.FormObject, bpr));
                    bpr = drm.BPR;
                    var courseId = drm.ReturnObject.Id;
                    if (bpr.Failed > 0)
                    {
                        return PartialView("ProcessResult", bpr);
                    }
                    else
                    {
                        if (model.FormObject.EndDate.HasValue && model.FormObject.StartDate.HasValue)
                        {
                            var days = (model.FormObject.EndDate.Value - model.FormObject.StartDate.Value).TotalDays;
                            var weekCounter = true;
                            var bDate = model.FormObject.StartDate.Value;
                            int rank = 0;
                            while (weekCounter)
                            {
                                rank++;
                                if (bDate.AddDays(7) < model.FormObject.EndDate.Value)
                                {
                                    var courseSchedule = new CourseSchedule
                                    {
                                        BeginDate = bDate,
                                        EndDate = bDate.AddDays(7),
                                        CourseId = courseId,
                                        Rank = rank
                                    };
                                    bpr = new BatchProcessResultModel();
                                    var drm2 = ((DALReturnModel<CourseSchedule>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.CourseSchedule),
                                            new Func<CourseSchedule, BatchProcessResultModel, DALReturnModel<CourseSchedule>>(CourseScheduleDAL.Add), courseSchedule, bpr));
                                    bpr = drm2.BPR;
                                    if (bpr.Failed > 0)
                                    {
                                        return PartialView("ProcessResult", bpr);
                                    }
                                }
                                else
                                {
                                    var courseSchedule = new CourseSchedule
                                    {
                                        BeginDate = bDate,
                                        EndDate = model.FormObject.EndDate.Value,
                                        CourseId = courseId,
                                        Rank = rank
                                    };
                                    bpr = new BatchProcessResultModel();
                                    var drm2 = ((DALReturnModel<CourseSchedule>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.CourseSchedule),
                                            new Func<CourseSchedule, BatchProcessResultModel, DALReturnModel<CourseSchedule>>(CourseScheduleDAL.Add), courseSchedule, bpr));
                                    bpr = drm2.BPR;
                                    if (bpr.Failed > 0)
                                    {
                                        return PartialView("ProcessResult", bpr);
                                    }
                                    weekCounter = false;
                                }
                                bDate = bDate.AddDays(7);
                            }
                        }
                        if (model.ExtraKnownData != null && model.ExtraKnownData.SelectedIds != null)
                            foreach (var item in model.ExtraKnownData.SelectedIds)
                            {
                                var ccm = new CategoryMapper
                                {
                                    CategoryId = Int32.Parse(item),
                                    CourseId = courseId
                                };
                                bpr = new BatchProcessResultModel();
                                var drm2 = ((DALReturnModel<CategoryMapper>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.CategoryMapper),
                                            new Func<CategoryMapper, BatchProcessResultModel, DALReturnModel<CategoryMapper>>(CategoryMapperDAL.Add), ccm, bpr));
                                bpr = drm2.BPR;
                                if (bpr.Failed > 0)
                                {
                                    return PartialView("ProcessResult", bpr);
                                }
                            }
                        //bpr.SuccessClientScript = "$('#searchForm').submit();";
                        bpr.SuccessClientScript = "window.location.replace('" + Url.Action("CourseView", "Course", new { area = "Admin", id = courseId }) + "');";
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

        [AllowAnonymous]
        public ActionResult ViewImage(int Id)
        {
            var model = CourseDAL.Get(Id);
            return File(model.ImageData.ToArray(), model.ImageMIME);
        }

        public ActionResult DeleteImage(int Id)
        {
            var model = CourseDAL.DeletePic(new Course { Id = Id });
            return Content("true");
        }

        public ActionResult SaveImage(int Id, HttpPostedFileBase image)
        {
            var bpr = new BatchProcessResultModel();
            var drm = ((DALReturnModel<Course>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Course),
                           new Func<int, HttpPostedFileBase, BatchProcessResultModel, DALReturnModel<Course>>(CourseDAL.SaveImage), Id, image, bpr));
            return Content(drm.ReturnObject.Id.ToString());
        }

        public ActionResult Edit(int Id)
        {
            var formObject = CourseDAL.Get(Id);
            var listItems = new SelectListItems
            {
                SelectedIds = formObject.CategoryMappers.Select(x => x.CategoryId.ToString()).ToArray(),
                Items = CategoryDAL.GetAll().Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = (x.ParentId == null ? x.Title : x.Title + "-" + x.Category1.Title)
                })
            };
            var model = new FormModel<Course, SelectListItems>
            {
                FormObject = formObject,
                ExtraKnownData = listItems
            };
            return PartialView(model);
        }

        public ActionResult EditCourseMadeOfAbstract(int Id)
        {
            var formObject = CourseDAL.Get(Id);
            if (formObject.CourseAbstractId.HasValue)
            {
                var listItems = new SelectListItems
                {
                    Items = TopicDAL.GetAllSelectedTopics(formObject.CourseAbstractId.Value).Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Title
                    })
                };
                var courseAbstract = CourseAbstractDAL.Get(formObject.CourseAbstractId.Value);
                var category = String.Format("{0} - {1}", courseAbstract.Category.Category1.Title, courseAbstract.Category.Title);
                var model = new FormModel<Course, SelectListItems>
                {
                    FormObject = formObject,
                    ExtraKnownData = listItems
                };
                model.ExtraData = new List<object>();
                model.ExtraData.Add(category);
                return PartialView(model);
            }
            var model2 = new FormModel<Course, SelectListItems>
            {
                FormObject = formObject,
                ExtraKnownData = new SelectListItems()
            };
            model2.ExtraData = new List<object>();
            model2.ExtraData.Add(String.Empty);
            return PartialView(model2);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        [ValidateInput(false)]
        public ActionResult Edit(FormModel<Course, SelectListItems> model)
        {
            var bpr = new BatchProcessResultModel();
            if (ModelState.IsValid)
            {
                try
                {
                    var drm = ((DALReturnModel<Course>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Course),
                            new Func<Course, BatchProcessResultModel, DALReturnModel<Course>>(CourseDAL.Update), model.FormObject, bpr));
                    bpr = drm.BPR;
                    if (bpr.Failed > 0)
                    {
                        return PartialView("ProcessResult", bpr);
                    }
                    else
                    {
                        ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.CourseSchedule),
                                       new Func<int, DALReturnModel<CourseSchedule>>(CourseScheduleDAL.DeleteAll), model.FormObject.Id);
                        if (model.FormObject.EndDate.HasValue && model.FormObject.StartDate.HasValue)
                        {
                            var days = (model.FormObject.EndDate.Value - model.FormObject.StartDate.Value).TotalDays;
                            var weekCounter = true;
                            var bDate = model.FormObject.StartDate.Value;
                            int rank = 0;
                            while (weekCounter)
                            {
                                rank++;
                                if (bDate.AddDays(7) < model.FormObject.EndDate.Value)
                                {
                                    var courseSchedule = new CourseSchedule
                                    {
                                        BeginDate = bDate,
                                        EndDate = bDate.AddDays(7),
                                        CourseId = model.FormObject.Id,
                                        Rank = rank
                                    };
                                    bpr = new BatchProcessResultModel();
                                    var drm2 = ((DALReturnModel<CourseSchedule>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.CourseSchedule),
                                            new Func<CourseSchedule, BatchProcessResultModel, DALReturnModel<CourseSchedule>>(CourseScheduleDAL.Add), courseSchedule, bpr));
                                    bpr = drm2.BPR;
                                    if (bpr.Failed > 0)
                                    {
                                        return PartialView("ProcessResult", bpr);
                                    }
                                }
                                else
                                {
                                    var courseSchedule = new CourseSchedule
                                    {
                                        BeginDate = bDate,
                                        EndDate = model.FormObject.EndDate.Value,
                                        CourseId = model.FormObject.Id,
                                        Rank = rank
                                    };
                                    bpr = new BatchProcessResultModel();
                                    var drm2 = ((DALReturnModel<CourseSchedule>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.CourseSchedule),
                                            new Func<CourseSchedule, BatchProcessResultModel, DALReturnModel<CourseSchedule>>(CourseScheduleDAL.Add), courseSchedule, bpr));
                                    bpr = drm2.BPR;
                                    if (bpr.Failed > 0)
                                    {
                                        return PartialView("ProcessResult", bpr);
                                    }
                                    weekCounter = false;
                                }
                                bDate = bDate.AddDays(7);
                            }

                        }
                        //CategoryMapperDAL.DeleteAll(model.FormObject.Id);
                        //foreach (var item in model.ExtraKnownData.SelectedIds)
                        //{
                        //    var ccm = new CategoryMapper
                        //    {
                        //        CategoryId = Int32.Parse(item),
                        //        CourseId = model.FormObject.Id
                        //    };
                        //    bpr = new BatchProcessResultModel();
                        //    CategoryMapperDAL.Add(ccm, out bpr);
                        //    if (bpr.Failed > 0)
                        //    {
                        //        return PartialView("ProcessResult", bpr);
                        //    }
                        //}
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

        public ActionResult Delete(int Id)
        {
            var formObject = CourseDAL.Get(Id);
            var listItems = new SelectListItems
            {
                Items = formObject.CategoryMappers.Select(x => new SelectListItem
                {
                    Value = x.Category.Id.ToString(),
                    Text = x.Category.Title
                })
            };
            var model = new FormModel<Course, SelectListItems>
            {
                FormObject = formObject,
                ExtraKnownData = listItems
            };
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(FormModel<Course, SelectListItems> model)
        {
            var bpr = new BatchProcessResultModel();
            try
            {
                ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.CourseSchedule),
                                      new Func<int, DALReturnModel<CourseSchedule>>(CourseScheduleDAL.DeleteAll), model.FormObject.Id);
                var drm = (DALReturnModel<Course>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Course),
                                            new Func<Course, DALReturnModel<Course>>(CourseDAL.Delete), model.FormObject);

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

        public ActionResult GetCourseTitle(System.Nullable<int> Id)
        {
            if (Id.HasValue)
            {
                try
                {
                    var model = CourseDAL.Get(Id.Value).Title;
                    return Content(model);
                }
                catch (System.Exception)
                {
                    Content("");
                }
            }
            return Content("");
        }

        public ActionResult GetCourseAbout(System.Nullable<int> Id)
        {
            if (Id.HasValue)
            {
                try
                {
                    var model = CourseDAL.Get(Id.Value).About;
                    return Content(model);
                }
                catch (System.Exception)
                {
                    Content("");
                }
            }
            return Content("");
        }

        public ActionResult EditPage(int Id)
        {
            var model = CourseDAL.Get(Id);
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        public ActionResult EditCourseDetails(int Id, int? rank)
        {
            var model = CourseDAL.Get(Id);
            if (rank.HasValue)
                ViewBag.LinkRank = rank.Value;
            return View(model);
        }

        public ActionResult View(int Id)
        {
            var formObject = CourseDAL.Get(Id);
            var listItems = new SelectListItems
            {
                Items = formObject.CategoryMappers.Select(x => new SelectListItem
                {
                    Value = x.Category.Id.ToString(),
                    Text = x.Category.Title
                })
            };
            var model = new FormModel<Course, SelectListItems>
            {
                FormObject = formObject,
                ExtraKnownData = listItems
            };
            return PartialView(model);
        }

        public ActionResult ViewCourseMadeOfAbstract(int Id)
        {
            var formObject = CourseDAL.Get(Id);
            if (formObject.CourseAbstractId.HasValue)
            {
                var listItems = new SelectListItems
                {
                    Items = TopicDAL.GetAllSelectedTopics(formObject.CourseAbstractId.Value).Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Title
                    })
                };
                var courseAbstract = CourseAbstractDAL.Get(formObject.CourseAbstractId.Value);
                var category = String.Format("{0} - {1}", courseAbstract.Category.Category1.Title, courseAbstract.Category.Title);
                var model = new FormModel<Course, SelectListItems>
                {
                    FormObject = formObject,
                    ExtraKnownData = listItems
                };
                model.ExtraData = new List<object>();
                model.ExtraData.Add(category);
                return PartialView(model);
            }
            var model2 = new FormModel<Course, SelectListItems>
            {
                FormObject = formObject,
                ExtraKnownData = new SelectListItems()
            };
            model2.ExtraData = new List<object>();
            model2.ExtraData.Add(String.Empty);
            return PartialView(model2);
        }

        public ActionResult EditDetails(int Id)
        {
            var model = CourseDAL.Get(Id).CourseSchedules.ToList();
            return PartialView(model);
        }

        public ActionResult EditSummary(int Id)
        {
            var model = CourseScheduleDAL.Get(Id);
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        [ValidateInput(false)]
        public ActionResult EditSummary(CourseSchedule model)
        {
            var bpr = new BatchProcessResultModel();
            ModelState.Remove("CourseId");
            ModelState.Remove("BeginDate");
            ModelState.Remove("EndDate");
            ModelState.Remove("Rank");
            if (ModelState.IsValid)
            {
                try
                {
                    var drm = (DALReturnModel<CourseSchedule>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.CourseSchedule),
                                       new Func<CourseSchedule, BatchProcessResultModel, DALReturnModel<CourseSchedule>>(CourseScheduleDAL.Update), model);
                    bpr = drm.BPR;
                    if (bpr.Failed > 0)
                    {
                        return PartialView("ProcessResult", bpr);
                    }
                    else
                    {
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

        public ActionResult AddResource()
        {
            return PartialView();
        }

        public ActionResult Members(int id)
        {
            var members = App_UserEnrolementDAL.GetAllByCourse(id).OrderByDescending(x => x.Type).ThenBy(x => x.App_User.LastName).ThenBy(x => x.App_User.LastName).ToList();
            ViewBag.CourseId = id;
            return PartialView(members);
        }

        public ActionResult LearrningGroups(int id)
        {
            var groups = LearningGroupDAL.GetAllByCourse(id).OrderBy(x => x.Title).ToList();
            ViewBag.CourseId = id;
            return PartialView(groups);
        }

        public ActionResult LearrningGroupsUserCount(int id)
        {
            var groupsMembers = LearningGroupDAL.GetCount(id);
            return Content(groupsMembers.ToString());
        }

        public ActionResult SaveResource(HttpPostedFileBase resourceFile, int id, int? overwite)
        {
            int fileId = 0;
            try
            {
                if (resourceFile != null)
                {
                    if (overwite.HasValue)
                    {
                        var bpr = new BatchProcessResultModel();
                        var resource = ResourceDAL.Get(id);
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
                            CourseId = id
                        };

                        byte[] tempFile = null;
                        tempFile = new byte[resourceFile.ContentLength];
                        resourceFile.InputStream.Read(tempFile, 0, resourceFile.ContentLength);
                        newFile.FileContent = tempFile;

                        var bpr = new BatchProcessResultModel();
                        var drm = ((DALReturnModel<Resource>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Resource),
                            new Func<Resource, BatchProcessResultModel, DALReturnModel<Resource>>(ResourceDAL.Add), newFile, bpr));
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
        public ActionResult PostResource(FormModel<Resource, List<SelectListItems>> model, string[] topicIds, string[] groupIds)
        {
            int resourceId = 0;
            try
            {
                var bpr = new BatchProcessResultModel();
                if (model.FormObject.Id > 0)
                {
                    var obj = ResourceDAL.Get(model.FormObject.Id);
                    if (obj != null)
                    {
                        obj.Title = model.FormObject.Title.StringNormalizer();
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
                                Title = model.FormObject.Title.StringNormalizer(),
                                CourseId = model.FormObject.CourseId,
                                CreateDate = DateTime.Now,
                                CreateUserId = CurrentUser.Id,
                                GuidId = Guid.NewGuid(),
                                IsPublishd = true,
                                IsValid = true
                            };
                            var drm = (DALReturnModel<Resource>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Resource),
                                      new Func<Resource, BatchProcessResultModel, DALReturnModel<Resource>>(ResourceDAL.Add), obj, bpr);
                            bpr = drm.BPR;
                            resourceId = drm.ReturnObject.Id;
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
                             Title = model.FormObject.Title.StringNormalizer(),
                             CourseId = model.FormObject.CourseId,
                             CreateDate = DateTime.Now,
                             CreateUserId = CurrentUser.Id,
                             GuidId = Guid.NewGuid(),
                             IsPublishd = true,
                             IsValid = true
                         };
                        var drm = (DALReturnModel<Resource>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Resource),
                                      new Func<Resource, BatchProcessResultModel, DALReturnModel<Resource>>(ResourceDAL.Add), obj, bpr);
                        bpr = drm.BPR;
                        resourceId = drm.ReturnObject.Id;

                    }
                }

                if (resourceId > 0)
                {
                    if (groupIds != null && !groupIds.Any(x => x.StartsWith("D_")))
                        ObjectStreamManager.ObjectResourceToStream(resourceId, (int)UT.SL.Model.Enumeration.ObjectType.Resource, new ToEmergeIds { LearningGroups = groupIds.ToList() }, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
                    else
                        ObjectStreamManager.ObjectResourceToStream(resourceId, (int)UT.SL.Model.Enumeration.ObjectType.Resource, new ToEmergeIds(), ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
                    if (topicIds != null)
                    {
                        foreach (var item in topicIds)
                        {
                            var topicMapper = new ObjectTopicMapper
                            {
                                ObjectId = resourceId,
                                ObjectType = (int)UT.SL.Model.Enumeration.ObjectType.Resource,
                                TopicId = Int32.Parse(item)
                            };
                            var drm = (DALReturnModel<ObjectTopicMapper>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectTopicMapper),
                                     new Func<ObjectTopicMapper, BatchProcessResultModel, DALReturnModel<ObjectTopicMapper>>(ObjectTopicMapperDAL.Add), topicMapper, bpr);
                            bpr = drm.BPR;
                        }
                    }

                    //updaet credit 
                    ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.Create, resourceId, (int)UT.SL.Model.Enumeration.ObjectType.Resource, CurrentUser.Id);
                    UserInfoManager.UpdateUserActSummary(CurrentUser.Id, resourceId, (int)UT.SL.Model.Enumeration.ObjectType.Resource, (int)UT.SL.Model.Enumeration.ActivityType.Create);
                    //updaet credit
                }

            }
            catch
            {
                return Content("0");
            }
            return Content(resourceId.ToString());
        }

        //[HttpPost]
        //public ActionResult EditResource(FormModel<Resource, List<SelectListItems>> model, string[] topicIds, string[] groupIds)
        //{
        //    int resourceId = model.FormObject.Id;
        //    try
        //    {
        //        var obj = ResourceDAL.Get(model.FormObject.Id);
        //        if (obj != null)
        //        {
        //            var bpr = new BatchProcessResultModel();
        //            obj.Body = model.FormObject.Body;
        //            ResourceDAL.Update(obj, out bpr);
        //            if (groupIds != null && !groupIds.Any(x => x.StartsWith("D_")))
        //                ObjectStreamManager.ObjectResourceToStream(resourceId, (int)UT.SL.Model.Enumeration.ObjectType.Resource, new ToEmergeIds { LearningGroups = groupIds.ToList() });
        //            else
        //                ObjectStreamManager.ObjectResourceToStream(resourceId, (int)UT.SL.Model.Enumeration.ObjectType.Resource, new ToEmergeIds());
        //            ObjectTopicMapperDAL.DeleteObjectTopics(resourceId, (int)UT.SL.Model.Enumeration.ObjectType.Resource);
        //            if (topicIds != null)
        //            {
        //                foreach (var item in topicIds)
        //                {
        //                    var topicMapper = new ObjectTopicMapper
        //                    {
        //                        ObjectId = resourceId,
        //                        ObjectType = (int)UT.SL.Model.Enumeration.ObjectType.Resource,
        //                        TopicId = Int32.Parse(item)
        //                    };
        //                    ObjectTopicMapperDAL.Add(topicMapper, out bpr);
        //                }
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return Content("0");
        //    }
        //    return Content(resourceId.ToString());
        //}

        public ActionResult GetCourseResources(int Id, int? page, DateTime? date, string resourceSelections)
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
                        return PartialView("ResourcesCompact", ResourceFinder.GetCourseResources(Id, page, date, CurrentUser.Id, resourceSelections, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream)));
                    }
                }
                return PartialView("Resources", ResourceFinder.GetCourseResources(Id, page, date, CurrentUser.Id, resourceSelections, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream)));
            }
            else
            {
                return PartialView("Resources", ResourceFinder.GetCourseResources(Id, page, date, CurrentUser.Id, resourceSelections, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream)));
            }

        }

        public ActionResult GetNewCourseResources(int Id, DateTime date, string resourceSelections)
        {
            var resources = ResourceFinder.GetNewCourseResources(Id, date, CurrentUser.Id, resourceSelections, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
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

        public ActionResult GetOneResource(int Id, int type)
        {
            var models = new List<ObjectViewModelList>();
            var tempObjectViewModelList = new ObjectViewModelList();
            var item = ManageObject.GetSharedObject(Id, type);
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

        public ActionResult GetOneObject(int Id, int type)
        {
            var model = ManageObject.GetSharedObject(Id, type);
            return PartialView("ObjectViewerForPreview", model);
        }

        public ActionResult GetOneReadenObject(int Id, int type)
        {
            var model = ManageObject.GetSharedObject(Id, type);
            model.IsReaden = false;
            var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream)
                , new Func<int, int, int, DALReturnModel<ObjectStream>>(ObjectStreamDAL.UpdateClickedDate), Id, type, CurrentUser.Id);
            if (drm.ReturnObject != null && drm.ReturnObject.Id > 0 && drm.ReturnObject.LastClcikedDate.HasValue)
            {
                model.ClickedDate = drm.ReturnObject.LastClcikedDate;
            }
            return PartialView("ObjectViewer", model);
        }

        public ActionResult GetOneObjectTry(int Id, int type)
        {
            var model = ManageObject.GetSharedObject(Id, type);
            var drm = (DALReturnModel<ObjectStream>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream)
                , new Func<int, int, int, DALReturnModel<ObjectStream>>(ObjectStreamDAL.UpdateClickedDate), Id, type, CurrentUser.Id);
            if (type == (int)ObjectType.Resource || type == (int)ObjectType.Forum || type == (int)ObjectType.Assignment)
                return PartialView("ObjectViewerForPreview", model);
            if (model.CameFromType > 0 && model.CameFromId > 0)
            {
                model = ManageObject.GetSharedObject(model.CameFromId, model.CameFromType);
                if (model.Id > 0 && (model.Type == (int)ObjectType.Resource || model.Type == (int)ObjectType.Forum || model.Type == (int)ObjectType.Assignment))
                    return PartialView("ObjectViewerForPreview", model);
            }
            model = new ObjectViewModel();
            return PartialView("ObjectViewerForPreview", model);
        }

        public ActionResult DownloadResource(int Id)
        {
            var model = ResourceDAL.Get(Id);
            return File(model.FileContent.ToArray(), model.FileMime, model.FileTitle);
        }

        public ActionResult ViewImageResource(int Id)
        {
            var model = ResourceDAL.Get(Id);
            return File(model.FileContent.ToArray(), model.FileMime);
        }

        [HttpPost]
        public ActionResult DeleteObject(int ObjectId, int ObjecType)
        {
            ManageObject.DeleteObject(ObjectId, ObjecType, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
            return Content("true");
        }

        [HttpPost]
        public ActionResult DeleteFile(int id)
        {
            ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Resource),
                                      new Func<Resource, DALReturnModel<Resource>>(ResourceDAL.DeleteFile), new Resource { Id = id });
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
                tempItems.AddRange(CourseDAL.Get(model.FormObject.CourseId.Value).CourseAbstract.CourseTopcMappers.Select(x => new SelectListItem
                {
                    Value = x.TopicId.ToString(),
                    Text = x.Topic.Title
                }).ToList());
                model.ExtraKnownData.Add(new SelectListItems
                {
                    Items = tempItems,
                    SelectedIds = ObjectTopicMapperDAL.GetAll(ObjectId, ObjecType).Select(x => x.Id.ToString()).ToArray()
                });
                tempItems = new List<SelectListItem>();
                tempItems.Add(new SelectListItem
                {
                    Text = CourseDAL.Get(model.FormObject.CourseId.Value).Title,
                    Value = "D_" + model.FormObject.CourseId.Value.ToString()
                });
                var shared = ObjectStreamManager.FindObjectWhichStream(ObjectId, ObjecType);
                var sharedIds = new List<string>();
                foreach (var item in shared.CourseIds)
                {
                    sharedIds.Add("D_" + item);
                }
                foreach (var item in shared.LearningGroups)
                {
                    sharedIds.Add(item.ToString());
                }
                tempItems.AddRange(LearningGroupDAL.GetAllByCourse(model.FormObject.CourseId.Value).Select(x => new SelectListItem
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
        public ActionResult EditResource(FormModel<Resource, List<SelectListItems>> model, string[] topicIds, string[] groupIds)
        {
            int resourceId = model.FormObject.Id;
            try
            {
                var obj = ResourceDAL.Get(model.FormObject.Id);
                if (obj != null)
                {
                    var bpr = new BatchProcessResultModel();
                    obj.Body = model.FormObject.Body.StringNormalizer();
                    obj.Title = model.FormObject.Title.StringNormalizer();

                    var drm = (DALReturnModel<Resource>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Resource),
                            new Func<Resource, BatchProcessResultModel, DALReturnModel<Resource>>(ResourceDAL.Update), obj, bpr);
                    bpr = drm.BPR;
                    ObjectStreamManager.UpdateEditedObjectStream(resourceId, (int)UT.SL.Model.Enumeration.ObjectType.Resource, CurrentUser.Id);
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
                    ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectTopicMapper),
                                       new Func<int, int, DALReturnModel<ObjectTopicMapper>>(ObjectTopicMapperDAL.DeleteObjectTopics), resourceId, (int)ObjectType.Resource);

                    if (topicIds != null)
                    {
                        foreach (var item in topicIds)
                        {
                            var topicMapper = new ObjectTopicMapper
                            {
                                ObjectId = resourceId,
                                ObjectType = (int)UT.SL.Model.Enumeration.ObjectType.Resource,
                                TopicId = Int32.Parse(item)
                            };
                            var drm2 = (DALReturnModel<ObjectTopicMapper>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectTopicMapper),
                                         new Func<ObjectTopicMapper, BatchProcessResultModel, DALReturnModel<ObjectTopicMapper>>(ObjectTopicMapperDAL.Add), topicMapper, bpr);
                            bpr = drm2.BPR;
                        }
                    }
                }
            }
            catch
            {
                return Content("0");
            }
            return Content(resourceId.ToString());
        }

        #region Assignment

        public ActionResult AssignmentPost(Course obj)
        {
            var model = new Assignment
            {
                CourseId = obj.Id
            };
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AssignmentPost(Assignment model)
        {

            var bpr = new BatchProcessResultModel();
            if (ModelState.IsValid)
            {
                model.CreateUserId = CurrentUser.Id;
                var drm = (DALReturnModel<Assignment>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Assignment),
                            new Func<Assignment, BatchProcessResultModel, DALReturnModel<Assignment>>(AssignmentDAL.Add), model, bpr);
                bpr = drm.BPR;
                var insertedObject = drm.ReturnObject;
                if (insertedObject != null && bpr.Failed == 0)
                {
                    ObjectStreamManager.ObjectResourceToStream(insertedObject.Id, (int)UT.SL.Model.Enumeration.ObjectType.Assignment, new ToEmergeIds(), ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
                    return Content(insertedObject.ToString());
                }

            }
            return Content("0");
        }

        #endregion

        #region Panel

        public ActionResult ManageCourseMembersPanel(Course course)
        {
            var members = App_UserEnrolementDAL.GetAllByCourse(course.Id).OrderByDescending(x => x.Type).ThenBy(x => x.App_User.LastName).ThenBy(x => x.App_User.LastName).ToList();
            ViewBag.CourseId = course.Id;
            return PartialView(members);
        }

        public ActionResult GetTopMembersForPanel(int Id, int memberType)
        {
            var model = App_UserEnrolementDAL.GetAllByCourse(Id).Where(x => x.Type == memberType).ToList();
            var role = App_UserEnrolementDAL.GetAllByCourseAndUser(Id, CurrentUser.Id);
            if (role.Any(x => x.Type == (int)UT.SL.Model.Enumeration.MemebrshipType.Teacher))
                ViewBag.Teacher = true;
            else
                ViewBag.Teacher = false;
            return PartialView(model);
        }

        public ActionResult GetMembersForManageForPanel(int Id, int memberType)
        {
            var model = App_UserEnrolementDAL.GetAllByCourse(Id).Where(x => x.Type == memberType).ToList();
            var role = App_UserEnrolementDAL.GetAllByCourseAndUser(Id, CurrentUser.Id);
            if (role.Any(x => x.Type == (int)UT.SL.Model.Enumeration.MemebrshipType.Teacher))
                ViewBag.Teacher = true;
            else
                ViewBag.Teacher = false;
            if (role.Any(x => x.Type == (int)UT.SL.Model.Enumeration.MemebrshipType.TA))
                ViewBag.TA = true;
            else
                ViewBag.TA = false;
            return PartialView(model);
        }

        public ActionResult GetMembersForPanelCount(int Id, int memberType)
        {
            var model = App_UserEnrolementDAL.GetAllByCourse(Id).Where(x => x.Type == memberType).ToList();
            return Content(model.Count().ToString());
        }

        public ActionResult EditForPanel(int Id, int memberType)
        {
            ViewBag.CourseId = Id;
            ViewBag.MemberType = memberType;
            var model = App_UserEnrolementDAL.GetAllByCourse(Id).Where(x => x.Type == memberType).ToList();
            return PartialView(model);
        }

        public PagedList<Resource> SearchResourceFilters(ResourceSearchModel model)
        {
            model.Area = "Admin";
            var qry = ResourceDAL.Find(model);
            model.Update(5, qry.Count());
            var ls = qry.Skip(model.PageSize * model.PageIndex).Take(model.PageSize).ToList();
            var ql = new PagedList<Resource>(ls, model);
            return ql;
        }

        public ActionResult CourseResourceSummary(ResourceSearchModel model)
        {
            model.Area = "Admin";
            if (model.CourseId.HasValue)
                ViewBag.Title = CourseDAL.Get(model.CourseId.Value).Title;
            return PartialView(SearchResourceFilters(model));
        }

        public PagedList<Forum> SearchForumFilters(ForumsearchModel model)
        {
            model.Area = "Admin";
            var qry = ForumDAL.Find(model);
            model.Update(5, qry.Count());
            var ls = qry.Skip(model.PageSize * model.PageIndex).Take(model.PageSize).ToList();
            var ql = new PagedList<Forum>(ls, model);
            return ql;
        }

        public ActionResult CourseForumSummary(ForumsearchModel model)
        {
            model.Area = "Admin";
            if (model.CourseId.HasValue)
                ViewBag.Title = CourseDAL.Get(model.CourseId.Value).Title;
            return PartialView(SearchForumFilters(model));
        }

        public PagedList<Assignment> SearchAssignmentFilters(AssignmentSearchModel model)
        {
            model.Area = "Admin";
            var qry = AssignmentDAL.Find(model);
            model.Update(5, qry.Count());
            var ls = qry.Skip(model.PageSize * model.PageIndex).Take(model.PageSize).ToList();
            var ql = new PagedList<Assignment>(ls, model);
            return ql;
        }

        public ActionResult CourseAssignmentSummary(AssignmentSearchModel model)
        {
            model.Area = "Admin";
            if (model.CourseId.HasValue)
                ViewBag.Title = CourseDAL.Get(model.CourseId.Value).Title;
            return PartialView(SearchAssignmentFilters(model));
        }

        public ActionResult CourseAllSummary(Course course)
        {

            var formObject = CourseDAL.Get(course.Id);
            if (formObject.CourseAbstractId.HasValue)
            {
                var listItems = new SelectListItems
                {
                    Items = TopicDAL.GetAllSelectedTopics(formObject.CourseAbstractId.Value).Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Title
                    })
                };
                var courseAbstract = CourseAbstractDAL.Get(formObject.CourseAbstractId.Value);
                var category = String.Format("{0} - {1}", courseAbstract.Category.Category1.Title, courseAbstract.Category.Title);
                var model = new FormModel<Course, SelectListItems>
                {
                    FormObject = formObject,
                    ExtraKnownData = listItems
                };
                model.ExtraData = new List<object>();
                model.ExtraData.Add(category);
                var courseSummary = new CourseSummaryModel();
                var enrols = CourseDAL.Get(course.Id).App_UserEnrolements;
                courseSummary.ParticipantCount = enrols.Count();
                courseSummary.ResourceCount = CourseDAL.GetAllResources(course.Id).Count();
                courseSummary.AssignmentCount = CourseDAL.GetAssigmentCount(course.Id);
                courseSummary.ForumCount = CourseDAL.GetForumCount(course.Id);
                courseSummary.LearningGroupsCount = CourseDAL.GetLearningGroupsCount(course.Id);
                courseSummary.ParticipantIsd = enrols.Select(x => x.UserId).ToList();
                model.ExtraData.Add(courseSummary);
                return PartialView(model);
            }
            var model2 = new FormModel<Course, SelectListItems>
            {
                FormObject = formObject,
                ExtraKnownData = new SelectListItems()
            };
            model2.ExtraData = new List<object>();
            model2.ExtraData.Add(String.Empty);
            return PartialView(model2);
        }

        public ActionResult CourseActivitiesSummary(Course course)
        {
            var model = new FormModel<Course, CourseActivitiesSummaryModel>();
            model.FormObject = CourseDAL.Get(course.Id);
            model.ExtraKnownData = new CourseActivitiesSummaryModel();
            model.ExtraKnownData.Assignments = CourseDAL.Get(course.Id).Assignments.ToList();
            model.ExtraKnownData.Quizes = CourseDAL.Get(course.Id).Quizs.ToList();
            model.ExtraKnownData.Forumes = CourseDAL.Get(course.Id).Forums.ToList();
            return PartialView(model);
        }

        public PagedList<ObjectViewModel> SearchActivitesFilters(CourseSearchModel model)
        {
            model.Area = "Admin";
            var qry = CourseDAL.FindActivities(model);
            model.Update(5, qry.Count());
            var ls = qry.Skip(model.PageSize * model.PageIndex).Take(model.PageSize).ToList();
            var ql = new PagedList<ObjectViewModel>(ls, model);
            return ql;
        }

        public ActionResult CourseActivitiesSummaryTable(CourseSearchModel model)
        {
            model.Area = "Admin";
            if (model.CourseId.HasValue)
                ViewBag.Title = CourseDAL.Get(model.CourseId.Value).Title;
            return PartialView(SearchActivitesFilters(model));
        }

        public PagedList<ObjectViewModel> SearchHotFilters(CourseSearchModel model)
        {
            //در این تکه کد نوشته ام که برای درس تستی هات را نشان ندهد
            if ((model.CourseId == 39 || model.CourseId == 41) && !CurrentUser.IsAdmin)
            {
                ViewBag.WillBeActivated = true;
            }
            //تا اینجا

            model.Area = "Admin";
            var qry = ModelManager.UserContentInterests(model, CurrentUser.Id);
            model.Update(5, qry.Count());
            var ls = qry.Skip(model.PageSize * model.PageIndex).Take(model.PageSize).ToList();
            var ql = new PagedList<ObjectViewModel>(ls, model);
            return ql;
            
        }

        public ActionResult CourseHotSummaryTable(CourseSearchModel model)
        {
            model.Area = "Admin";
                if (model.CourseId.HasValue)
                    ViewBag.Title = CourseDAL.Get(model.CourseId.Value).Title;
                var role = App_UserEnrolementDAL.GetAllByCourseAndUser(model.CourseId.Value, CurrentUser.Id);
                if (role.Any(x => x.Type == (int)UT.SL.Model.Enumeration.MemebrshipType.Teacher))
                    ViewBag.Teacher = true;
                else
                    ViewBag.Teacher = false;
                return PartialView(SearchHotFilters(model));
            }

        public PagedList<ObjectViewModel> SearchRecommendedFilters(CourseSearchModel model)
        {
            //در این تکه کد نوشته ام که برای درس تستی ریکامندد را نشان ندهد
            if ((model.CourseId == 39 || model.CourseId == 41) && !CurrentUser.IsAdmin)
            {
                ViewBag.WillBeActivated = true;
            }
            //تا اینجا

            model.Area = "Admin";
            var qry = ModelManager.UserContentKnowledges(model, CurrentUser.Id);
            model.Update(5, qry.Count());
            var ls = qry.Skip(model.PageSize * model.PageIndex).Take(model.PageSize).ToList();
            var ql = new PagedList<ObjectViewModel>(ls, model);
            return ql;
        }

        public ActionResult CourseRecommendedSummaryTable(CourseSearchModel model)
        {
            model.Area = "Admin";
            if (model.CourseId.HasValue)
                ViewBag.Title = CourseDAL.Get(model.CourseId.Value).Title;
            var role = App_UserEnrolementDAL.GetAllByCourseAndUser(model.CourseId.Value, CurrentUser.Id);
            if (role.Any(x => x.Type == (int)UT.SL.Model.Enumeration.MemebrshipType.Teacher))
                ViewBag.Teacher = true;
            else
                ViewBag.Teacher = false;
            return PartialView(SearchRecommendedFilters(model));
        }

        public ActionResult CourseRecommendedSummary(Course course)
        {
            var model = new FormModel<Course, ContentKnowledgeModel>();
            model.FormObject = CourseDAL.Get(course.Id);
            model.ExtraKnownData = new ContentKnowledgeModel();
            model.ExtraKnownData.ContentKnowledgeModels = ModelManager.UserContentKnowledges(CurrentUser.Id, course.Id);
            return PartialView(model);
        }

        public ActionResult CourseHotSummary(Course course)
        {
            var model = new FormModel<Course, ContentInterestModel>();
            model.FormObject = CourseDAL.Get(course.Id);
            model.ExtraKnownData = new ContentInterestModel();
            model.ExtraKnownData.ContentInterestModels = ModelManager.UserContentInterests(CurrentUser.Id, course.Id);
            return PartialView(model);
        }

        public ActionResult GetPanel(Course course, string filter)
        {
            ViewBag.Filter = filter;
            return PartialView(course);
        }

        public ActionResult LeadersPanel(Course course, string filter)
        {
            //برای تمام کلاسها به جز کلاس ب باکس پیشتازان فعال است
            if (course.Id == 41 && !CurrentUser.IsAdmin) 
                return null;
            //تا اینجا

            ViewBag.Filter = filter;
            return PartialView(course);
        }

        public ActionResult Leaders(Course course, string filter, byte? timePeriod)
        {
            var model = new List<LeadersModel>();
            var beginDate = DateTime.Now.GetFirstDateOfWeek(DayOfWeek.Saturday).Date;
            var endDate = DateTime.Now.GetLastDateOfWeek(DayOfWeek.Friday).Date;
            //var beginDate = DateTime.Now.Date.AddDays(-6).Date;
            //var endDate = DateTime.Now.Date;
            ViewBag.TimePeriod = 1;
            ViewBag.CourseId = course.Id;
            if (timePeriod.HasValue && timePeriod == 2)
            {
                //beginDate = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)).Date;
                //endDate = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))).Date;
                beginDate = DateTime.Now.Date.AddDays(-30).Date;
                endDate = DateTime.Now.Date;

                ViewBag.TimePeriod = 2;
            }
            else if (timePeriod.HasValue && timePeriod == 3)
            {
                var activeMin = UserActSummaryDAL.GetMinDate();
                var popularMin = ContentPopularityModelDAL.GetMinDate();
                var activeMax = UserActSummaryDAL.GetMaxDate();
                var popularMax = ContentPopularityModelDAL.GetMaxDate();
                beginDate = activeMin.Date <= popularMin.Date ? activeMin.Date : popularMin.Date;
                endDate = activeMax.Date >= popularMax.Date ? activeMax.Date : popularMax.Date;
                ViewBag.TimePeriod = 3;
            }
            var popular = new List<ContentPopularityModel>();
            var confilter = 0;
            Int32.TryParse(filter, out confilter);
            if (confilter == 1)
            {
                ViewBag.Filter = confilter;
                popular = ContentPopularityModelDAL.GetMostPopularResource(course.Id, beginDate, endDate);
            }
            else if (confilter == 4)
            {
                ViewBag.Filter = confilter;
                popular = ContentPopularityModelDAL.GetMostPopularActivities(course.Id, beginDate, endDate);
            }
            else
            {
                ViewBag.Filter = confilter;
                popular = ContentPopularityModelDAL.GetMostPopular(course.Id, beginDate, endDate);
            }
            var addedValue = new Dictionary<int, double>();
            var top3ForOne = new List<ContentPopularityModel>();
            var otherTop = new List<ContentPopularityModel>();
            if (popular.Any())
            {
                foreach (var item in popular)
                {
                    var sharedObject = ManageObject.GetSharedObject(item.ObjectId, item.ObjectType);
                    if (!App_UserEnrolementDAL.GetAllByCourseAndUser(course.Id, sharedObject.CreateUser.Id).Any(x => x.Type == (int)UT.SL.Model.Enumeration.MemebrshipType.Teacher))
                    {
                        if (addedValue.Any(x => x.Key == sharedObject.CreateUser.Id))
                        {
                            addedValue[sharedObject.CreateUser.Id] = addedValue[sharedObject.CreateUser.Id] + item.SoActValue;
                        }
                        else
                        {
                            addedValue.Add(sharedObject.CreateUser.Id, item.SoActValue);
                        }
                    }
                }
            }
            if (addedValue.Any())
            {
                var maxVal = addedValue.Max(x => x.Value);
                var maxUserId = addedValue.FirstOrDefault(x => x.Value == maxVal).Key;
                foreach (var item in popular.OrderByDescending(x => x.SoActValue))
                {
                    var sharedObject = ManageObject.GetSharedObject(item.ObjectId, item.ObjectType);
                    if (sharedObject.CreateUser.Id == maxUserId)
                    {
                        top3ForOne.Add(item);
                    }
                    else
                    {
                        otherTop.Add(item);
                    }
                }
            }
            if (top3ForOne.Any())
            {
                var i = 0;
                foreach (var item in top3ForOne.Take(3))
                {
                    var sharedObject = ManageObject.GetSharedObject(item.ObjectId, item.ObjectType);
                    var commentCount = CommentDAL.GetAllCount(item.ObjectId, item.ObjectType);
                    var voteCount = VoteDAL.GetAllCount(item.ObjectId, item.ObjectType);
                    model.Add(new LeadersModel
                    {
                        BeginDate = beginDate,
                        EndDate = endDate,
                        CreateDate = sharedObject.CreateDate,
                        IsPopular = true,
                        IsTop = true,
                        ObjectId = item.ObjectId,
                        ObjectType = item.ObjectType,
                        Rank = ++i,
                        Title = sharedObject.Title ?? sharedObject.Body,
                        User = sharedObject.CreateUser,
                        CommentCount = commentCount,
                        VoteCount = voteCount,
                        TotalScore = commentCount + voteCount
                    });
                }
            }
            if (otherTop.Any())
            {
                var i = 0;
                foreach (var item in otherTop.Take(2))
                {
                    var sharedObject = ManageObject.GetSharedObject(item.ObjectId, item.ObjectType);
                    if (!model.Any(x => x.User.Id == sharedObject.CreateUser.Id))
                    {
                        var commentCount = CommentDAL.GetAllCount(item.ObjectId, item.ObjectType);
                        var voteCount = VoteDAL.GetAllCount(item.ObjectId, item.ObjectType);
                        model.Add(new LeadersModel
                        {
                            BeginDate = beginDate,
                            EndDate = endDate,
                            CreateDate = sharedObject.CreateDate,
                            IsPopular = true,
                            ObjectId = item.ObjectId,
                            ObjectType = item.ObjectType,
                            Rank = ++i,
                            Title = sharedObject.Title ?? sharedObject.Body,
                            User = sharedObject.CreateUser,
                            CommentCount = commentCount,
                            VoteCount = voteCount,
                            TotalScore = commentCount + voteCount
                        });
                    }
                }
            }
            var actives = UserActSummaryDAL.GetMostActive(course.Id, beginDate, endDate);
            if (timePeriod == 3)
                actives = UserActSummaryDAL.GetAllCourse(course.Id);
            var addedScore = new Dictionary<int, int>();
            if (actives.Any())
            {
                foreach (var item in actives)
                {
                    if (addedScore.Any(x => x.Key == item.UserId))
                    {
                        addedScore[item.UserId] = addedScore[item.UserId] + item.TotalScore.Value;
                    }
                    else
                    {
                        addedScore.Add(item.UserId, item.TotalScore.Value);
                    }
                }
            }
            if (addedScore.Any())
            {
                var maxVal = addedScore.OrderByDescending(x => x.Value).Take(3);
                var i = 0;
                foreach (var val in maxVal)
                {
                    var tempModel = new LeadersModel();
                    var maxUserId = addedScore.FirstOrDefault(x => x.Value == val.Value).Key;
                    tempModel.BeginDate = beginDate;
                    tempModel.EndDate = endDate;
                    tempModel.IsActive = true;
                    tempModel.Rank = ++i;
                    tempModel.User = App_UserDAL.Get(maxUserId);
                    foreach (var item in actives.Where(x => x.UserId == maxUserId))
                    {
                        tempModel.CommentCount = tempModel.CommentCount + item.CommentCount.Value;
                        tempModel.VoteCount = tempModel.VoteCount + item.VoteCount.Value;
                        tempModel.TagCount = tempModel.TagCount + item.TagCount.Value;
                        tempModel.CreatedCount = tempModel.CreatedCount + item.CreateCount.Value;
                    }
                    tempModel.TotalScore = tempModel.CommentCount + tempModel.VoteCount + tempModel.TagCount + tempModel.CreatedCount;
                    model.Add(tempModel);
                }
            }
            return PartialView(model);
        }

        #endregion

        public ActionResult ResourcePostPanel(Course obj)
        {
            return PartialView(obj);
        }

        public ActionResult ResourcePostButton()
        {
            return PartialView();
        }

        public ActionResult Portfolio(int Id, int? rank)
        {
            var model = CourseDAL.Get(Id);
            if (rank.HasValue)
                ViewBag.LinkRank = rank.Value;
            ViewBag.GuidId = CurrentUser.GuidId;
            return View(model);
        }

        public ActionResult UserPortfolio(int courseId, Guid userId)
        {
            ViewBag.FullName = App_UserDAL.Get(userId).FirstName + " " + App_UserDAL.Get(userId).LastName;
            ViewBag.CourseId = courseId;
            ViewBag.GuidId = userId;
            return PartialView();
        }

        public ActionResult Participants(int Id, int? rank, string filter)
        {
            var model = CourseDAL.Get(Id);
            if (rank.HasValue)
                ViewBag.LinkRank = rank.Value;
            ViewBag.GuidId = CurrentUser.GuidId;
            if (!String.IsNullOrEmpty(filter))
            {
                ViewBag.Filter = filter;
            }
            return View(model);
        }

        public ActionResult GetCourseUserActivities(Guid userId, int courseId)
        {
            var model = UserInfoManager.GetUserCourseSummary(App_UserDAL.Get(userId).Id, courseId);
            ViewBag.CourseTitle = CourseDAL.Get(courseId).Title;
            ViewBag.CourseId = courseId;
            ViewBag.UserId = userId;
            return PartialView(model);
        }

        public ActionResult GetCourseUserActivityDetails(Guid userId, int courseId, int position)
        {
            var items = UserInfoManager.GetUserCourseSummary(App_UserDAL.Get(userId).Id, courseId);
            var model = items[position].Objects.OrderByDescending(x => x.CreateDate).ToList();
            if (model.Any())
            {
                ViewBag.FullName = App_UserDAL.Get(userId).FirstName + " " + App_UserDAL.Get(userId).LastName;
                ViewBag.Date = model.First().CreateDate;
            }
            ViewBag.UserId = App_UserDAL.Get(userId).Id;
            return PartialView(model);
        }

        public ActionResult GetCourseEachUserActivities(Guid userId, int courseId)
        {
            var model = UserInfoManager.GetUserCourseSummary(App_UserDAL.Get(userId).Id, courseId);
            ViewBag.CourseTitle = CourseDAL.Get(courseId).Title;
            return PartialView(model);
        }

        [AllowAnonymous]
        public ActionResult CoursePreviewAndSignUp(int id)
        {
            var formObject = CourseDAL.Get(id);
            if (formObject.CourseAbstractId.HasValue)
            {
                var listItems = new SelectListItems
                {
                    Items = TopicDAL.GetAllSelectedTopics(formObject.CourseAbstractId.Value).Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Title
                    })
                };
                var courseAbstract = CourseAbstractDAL.Get(formObject.CourseAbstractId.Value);
                var category = String.Format("{0} - {1}", courseAbstract.Category.Category1.Title, courseAbstract.Category.Title);
                var model = new FormModel<Course, SelectListItems>
                {
                    FormObject = formObject,
                    ExtraKnownData = listItems
                };
                model.ExtraData = new List<object>();
                model.ExtraData.Add(category);
                var courseSummary = new CourseSummaryModel();
                var enrols = CourseDAL.Get(id).App_UserEnrolements;
                courseSummary.ParticipantCount = enrols.Count();
                courseSummary.ResourceCount = CourseDAL.GetAllResources(id).Count();
                courseSummary.AssignmentCount = CourseDAL.GetAssigmentCount(id);
                courseSummary.ForumCount = CourseDAL.GetForumCount(id);
                courseSummary.LearningGroupsCount = CourseDAL.GetLearningGroupsCount(id);
                courseSummary.ParticipantIsd = enrols.Select(x => x.UserId).ToList();
                model.ExtraData.Add(courseSummary);
                if (Request.IsAuthenticated)
                {
                    if (App_UserEnrolementDAL.IsAMember(id, CurrentUser.Id))
                    {
                        ViewBag.IsMember = true;
                    }
                    else
                    {
                        ViewBag.IsMember = false;
                        var teachers = App_UserEnrolementDAL.GetAllCourseTeachers(id).FirstOrDefault();
                        ViewBag.TeacherId = teachers != null ? teachers.Id : 0;
                    }
                }
                else
                {
                    ViewBag.IsMember = false;
                    var teachers = App_UserEnrolementDAL.GetAllCourseTeachers(id).FirstOrDefault();
                    ViewBag.TeacherId = teachers != null ? teachers.Id : 0;
                }
                return PartialView(model);
            }
            var model2 = new FormModel<Course, SelectListItems>
            {
                FormObject = formObject,
                ExtraKnownData = new SelectListItems()
            };
            model2.ExtraData = new List<object>();
            model2.ExtraData.Add(String.Empty);
            if (Request.IsAuthenticated)
            {
                if (App_UserEnrolementDAL.IsAMember(id, CurrentUser.Id))
                {
                    ViewBag.IsMember = true;
                }
                else
                {
                    ViewBag.IsMember = false;
                    var teachers = App_UserEnrolementDAL.GetAllCourseTeachers(id).FirstOrDefault();
                    ViewBag.TeacherId = teachers != null ? teachers.Id : 0;
                }
            }
            else
            {
                ViewBag.IsMember = false;
                var teachers = App_UserEnrolementDAL.GetAllCourseTeachers(id).FirstOrDefault();
                ViewBag.TeacherId = teachers != null ? teachers.Id : 0;
            }
            return PartialView(model2);
        }

    }
}
