using System;
using System.Linq;
using System.Web.Mvc;
using UT.SL.Helper;
using UT.SL.UI.WebUI.Controllers;
using UT.SL.Model;
using UT.SL.Data.LINQ;
using UT.SL.DAL;
using System.Collections.Generic;
using System.Web;
using UT.SL.BLL;
using UT.SL.Model.Enumeration;


namespace UT.SL.UI.WebUI.Areas.Admin.Controllers
{


    [Authorize()]
    public class CategoryController : BaseController
    {

        public ActionResult Index(CategorySearchModel model)
        {
            model.Area = "Admin";
            return View(model);
        }

        public ActionResult EditIndex(CategorySearchModel model)
        {
            model.Area = "Admin";
            return View(model);
        }

        public ActionResult CategorySearchModelView(CategorySearchModel model)
        {
            model.Area = "Admin";
            return PartialView(model);
        }

        public PagedList<Category> SearchFilters(CategorySearchModel model)
        {
            model.Area = "Admin";
            var qry = CategoryDAL.Find(model);
            model.Update(model.PageSize, qry.Count());
            var ls = qry.Skip(model.PageSize * model.PageIndex).Take(model.PageSize).ToList();
            var ql = new PagedList<Category>(ls, model);
            return ql;
        }

        public ActionResult IX(CategorySearchModel model)
        {
            model.Area = "Admin";
            return PartialView(SearchFilters(model));
        }

        public ActionResult CategoriesList()
        {
            var model = CategoryDAL.GetAll().OrderBy(x => x.Title).ThenBy(x => x.ParentId).ToList();
            return PartialView(model);
        }

