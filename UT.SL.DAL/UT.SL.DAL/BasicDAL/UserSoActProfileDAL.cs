using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;

namespace UT.SL.DAL
{
    public partial class UserSoActProfileDAL
    {

        #region Get

        public static UserSoActProfile GetIfExist(int userId, int? courseId, int? topicId)
        {
            return GetIfExist(DBUtility.GetSocialLearningDataContext, userId, courseId, topicId);
        }

        public static UserSoActProfile GetIfExist(SocialLearningDataContext dc, int userId, int? courseId, int? topicId)
        {
            if (courseId != null && topicId != null)
                return dc.UserSoActProfiles.SingleOrDefault(u => u.UserId == userId && u.CourseId == courseId && u.TopicId == topicId);
            else if (courseId != null)
                return dc.UserSoActProfiles.SingleOrDefault(u => u.UserId == userId && u.CourseId == courseId && u.TopicId == (int?)null);
            else
                return dc.UserSoActProfiles.SingleOrDefault(u => u.UserId == userId && u.CourseId == (int?)null && u.TopicId == (int?)null);

        }

        public static double GetUserInterestValue(int courseId, int userId, int objectId, int objectType)
        {
            double userInterestValue = 0;
            var contentPopularityModel = ContentPopularityModelDAL.GetIfExist(objectId, objectType, courseId);
            var userSoActProfiles = UserSoActProfileDAL.GetAll(userId, courseId);
            var topics = ObjectTopicMapperDAL.GetAll(objectId, objectType);
            if (topics.Count != 0)
            {
                if (userSoActProfiles.Count != 0)
                {
                    double userInteresrSum = 0;
                    foreach (var topic in topics)
                    {
                        var item = userSoActProfiles.SingleOrDefault(u => u.TopicId == topic.Id);
                        if (item != null)
                            userInteresrSum += item.SoActValue;
                    }
                    userInterestValue = contentPopularityModel.SoActValue * Math.Round(((double)userInteresrSum / topics.Count), 2);
                }
                else
                {
                    userInterestValue = 0;
                }
            }
            else
            {
                userInterestValue = 0;
            }

            if (objectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum)
            {
                var model = ForumDAL.Get(objectId);
                foreach (var discussion in model.ForumDiscussions)
                {
                    userInterestValue += GetUserInterestValue(courseId, userId, discussion.Id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);
                }
            }
            return userInterestValue;
        }

        #endregion

        #region GetAll

        public static List<UserSoActProfile> GetAll(SocialLearningDataContext dc, int userId, int? courseId)
        {
            if (courseId != null)
                return dc.UserSoActProfiles.Where(u => u.UserId == userId && u.CourseId == courseId && u.TopicId != null && u.SoActValue != 0).ToList();
            else
                return dc.UserSoActProfiles.Where(u => u.UserId == userId && u.CourseId == (int?)null && u.TopicId != null && u.SoActValue != 0).ToList();
        }

        public static List<UserSoActProfile> GetAll(int userId, int? courseId)
        {
            return GetAll(DBUtility.GetSocialLearningDataContext, userId, courseId);
        }

        #endregion

        #region Add

        public static DALReturnModel<UserSoActProfile> Add(UserSoActProfile model)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<UserSoActProfile> Add(SocialLearningDataContext dc, UserSoActProfile model)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new UserSoActProfile();
                    obj.CourseId = model.CourseId;
                    obj.UserId = model.UserId;
                    obj.TopicId = model.TopicId;
                    obj.SoActValue = model.SoActValue;
                    obj.CreateDate = model.CreateDate;
                    dc.UserSoActProfiles.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<UserSoActProfile>(obj);
                }
            }
            catch
            {

            }
            return new DALReturnModel<UserSoActProfile>(new UserSoActProfile { Id = 0 });
        }

        #endregion

        #region Update

        public static DALReturnModel<UserSoActProfile> Update(UserSoActProfile model)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<UserSoActProfile> Update(SocialLearningDataContext dc, UserSoActProfile model)
        {
            UserSoActProfile obj = null;
            bool noErrorFlag = true;
            obj = dc.UserSoActProfiles.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.CourseId = model.CourseId;
                        obj.UserId = model.UserId;
                        obj.TopicId = model.TopicId;
                        obj.SoActValue = model.SoActValue;
                        obj.CreateDate = model.CreateDate;
                        dc.SubmitChanges();
                    }
                }
            }
            catch
            {

            }
            return new DALReturnModel<UserSoActProfile>(obj);
        }

        #endregion

        #region Delete

        public static DALReturnModel<UserSoActProfile> Delete(UserSoActProfile model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<UserSoActProfile> Delete(SocialLearningDataContext dc, UserSoActProfile model)
        {
            try
            {
                var obj = dc.UserSoActProfiles.Single(q => q.Id == model.Id);
                dc.UserSoActProfiles.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<UserSoActProfile>(new UserSoActProfile { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<UserSoActProfile>(new UserSoActProfile { Id = 0 });
            }
        }

        #endregion


    }
}
