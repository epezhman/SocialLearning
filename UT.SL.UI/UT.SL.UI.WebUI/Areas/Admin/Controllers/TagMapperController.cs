using System;
using System.Linq;
using System.Web.Mvc;
using UT.SL.Helper;
using UT.SL.UI.WebUI.Controllers;
using UT.SL.Model;
using UT.SL.Data.LINQ;
using UT.SL.DAL;
using System.Collections.Generic;
using UT.SL.BLL;
using UT.SL.Model.Enumeration;


namespace UT.SL.UI.WebUI.Areas.Admin.Controllers
{


    [Authorize()]
    public class TagMapperController : BaseController
    {

        [HttpPost()]
        public ActionResult DeleteTagMapper(int id)
        {
            var bpr = new BatchProcessResultModel();
            var objectId = 0;
            var objectType = 0;
            try
            {
                var tagMapper = TagMapperDAL.Get(id);
                objectId = tagMapper.ObjectId;
                objectType = tagMapper.ObjectType;
                //update Credit
                ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.DeleteTag, objectId, objectType, tagMapper.UserId.Value);
                UserInfoManager.DeleteUserActSummary(CurrentUser.Id, tagMapper.ObjectId, tagMapper.ObjectType, (int)UT.SL.Model.Enumeration.ActivityType.Tag);
                //update credit
                var drm = (DALReturnModel<TagMapper>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.TagMapper),
                                       new Func<TagMapper, DALReturnModel<TagMapper>>(TagMapperDAL.Delete), new TagMapper { Id = id });
                bpr = drm.BPR;
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
            return Content(objectId + "_" + objectType);
        }

        public ActionResult TagComponent(int ObjectId = 0, int Type = 0)
        {
            var model = new ObjectModel();
            if (ObjectId == 0 || Type == 0)
            {
                var bpr = new BatchProcessResultModel();
                bpr.AddError(UT.SL.Model.Resource.App_Errors.ObjectIdMissing, true, true);
                return PartialView("ErrorDialog", bpr);
            }
            model.ObjectId = ObjectId;
            model.Type = Type;
            model.Count = TagMapperDAL.GetAllCount(ObjectId, Type, CurrentUser.Id);
            return PartialView(model);
        }

        public ActionResult TagComponentResource(int ObjectId = 0, int Type = 0, int tagType = 0, int newTags = 0)
        {
            var model = new ObjectModel();
            if (ObjectId == 0 || Type == 0)
            {
                var bpr = new BatchProcessResultModel();
                bpr.AddError(UT.SL.Model.Resource.App_Errors.ObjectIdMissing, true, true);
                return PartialView("ErrorDialog", bpr);
            }
            model.ObjectId = ObjectId;
            model.Type = Type;
            model.Count = TagMapperDAL.GetAllCount(ObjectId, Type, CurrentUser.Id);
            if (tagType == 2)
            {
                ViewBag.TagType = tagType;
            }
            if (newTags == 1)
            {
                model.IsNew = true;
            }
            return PartialView(model);
        }