        public ActionResult Create()
        {
            var model = new Category();
            ViewBag.Categories = CategoryDAL.GetAllWithoutParent().Select(x => new { id = x.Id, title = x.Title });
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Category model)
        {
            var bpr = new BatchProcessResultModel();
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    var drm = (DALReturnModel<Category>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Category),
                                      new Func<Category, BatchProcessResultModel, DALReturnModel<Category>>(CategoryDAL.Add), model, bpr);
                    bpr = drm.BPR;
                    var catId = drm.ReturnObject.Id;
                    if (bpr.Failed > 0)
                    {
                        return PartialView("ProcessResult", bpr);
                    }
                    else
                    {
                        if (model.ParentId.HasValue && model.ParentId.Value > 0)
                        {
                            bpr.SuccessClientScript = "$('#searchFormCategory').submit();";
                        }
                        else
                        {
                            bpr.SuccessClientScript = "$('#searchFormCategory').submit();";
                            //bpr.SuccessClientScript = "$('#searchFormCategory').submit(); $('#image').attr('data-contentid', '" + catId + "'); $('#categorypic').show('blind', 500);  initializeKendoUploader();";
                        }
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

        public ActionResult Edit(int Id)
        {
            var model = CategoryDAL.Get(Id);
            ViewBag.Categories = CategoryDAL.GetAllWithoutParent().Select(x => new { id = x.Id, title = x.Title });
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(Category model)
        {
            var bpr = new BatchProcessResultModel();
            if (ModelState.IsValid)
            {
                try
                {
                    var drm = (DALReturnModel<Category>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Category),
                                      new Func<Category, BatchProcessResultModel, DALReturnModel<Category>>(CategoryDAL.Update), model, bpr);
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

        public ActionResult Delete(int Id)
        {
            var model = CategoryDAL.Get(Id);
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(Category model)
        {
            var bpr = new BatchProcessResultModel();
            try
            {
                var drm = (DALReturnModel<Category>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Category),
                                       new Func<Category, DALReturnModel<Category>>(CategoryDAL.Delete), model);
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

        public ActionResult GetCategoryTitle(System.Nullable<int> Id)
        {
            if (Id.HasValue)
            {
                try
                {
                    var model = CategoryDAL.Get(Id.Value).Title;
                    return Content(model);
                }
                catch (System.Exception)
                {
                    Content("");
                }
            }
            return Content("");
        }

        public ActionResult GetCources(int Id)
        {
            var model = new FormModel<List<Course>, Category>();
            //model.FormObject = CourseDAL.GetAllWithCategort(Id);
            model.FormObject = CourseDAL.GetAllWithCategory(Id);
            model.ExtraKnownData = CategoryDAL.Get(Id);
            return PartialView(model);
        }

        public ActionResult GetSubCategories(System.Nullable<int> Id)
        {
            if (Id.HasValue)
            {
                try
                {
                    /*var category = CategoryDAL.Get(Id.Value);
                    var model = new List<CategoryCourseModel>();
                    model.Add(new CategoryCourseModel
                    {
                        CategoryId = 0,
                        CourseTitle = string.Empty,
                        CategoryTitle = category.Title
                    });
                    foreach (var item in category.Categories)
                    {
                        var courses = CourseDAL.GetAllWithCategory(item.Id);
                        string courseTitles = String.Empty;
                        foreach (var course in courses)
                        {
                            courseTitles += string.Format("{0}, ", course.Title); //  string.Format("<span class='badge badge-success'>{0}</span> , ", course.Title);
                        }
                        model.Add(new CategoryCourseModel
                        {
                            CategoryId = item.Id,
                            CourseTitle = courseTitles,
                            CategoryTitle = item.Title
                        });
                    }
                    return Json(model, JsonRequestBehavior.AllowGet);*/
                }
                catch (System.Exception)
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewImage(int Id)
        {
            var model = CategoryDAL.Get(Id);
            return File(model.ImageData.ToArray(), model.ImageMIME);
        }

        public ActionResult DeleteImage(int Id)
        {
            var model = CategoryDAL.DeletePic(new Category { Id = Id });
            return Content("true");
        }

        public ActionResult SaveImage(int Id, HttpPostedFileBase image)
        {
            var bpr = new BatchProcessResultModel();
            var drm = ((DALReturnModel<Category>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Category),
                           new Func<int, HttpPostedFileBase, BatchProcessResultModel, DALReturnModel<Category>>(CategoryDAL.SaveImage), Id, image, bpr));
            return Content(drm.ReturnObject.Id.ToString());
        }

        public ActionResult GetCategoryCources(int Id)
        {
            var model = CategoryDAL.GetAllCources(Id);
            return PartialView(model);
        }

        public ActionResult CreateCourseAbstract(int Id)
        {
            var formObject = CourseAbstractDAL.Get(Id);
            if (formObject == null)
            {
                formObject = new CourseAbstract { SubCategoryId = Id, Category = CategoryDAL.Get(Id) };
            }
            var listItems = new SelectListItems
            {
                SelectedIds = TopicDAL.GetAllSelectedTopics(formObject.Id).Select(x => x.Id.ToString()).ToArray(),
                Items = TopicDAL.GetAll().Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Title
                })
            };
            var model = new FormModel<CourseAbstract, SelectListItems>
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
        [ValidateInput(false)]
        public ActionResult CreateCourseAbstract(FormModel<CourseAbstract, SelectListItems> model, string newTopics)
        {
            var bpr = new BatchProcessResultModel();
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    var courseAbstractId = model.FormObject.Id;
                    if (model.FormObject.Id == 0)
                    {
                        var drm = ((DALReturnModel<CourseAbstract>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.CourseAbstract),
                                    new Func<CourseAbstract, BatchProcessResultModel, DALReturnModel<CourseAbstract>>(CourseAbstractDAL.Add), model.FormObject, bpr));
                        bpr = drm.BPR;
                        courseAbstractId = drm.ReturnObject.Id;
                    }
                    else
                    {
                        var drm = ((DALReturnModel<CourseAbstract>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.CourseAbstract),
                                   new Func<CourseAbstract, BatchProcessResultModel, DALReturnModel<CourseAbstract>>(CourseAbstractDAL.Update), model.FormObject, bpr));
                        bpr = drm.BPR;
                    }
                    if (bpr.Failed > 0)
                    {
                        return PartialView("ProcessResult", bpr);
                    }
                    else
                    {
                        ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Topic),
                                       new Func<int, DALReturnModel<Topic>>(TopicDAL.DeleteAllMappers), courseAbstractId);
                        if (model.ExtraKnownData != null && model.ExtraKnownData.SelectedIds != null)
                            foreach (var item in model.ExtraKnownData.SelectedIds)
                            {
                                var tcm = new CourseTopcMapper
                                {
                                    TopicId = Int32.Parse(item),
                                    CourseAbstractId = courseAbstractId
                                };
                                bpr = new BatchProcessResultModel();
                                var drm = ((DALReturnModel<CourseTopcMapper>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.CourseTopcMapper),
                                   new Func<CourseTopcMapper, BatchProcessResultModel, DALReturnModel<CourseTopcMapper>>(TopicDAL.AddTopicMapper), tcm, bpr));
                                bpr = drm.BPR;
                                if (bpr.Failed > 0)
                                {
                                    return PartialView("ProcessResult", bpr);
                                }
                            }

                        if (!string.IsNullOrEmpty(newTopics))
                        {
                            foreach (var topic in newTopics.Split(new char[] { ',', ' ' }))
                            {
                                if (!String.IsNullOrEmpty(topic))
                                {
                                    var dbTopic = TopicDAL.Get(topic);
                                    if (dbTopic != null)
                                    {
                                        var tcm = new CourseTopcMapper
                                        {
                                            TopicId = dbTopic.Id,
                                            CourseAbstractId = courseAbstractId
                                        };
                                        bpr = new BatchProcessResultModel();
                                        var drm = ((DALReturnModel<CourseTopcMapper>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.CourseTopcMapper),
                                           new Func<CourseTopcMapper, BatchProcessResultModel, DALReturnModel<CourseTopcMapper>>(TopicDAL.AddTopicMapper), tcm, bpr));
                                        bpr = drm.BPR;
                                        if (bpr.Failed > 0)
                                        {
                                            return PartialView("ProcessResult", bpr);
                                        }
                                    }
                                    else
                                    {
                                        var tempTopic = new Topic
                                        {
                                            Title = topic
                                        };
                                        bpr = new BatchProcessResultModel();
                                        var drm = ((DALReturnModel<Topic>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Topic),
                                           new Func<Topic, BatchProcessResultModel, DALReturnModel<Topic>>(TopicDAL.Add), tempTopic, bpr));
                                        bpr = drm.BPR;
                                        var tempTopicId = drm.ReturnObject.Id;
                                        var tcm = new CourseTopcMapper
                                        {
                                            TopicId = tempTopicId,
                                            CourseAbstractId = courseAbstractId
                                        };
                                        bpr = new BatchProcessResultModel();
                                        var drm2 = ((DALReturnModel<CourseTopcMapper>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.CourseTopcMapper),
                                           new Func<CourseTopcMapper, BatchProcessResultModel, DALReturnModel<CourseTopcMapper>>(TopicDAL.AddTopicMapper), tcm, bpr));
                                        bpr = drm2.BPR;
                                        if (bpr.Failed > 0)
                                        {
                                            return PartialView("ProcessResult", bpr);
                                        }
                                    }
                                }
                            }
                        }

                        bpr.SuccessClientScript = "$('#searchFormCategory').submit();";
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

    }
}
