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
    public class ImageAnnotationController : BaseController
    {

        public ActionResult ViewImageResourceForAnnotation(int Id)
        {
            var model = ResourceDAL.Get(Id);
            return PartialView(model);
        }

        public ActionResult GetImageAnnotations(int objectId, int objectType)
        {
            try
            {
                var model = new List<ImageAnnotationCoord>();
                var coords = ImageAnnotationDAL.GetAll(objectId, objectType);                
                foreach (var item in coords)
                {
                    var tagMappers = TagMapperDAL.GetAll(item.Id, (int)UT.SL.Model.Enumeration.ObjectType.ImageAnnotation);
                    var tags = string.Empty;
                    if (tagMappers.Any())
                    {
                        foreach (var tag in tagMappers)
                        {
                            var tempTag = string.Empty;
                            if (tag.Tag.Category != null)
                            {
                                if (tag.Tag.Category.Category1 != null)
                                {
                                    tags += tag.Tag.Category.Category1.Title + "-";
                                }
                                tags += tag.Tag.Category.Title + "-";
                            }
                            tags += tag.Tag.Title + ", ";
                        }
                    }
                    model.Add(new ImageAnnotationCoord {
                        top = item.TopCoord.ToString(),
                        left = item.LeftCoord.ToString(),
                        width = item.Width.ToString(),
                        height = item.Height.ToString(),
                        text = tags,
                        id = item.Id.ToString(),
                        editable = "true"
                    });
                }
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddImageAnnotations(ImageAnnotationCoord model)
        {
            try
            {
                var bpr = new BatchProcessResultModel();
                if (model != null)
                {
                    if (model.id == "new")
                    {
                        var imageAnnot = new ImageAnnotation
                        {
                            ObjectId = model.objectId,
                            ObjectType = model.objectType,
                            TopCoord = (int)Math.Floor(double.Parse(model.height)),
                            LeftCoord = (int)Math.Floor(double.Parse(model.left)),
                            Width = (int)Math.Floor(double.Parse(model.width)),
                            Height = (int)Math.Floor(double.Parse(model.height)),
                        };
                        var drm = (DALReturnModel<ImageAnnotation>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ImageAnnotation),
                            new Func<ImageAnnotation, BatchProcessResultModel, DALReturnModel<ImageAnnotation>>(ImageAnnotationDAL.Add), imageAnnot, bpr);
                        bpr = drm.BPR;
                        var newId = drm.ReturnObject.Id;
                        model.objectType = (int)UT.SL.Model.Enumeration.ObjectType.ImageAnnotation;
                        model.objectId = newId;
                    }
                    else
                    {
                        var imageAnnot = ImageAnnotationDAL.Get(Int32.Parse(model.id));
                        imageAnnot.TopCoord = (int)Math.Floor(double.Parse(model.height));
                        imageAnnot.LeftCoord = (int)Math.Floor(double.Parse(model.left));
                        imageAnnot.Width = (int)Math.Floor(double.Parse(model.width));
                        imageAnnot.Height = (int)Math.Floor(double.Parse(model.height));
                        var drm = (DALReturnModel<ImageAnnotation>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ImageAnnotation),
                            new Func<ImageAnnotation, BatchProcessResultModel, DALReturnModel<ImageAnnotation>>(ImageAnnotationDAL.Update), imageAnnot, bpr);
                        bpr = drm.BPR;
                        var newId = drm.ReturnObject.Id;
                        model.objectType = (int)UT.SL.Model.Enumeration.ObjectType.ImageAnnotation;
                        model.objectId = newId;

                    }
                    ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.TagMapper),
                                      new Func<int, int, DALReturnModel<TagMapper>>(TagMapperDAL.DeleteAll), model.objectId, model.objectType);            
                    if (!string.IsNullOrEmpty(model.text))
                    {
                        var tags = model.text.Split(new char[] { '،', ',', ' ' });
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
                                                        CategoryId = category.Id
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
                                                        CategoryId = category.Id
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
                                                    IsValid = false
                                                };
                                                var drm = (DALReturnModel<Tag>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Tag),
                                                      new Func<Tag, DALReturnModel<Tag>>(TagDAL.Add), newTag);
                                                tag = drm.ReturnObject;
                                            }
                                        }
                                    }
                                    if (tag != null)
                                    {
                                        var tagMappr = TagMapperDAL.GetByDetail(model.objectId, model.objectType, tag.Id);
                                        if (tagMappr == null)
                                        {
                                            var newMap = new TagMapper
                                            {
                                                CreateDate = DateTime.Now,
                                                ObjectId = model.objectId,
                                                ObjectType = model.objectType,
                                                TagId = tag.Id
                                            };
                                            var drm = (DALReturnModel<TagMapper>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.TagMapper),
                                                     new Func<TagMapper, DALReturnModel<TagMapper>>(TagMapperDAL.Add), newMap);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return Content("true");
                }
                return Content("false");
            }
            catch
            {
                return Content("false");
            }
        }

        public ActionResult DeleteImageAnnotations(ImageAnnotationCoord model)
        {
            try
            {
                var bpr = new BatchProcessResultModel();
                if (model != null)
                {
                    ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ImageAnnotation),
                                      new Func<ImageAnnotation, DALReturnModel<ImageAnnotation>>(ImageAnnotationDAL.Delete), new ImageAnnotation { Id = Int32.Parse(model.id) });           
                }
                return Content("true");
            }
            catch
            {
                return Content("false");
            }
        }

    }
}