        public ActionResult Tag(int ObjectId = 0, int Type = 0, byte viewType = 0, int tagType = 0)
        {
            var model = new FormModel<TagModel, ObjectViewModel>
            {
                FormObject = new TagModel(),
                ExtraKnownData = new ObjectViewModel()
            };
            if (ObjectId == 0 || Type == 0)
            {
                var bpr = new BatchProcessResultModel();
                bpr.AddError(UT.SL.Model.Resource.App_Errors.ObjectIdMissing, true, true);
                return PartialView("ErrorDialog", bpr);
            }
            model.ExtraKnownData = ManageObject.GetSharedObject(ObjectId, Type);
            ViewBag.Categories = CategoryDAL.GetAll().Where(x => x.Category1 == null);
            //var tagMappers = TagMapperDAL.GetAll(ObjectId, Type);
            //var tags = string.Empty;
            //if (tagMappers.Any())
            //{
            //    foreach (var item in tagMappers)
            //    {
            //        var tempTag = string.Empty;
            //        if (item.Tag.Category != null)
            //        {
            //            if (item.Tag.Category.Category1 != null)
            //            {
            //                tags += item.Tag.Category.Category1.Title + "-";
            //            }
            //            tags += item.Tag.Category.Title + "-";
            //        }
            //        tags += item.Tag.Title + ", ";
            //    }
            //}
            //model.FormObject.Tags = tags;
            model.FormObject.Tags = String.Empty; ;

            if (viewType == 1)
            {
                ViewBag.TagType = tagType;
                return PartialView("TagResource", model);
            }

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Tag(FormModel<TagModel, ObjectViewModel> model, byte viewType = 0)
        {
            var bpr = new BatchProcessResultModel();
            bool moreThan3 = false;
            try
            {
                //TagMapperDAL.DeleteAll(model.ExtraKnownData.ObjectId, model.ExtraKnownData.Type);
                if (!string.IsNullOrEmpty(model.FormObject.Tags))
                {
                    var tags = model.FormObject.Tags.Split(new char[] { '،', ',', ' ' });
                    if (tags.Any())
                    {
                        foreach (var item in tags)
                        {
                            if (!string.IsNullOrEmpty(item.StringNormalizer().Trim()))
                            {
                                var tag = TagDAL.Get(item.StringNormalizer().Trim().Split('-').Last());
                                if (tag == null)
                                {
                                    var tagTokens = item.StringNormalizer().Trim().Split('-');
                                    if (tagTokens.Count() == 3)
                                    {
                                        if (!string.IsNullOrEmpty(tagTokens[1].Trim()) && !string.IsNullOrEmpty(tagTokens[2].Trim()))
                                        {
                                            var category = CategoryDAL.Get(tagTokens[1].Trim());
                                            if (category != null)
                                            {
                                                var newTag = new Tag
                                                {
                                                    CreateDate = DateTime.Now,
                                                    Title = tagTokens[2].Trim().ToLower(),
                                                    IsValid = false,
                                                    CategoryId = category.Id,
                                                    UserId = CurrentUser.Id
                                                };
                                                var drm = (DALReturnModel<Tag>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Tag),
                                                          new Func<Tag, DALReturnModel<Tag>>(TagDAL.Add), newTag);
                                                tag = drm.ReturnObject;
                                            }
                                        }
                                    }
                                    else if (tagTokens.Count() == 2)
                                    {
                                        if (!string.IsNullOrEmpty(tagTokens[0].Trim()) && !string.IsNullOrEmpty(tagTokens[1].Trim()))
                                        {
                                            var category = CategoryDAL.Get(tagTokens[0].Trim());
                                            if (category != null)
                                            {
                                                var newTag = new Tag
                                                {
                                                    CreateDate = DateTime.Now,
                                                    Title = tagTokens[1].Trim().ToLower(),
                                                    IsValid = false,
                                                    CategoryId = category.Id,
                                                    UserId = CurrentUser.Id
                                                };
                                                var drm = (DALReturnModel<Tag>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Tag),
                                                         new Func<Tag, DALReturnModel<Tag>>(TagDAL.Add), newTag);
                                                tag = drm.ReturnObject;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(tagTokens[0].Trim()))
                                        {
                                            var newTag = new Tag
                                            {
                                                CreateDate = DateTime.Now,
                                                Title = tagTokens[0].Trim().ToLower(),
                                                IsValid = false,
                                                UserId = CurrentUser.Id
                                            };
                                            var drm = (DALReturnModel<Tag>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Tag),
                                                         new Func<Tag, DALReturnModel<Tag>>(TagDAL.Add), newTag);
                                            tag = drm.ReturnObject;
                                        }
                                    }
                                }
                                if (tag != null)
                                {
                                    var tagMappr = TagMapperDAL.GetByDetail(model.ExtraKnownData.Id, model.ExtraKnownData.Type, tag.Id);
                                    if (tagMappr == null)
                                    {
                                        var newMap = new TagMapper
                                        {
                                            CreateDate = DateTime.Now,
                                            ObjectId = model.ExtraKnownData.Id,
                                            ObjectType = model.ExtraKnownData.Type,
                                            TagId = tag.Id,
                                            UserId = CurrentUser.Id
                                        };
                                        if (TagMapperDAL.GetAssingedTagCount(newMap.ObjectId, newMap.ObjectType) < 3)
                                        {
                                            var drm = (DALReturnModel<TagMapper>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.TagMapper),
                                                         new Func<TagMapper, DALReturnModel<TagMapper>>(TagMapperDAL.Add), newMap);                                          
                                        }                                        
                                        else
                                            moreThan3 = true;
                                        //update Credit
                                        ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.Tag, model.ExtraKnownData.Id, model.ExtraKnownData.Type, CurrentUser.Id);
                                        UserInfoManager.UpdateUserActSummary(CurrentUser.Id, model.ExtraKnownData.Id, model.ExtraKnownData.Type, (int)UT.SL.Model.Enumeration.ActivityType.Tag);
                                        //update credit
                                    }
                                }
                            }
                        }
                    }
                }
                bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                if (moreThan3)
                    bpr.SuccessClientScript = "updateTagCount(" + model.ExtraKnownData.Id + "," + model.ExtraKnownData.Type + ")";
                else
                    bpr.SuccessClientScript = "updateTagCount(" + model.ExtraKnownData.Id + "," + model.ExtraKnownData.Type + ")";
            }
            catch
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.BprMainUnknownError, true, true);
            }
            if (viewType == 0)
                return PartialView("ProcessResult", bpr);
            if (moreThan3)
                return Content(model.ExtraKnownData.Id + "_" + model.ExtraKnownData.Type + "_1");
            else
                return Content(model.ExtraKnownData.Id + "_" + model.ExtraKnownData.Type);
        }

        public ActionResult GetTags(string title)
        {
            var tag = title.Split(new char[] { '،', ',', ' ' }).Last().StringNormalizer().Trim();
            var tagTokens = tag.Split('-');
            if (tagTokens.Count() == 3)
            {
                if (!string.IsNullOrEmpty(tagTokens[1].Trim()) && !string.IsNullOrEmpty(tagTokens[2].Trim()))
                {
                    var category = CategoryDAL.GetFirst(tagTokens[1].Trim());
                    if (category != null)
                    {
                        var category2 = CategoryDAL.GetFirst(tagTokens[1].Trim(), category.Id);
                        if (category2 != null)
                        {
                            var model2 = TagDAL.GetAll(category2.Id, tagTokens[2].Trim()).Take(10).Select(x => new { id = x.Id, title = (x.Category.ParentId != null ? x.Category.Category1.Title + "-" + x.Category.Title + "-" + x.Title : x.Category.Title + "-" + x.Title) }).ToList();
                            if (model2.Any())
                                return Json(model2, JsonRequestBehavior.AllowGet);
                        }
                        var model = TagDAL.GetAll(category.Id, tagTokens[2].Trim()).Take(10).Select(x => new { id = x.Id, title = (x.Category.ParentId != null ? x.Category.Category1.Title + "-" + x.Category.Title + "-" + x.Title : x.Category.Title + "-" + x.Title) }).ToList();
                        return Json(model, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else if (tagTokens.Count() == 2)
            {
                if (!string.IsNullOrEmpty(tagTokens[0].Trim()) && !string.IsNullOrEmpty(tagTokens[1].Trim()))
                {
                    var category = CategoryDAL.GetFirst(tagTokens[0].Trim());
                    if (category != null)
                    {
                        var category2 = CategoryDAL.GetFirst(tagTokens[1].Trim(), category.Id);
                        if (category2 != null)
                        {
                            var model2 = TagDAL.GetAll(category2.Id).Take(10).Select(x => new { id = x.Id, title = (x.Category.ParentId != null ? x.Category.Category1.Title + "-" + x.Category.Title + "-" + x.Title : x.Category.Title + "-" + x.Title) }).ToList();
                            if (model2.Any())
                                return Json(model2, JsonRequestBehavior.AllowGet);
                        }
                        var model = TagDAL.GetAll(category.Id, tagTokens[1].Trim()).Take(10).Select(x => new { id = x.Id, title = (x.Category.ParentId != null ? x.Category.Category1.Title + "-" + x.Category.Title + "-" + x.Title : x.Category.Title + "-" + x.Title) }).ToList();
                        if (model.Any())
                            return Json(model, JsonRequestBehavior.AllowGet);
                        model = TagDAL.GetAll(category.Id).Take(10).Select(x => new { id = x.Id, title = (x.Category.ParentId != null ? x.Category.Category1.Title + "-" + x.Category.Title + "-" + x.Title : x.Category.Title + "-" + x.Title) }).ToList();
                        if (model.Any())
                            return Json(model, JsonRequestBehavior.AllowGet);
                    }
                    category = CategoryDAL.GetFirst(tagTokens[1].Trim());
                    if (category != null)
                    {
                        var model = TagDAL.GetAll(category.Id).Take(10).Select(x => new { id = x.Id, title = (x.Category.ParentId != null ? x.Category.Category1.Title + "-" + x.Category.Title + "-" + x.Title : x.Category.Title + "-" + x.Title) }).ToList();
                        if (model.Any())
                            return Json(model, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else if (tagTokens.Count() == 1)
            {
                var category = CategoryDAL.GetFirst(tagTokens[0].Trim());
                if (category != null)
                {
                    var model = TagDAL.GetAll(category.Id).Take(10).Select(x => new { id = x.Id, title = (x.Category.ParentId != null ? x.Category.Category1.Title + "-" + x.Category.Title + "-" + x.Title : x.Category.Title + "-" + x.Title) }).ToList();
                    if (model.Any())
                        return Json(model, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var model = TagDAL.GetAll(tagTokens[0].Trim()).Take(10).Select(x => new { id = x.Id, title = (x.CategoryId != null ? (x.Category.ParentId != null ? x.Category.Category1.Title + "-" + x.Category.Title + "-" + x.Title : x.Category.Title + "-" + x.Title) : x.Title) }).ToList();
                    if (model.Any())
                        return Json(model, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { title = string.Empty, id = 0 }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetTagsForCourse(string title, int Id)
        {
            var tag = title.Split(new char[] { '،', ',', ' ' }).Last().StringNormalizer().Trim();
            var tagTokens = title.Split('-');
            if (tagTokens.Count() == 3)
            {
                if (!string.IsNullOrEmpty(tagTokens[1].Trim()) && !string.IsNullOrEmpty(tagTokens[2].Trim()))
                {
                    var category = CategoryDAL.GetFirst(tagTokens[1].Trim());
                    if (category != null)
                    {
                        var category2 = CategoryDAL.GetFirst(tagTokens[1].Trim(), category.Id);
                        if (category2 != null)
                        {
                            var model2 = TagDAL.GetAll(category2.Id, tagTokens[2].Trim()).Take(10).Select(x => new { id = x.Id, title = (x.Category.ParentId != null ? x.Category.Category1.Title + "-" + x.Category.Title + "-" + x.Title : x.Category.Title + "-" + x.Title) }).ToList();
                            if (model2.Any())
                                return Json(model2, JsonRequestBehavior.AllowGet);
                        }
                        var model = TagDAL.GetAll(category.Id, tagTokens[2].Trim()).Take(10).Select(x => new { id = x.Id, title = (x.Category.ParentId != null ? x.Category.Category1.Title + "-" + x.Category.Title + "-" + x.Title : x.Category.Title + "-" + x.Title) }).ToList();
                        return Json(model, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else if (tagTokens.Count() == 2)
            {
                if (!string.IsNullOrEmpty(tagTokens[0].Trim()) && !string.IsNullOrEmpty(tagTokens[1].Trim()))
                {
                    var category = CategoryDAL.GetFirst(tagTokens[0].Trim());
                    if (category != null)
                    {
                        var category2 = CategoryDAL.GetFirst(tagTokens[1].Trim(), category.Id);
                        if (category2 != null)
                        {
                            var model2 = TagDAL.GetAll(category2.Id).Take(10).Select(x => new { id = x.Id, title = (x.Category.ParentId != null ? x.Category.Category1.Title + "-" + x.Category.Title + "-" + x.Title : x.Category.Title + "-" + x.Title) }).ToList();
                            if (model2.Any())
                                return Json(model2, JsonRequestBehavior.AllowGet);
                        }
                        var model = TagDAL.GetAll(category.Id, tagTokens[1].Trim()).Take(10).Select(x => new { id = x.Id, title = (x.Category.ParentId != null ? x.Category.Category1.Title + "-" + x.Category.Title + "-" + x.Title : x.Category.Title + "-" + x.Title) }).ToList();
                        if (model.Any())
                            return Json(model, JsonRequestBehavior.AllowGet);
                        model = TagDAL.GetAll(category.Id).Take(10).Select(x => new { id = x.Id, title = (x.Category.ParentId != null ? x.Category.Category1.Title + "-" + x.Category.Title + "-" + x.Title : x.Category.Title + "-" + x.Title) }).ToList();
                        if (model.Any())
                            return Json(model, JsonRequestBehavior.AllowGet);
                    }
                    category = CategoryDAL.GetFirst(tagTokens[1].Trim());
                    if (category != null)
                    {
                        var model = TagDAL.GetAll(category.Id).Take(10).Select(x => new { id = x.Id, title = (x.Category.ParentId != null ? x.Category.Category1.Title + "-" + x.Category.Title + "-" + x.Title : x.Category.Title + "-" + x.Title) }).ToList();
                        if (model.Any())
                            return Json(model, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else if (tagTokens.Count() == 1)
            {
                var category = CategoryDAL.GetFirstWithCourse(tagTokens[0].Trim(), Id);
                if (category != null)
                {
                    var model = TagDAL.GetAll(category.Id).Take(10).Select(x => new { id = x.Id, title = (x.Category.ParentId != null ? x.Category.Category1.Title + "-" + x.Category.Title + "-" + x.Title : x.Category.Title + "-" + x.Title) }).ToList();
                    if (model.Any())
                        return Json(model, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var model = TagDAL.GetAll(tagTokens[0].Trim()).Take(10).Select(x => new { id = x.Id, title = (x.CategoryId != null ? (x.Category.ParentId != null ? x.Category.Category1.Title + "-" + x.Category.Title + "-" + x.Title : x.Category.Title + "-" + x.Title) : x.Title) }).ToList();
                    if (model.Any())
                        return Json(model, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { title = string.Empty, id = 0 }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetSubjects(int Id)
        {
            var model = CategoryDAL.GetAll(Id).Select(x => new { id = x.Id, title = x.Title }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTagsCategory(int Id)
        {
            var model = TagDAL.GetAll(Id).Select(x => new { id = x.Id, title = x.Title }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TopTags(int ObjectId = 0, int Type = 0, int tagType = 0)
        {
            var tags = TagMapperDAL.GetAll(ObjectId, Type);
            var topics = ObjectTopicMapperDAL.GetAll(ObjectId, Type);
            var model = new List<TagAndTopicModel>();
            foreach (var item in tags)
            {
                var tempTtm = new TagAndTopicModel
                {
                    IsTag = true,
                    Tag = item.Tag
                };
                model.Add(tempTtm);
            }
            foreach (var item in topics)
            {
                var tempTtm = new TagAndTopicModel
                {
                    IsTag = false,
                    Topic = item
                };
                model.Add(tempTtm);
            }
            if (tagType == 2)
            {
                return PartialView("TopTagsType2", model);
            }
            return PartialView(model);
        }

        public ActionResult TopTopics(int ObjectId = 0, int Type = 0, int viewType = 0)
        {
            var topics = ObjectTopicMapperDAL.GetAll(ObjectId, Type);
            var model = new List<TagAndTopicModel>();           
            foreach (var item in topics)
            {
                var tempTtm = new TagAndTopicModel
                {
                    IsTag = false,
                    Topic = item
                };
                model.Add(tempTtm);
            }
            if (viewType > 0)
            {
                ViewBag.ViewType = viewType;
            }
            return PartialView(model);
        }

        public ActionResult TopOnlyTags(int ObjectId = 0, int Type = 0)
        {
            var tags = TagMapperDAL.GetAll(ObjectId, Type);
            var model = new List<TagAndTopicModel>();
            foreach (var item in tags)
            {
                var tempTtm = new TagAndTopicModel
                {
                    IsTag = true,
                    Tag = item.Tag
                };
                model.Add(tempTtm);
            }
            
            return PartialView(model);
        }

        public ActionResult TopTagsAny(int ObjectId = 0, int Type = 0)
        {
            ViewBag.Any = TagMapperDAL.TagAny(ObjectId, Type);
            return PartialView();
        }

        public ActionResult TagsForViewAndDelete(int objectId = 0, int objectType = 0, int tagType = 0)
        {
            var model = TagMapperDAL.GetAll(objectId, objectType);
            ViewBag.TagType = tagType;
            return PartialView(model);
        }

        public ActionResult CountNewTags(int Id, int type, DateTime clickedDate)
        {
            var cnt = TagMapperDAL.CountNewTags(Id, type, clickedDate, CurrentUser.Id);
            if (cnt > 0)
                ViewBag.Count = cnt;
            return PartialView();
        }

    }
}
