using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;
namespace UT.SL.DAL
{
    public partial class ContentPopularityModelDAL
    {

        #region Get

        public static ContentPopularityModel GetIfExist(int objectId, int objectType, int? courseId)
        {
            return GetIfExist(DBUtility.GetSocialLearningDataContext, objectId, objectType, courseId);
        }

        public static ContentPopularityModel GetIfExist(SocialLearningDataContext dc, int objectId, int objectType, int? courseId)
        {
            if (courseId == null)
                return dc.ContentPopularityModels.SingleOrDefault(u => u.ObjectId == objectId && u.ObjectType == objectType && u.CourseId == (int?)null);
            else
                return dc.ContentPopularityModels.SingleOrDefault(u => u.ObjectId == objectId && u.ObjectType == objectType && u.CourseId == courseId);

        }

        public static double GetScore(int objectId, int objectType, int courseId)
        {
            return GetScore(DBUtility.GetSocialLearningDataContext, objectId, objectType, courseId);
        }

        public static double GetScore(SocialLearningDataContext dc, int objectId, int objectType, int courseId)
        {
            var scores = dc.ContentPopularityModels.Where(x => x.CourseId == courseId && x.ObjectId == objectId && x.ObjectType == objectType).ToList();
            if (scores.Any())
            {
                return scores.Max(x => x.SoActValue);
            }
            else return 0;
        }


        public static DateTime GetMinDate()
        {
            return GetMinDate(DBUtility.GetSocialLearningDataContext);
        }

        public static DateTime GetMinDate(SocialLearningDataContext dc)
        {
            return dc.ContentPopularityModels.Min(x => x.CreateDate);
        }

        public static DateTime GetMaxDate()
        {
            return GetMaxDate(DBUtility.GetSocialLearningDataContext);
        }

        public static DateTime GetMaxDate(SocialLearningDataContext dc)
        {
            return dc.ContentPopularityModels.Max(x => x.CreateDate);
        }

        #endregion

        #region GetAll

        public static List<ContentPopularityModel> GetCourseContent(int courseId)
        {
            return GetCourseContent(DBUtility.GetSocialLearningDataContext, courseId);
        }

        public static List<ContentPopularityModel> GetCourseContent(SocialLearningDataContext dc, int courseId)
        {
            return dc.ContentPopularityModels.Where(u => u.CourseId == courseId && u.SoActValue != 0).ToList();
        }

        public static List<ContentPopularityModel> GetMostPopular(int courseId, DateTime beginDate, DateTime endDate)
        {
            return GetMostPopular(DBUtility.GetSocialLearningDataContext, courseId, beginDate, endDate);
        }

        public static List<ContentPopularityModel> GetMostPopular(SocialLearningDataContext dc, int courseId, DateTime beginDate, DateTime endDate)
        {
            return dc.ContentPopularityModels.Where(x => x.CourseId == courseId && x.CreateDate.Date >= beginDate.Date && x.CreateDate.Date <= endDate.Date).ToList();
        }

        public static List<ContentPopularityModel> GetMostPopularResource(int courseId, DateTime beginDate, DateTime endDate)
        {
            return GetMostPopularResource(DBUtility.GetSocialLearningDataContext, courseId, beginDate, endDate);
        }

        public static List<ContentPopularityModel> GetMostPopularResource(SocialLearningDataContext dc, int courseId, DateTime beginDate, DateTime endDate)
        {
            return dc.ContentPopularityModels.Where(x => x.CourseId == courseId && x.CreateDate.Date >= beginDate.Date && x.CreateDate.Date <= endDate.Date && x.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Resource).ToList();
        }

        public static List<ContentPopularityModel> GetMostPopularActivities(int courseId, DateTime beginDate, DateTime endDate)
        {
            return GetMostPopularActivities(DBUtility.GetSocialLearningDataContext, courseId, beginDate, endDate);
        }

        public static List<ContentPopularityModel> GetMostPopularActivities(SocialLearningDataContext dc, int courseId, DateTime beginDate, DateTime endDate)
        {
            return dc.ContentPopularityModels.Where(x => x.CourseId == courseId && x.CreateDate.Date >= beginDate.Date && x.CreateDate.Date <= endDate.Date
                && (x.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum || x.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion || x.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussionPost || x.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment || x.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission)).ToList();
        }

        public static List<ContentPopularityModel> GetMostPopularForum(int courseId, DateTime beginDate, DateTime endDate)
        {
            return GetMostPopularForum(DBUtility.GetSocialLearningDataContext, courseId, beginDate, endDate);
        }

        public static List<ContentPopularityModel> GetMostPopularForum(SocialLearningDataContext dc, int courseId, DateTime beginDate, DateTime endDate)
        {
            return dc.ContentPopularityModels.Where(x => x.CourseId == courseId && x.CreateDate.Date >= beginDate.Date && x.CreateDate.Date <= endDate.Date
                && (x.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum || x.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion || x.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussionPost)).ToList();
        }

        public static List<ContentPopularityModel> GetMostPopularAssignment(int courseId, DateTime beginDate, DateTime endDate)
        {
            return GetMostPopularAssignment(DBUtility.GetSocialLearningDataContext, courseId, beginDate, endDate);
        }

        public static List<ContentPopularityModel> GetMostPopularAssignment(SocialLearningDataContext dc, int courseId, DateTime beginDate, DateTime endDate)
        {
            return dc.ContentPopularityModels.Where(x => x.CourseId == courseId && x.CreateDate.Date >= beginDate.Date && x.CreateDate.Date <= endDate.Date
                && (x.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment || x.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission)).ToList();
        }

        #endregion

        #region Add

        public static DALReturnModel<ContentPopularityModel> Add(ContentPopularityModel model)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<ContentPopularityModel> Add(SocialLearningDataContext dc, ContentPopularityModel model)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new ContentPopularityModel();
                    obj.CourseId = model.CourseId;
                    obj.ObjectId = model.ObjectId;
                    obj.ObjectType = model.ObjectType;
                    obj.SoActValue = model.SoActValue;
                    obj.CreateDate = model.CreateDate;
                    dc.ContentPopularityModels.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<ContentPopularityModel>(obj);
                }
            }
            catch
            {

            }
            return new DALReturnModel<ContentPopularityModel>(new ContentPopularityModel { Id = 0 });
        }

        #endregion

        #region Update

        public static DALReturnModel<ContentPopularityModel> Update(ContentPopularityModel model)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<ContentPopularityModel> Update(SocialLearningDataContext dc, ContentPopularityModel model)
        {
            ContentPopularityModel obj = null;

            bool noErrorFlag = true;
            obj = dc.ContentPopularityModels.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.CourseId = model.CourseId;
                        obj.ObjectId = model.ObjectId;
                        obj.ObjectType = model.ObjectType;
                        obj.SoActValue = model.SoActValue;
                        obj.CreateDate = model.CreateDate;
                        dc.SubmitChanges();

                    }
                }
            }
            catch
            {

            }
            return new DALReturnModel<ContentPopularityModel>(obj);
        }

        #endregion

        #region Delete

        public static DALReturnModel<ContentPopularityModel> Delete(ContentPopularityModel model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<ContentPopularityModel> Delete(SocialLearningDataContext dc, ContentPopularityModel model)
        {
            try
            {
                var obj = dc.ContentPopularityModels.Single(q => q.Id == model.Id);
                dc.ContentPopularityModels.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<ContentPopularityModel>(new ContentPopularityModel { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<ContentPopularityModel>(new ContentPopularityModel { Id = 0 });
            }
        }

        #endregion

    }
}
