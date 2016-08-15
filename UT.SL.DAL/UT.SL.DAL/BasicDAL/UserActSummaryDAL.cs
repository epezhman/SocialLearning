using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class UserActSummaryDAL
    {

        #region Get

        public static UserActSummary Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static UserActSummary Get(SocialLearningDataContext dc, int Id)
        {
            return dc.UserActSummaries.SingleOrDefault(u => u.Id == Id);
        }

        public static DateTime GetMinDate()
        {
            return GetMinDate(DBUtility.GetSocialLearningDataContext);
        }

        public static DateTime GetMinDate(SocialLearningDataContext dc)
        {
            return dc.UserActSummaries.Min(x => x.BeginDate).Value;
        }

        public static DateTime GetMaxDate()
        {
            return GetMaxDate(DBUtility.GetSocialLearningDataContext);
        }

        public static DateTime GetMaxDate(SocialLearningDataContext dc)
        {
            return dc.UserActSummaries.Max(x => x.EndDate).Value;
        }

        public static UserActSummary Get(int userId, int courseId)
        {
            return Get(DBUtility.GetSocialLearningDataContext, userId, courseId);
        }

        public static UserActSummary Get(SocialLearningDataContext dc, int userId, int courseId)
        {
            return dc.UserActSummaries.SingleOrDefault(u => u.UserId == userId && u.CourseId == courseId);
        }

        public static UserActSummary Get(int userId, int courseId, DateTime date)
        {
            return Get(DBUtility.GetSocialLearningDataContext, userId, courseId, date);
        }

        public static UserActSummary Get(SocialLearningDataContext dc, int userId, int courseId, DateTime date)
        {
            return dc.UserActSummaries.SingleOrDefault(u => u.UserId == userId && u.CourseId == courseId && u.BeginDate.Value.Date <= date.Date && u.EndDate.Value.Date >= date.Date);
        }

        #endregion

        #region GetAll

        public static List<UserActSummary> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<UserActSummary> GetAll(SocialLearningDataContext dc)
        {
            return dc.UserActSummaries.ToList();
        }

        public static List<UserActSummary> GetAllCourse(int courseId)
        {
            return GetAllCourse(DBUtility.GetSocialLearningDataContext, courseId);
        }

        public static List<UserActSummary> GetAllCourse(SocialLearningDataContext dc, int courseId)
        {
            return dc.UserActSummaries.Where(x => x.CourseId == courseId).ToList();
        }

        public static List<UserActSummary> GetMostActive(int courseId, DateTime date)
        {
            return GetMostActive(DBUtility.GetSocialLearningDataContext, courseId, date);
        }

        public static List<UserActSummary> GetMostActive(SocialLearningDataContext dc, int courseId, DateTime date)
        {
            return dc.UserActSummaries.Where(x => x.CourseId == courseId && x.BeginDate.Value.Date <= date.Date && x.EndDate.Value.Date >= date.Date).ToList();
        }

        public static List<UserActSummary> GetMostActive(int courseId, DateTime beginDate, DateTime endDate)
        {
            return GetMostActive(DBUtility.GetSocialLearningDataContext, courseId, beginDate, endDate);
        }

        public static List<UserActSummary> GetMostActive(SocialLearningDataContext dc, int courseId, DateTime beginDate, DateTime endDate)
        {
            return dc.UserActSummaries.Where(x => x.CourseId == courseId
                && ((x.BeginDate.Value.Date <= beginDate.Date && x.EndDate.Value.Date >= beginDate.Date)
                || ((x.BeginDate.Value.Date >= beginDate.Date && x.EndDate.Value.Date >= beginDate.Date) && (x.BeginDate.Value.Date <= endDate.Date && x.EndDate.Value.Date <= endDate.Date))
                || (x.BeginDate.Value.Date <= endDate.Date && x.EndDate.Value.Date >= endDate.Date))).ToList();
        }

        #endregion

        #region Add

        public static DALReturnModel<UserActSummary> Add(UserActSummary model)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<UserActSummary> Add(SocialLearningDataContext dc, UserActSummary model)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new UserActSummary();
                    obj.UserId = model.UserId;
                    obj.CourseId = model.CourseId;
                    obj.BeginDate = model.BeginDate;
                    obj.EndDate = model.EndDate;
                    dc.UserActSummaries.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<UserActSummary>(obj);
                }
            }
            catch
            {
            }
            return new DALReturnModel<UserActSummary>(new UserActSummary { Id = 0 });
        }

        #endregion

        #region Update

        public static DALReturnModel<UserActSummary> Update(UserActSummary model)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<UserActSummary> Update(SocialLearningDataContext dc, UserActSummary model)
        {
            UserActSummary obj = null;
            bool noErrorFlag = true;
            obj = dc.UserActSummaries.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.TagCount = model.TagCount;
                        obj.CommentCount = model.CommentCount;
                        obj.CreateCount = model.CreateCount;
                        obj.VoteCount = model.VoteCount;
                        obj.TotalScore = model.TotalScore;
                        dc.SubmitChanges();
                    }
                }
            }
            catch
            {
            }
            return new DALReturnModel<UserActSummary>(obj);
        }

        #endregion

        #region Delete

        public static DALReturnModel<UserActSummary> Delete(UserActSummary model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<UserActSummary> Delete(SocialLearningDataContext dc, UserActSummary model)
        {
            try
            {
                var obj = dc.UserActSummaries.Single(q => q.Id == model.Id);
                dc.UserActSummaries.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<UserActSummary>(new UserActSummary { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<UserActSummary>(new UserActSummary { Id = 0 });
            }
        }

        public static DALReturnModel<UserActSummary> DeleteUserActs(int userId)
        {
            return DeleteUserActs(DBUtility.GetSocialLearningDataContext, userId);
        }

        public static DALReturnModel<UserActSummary> DeleteUserActs(SocialLearningDataContext dc, int userId)
        {
            try
            {
                var obj = dc.UserActSummaries.Where(q => q.UserId == userId).ToList();
                foreach (var item in obj)
                {
                    dc.UserActSummaries.DeleteOnSubmit(item);
                }
                dc.SubmitChanges();
                return new DALReturnModel<UserActSummary>(new UserActSummary { Id = userId }); ///!!!wrong
            }
            catch (System.Exception)
            {
                return new DALReturnModel<UserActSummary>(new UserActSummary { Id = 0 });
            }
        }

        #endregion

    }
}
