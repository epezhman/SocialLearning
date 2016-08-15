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
    public class ShareController : BaseController
    {
        public ActionResult ShareComponent(int ObjectId = 0, int Type = 0)
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
            return PartialView(model);
        }

        public ActionResult ShareComponentResource(int ObjectId = 0, int Type = 0)
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
            return PartialView(model);
        }

        public ActionResult Share(int ObjectId = 0, int Type = 0, byte viewType = 0)
        {
            var model = new FormModel<ShareModel, ObjectViewModel>();
            if (ObjectId == 0 || Type == 0)
            {
                var bpr = new BatchProcessResultModel();
                bpr.AddError(UT.SL.Model.Resource.App_Errors.ObjectIdMissing, true, true);
                if (Request.IsAjaxRequest())
                {
                    return PartialView("ErrorDialog", bpr);
                }
                return View("ErrorPage", bpr);
            }
            model.FormObject = new ShareModel();
            model.ExtraKnownData = ManageObject.GetSharedObject(ObjectId, Type);
            var thisAlreadyShared = ShareDAL.GetAllObject(ObjectId, Type, CurrentUser.Id);
            var usersSocialGroups = SocialGroupDAL.GetAllByUserId(CurrentUser.Id);
            foreach (var item in thisAlreadyShared.Where(x => x.UserShareId.HasValue && x.UserShareId.Value > 0))
            {
                var thisUser = App_UserDAL.Get(item.UserShareId.Value);
                model.FormObject.ShareUserIds += string.Format("{0} {1}-{2}, ", thisUser.FirstName, thisUser.LastName, thisUser.UserName);
                model.FormObject.HiddentShareUserIds += string.Format("{0}, ", thisUser.GuidId);
            }
            model.FormObject.SocialGroup = new SelectListItems
            {
                SelectedIds = thisAlreadyShared.Where(x => x.SocialGroupId.HasValue && x.SocialGroupId > 0).Select(x => x.SocialGroupId.Value.ToString()).ToArray(),
                Items = usersSocialGroups.Select(x => new SelectListItem { Text = x.Title, Value = x.Id.ToString() })
            };
            if ((Type == 1 || Type == 2) && model.ExtraKnownData.CourseId > 0)
            {
                var usersLearningGroups = new List<LearningGroup>();
                if (Type == 1)
                    usersLearningGroups = LearningGroupDAL.GetAllByUserId(CurrentUser.Id, ObjectId);
                else
                    usersLearningGroups = LearningGroupDAL.GetAllByUserId(CurrentUser.Id, ResourceDAL.Get(ObjectId).CourseId.Value);
                model.FormObject.LearningGroup = new SelectListItems
                {
                    SelectedIds = thisAlreadyShared.Where(x => x.LearningGroupId.HasValue && x.LearningGroupId > 0).Select(x => x.LearningGroupId.Value.ToString()).ToArray(),
                    Items = usersLearningGroups.Select(x => new SelectListItem { Text = x.Title, Value = x.Id.ToString() })
                };
            }
            if (Request.IsAjaxRequest())
            {
                if (viewType == 1)
                    return PartialView("ShareResource", model);
                return PartialView(model);
            }
            if (viewType == 1)
                return PartialView("ShareResource", model);
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Share(FormModel<ShareModel, ObjectViewModel> model)
        {
            var bpr = new BatchProcessResultModel();
            try
            {
                var thisAlreadyShared = ShareDAL.GetAllObject(model.ExtraKnownData.Id, model.ExtraKnownData.Type, CurrentUser.Id);
                var usersSocialGroups = SocialGroupDAL.GetAllByUserId(CurrentUser.Id);
                if (model.FormObject.SocialGroup != null)
                {
                    if (model.FormObject.SocialGroup.SelectedIds.Any())
                    {
                        foreach (var item in model.FormObject.SocialGroup.SelectedIds)
                        {
                            var isThere = thisAlreadyShared.SingleOrDefault(x => x.SocialGroupId == Int32.Parse(item) && x.ObjectId == model.ExtraKnownData.Id && x.Type == model.ExtraKnownData.Type && x.UserId == CurrentUser.Id);
                            if (isThere == null)
                            {
                                var share = new Share
                                {
                                    CreateDate = DateTime.Now,
                                    SocialGroupId = Int32.Parse(item),
                                    Type = model.ExtraKnownData.Type,
                                    ObjectId = model.ExtraKnownData.Id,
                                    UserId = CurrentUser.Id
                                };
                                var drm = (DALReturnModel<Share>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Share),
                                       new Func<Share, BatchProcessResultModel, DALReturnModel<Share>>(ShareDAL.Add), share, bpr);
                                bpr = drm.BPR;
                                var tempGIds = new List<string>();
                                tempGIds.Add(item);
                                ObjectStreamManager.ObjectResourceToStream(model.ExtraKnownData.Id, model.ExtraKnownData.Type, new ToEmergeIds { SocialGroups = tempGIds }, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
                            }
                            else
                            {
                                thisAlreadyShared.Remove(isThere);
                            }
                        }
                    }
                }
                if (model.FormObject.LearningGroup != null)
                {
                    if (model.FormObject.LearningGroup.SelectedIds.Any())
                    {
                        foreach (var item in model.FormObject.LearningGroup.SelectedIds)
                        {
                            var isThere = thisAlreadyShared.SingleOrDefault(x => x.LearningGroupId == Int32.Parse(item) && x.ObjectId == model.ExtraKnownData.Id && x.Type == model.ExtraKnownData.Type && x.UserId == CurrentUser.Id);
                            if (isThere == null)
                            {
                                var share = new Share
                                {
                                    CreateDate = DateTime.Now,
                                    LearningGroupId = Int32.Parse(item),
                                    Type = model.ExtraKnownData.Type,
                                    ObjectId = model.ExtraKnownData.Id,
                                    UserId = CurrentUser.Id
                                };
                                var drm = (DALReturnModel<Share>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Share),
                                      new Func<Share, BatchProcessResultModel, DALReturnModel<Share>>(ShareDAL.Add), share, bpr);
                                bpr = drm.BPR;
                                var tempGIds = new List<string>();
                                tempGIds.Add(item);
                                ObjectStreamManager.ObjectResourceToStream(model.ExtraKnownData.Id, model.ExtraKnownData.Type, new ToEmergeIds { LearningGroups = tempGIds }, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
                            }
                            else
                            {
                                thisAlreadyShared.Remove(isThere);
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(model.FormObject.ShareUserIds) && model.FormObject.ShareUserIds.Split(',').Any())
                {
                    foreach (var item in model.FormObject.ShareUserIds.Split(','))
                    {
                        var tokens = item.Split('-');
                        if (tokens.Count() == 2 && !String.IsNullOrEmpty(tokens.Last().Trim()))
                        {
                            var isThere = thisAlreadyShared.SingleOrDefault(x => x.App_User.UserName == tokens.Last().StringNormalizer().Trim() && x.ObjectId == model.ExtraKnownData.Id && x.Type == model.ExtraKnownData.Type && x.UserId == CurrentUser.Id);
                            var user = App_UserDAL.Get(tokens.Last().StringNormalizer().Trim());
                            if (isThere == null && user != null)
                            {
                                var share = new Share
                                {
                                    CreateDate = DateTime.Now,
                                    UserShareId = user.Id,
                                    Type = model.ExtraKnownData.Type,
                                    ObjectId = model.ExtraKnownData.Id,
                                    UserId = CurrentUser.Id
                                };
                                var drm = (DALReturnModel<Share>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Share),
                                      new Func<Share, BatchProcessResultModel, DALReturnModel<Share>>(ShareDAL.Add), share, bpr);
                                bpr = drm.BPR;
                                ObjectStreamManager.ObjectResourceToStream(model.ExtraKnownData.Id, model.ExtraKnownData.Type, new ToEmergeIds(), ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
                            }
                            else
                            {
                                thisAlreadyShared.Remove(isThere);
                            }
                        }
                    }
                }
                foreach (var item in thisAlreadyShared)
                {
                    ObjectStreamManager.ObjectResourceDeleteFromStream(item.ObjectId, item.Type.Value, ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.ObjectStream));
                    ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Share),
                                      new Func<Share, DALReturnModel<Share>>(ShareDAL.Delete), item);
                }
            }
            catch
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.BprMainUnknownError, true, true);
            }
            if (bpr.Failed == 0)
            {
                bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
            }
            return PartialView("ProcessResult", bpr);
        }

        public ActionResult FindViaWhatItWasShared(int objectId, int objectType)
        {
            var model = ObjectStreamManager.FindViaWhatItWasShared(objectId, objectType, CurrentUser.Id);
            return PartialView(model);
        }

    }
}
