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
    public class CommentController : BaseController
    {
        [HttpPost()]
        public ActionResult DeleteComment(Comment model)
        {
            var result = "0";
            try
            {
                var obj = CommentDAL.Get(model.Id);
                int objid = obj.ObjectId;
                int objtype = obj.Type;
                //update credit value
                UserInfoManager.DeleteUserActSummary(CurrentUser.Id, obj.ObjectId, obj.Type, (int)UT.SL.Model.Enumeration.ActivityType.Comment);
                ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.DeleteComment, obj.Id, objtype, obj.OwnerId);
                //update credit value
                var drm = (DALReturnModel<Comment>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Comment),
                                       new Func<Comment, DALReturnModel<Comment>>(CommentDAL.Delete), model);
                if (drm.ReturnObject.Id > 0)
                {
                    result = string.Format("{0}", CommentDAL.GetAllCount(objid, objtype));
                    ////update credit value
                    //ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.DeleteComment, objid, objtype,obj.OwnerId);
                    ////update credit value
                }

            }
            catch (System.Exception)
            {
            }
            return Content(result);
        }

        public ActionResult CommentComponentResource(int ObjectId = 0, int Type = 0)
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
            model.Count = CommentDAL.GetAllCount(ObjectId, Type);

            return PartialView(model);
        }

        public ActionResult CommentComponent(int ObjectId = 0, int Type = 0)
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
            model.Count = CommentDAL.GetAllCount(ObjectId, Type);

            if (Type == (int)UT.SL.Model.Enumeration.ObjectType.Forum)
            {
                ViewBag.Forum = true;
            }
            return PartialView(model);
        }

        public ActionResult Comment(string comment, int ObjectId = 0, int Type = 0, int actionType = 0, int commentId = 0)
        {
            var model = new FormModel<List<Comment>, ObjectViewModel>
            {
                FormObject = new List<Comment>(),
                ExtraKnownData = new ObjectViewModel()
            };
            if (ObjectId == 0 || Type == 0)
            {
                var bpr = new BatchProcessResultModel();
                bpr.AddError(UT.SL.Model.Resource.App_Errors.ObjectIdMissing, true, true);
                return PartialView("ErrorDialog", bpr);
            }
            if (!string.IsNullOrEmpty(comment) && commentId == 0)
            {
                var newComment = new Comment
                {
                    CreateDate = DateTime.Now,
                    ObjectId = ObjectId,
                    Type = Type,
                    OwnerId = CurrentUser.Id,
                    Title = comment
                };
                var drm = ((DALReturnModel<Comment>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Comment),
                           new Func<Comment, DALReturnModel<Comment>>(CommentDAL.Add), newComment));
            }
            else if (!string.IsNullOrEmpty(comment) && commentId > 0)
            {
                var oldComment = CommentDAL.Get(commentId);
                if (oldComment != null)
                {
                    oldComment.Title = comment;
                }
                var drm = ((DALReturnModel<Comment>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Comment),
                           new Func<Comment, DALReturnModel<Comment>>(CommentDAL.Update), oldComment));
            }
            if (commentId > 0 && actionType == 2)
            {
                ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Comment),
                                       new Func<Comment, DALReturnModel<Comment>>(CommentDAL.Delete), CommentDAL.Get(commentId));
            }
            model.FormObject.AddRange(CommentDAL.GetAll(ObjectId, Type));
            if (actionType == 0)
            {
                model.FormObject.Add(new Comment { App_User = CurrentUser });
            }
            else if (actionType == 1)
            {
                model.FormObject.Add(CommentDAL.Get(commentId));
            }
            else if (actionType == 2)
            {
                model.FormObject.Add(new Comment { App_User = CurrentUser });
            }
            model.ExtraKnownData = ManageObject.GetSharedObject(ObjectId, Type);
            model.ExtraData.Add(CommentDAL.GetAllCount(ObjectId, Type).ToString());
            return PartialView("CommentList", model);
        }

        public ActionResult CommentThread(string comment,DateTime? clickedDate, int ObjectId = 0, int Type = 0, int actionType = 0, int commentId = 0)
        {
            var model = new FormModel<List<Comment>, ObjectViewModel>
            {
                FormObject = new List<Comment>(),
                ExtraKnownData = new ObjectViewModel()
            };
            if (ObjectId == 0 || Type == 0)
            {
                var bpr = new BatchProcessResultModel();
                bpr.AddError(UT.SL.Model.Resource.App_Errors.ObjectIdMissing, true, true);
                return PartialView("ErrorDialog", bpr);
            }
            if (clickedDate.HasValue)
                ViewBag.clickedDate = clickedDate.Value;
            if (!string.IsNullOrEmpty(comment) && commentId == 0)
            {
                var newComment = new Comment
                {
                    CreateDate = DateTime.Now,
                    ObjectId = ObjectId,
                    Type = Type,
                    OwnerId = CurrentUser.Id,
                    Title = comment
                };
                var drm = ((DALReturnModel<Comment>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Comment),
                           new Func<Comment, DALReturnModel<Comment>>(CommentDAL.Add), newComment));
            }
            else if (!string.IsNullOrEmpty(comment) && commentId > 0)
            {
                var oldComment = CommentDAL.Get(commentId);
                if (oldComment != null)
                {
                    oldComment.Title = comment;
                }
                var drm = ((DALReturnModel<Comment>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Comment),
                          new Func<Comment, DALReturnModel<Comment>>(CommentDAL.Update), oldComment));
            }
            if (commentId > 0 && actionType == 2)
            {
                ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Comment),
                                      new Func<Comment, DALReturnModel<Comment>>(CommentDAL.Delete), CommentDAL.Get(commentId));
            }
            model.FormObject.AddRange(CommentDAL.GetAll(ObjectId, Type));
            if (actionType == 0)
            {
                model.FormObject.Add(new Comment { App_User = CurrentUser });
            }
            else if (actionType == 1)
            {
                model.FormObject.Add(CommentDAL.Get(commentId));
            }
            else if (actionType == 2)
            {
                model.FormObject.Add(new Comment { App_User = CurrentUser });
            }
            model.ExtraKnownData = ManageObject.GetSharedObject(ObjectId, Type);
            model.ExtraData.Add(CommentDAL.GetAllCount(ObjectId, Type).ToString());
            return PartialView(model);
        }
        public ActionResult CommentThreadAny(int ObjectId = 0, int Type = 0)
        {
            ViewBag.Any = CommentDAL.CommentAny(ObjectId, Type);
            return PartialView();
        }

        public ActionResult PostComment(string comment, int ObjectId = 0, int Type = 0, int actionType = 0, int commentId = 0)
        {
            if (!string.IsNullOrEmpty(comment) && commentId == 0)
            {
                var newComment = new Comment
                {
                    CreateDate = DateTime.Now,
                    ObjectId = ObjectId,
                    Type = Type,
                    OwnerId = CurrentUser.Id,
                    Title = comment.StringNormalizer()
                };
                var drm = (DALReturnModel<Comment>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Comment),
                          new Func<Comment, DALReturnModel<Comment>>(CommentDAL.Add), newComment);
                commentId = drm.ReturnObject.Id;
                //update Credit
                ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.Comment, ObjectId, Type, CurrentUser.Id);
                UserInfoManager.UpdateUserActSummary(CurrentUser.Id, ObjectId, Type, (int)UT.SL.Model.Enumeration.ActivityType.Comment);

                //update credit
            }
            else if (!string.IsNullOrEmpty(comment) && commentId > 0)
            {
                var oldComment = CommentDAL.Get(commentId);
                if (oldComment != null)
                {
                    oldComment.Title = comment.StringNormalizer();
                }
                var drm = (DALReturnModel<Comment>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Comment),
                          new Func<Comment, DALReturnModel<Comment>>(CommentDAL.Update), oldComment);
                commentId = commentId * -1;
            }
            if (commentId > 0 && actionType == 2)
            {
                ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Comment),
                                      new Func<Comment, DALReturnModel<Comment>>(CommentDAL.Delete), CommentDAL.Get(commentId));
                commentId = commentId * -1;
            }
            var result = string.Format("{0},{1},{2},{3}", commentId, ObjectId, Type, CommentDAL.GetAllCount(ObjectId, Type));
            return Content(result);
        }

        public ActionResult GetOneComment(int commentId)
        {
            var model = CommentDAL.Get(commentId);
            return PartialView(model);
        }

        public ActionResult GetOneCommentAfterEdit(int commentId)
        {
            var model = CommentDAL.Get(commentId);
            return PartialView(model);
        }

        public ActionResult EditOneComment(int Id)
        {
            var model = CommentDAL.Get(Id);
            return PartialView(model);
        }

        public ActionResult CountNewComments(int Id, int type, DateTime clickedDate)
        {
            var cnt = CommentDAL.CountNewComments(Id, type, clickedDate, CurrentUser.Id);
            if (cnt > 0)
                ViewBag.Count = cnt;
            return PartialView();
        }

    }
}
