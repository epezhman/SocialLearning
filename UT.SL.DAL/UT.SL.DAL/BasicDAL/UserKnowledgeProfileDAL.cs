using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UT.SL.Data.LINQ;
using UT.SL.Helper;

namespace UT.SL.DAL
{
    public partial class UserKnowledgeProfileDAL
    {

        #region Get

        public static UserKnowledgeProfile GetIfExist(int userId, int? courseId, int? topicId)
        {
            return GetIfExist(DBUtility.GetSocialLearningDataContext, userId, courseId, topicId);
        }

        public static UserKnowledgeProfile GetIfExist(SocialLearningDataContext dc, int userId, int? courseId, int? topicId)
        {
            if (courseId != null && topicId != null)
                return dc.UserKnowledgeProfiles.SingleOrDefault(u => u.UserId == userId && u.CourseId == courseId && u.TopicId == topicId);
            else if (courseId != null)
                return dc.UserKnowledgeProfiles.SingleOrDefault(u => u.UserId == userId && u.CourseId == courseId && u.TopicId == (int?)null);
            else
                return dc.UserKnowledgeProfiles.SingleOrDefault(u => u.UserId == userId && u.CourseId == (int?)null && u.TopicId == (int?)null);
        }

        public static double GetUserKnowledgeValue(int courseId, int userId, int objectId, int objectType)
        {
            double userKnowledgeValue = 0;
            var contentPopularityModel = ContentPopularityModelDAL.GetIfExist(objectId, objectType, courseId);
            var userKnowledgeProfiles = UserKnowledgeProfileDAL.GetAll(userId, courseId);
            var topics = ObjectTopicMapperDAL.GetAll(objectId, objectType);
            if (topics.Count != 0)
            {
                if (userKnowledgeProfiles.Count != 0)
                {
                    double userKnowledgeSum = 0;
                    foreach (var topic in topics)
                    {
                        var item = userKnowledgeProfiles.SingleOrDefault(u => u.TopicId == topic.Id);
                        if (item != null)
                            userKnowledgeSum += item.Knowledge;
                    }
                    userKnowledgeValue = contentPopularityModel.SoActValue * Math.Round(((double)userKnowledgeSum / topics.Count), 2);
                }
                else
                {
                    userKnowledgeValue = 0;
                }
            }
            else
            {
                userKnowledgeValue = 0;
            }

            if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum)
            {
                var model = ForumDAL.Get(objectId);
                foreach (var discussion in model.ForumDiscussions)
                {
                    userKnowledgeValue += GetUserKnowledgeValue(courseId, userId, discussion.Id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);
                }
            }

            return userKnowledgeValue;
        }

        #endregion

        #region GetAll

        public static List<UserKnowledgeProfile> GetAll(int userId, int? courseId)
        {
            return GetAll(DBUtility.GetSocialLearningDataContext, userId, courseId);
        }

        public static List<UserKnowledgeProfile> GetAll(SocialLearningDataContext dc, int userId, int? courseId)
        {
            if (courseId != null)
                return dc.UserKnowledgeProfiles.Where(u => u.UserId == userId && u.CourseId == courseId).ToList();
            else
                return dc.UserKnowledgeProfiles.Where(u => u.UserId == userId && u.CourseId == (int?)null).ToList();
        }

        #endregion

        #region Add

        public static DALReturnModel<UserKnowledgeProfile> Add(UserKnowledgeProfile model)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<UserKnowledgeProfile> Add(SocialLearningDataContext dc, UserKnowledgeProfile model)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new UserKnowledgeProfile();
                    obj.CourseId = model.CourseId;
                    obj.UserId = model.UserId;
                    obj.TopicId = model.TopicId;
                    obj.Knowledge = model.Knowledge;
                    obj.CreateDate = model.CreateDate;
                    dc.UserKnowledgeProfiles.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<UserKnowledgeProfile>(obj);
                }
            }
            catch
            {

            }
            return new DALReturnModel<UserKnowledgeProfile>(new UserKnowledgeProfile { Id = 0 });
        }

        #endregion

        #region Update

        public static DALReturnModel<UserKnowledgeProfile> Update(UserKnowledgeProfile model)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<UserKnowledgeProfile> Update(SocialLearningDataContext dc, UserKnowledgeProfile model)
        {
            UserKnowledgeProfile obj = null;
            bool noErrorFlag = true;
            obj = dc.UserKnowledgeProfiles.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.CourseId = model.CourseId;
                        obj.UserId = model.UserId;
                        obj.TopicId = model.TopicId;
                        obj.Knowledge = model.Knowledge;
                        obj.CreateDate = model.CreateDate;
                        dc.SubmitChanges();
                    }
                }
            }
            catch
            {

            }
            return new DALReturnModel<UserKnowledgeProfile>(obj);
        }

        #endregion

        #region Delete

        public static DALReturnModel<UserKnowledgeProfile> Delete(UserKnowledgeProfile model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<UserKnowledgeProfile> Delete(SocialLearningDataContext dc, UserKnowledgeProfile model)
        {
            try
            {
                var obj = dc.UserKnowledgeProfiles.Single(q => q.Id == model.Id);
                dc.UserKnowledgeProfiles.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<UserKnowledgeProfile>(new UserKnowledgeProfile { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<UserKnowledgeProfile>(new UserKnowledgeProfile { Id = 0 });
            }
        }

        #endregion

    }
}
