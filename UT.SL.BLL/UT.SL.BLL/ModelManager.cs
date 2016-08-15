using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using UT.SL.DAL;
using UT.SL.Data.LINQ;
using UT.SL.Model;

namespace UT.SL.BLL
{
    /// <summary>
    /// 
    /// </summary>
    public class ModelManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <param name="userId"></param>
        /// <param name="gradeValue"></param>
        public static void UpdateCreditValue(int actionType, int objectId, int objectType, int userId, double? gradeValue = 0)
        {
            if (actionType == (int)UT.SL.Model.Enumeration.ActionType.Create ||
                actionType == (int)UT.SL.Model.Enumeration.ActionType.Comment ||
                actionType == (int)UT.SL.Model.Enumeration.ActionType.Tag ||
                actionType == (int)UT.SL.Model.Enumeration.ActionType.Vote)
            {
                if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission)
                {
                    var model = AssignmentSubmissionDAL.Get(objectId);
                    UpdateContentProfile(actionType, model.AssignmentId, (int)UT.SL.Model.Enumeration.ObjectType.Assignment);
                    UpdateUserProfile(actionType, objectId, objectType, userId);
                }
                else if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussionPost)
                {
                    var model = ForumDiscussionPostDAL.Get(objectId);
                    var parentModel = ForumDiscussionDAL.Get(model.ParentId);
                    if (parentModel.ParentId != null)
                    {
                        UpdateContentProfile(actionType, parentModel.ParentId.Value, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);
                        UpdateUserProfile(actionType, objectId, objectType, userId);
                    }
                    else
                    {
                        UpdateContentProfile(actionType, parentModel.Id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);
                        UpdateUserProfile(actionType, objectId, objectType, userId);
                    }
                }
                else if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion)
                {
                    var model = ForumDiscussionDAL.Get(objectId);
                    if (model.ParentId != null)
                    {
                        UpdateContentProfile(actionType, model.ParentId.Value, objectType);
                        UpdateUserProfile(actionType, objectId, objectType, userId);
                    }
                    else
                    {
                        UpdateContentProfile(actionType, objectId, objectType);
                        UpdateUserProfile(actionType, objectId, objectType, userId);
                    }
                }
                else if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.Comment)
                {
                    var comment = CommentDAL.Get(objectId);
                    if (comment.Type == (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission)
                    {
                        var model = AssignmentSubmissionDAL.Get(objectId);
                        UpdateContentProfile(actionType, model.AssignmentId, (int)UT.SL.Model.Enumeration.ObjectType.Assignment);
                        UpdateUserProfile(actionType, objectId, objectType, userId);
                    }
                    else if (comment.Type == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussionPost)
                    {
                        var model = ForumDiscussionPostDAL.Get(objectId);
                        var parentModel = ForumDiscussionDAL.Get(model.ParentId);
                        if (parentModel.ParentId != null)
                        {
                            UpdateContentProfile(actionType, parentModel.ParentId.Value, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);
                            UpdateUserProfile(actionType, objectId, objectType, userId);
                        }
                        else
                        {
                            UpdateContentProfile(actionType, parentModel.Id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);
                            UpdateUserProfile(actionType, objectId, objectType, userId);
                        }
                    }
                    else if (comment.Type == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion)
                    {
                        var model = ForumDiscussionDAL.Get(objectId);
                        if (model.ParentId != null)
                        {
                            UpdateContentProfile(actionType, model.ParentId.Value, objectType);
                            UpdateUserProfile(actionType, objectId, objectType, userId);
                        }
                        else
                        {
                            UpdateContentProfile(actionType, objectId, objectType);
                            UpdateUserProfile(actionType, objectId, objectType, userId);
                        }
                    }
                    else
                    {
                        UpdateContentProfile(actionType, comment.ObjectId, comment.Type);
                        UpdateUserProfile(actionType, comment.ObjectId, comment.Type, userId);
                    }
                }
                else
                {
                    UpdateContentProfile(actionType, objectId, objectType);
                    UpdateUserProfile(actionType, objectId, objectType, userId);
                }
            }
            else if (actionType == (int)UT.SL.Model.Enumeration.ActionType.Delete)
            {

                if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.Resource ||
                    objectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment ||
                    objectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum)
                {
                    DeleteContentProfile(actionType, objectId, objectType, userId);
                    var comments = CommentDAL.GetAll(objectId, objectType);
                    foreach (var comment in comments)
                    {
                        UpdateUserProfile((int)UT.SL.Model.Enumeration.ActionType.DeleteComment, objectId, objectType, comment.OwnerId);
                        var voteParentForComment = VoteParentDAL.Get(comment.Id, (int)UT.SL.Model.Enumeration.ObjectType.Comment);
                        if (voteParentForComment != null)
                        {
                            if (voteParentForComment.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment)
                            {
                                var votes = VoteDAL.GetByParent(voteParentForComment.Id);
                                foreach (var vote in votes)
                                {
                                    UpdateUserProfile((int)UT.SL.Model.Enumeration.ActionType.DeleteVote, objectId, objectType, vote.UserId);
                                }
                            }
                            if (voteParentForComment.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Comment)
                            {
                                var votes = VoteDAL.GetByParent(voteParentForComment.Id);
                                foreach (var vote in votes)
                                {
                                    UpdateUserProfile((int)UT.SL.Model.Enumeration.ActionType.DeleteVote, comment.ObjectId, objectType, vote.UserId);
                                }

                            }
                        }
                    }
                    var tagMappers = TagMapperDAL.GetAll(objectId, objectType);
                    foreach (var tagMapper in tagMappers)
                    {
                        UpdateUserProfile((int)UT.SL.Model.Enumeration.ActionType.DeleteTag, objectId, objectType, tagMapper.UserId.Value);
                    }
                    var voteParent = VoteParentDAL.Get(objectId, objectType);
                    if (voteParent != null)
                    {
                        if (voteParent.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment)
                        {
                            var votes = VoteDAL.GetByParent(voteParent.Id);
                            foreach (var vote in votes)
                            {
                                UpdateUserProfile((int)UT.SL.Model.Enumeration.ActionType.DeleteVote, objectId, objectType, vote.UserId);
                            }
                        }
                    }

                    UpdateUserProfile(actionType, objectId, objectType, userId);
                }
                else if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission)
                {
                    var model = AssignmentSubmissionDAL.Get(objectId);
                    var grades = GradeDAL.GetAll(objectId, objectType);
                    if (grades.Count != 0)
                    {
                        UpdateUserKnowledgeProfile((int)UT.SL.Model.Enumeration.ActionType.Grade, objectId, objectType, 0, -(grades.LastOrDefault().GradeValue));
                    }
                    UpdateContentProfile(actionType, model.AssignmentId, (int)UT.SL.Model.Enumeration.ObjectType.Assignment);
                    UpdateUserProfile(actionType, objectId, objectType, userId);
                }
                else if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussionPost)
                {
                    var model = ForumDiscussionPostDAL.Get(objectId);
                    var parentModel = ForumDiscussionDAL.Get(model.ParentId);
                    //var grade = ForumDiscussionPostGradeDAL.GetWithDiscussionReply(model.Id);
                    //if (grade != null)
                    //{
                    //    UpdateUserKnowledgeProfile((int)UT.SL.Model.Enumeration.ActionType.Grade, objectId, objectType, 0, -grade.grade);
                    //}
                    if (parentModel.ParentId != null)
                    {
                        UpdateContentProfile(actionType, parentModel.ParentId.Value, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);
                        UpdateUserProfile(actionType, objectId, objectType, userId);
                    }
                    else
                    {
                        //UpdateContentProfile(actionType, parentModel.Id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);
                        DeleteContentProfile(actionType, parentModel.Id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion, userId);
                        UpdateUserProfile(actionType, objectId, objectType, userId);
                    }


                }
                else if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion)
                {
                    var model = ForumDiscussionDAL.Get(objectId);
                    //var grade = ForumDiscussionPostGradeDAL.GetWithDiscussion(model.Id);
                    //if (grade != null)
                    //{
                    //    UpdateUserKnowledgeProfile((int)UT.SL.Model.Enumeration.ActionType.Grade, objectId, objectType, 0, -grade.grade);
                    //}
                    var tagMappers = TagMapperDAL.GetAll(objectId, objectType);
                    foreach (var tagMapper in tagMappers)
                    {
                        UpdateUserProfile((int)UT.SL.Model.Enumeration.ActionType.DeleteTag, objectId, objectType, tagMapper.UserId.Value);
                        UpdateContentProfile((int)UT.SL.Model.Enumeration.ActionType.DeleteTag, objectId, objectType);
                    }
                    if (model.ParentId != null)
                    {
                        UpdateContentProfile(actionType, model.ParentId.Value, objectType);
                        UpdateUserProfile(actionType, objectId, objectType, userId);
                    }
                    else
                    {
                        //UpdateContentProfile(actionType, objectId, objectType);
                        DeleteContentProfile(actionType, objectId, objectType, userId);
                        UpdateUserProfile(actionType, objectId, objectType, userId);
                    }
                }
            }
            else if (actionType == (int)UT.SL.Model.Enumeration.ActionType.DeleteComment ||
                     actionType == (int)UT.SL.Model.Enumeration.ActionType.DeleteTag ||
                     actionType == (int)UT.SL.Model.Enumeration.ActionType.DeleteVote)
            {
                if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion)
                {
                    var model = ForumDiscussionDAL.Get(objectId);
                    if (model.ParentId != null)
                    {
                        UpdateContentProfile(actionType, model.ParentId.Value, objectType);
                        UpdateUserProfile(actionType, objectId, objectType, userId);
                    }
                    else
                    {
                        UpdateContentProfile(actionType, objectId, objectType);
                        UpdateUserProfile(actionType, objectId, objectType, userId);
                    }
                }
                else if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussionPost)
                {
                    var model = ForumDiscussionPostDAL.Get(objectId);
                    var parentmodel = ForumDiscussionDAL.Get(model.ParentId);
                    if (parentmodel.ParentId != null)
                    {
                        UpdateContentProfile(actionType, parentmodel.Id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);
                        UpdateUserProfile(actionType, objectId, objectType, userId);
                    }
                    else
                    {
                        UpdateContentProfile(actionType, parentmodel.Id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);
                        UpdateUserProfile(actionType, objectId, objectType, userId);
                    }
                }
                else if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.Comment)
                {
                    var comment = CommentDAL.Get(objectId);
                    if (comment.Type == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion)
                    {
                        var model = ForumDiscussionDAL.Get(objectId);
                        if (model.ParentId != null)
                        {
                            UpdateContentProfile(actionType, model.ParentId.Value, objectType);
                            UpdateUserProfile(actionType, objectId, objectType, userId);
                        }
                        else
                        {
                            UpdateContentProfile(actionType, objectId, objectType);
                            UpdateUserProfile(actionType, objectId, objectType, userId);
                        }
                    }
                    else if (comment.Type == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussionPost)
                    {
                        var model = ForumDiscussionPostDAL.Get(objectId);
                        var parentmodel = ForumDiscussionDAL.Get(model.ParentId);
                        if (parentmodel.ParentId != null)
                        {
                            UpdateContentProfile(actionType, parentmodel.Id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);
                            UpdateUserProfile(actionType, objectId, objectType, userId);
                        }
                        else
                        {
                            UpdateContentProfile(actionType, parentmodel.Id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);
                            UpdateUserProfile(actionType, objectId, objectType, userId);
                        }
                    }
                    else
                    {
                        UpdateContentProfile(actionType, comment.ObjectId, comment.Type);
                        UpdateUserProfile(actionType, comment.ObjectId, comment.Type, userId);
                    }
                }
                else
                {
                    if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.Resource ||
                        objectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment)
                    {

                        var comment = CommentDAL.Get(objectId);
                        //if (comment != null)
                        //{
                        //    var voteParent = VoteParentDAL.Get(comment.Id, (int)UT.SL.Model.Enumeration.ObjectType.Comment);
                        //    if (voteParent != null)
                        //    {
                        //        if (voteParent.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment)
                        //        {
                        //            var votes = VoteDAL.GetByParent(voteParent.Id);
                        //            foreach (var vote in votes)
                        //            {
                        //                UpdateUserProfile((int)UT.SL.Model.Enumeration.ActionType.DeleteVote, objectId, objectType, vote.UserId);
                        //            }
                        //        }
                        //        if (voteParent.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Comment)
                        //        {
                        //            var votes = VoteDAL.GetByParent(voteParent.Id);
                        //            foreach (var vote in votes)
                        //            {
                        //                UpdateContentProfile((int)UT.SL.Model.Enumeration.ActionType.DeleteVote, comment.ObjectId, objectType);
                        //                UpdateUserProfile((int)UT.SL.Model.Enumeration.ActionType.DeleteVote, comment.ObjectId, objectType, vote.UserId);
                        //            }

                        //        }
                        //    }
                        //    UpdateContentProfile(actionType, comment.ObjectId, objectType);
                        //    UpdateUserProfile(actionType, comment.ObjectId, objectType, userId);
                        //}
                        if (actionType == (int)UT.SL.Model.Enumeration.ActionType.DeleteTag ||
                               actionType == (int)UT.SL.Model.Enumeration.ActionType.DeleteVote)
                        {
                            UpdateContentProfile(actionType, objectId, objectType);
                            UpdateUserProfile(actionType, objectId, objectType, userId);
                        }
                    }


                }
            }
            else if (actionType == (int)UT.SL.Model.Enumeration.ActionType.Grade)
            {
                UpdateUserKnowledgeProfile(actionType, objectId, objectType, 0, gradeValue);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        public static void UpdateContentProfile(int actionType, int objectId, int objectType)
        {
            int? courseId = null;
            if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.Resource)
            {
                var model = ResourceDAL.Get(objectId);
                courseId = model.CourseId;

            }
            if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment)
            {
                var model = AssignmentDAL.Get(objectId);
                courseId = model.CourseId;

            }
            if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum)
            {
                var model = ForumDAL.Get(objectId);
                courseId = model.CourseId;

            }
            if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion)
            {
                var model = ForumDiscussionDAL.Get(objectId);
                courseId = model.Forum.CourseId;
            }
            var contentPopularityModel = ContentPopularityModelDAL.GetIfExist(objectId, objectType, courseId);
            if (contentPopularityModel == null)
            {
                contentPopularityModel = new ContentPopularityModel
                {

                    CourseId = courseId,
                    ObjectId = objectId,
                    ObjectType = objectType,
                    SoActValue = actionType,
                    CreateDate = DateTime.Now

                };
                ContentPopularityModelDAL.Add(contentPopularityModel);
            }
            else
            {
                contentPopularityModel.SoActValue += actionType;
                ContentPopularityModelDAL.Update(contentPopularityModel);
            }

        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <param name="userId"></param>
        /// <param name="gradeValue"></param>
        public static void UpdateUserProfile(int actionType, int objectId, int objectType, int userId, double? gradeValue = 0)
        {
            if (actionType != (int)UT.SL.Model.Enumeration.ActionType.Grade)
                UpdateUserSoActProfile(actionType, objectId, objectType, userId);
            if (actionType == (int)UT.SL.Model.Enumeration.ActionType.Grade)
               UpdateUserKnowledgeProfile(actionType, objectId, objectType, userId, gradeValue);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <param name="userId"></param>
        public static void UpdateUserSoActProfile(int actionType, int objectId, int objectType, int userId)
        {
            int? courseId = null;
            List<Topic> topics = null;
            if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.Resource)
            {
                var model = ResourceDAL.Get(objectId);
                courseId = model.CourseId;
                if (userId == 0)
                    userId = model.CreateUserId;
                topics = ObjectTopicMapperDAL.GetAll(objectId, objectType);
            }
            else if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment)
            {
                var model = AssignmentDAL.Get(objectId);
                courseId = model.CourseId;
                if (userId == 0)
                    userId = model.CreateUserId;
                topics = ObjectTopicMapperDAL.GetAll(objectId, objectType);
            }
            else if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum)
            {
                var model = ForumDAL.Get(objectId);
                courseId = model.CourseId;
                if (userId == 0)
                    userId = model.CreateUserId;
                topics = ObjectTopicMapperDAL.GetAll(objectId, objectType);

            }
            else if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion)
            {
                var model = ForumDiscussionDAL.Get(objectId);
                courseId = model.Forum.CourseId;
                if (userId == 0)
                    userId = model.UserId;
                if (model.ParentId == null)
                    topics = ObjectTopicMapperDAL.GetAll(objectId, objectType);
                else
                    topics = ObjectTopicMapperDAL.GetAll(model.ParentId.Value, objectType);

            }
            else if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission)
            {
                var model = AssignmentSubmissionDAL.Get(objectId);
                courseId = model.Assignment.CourseId;
                if (userId == 0)
                    userId = model.UserId;
                topics = ObjectTopicMapperDAL.GetAll(model.Assignment.Id, (int)UT.SL.Model.Enumeration.ObjectType.Assignment);

            }
            else if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussionPost)
            {
                var model = ForumDiscussionPostDAL.Get(objectId);
                courseId = model.ForumDiscussion.Forum.CourseId;
                if (userId == 0)
                    userId = model.UserId;
                var parentModel = ForumDiscussionDAL.Get(model.ParentId);
                if (parentModel.ParentId == null)
                    topics = ObjectTopicMapperDAL.GetAll(parentModel.Id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);
                else
                    topics = ObjectTopicMapperDAL.GetAll(parentModel.ParentId.Value, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);

            }
            else
            {
                topics = ObjectTopicMapperDAL.GetAll(objectId, objectType);
            }

            if (courseId != null)
            {
                if (topics.Count != 0)
                {
                    foreach (var objectTopicMapper in topics)
                    {
                        var userSoActProfile = UserSoActProfileDAL.GetIfExist(userId, courseId, objectTopicMapper.Id);
                        if (userSoActProfile == null)
                        {
                            userSoActProfile = new UserSoActProfile
                            {

                                CourseId = courseId.Value,
                                TopicId = objectTopicMapper.Id,
                                CreateDate = DateTime.Now,
                                UserId = userId,

                                SoActValue = Math.Round(((double)actionType / (double)topics.Count), 2)
                            };
                            UserSoActProfileDAL.Add(userSoActProfile);
                        }
                        else
                        {
                            userSoActProfile.SoActValue = Math.Round(userSoActProfile.SoActValue) + Math.Round(((double)actionType / (double)topics.Count), 2);
                            UserSoActProfileDAL.Update(userSoActProfile);
                        }
                    }
                }
                else
                {
                    var userSoActProfile = UserSoActProfileDAL.GetIfExist(userId, courseId, null);
                    if (userSoActProfile == null)
                    {
                        userSoActProfile = new UserSoActProfile
                        {

                            CourseId = courseId.Value,
                            TopicId = null,
                            CreateDate = DateTime.Now,
                            UserId = userId,

                            SoActValue = (double)actionType
                        };
                        UserSoActProfileDAL.Add(userSoActProfile);
                    }
                    else
                    {
                        userSoActProfile.SoActValue += (double)actionType;
                        UserSoActProfileDAL.Update(userSoActProfile);
                    }
                }
            }

            else
            {
                var userSoActProfile = UserSoActProfileDAL.GetIfExist(userId, null, null);
                if (userSoActProfile == null)
                {
                    userSoActProfile = new UserSoActProfile
                    {

                        CourseId = null,
                        TopicId = null,
                        CreateDate = DateTime.Now,
                        UserId = userId,

                        SoActValue = (double)actionType
                    };
                    UserSoActProfileDAL.Add(userSoActProfile);
                }
                else
                {
                    userSoActProfile.SoActValue += (double)actionType;
                    UserSoActProfileDAL.Update(userSoActProfile);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void UpdateUserKnowledgeProfile(int actionType, int objectId, int objectType, int userId, double? gradeValue = 0)
        {
            if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission ||
                objectType == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion ||
                objectType == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussionPost)
            {
                int? courseId = null;
                List<Topic> topics = null;
                double actionValue = actionType;

                if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission)
                {
                    var model = AssignmentSubmissionDAL.Get(objectId);
                    courseId = model.Assignment.CourseId;
                    if (userId == 0)
                        userId = model.UserId;

                    topics = ObjectTopicMapperDAL.GetAll(model.Assignment.Id, (int)UT.SL.Model.Enumeration.ObjectType.Assignment);

                    if (actionType == (int)UT.SL.Model.Enumeration.ActionType.Grade)
                    {
                        actionValue = gradeValue.Value;
                    }
                }
                else if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion)
                {
                    var model = ForumDiscussionDAL.Get(objectId);
                    courseId = model.Forum.CourseId;
                    if (userId == 0)
                        userId = model.UserId;

                    if (model.ParentId == null)
                        topics = ObjectTopicMapperDAL.GetAll(objectId, objectType);
                    else
                        topics = ObjectTopicMapperDAL.GetAll(model.ParentId.Value, objectType);
                    if (actionType == (int)UT.SL.Model.Enumeration.ActionType.Grade)
                    {
                        actionValue = gradeValue.Value;
                    }
                }
                else if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussionPost)
                {
                    var model = ForumDiscussionPostDAL.Get(objectId);
                    courseId = model.ForumDiscussion.Forum.CourseId;
                    if (userId == 0)
                        userId = model.UserId;
                    var parentModel = ForumDiscussionDAL.Get(model.ParentId);
                    if (parentModel.ParentId != null)
                        topics = ObjectTopicMapperDAL.GetAll(parentModel.ParentId.Value, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);
                    else
                        topics = ObjectTopicMapperDAL.GetAll(parentModel.Id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);
                    if (actionType == (int)UT.SL.Model.Enumeration.ActionType.Grade)
                    {
                        actionValue = gradeValue.Value;
                    }

                }

                if (courseId != null)
                {
                    if (topics.Count != 0)
                    {
                        foreach (var objectTopicMapper in topics)
                        {
                            var UserKnowledgeProfile = UserKnowledgeProfileDAL.GetIfExist(userId, courseId, objectTopicMapper.Id);
                            if (UserKnowledgeProfile == null)
                            {
                                UserKnowledgeProfile = new UserKnowledgeProfile
                                {

                                    CourseId = courseId.Value,
                                    TopicId = objectTopicMapper.Id,
                                    CreateDate = DateTime.Now,
                                    UserId = userId,

                                    Knowledge = Math.Round(((double)actionValue / (double)topics.Count), 2)
                                };
                                UserKnowledgeProfileDAL.Add(UserKnowledgeProfile);
                            }
                            else
                            {
                                UserKnowledgeProfile.Knowledge = Math.Round(UserKnowledgeProfile.Knowledge) + Math.Round((actionValue / (double)topics.Count), 2);
                                UserKnowledgeProfileDAL.Update(UserKnowledgeProfile);
                            }
                        }
                    }
                    else
                    {
                        var userKnowledgeProfile = UserKnowledgeProfileDAL.GetIfExist(userId, courseId, null);
                        if (userKnowledgeProfile == null)
                        {
                            userKnowledgeProfile = new UserKnowledgeProfile
                            {

                                CourseId = courseId.Value,
                                TopicId = null,
                                CreateDate = DateTime.Now,
                                UserId = userId,

                                Knowledge = actionValue
                            };
                            UserKnowledgeProfileDAL.Add(userKnowledgeProfile);
                        }
                        else
                        {
                            userKnowledgeProfile.Knowledge += actionValue;
                            UserKnowledgeProfileDAL.Update(userKnowledgeProfile);
                        }
                    }
                }

                else
                {
                    var userKnowledgeProfile = UserKnowledgeProfileDAL.GetIfExist(userId, null, null);
                    if (userKnowledgeProfile == null)
                    {
                        userKnowledgeProfile = new UserKnowledgeProfile
                        {

                            CourseId = null,
                            TopicId = null,
                            CreateDate = DateTime.Now,
                            UserId = userId,

                            Knowledge = actionValue
                        };
                        UserKnowledgeProfileDAL.Add(userKnowledgeProfile);
                    }
                    else
                    {
                        userKnowledgeProfile.Knowledge += actionValue;
                        UserKnowledgeProfileDAL.Update(userKnowledgeProfile);
                    }
                }
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <param name="userId"></param>
        public static void DeleteContentProfile(int actionType, int objectId, int objectType, int userId)
        {
            int? courseId = null;
            if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.Resource)
            {
                var model = ResourceDAL.Get(objectId);
                courseId = model.CourseId;
            }
            if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment)
            {
                var model = AssignmentDAL.Get(objectId);
                courseId = model.CourseId;
            }
            if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum)
            {
                var model = ForumDAL.Get(objectId);
                courseId = model.CourseId;
            }
            if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion)
            {
                var model = ForumDiscussionDAL.Get(objectId);
                courseId = model.Forum.CourseId;
            }
            var contentPopularityModel = ContentPopularityModelDAL.GetIfExist(objectId, objectType, courseId);
            if (contentPopularityModel != null)
            {
                ContentPopularityModelDAL.Delete(contentPopularityModel);
            }
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public static List<ContentInterestModel> UserContentInterests(int userId, int courseId)
        {
            var tempItems = new List<SelectListItem>();
            var contentPopularityModels = ContentPopularityModelDAL.GetCourseContent(courseId);
            List<ContentInterestModel> contentInterestModels = new List<ContentInterestModel>();
            var userSoActProfiles = UserSoActProfileDAL.GetAll(userId, courseId);
            foreach (var contentPopularityModel in contentPopularityModels)
            {
                if (contentPopularityModel.ObjectType != (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion)
                {
                    ContentInterestModel contentInterestModel = new ContentInterestModel();
                    contentInterestModel.ObjectId = contentPopularityModel.ObjectId;
                    contentInterestModel.ObjectType = contentPopularityModel.ObjectType;
                    contentInterestModel.UserId = userId;
                    contentInterestModel.UserInterestValue = UserSoActProfileDAL.GetUserInterestValue(courseId, userId, contentPopularityModel.ObjectId, contentPopularityModel.ObjectType);
                    if (contentInterestModel.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Resource)
                    {
                        var model = ResourceDAL.Get(contentInterestModel.ObjectId);
                        contentInterestModel.Title = model.Title;
                        contentInterestModel.Body = model.Body;
                        contentInterestModel.FileTitle = model.FileTitle;
                        contentInterestModel.ContentCreateUserId = model.CreateUserId;
                        contentInterestModel.ContentCreateDate = model.CreateDate;
                    }
                    else if (contentInterestModel.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment)
                    {
                        var model = AssignmentDAL.Get(contentInterestModel.ObjectId);
                        contentInterestModel.Title = model.Title;
                        contentInterestModel.Body = model.Body;
                        contentInterestModel.FileTitle = model.FileTitle;
                        contentInterestModel.ContentCreateUserId = model.CreateUserId;
                        contentInterestModel.ContentCreateDate = model.CreateDate;
                        contentInterestModel.ContentDueDate = model.DueDate;
                    }
                    else if (contentInterestModel.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum)
                    {
                        var model = ForumDAL.Get(contentInterestModel.ObjectId);
                        contentInterestModel.Title = model.Title;
                        contentInterestModel.Body = model.Body;
                        contentInterestModel.FileTitle = model.FileTitle;
                        contentInterestModel.ContentCreateUserId = model.CreateUserId;
                        contentInterestModel.ContentCreateDate = model.CreateDate.Value;
                        contentInterestModel.ContentDueDate = model.DueDate;

                        //foreach (var discussion in model.ForumDiscussions)
                        //{
                        //    contentInterestModel.UserInterestValue += UserSoActProfileDAL.GetUserInterestValue(courseId, userId, discussion.Id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);
                        //}
                    }
                    else if (contentInterestModel.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion)
                    {
                        var model = ForumDiscussionDAL.Get(contentInterestModel.ObjectId);
                        contentInterestModel.Title = model.Title;
                        contentInterestModel.Body = model.Body;
                        contentInterestModel.FileTitle = model.FileTitle;
                        contentInterestModel.ContentCreateUserId = model.UserId;
                        contentInterestModel.ContentCreateDate = model.CreateDate.Value;
                    }

                    contentInterestModels.Add(contentInterestModel);
                }
            }
            return contentInterestModels.OrderByDescending(u => u.UserInterestValue).ThenByDescending(u=>u.ContentCreateDate).ToList();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="course"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<ObjectViewModel> UserContentInterests(CourseSearchModel course, int userId)
        {
            var tempItems = new List<SelectListItem>();
            var contentPopularityModels = ContentPopularityModelDAL.GetCourseContent(course.CourseId.Value);
            var contentInterestModels = new List<ContentInterestModel>();
            var userSoActProfiles = UserSoActProfileDAL.GetAll(userId, course.CourseId.Value);
            foreach (var contentPopularityModel in contentPopularityModels)
            {
                if (contentPopularityModel.ObjectType != (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion)
                {
                    ContentInterestModel contentInterestModel = new ContentInterestModel();
                    contentInterestModel.ObjectId = contentPopularityModel.ObjectId;
                    contentInterestModel.ObjectType = contentPopularityModel.ObjectType;
                    contentInterestModel.UserId = userId;
                    contentInterestModel.UserInterestValue = UserSoActProfileDAL.GetUserInterestValue(course.CourseId.Value, userId, contentPopularityModel.ObjectId, contentPopularityModel.ObjectType);
                    if (contentInterestModel.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Resource)
                    {
                        var model = ResourceDAL.Get(contentInterestModel.ObjectId);
                        contentInterestModel.Title = model.Title;
                        contentInterestModel.Body = model.Body;
                        contentInterestModel.FileTitle = model.FileTitle;
                        contentInterestModel.ContentCreateUserId = model.CreateUserId;
                        contentInterestModel.ContentCreateDate = model.CreateDate;
                    }
                    else if (contentInterestModel.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment)
                    {
                        var model = AssignmentDAL.Get(contentInterestModel.ObjectId);
                        contentInterestModel.Title = model.Title;
                        contentInterestModel.Body = model.Body;
                        contentInterestModel.FileTitle = model.FileTitle;
                        contentInterestModel.ContentCreateUserId = model.CreateUserId;
                        contentInterestModel.ContentCreateDate = model.CreateDate;
                        contentInterestModel.ContentDueDate = model.DueDate;
                    }
                    else if (contentInterestModel.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum)
                    {
                        var model = ForumDAL.Get(contentInterestModel.ObjectId);
                        contentInterestModel.Title = model.Title;
                        contentInterestModel.Body = model.Body;
                        contentInterestModel.FileTitle = model.FileTitle;
                        contentInterestModel.ContentCreateUserId = model.CreateUserId;
                        contentInterestModel.ContentCreateDate = model.CreateDate.Value;
                        contentInterestModel.ContentDueDate = model.DueDate;

                        //foreach (var discussion in model.ForumDiscussions)
                        //{
                        //    contentInterestModel.UserInterestValue += UserSoActProfileDAL.GetUserInterestValue(courseId, userId, discussion.Id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);
                        //}
                    }
                    else if (contentInterestModel.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion)
                    {
                        var model = ForumDiscussionDAL.Get(contentInterestModel.ObjectId);
                        contentInterestModel.Title = model.Title;
                        contentInterestModel.Body = model.Body;
                        contentInterestModel.FileTitle = model.FileTitle;
                        contentInterestModel.ContentCreateUserId = model.UserId;
                        contentInterestModel.ContentCreateDate = model.CreateDate.Value;
                    }

                    contentInterestModels.Add(contentInterestModel);
                }
            }
            var tempModel =  contentInterestModels.OrderByDescending(u => u.UserInterestValue).ThenByDescending(u => u.ContentCreateDate).ToList();
            var returnModel = new List<ObjectViewModel>();
            foreach (var item in tempModel)
            {
                returnModel.Add(new ObjectViewModel
                {
                    CreateDate = item.ContentCreateDate,
                    Id = item.ObjectId,
                    Type = item.ObjectType,
                    Title = item.Title,
                    Body = item.Body,
                    FileTitle = item.FileTitle,
                    CreateUser = new App_User { Id = item.ContentCreateUserId},
                    Score = item.UserInterestValue
                });
            }
            return returnModel;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public static List<ContentKnowledgeModel> UserContentKnowledges(int userId, int courseId)
        {
            var tempItems = new List<SelectListItem>();
            var contentPopularityModels = ContentPopularityModelDAL.GetCourseContent(courseId);
            List<ContentKnowledgeModel> contentKnowledgeModels = new List<ContentKnowledgeModel>();
            var userKnowledgeProfiles = UserKnowledgeProfileDAL.GetAll(userId, courseId);
            foreach (var contentPopularityModel in contentPopularityModels)
            {
                if (contentPopularityModel.ObjectType != (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion)
                {
                    ContentKnowledgeModel contentKnowledgeModel = new ContentKnowledgeModel();
                    contentKnowledgeModel.ObjectId = contentPopularityModel.ObjectId;
                    contentKnowledgeModel.ObjectType = contentPopularityModel.ObjectType;
                    contentKnowledgeModel.UserId = userId;
                    contentKnowledgeModel.UserKnowledgeValue = UserKnowledgeProfileDAL.GetUserKnowledgeValue(courseId, userId, contentPopularityModel.ObjectId, contentPopularityModel.ObjectType);

                    if (contentKnowledgeModel.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Resource)
                    {
                        var model = ResourceDAL.Get(contentKnowledgeModel.ObjectId);
                        contentKnowledgeModel.Title = model.Title;
                        contentKnowledgeModel.Body = model.Body;
                        contentKnowledgeModel.FileTitle = model.FileTitle;
                        contentKnowledgeModel.ContentCreateUserId = model.CreateUserId;
                        contentKnowledgeModel.ContentCreateDate = model.CreateDate;
                    }
                    else if (contentKnowledgeModel.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment)
                    {
                        var model = AssignmentDAL.Get(contentKnowledgeModel.ObjectId);
                        contentKnowledgeModel.Title = model.Title;
                        contentKnowledgeModel.Body = model.Body;
                        contentKnowledgeModel.FileTitle = model.FileTitle;
                        contentKnowledgeModel.ContentCreateUserId = model.CreateUserId;
                        contentKnowledgeModel.ContentCreateDate = model.CreateDate;
                        contentKnowledgeModel.ContentDueDate = model.DueDate;
                    }
                    else if (contentKnowledgeModel.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum)
                    {
                        var model = ForumDAL.Get(contentKnowledgeModel.ObjectId);
                        contentKnowledgeModel.Title = model.Title;
                        contentKnowledgeModel.Body = model.Body;
                        contentKnowledgeModel.FileTitle = model.FileTitle;
                        contentKnowledgeModel.ContentCreateUserId = model.CreateUserId;
                        contentKnowledgeModel.ContentCreateDate = model.CreateDate.Value;
                        contentKnowledgeModel.ContentDueDate = model.DueDate;

                        //foreach (var discussion in model.ForumDiscussions)
                        //{
                        //    contentKnowledgeModel.UserKnowledgeValue += UserKnowledgeProfileDAL.GetUserKnowledgeValue(courseId, userId, discussion.Id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);
                        //}
                    }
                    else if (contentKnowledgeModel.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion)
                    {
                        var model = ForumDiscussionDAL.Get(contentKnowledgeModel.ObjectId);
                        contentKnowledgeModel.Title = model.Title;
                        contentKnowledgeModel.Body = model.Body;
                        contentKnowledgeModel.FileTitle = model.FileTitle;
                        contentKnowledgeModel.ContentCreateUserId = model.UserId;
                        contentKnowledgeModel.ContentCreateDate = model.CreateDate.Value;
                    }

                    contentKnowledgeModels.Add(contentKnowledgeModel);
                }
            }
            return contentKnowledgeModels.OrderByDescending(u => u.UserKnowledgeValue).ThenByDescending(u=>u.ContentCreateDate).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="course"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<ObjectViewModel> UserContentKnowledges(CourseSearchModel course, int userId)
        {
            var tempItems = new List<SelectListItem>();
            var contentPopularityModels = ContentPopularityModelDAL.GetCourseContent(course.CourseId.Value);
            List<ContentKnowledgeModel> contentKnowledgeModels = new List<ContentKnowledgeModel>();
            var userKnowledgeProfiles = UserKnowledgeProfileDAL.GetAll(userId, course.CourseId.Value);
            foreach (var contentPopularityModel in contentPopularityModels)
            {
                if (contentPopularityModel.ObjectType != (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion)
                {
                    ContentKnowledgeModel contentKnowledgeModel = new ContentKnowledgeModel();
                    contentKnowledgeModel.ObjectId = contentPopularityModel.ObjectId;
                    contentKnowledgeModel.ObjectType = contentPopularityModel.ObjectType;
                    contentKnowledgeModel.UserId = userId;
                    contentKnowledgeModel.UserKnowledgeValue = UserKnowledgeProfileDAL.GetUserKnowledgeValue(course.CourseId.Value, userId, contentPopularityModel.ObjectId, contentPopularityModel.ObjectType);

                    if (contentKnowledgeModel.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Resource)
                    {
                        var model = ResourceDAL.Get(contentKnowledgeModel.ObjectId);
                        contentKnowledgeModel.Title = model.Title;
                        contentKnowledgeModel.Body = model.Body;
                        contentKnowledgeModel.FileTitle = model.FileTitle;
                        contentKnowledgeModel.ContentCreateUserId = model.CreateUserId;
                        contentKnowledgeModel.ContentCreateDate = model.CreateDate;
                    }
                    else if (contentKnowledgeModel.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment)
                    {
                        var model = AssignmentDAL.Get(contentKnowledgeModel.ObjectId);
                        contentKnowledgeModel.Title = model.Title;
                        contentKnowledgeModel.Body = model.Body;
                        contentKnowledgeModel.FileTitle = model.FileTitle;
                        contentKnowledgeModel.ContentCreateUserId = model.CreateUserId;
                        contentKnowledgeModel.ContentCreateDate = model.CreateDate;
                        contentKnowledgeModel.ContentDueDate = model.DueDate;
                    }
                    else if (contentKnowledgeModel.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum)
                    {
                        var model = ForumDAL.Get(contentKnowledgeModel.ObjectId);
                        contentKnowledgeModel.Title = model.Title;
                        contentKnowledgeModel.Body = model.Body;
                        contentKnowledgeModel.FileTitle = model.FileTitle;
                        contentKnowledgeModel.ContentCreateUserId = model.CreateUserId;
                        contentKnowledgeModel.ContentCreateDate = model.CreateDate.Value;
                        contentKnowledgeModel.ContentDueDate = model.DueDate;

                        //foreach (var discussion in model.ForumDiscussions)
                        //{
                        //    contentKnowledgeModel.UserKnowledgeValue += UserKnowledgeProfileDAL.GetUserKnowledgeValue(courseId, userId, discussion.Id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);
                        //}
                    }
                    else if (contentKnowledgeModel.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion)
                    {
                        var model = ForumDiscussionDAL.Get(contentKnowledgeModel.ObjectId);
                        contentKnowledgeModel.Title = model.Title;
                        contentKnowledgeModel.Body = model.Body;
                        contentKnowledgeModel.FileTitle = model.FileTitle;
                        contentKnowledgeModel.ContentCreateUserId = model.UserId;
                        contentKnowledgeModel.ContentCreateDate = model.CreateDate.Value;
                    }

                    contentKnowledgeModels.Add(contentKnowledgeModel);
                }
            }
            var tempModel =  contentKnowledgeModels.OrderByDescending(u => u.UserKnowledgeValue).ThenByDescending(u => u.ContentCreateDate).ToList();
            var returnModel = new List<ObjectViewModel>();
            foreach (var item in tempModel)
            {
                returnModel.Add(new ObjectViewModel
                {
                    CreateDate = item.ContentCreateDate,
                    Id = item.ObjectId,
                    Type = item.ObjectType,
                    Title = item.Title,
                    Body = item.Body,
                    FileTitle = item.FileTitle,
                    CreateUser = new App_User { Id = item.ContentCreateUserId },
                    Score = item.UserKnowledgeValue
                });
            }
            return returnModel;
        }


    }
}
