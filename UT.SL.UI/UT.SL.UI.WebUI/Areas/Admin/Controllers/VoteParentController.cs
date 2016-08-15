using System;
using System.Linq;
using System.Web.Mvc;
using UT.SL.Helper;
using UT.SL.UI.WebUI.Controllers;
using UT.SL.Model;
using UT.SL.Data.LINQ;
using UT.SL.DAL;
using UT.SL.BLL;
using UT.SL.Model.Enumeration;


namespace UT.SL.UI.WebUI.Areas.Admin.Controllers
{


    [Authorize()]
    public class VoteParentController : BaseController
    {

        /// <summary>
        /// For Voting
        /// </summary>
        /// <param name="ObjectId"></param>
        /// <param name="Type"></param>
        /// <param name="vote"> 0 = initial , 1 = upvote , 2 = downvote</param>
        /// <returns></returns>
        public ActionResult VoteComponentResource(int ObjectId = 0, int Type = 0, byte vote = 0, int voteValue = 0, byte viewType = 0)
        {
            ViewBag.Type = Type;
            ViewBag.viewType = viewType;
            var model = new FormModel<VoteParent, VoteModel>();
            var removeFlag = false;
            if (ObjectId == 0 || Type == 0)
            {
                var bpr = new BatchProcessResultModel();
                bpr.AddError(UT.SL.Model.Resource.App_Errors.ObjectIdMissing, true, true);
                return PartialView("ErrorDialog", bpr);
            }
            var drm = (DALReturnModel<VoteParent>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.VoteParent),
                                       new Func<VoteParent, DALReturnModel<VoteParent>>(VoteParentDAL.AddNotExist), new VoteParent { ObjectId = ObjectId, ObjectType = Type });
            model.FormObject = drm.ReturnObject;
            var exitsVote = VoteDAL.GetIfExist(model.FormObject.Id, CurrentUser.Id);
            if (vote == 0)
            {
                if (exitsVote == null)
                {
                    model.ExtraKnownData = new VoteModel
                    {
                        MyReaction = 0
                    };
                }
                else
                {
                    model.ExtraKnownData = new VoteModel
                    {
                        MyReaction = exitsVote.VoteValue
                    };
                }
            }
            else if (vote == 1)
            {
                if (exitsVote == null)
                {
                    var newVote = new Vote
                    {
                        UserId = CurrentUser.Id,
                        VoteValue = voteValue,
                        ParentId = model.FormObject.Id,
                        CreateDate = DateTime.Now
                    };
                    var drm2 = (DALReturnModel<Vote>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Vote),
                                       new Func<Vote, DALReturnModel<Vote>>(VoteDAL.AddReaction), newVote);
                    //update Credit
                    UserInfoManager.UpdateUserActSummary(CurrentUser.Id, ObjectId, Type, (int)UT.SL.Model.Enumeration.ActivityType.Vote);
                    ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.Vote, ObjectId, Type, CurrentUser.Id);
                    //update credit
                    model.FormObject.Count++;
                }
                else if (exitsVote.VoteValue != voteValue)
                {
                    exitsVote.VoteValue = voteValue;
                    var drm2 = (DALReturnModel<Vote>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Vote),
                                       new Func<Vote, DALReturnModel<Vote>>(VoteDAL.UpdateReaction), exitsVote);
                }
                else if (exitsVote.VoteValue == voteValue)
                {
                    //update Credit
                    UserInfoManager.DeleteUserActSummary(CurrentUser.Id, ObjectId, Type, (int)UT.SL.Model.Enumeration.ActivityType.Vote);
                    ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.DeleteVote, ObjectId, Type, CurrentUser.Id);
                    //update credit
                    var drm2 = (DALReturnModel<Vote>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Vote),
                                       new Func<Vote, DALReturnModel<Vote>>(VoteDAL.Delete), exitsVote);
                    model.FormObject.Count--;
                    removeFlag = true;
                }
                var drm3 = (DALReturnModel<VoteParent>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.VoteParent),
                                      new Func<VoteParent, DALReturnModel<VoteParent>>(VoteParentDAL.UpdateReaction), model.FormObject);                   
                model.ExtraKnownData = new VoteModel
                {
                    MyReaction = voteValue
                };
            }
            if (removeFlag)
            {
                model.ExtraKnownData = new VoteModel
                {
                    MyReaction = 0
                };
            }

            // update froum score
            if (Type == 9)
            {
                //ForumController.updateForumScores(CurrentUser.Id, ObjectId);
            }

            return PartialView(model);
        }

        public void VoteComponentResourceForAssignment(int CurrentUserId, int ObjectId = 0, int Type = 0, byte vote = 0, int voteValue = 0, byte viewType = 0)
        {
            ViewBag.Type = Type;
            ViewBag.viewType = viewType;
            var model = new FormModel<VoteParent, VoteModel>();
            var removeFlag = false;
            if (ObjectId == 0 || Type == 0)
            {
                var bpr = new BatchProcessResultModel();
                bpr.AddError(UT.SL.Model.Resource.App_Errors.ObjectIdMissing, true, true);

            }
            var drm = (DALReturnModel<VoteParent>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.VoteParent),
                                      new Func<VoteParent, DALReturnModel<VoteParent>>(VoteParentDAL.AddNotExist), new VoteParent { ObjectId = ObjectId, ObjectType = Type });
            model.FormObject = drm.ReturnObject;
            var exitsVote = VoteDAL.GetIfExist(model.FormObject.Id, CurrentUserId);
            if (vote == 0)
            {
                if (exitsVote == null)
                {
                    model.ExtraKnownData = new VoteModel
                    {
                        MyReaction = 0
                    };
                }
                else
                {
                    model.ExtraKnownData = new VoteModel
                    {
                        MyReaction = exitsVote.VoteValue
                    };
                }
            }
            else if (vote == 1)
            {
                ViewBag.New = true;
                if (exitsVote == null)
                {
                    var newVote = new Vote
                    {
                        UserId = CurrentUserId,
                        VoteValue = voteValue,
                        ParentId = model.FormObject.Id,
                        CreateDate = DateTime.Now
                    };
                    var drm2 = (DALReturnModel<Vote>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Vote),
                           new Func<Vote, DALReturnModel<Vote>>(VoteDAL.AddReaction), newVote);
                    model.FormObject.Count++;
                }
                else if (exitsVote.VoteValue != voteValue)
                {
                    exitsVote.VoteValue = voteValue;
                    var drm2 = (DALReturnModel<Vote>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Vote),
                                     new Func<Vote, DALReturnModel<Vote>>(VoteDAL.UpdateReaction), exitsVote);
                }
                else if (exitsVote.VoteValue == voteValue)
                {
                    var drm2 = (DALReturnModel<Vote>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Vote),
                                       new Func<Vote, DALReturnModel<Vote>>(VoteDAL.Delete), exitsVote);                   
                    model.FormObject.Count--;
                    removeFlag = true;
                }
                var drm3 = (DALReturnModel<VoteParent>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.VoteParent),
                                      new Func<VoteParent, DALReturnModel<VoteParent>>(VoteParentDAL.UpdateReaction), model.FormObject);                                    
                model.ExtraKnownData = new VoteModel
                {
                    MyReaction = voteValue
                };
            }
            if (removeFlag)
            {
                model.ExtraKnownData = new VoteModel
                {
                    MyReaction = 0
                };
            }
            // update froum score
            if (Type == 9)
            {
                // ForumController.updateForumScores(CurrentUser.Id, ObjectId);
            }
        }

        public ActionResult VoteComponent(int ObjectId = 0, int Type = 0, byte vote = 0, bool button = false)
        {
            var model = new FormModel<VoteParent, VoteModel>();
            var removeFlag = false;
            if (ObjectId == 0 || Type == 0)
            {
                var bpr = new BatchProcessResultModel();
                bpr.AddError(UT.SL.Model.Resource.App_Errors.ObjectIdMissing, true, true);
                return PartialView("ErrorDialog", bpr);
            }
            if (button)
                ViewBag.Button = true;
            var drm = (DALReturnModel<VoteParent>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.VoteParent),
                                     new Func<VoteParent, DALReturnModel<VoteParent>>(VoteParentDAL.AddNotExist), new VoteParent { ObjectId = ObjectId, ObjectType = Type });
            model.FormObject = drm.ReturnObject;           
            var exitsVote = VoteDAL.GetIfExist(model.FormObject.Id, CurrentUser.Id);
            if (vote == 0)
            {
                if (exitsVote == null)
                {
                    model.ExtraKnownData = new VoteModel
                    {
                        DownVote = false,
                        UpVote = false
                    };
                }
                else
                {
                    model.ExtraKnownData = new VoteModel
                    {
                        DownVote = !exitsVote.Updown,
                        UpVote = exitsVote.Updown
                    };
                }
            }
            else if (vote == 1)
            {
                ViewBag.New = true;
                if (exitsVote == null)
                {
                    var newVote = new Vote
                    {
                        UserId = CurrentUser.Id,
                        Updown = true,
                        ParentId = model.FormObject.Id,
                        CreateDate = DateTime.Now
                    };
                    var drm2 = (DALReturnModel<Vote>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Vote),
                           new Func<Vote, DALReturnModel<Vote>>(VoteDAL.AddUpDownVote), newVote);
                    //update Credit
                    UserInfoManager.UpdateUserActSummary(CurrentUser.Id, ObjectId, Type, (int)UT.SL.Model.Enumeration.ActivityType.Vote);
                    ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.Vote, ObjectId, Type, CurrentUser.Id);
                    //update credit
                    model.FormObject.UpvoteCount++;
                    model.FormObject.Count++;
                }
                else if (!exitsVote.Updown)
                {
                    model.FormObject.DownvoteCount--;
                    model.FormObject.UpvoteCount++;
                    exitsVote.Updown = true;
                    var drm2 = (DALReturnModel<Vote>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Vote),
                           new Func<Vote, DALReturnModel<Vote>>(VoteDAL.UpdateUpDownVote), exitsVote);
                }
                else
                {
                    model.FormObject.UpvoteCount--;
                    model.FormObject.Count--;
                    var drm2 = (DALReturnModel<Vote>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Vote),
                          new Func<Vote, DALReturnModel<Vote>>(VoteDAL.Delete), exitsVote);              
                    //update Credit
                    UserInfoManager.DeleteUserActSummary(CurrentUser.Id, ObjectId, Type, (int)UT.SL.Model.Enumeration.ActivityType.Vote);
                    ModelManager.UpdateCreditValue((int)UT.SL.Model.Enumeration.ActionType.DeleteVote, ObjectId, Type, CurrentUser.Id);
                    //update credit
                    removeFlag = true;
                }
                var drm3 = (DALReturnModel<VoteParent>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.VoteParent),
                          new Func<VoteParent, DALReturnModel<VoteParent>>(VoteParentDAL.Update), model.FormObject);                                 
                model.ExtraKnownData = new VoteModel
                {
                    DownVote = false,
                    UpVote = true
                };
                ViewBag.SecondTimeAround = true;
            }
            else if (vote == 2)
            {
                ViewBag.New = true;
                if (exitsVote == null)
                {
                    var newVote = new Vote
                    {
                        UserId = CurrentUser.Id,
                        Updown = false,
                        ParentId = model.FormObject.Id,
                        CreateDate = DateTime.Now
                    };
                    var drm2 = (DALReturnModel<Vote>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Vote),
                           new Func<Vote, DALReturnModel<Vote>>(VoteDAL.AddUpDownVote), exitsVote);                
                    model.FormObject.DownvoteCount++;
                    model.FormObject.Count++;
                }
                else if (exitsVote.Updown)
                {
                    model.FormObject.UpvoteCount--;
                    model.FormObject.DownvoteCount++;
                    exitsVote.Updown = false;
                    var drm2 = (DALReturnModel<Vote>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Vote),
                       new Func<Vote, DALReturnModel<Vote>>(VoteDAL.UpdateUpDownVote), exitsVote);
                }
                else
                {
                    model.FormObject.DownvoteCount--;
                    model.FormObject.Count--;
                    var drm2 = (DALReturnModel<Vote>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.Vote),
                      new Func<Vote, DALReturnModel<Vote>>(VoteDAL.Delete), exitsVote);
                    removeFlag = true;
                }
                var drm3 = (DALReturnModel<VoteParent>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.VoteParent),
                      new Func<VoteParent, DALReturnModel<VoteParent>>(VoteParentDAL.Update), model.FormObject);                 
                model.ExtraKnownData = new VoteModel
                {
                    DownVote = true,
                    UpVote = false
                };
                ViewBag.SecondTimeAround = true;
            }
            if (removeFlag)
            {
                model.ExtraKnownData = new VoteModel
                {
                    DownVote = false,
                    UpVote = false
                };
            }
            return PartialView(model);
        }

        public ActionResult CountNewVotes(int Id, int type, DateTime clickedDate)
        {
            var cnt = VoteDAL.CountNewVotes(Id, type, clickedDate, CurrentUser.Id);
            if (cnt > 0)
                ViewBag.Count = cnt;
            return PartialView();
        }

    }
}
