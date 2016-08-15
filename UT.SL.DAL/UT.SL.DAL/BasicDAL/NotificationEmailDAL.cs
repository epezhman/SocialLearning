using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class NotificationEmailDAL
    {

        #region Get

        public static NotificationEmail Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static NotificationEmail Get(SocialLearningDataContext dc, int Id)
        {
            return dc.NotificationEmails.SingleOrDefault(u => u.Id == Id);
        }

        #endregion

        #region GetAll

        public static List<NotificationEmail> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<NotificationEmail> GetAll(SocialLearningDataContext dc)
        {
            return dc.NotificationEmails.ToList();
        }

        public static List<NotificationEmail> GetAllUser(int userId)
        {
            return GetAllUser(DBUtility.GetSocialLearningDataContext, userId);
        }

        public static List<NotificationEmail> GetAllUser(SocialLearningDataContext dc, int userId)
        {
            return dc.NotificationEmails.Where(x => x.UserId == userId).ToList();
        }

        public static List<NotificationEmail> GetAllUserAndDate(int userId, DateTime date)
        {
            return GetAllUserAndDate(DBUtility.GetSocialLearningDataContext, userId, date);
        }

        public static List<NotificationEmail> GetAllUserAndDate(SocialLearningDataContext dc, int userId, DateTime date)
        {
            return dc.NotificationEmails.Where(x => x.UserId == userId && x.CreateDate.Date == date.Date).ToList();
        }

        public static List<NotificationEmail> GetNotSent()
        {
            return GetNotSent(DBUtility.GetSocialLearningDataContext);
        }

        public static List<NotificationEmail> GetNotSent(SocialLearningDataContext dc)
        {
            return dc.NotificationEmails.Where(u => !u.Sent).ToList();
        }

        #endregion

        #region Add

        public static DALReturnModel<NotificationEmail> Add(NotificationEmail model)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<NotificationEmail> Add(SocialLearningDataContext dc, NotificationEmail model)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new NotificationEmail();
                    obj.UserId = model.UserId;
                    obj.CreateDate = model.CreateDate;
                    obj.ObjectId = model.ObjectId;
                    obj.ObjectType = model.ObjectType;
                    obj.CourseId = model.CourseId;
                    obj.Sent = obj.Sent;
                    dc.NotificationEmails.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<NotificationEmail>(obj);
                }
            }
            catch
            {
            }
            return new DALReturnModel<NotificationEmail>(new NotificationEmail { Id = 0 });
        }

        #endregion

        #region Update

        public static DALReturnModel<NotificationEmail> Update(NotificationEmail model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<NotificationEmail> Update(SocialLearningDataContext dc, NotificationEmail model, BatchProcessResultModel bpr)
        {
            NotificationEmail obj = null;
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            obj = dc.NotificationEmails.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<NotificationEmail>(obj, bpr);
        }

        public static DALReturnModel<NotificationEmail> UpdateSeenDate(int id)
        {
            return UpdateSeenDate(DBUtility.GetSocialLearningDataContext, id);
        }

        public static DALReturnModel<NotificationEmail> UpdateSeenDate(SocialLearningDataContext dc, int id)
        {
            NotificationEmail obj = null;
            bool noErrorFlag = true;
            obj = dc.NotificationEmails.SingleOrDefault(u => u.Id == id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.SentDate = DateTime.Now;
                        obj.Sent = true;
                        dc.SubmitChanges();
                    }
                }
            }
            catch 
            {
            }
            return new DALReturnModel<NotificationEmail>(obj);
        }

        #endregion

        #region Delete

        public static DALReturnModel<NotificationEmail> Delete(NotificationEmail model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<NotificationEmail> Delete(SocialLearningDataContext dc, NotificationEmail model)
        {
            try
            {
                var obj = dc.NotificationEmails.Single(q => q.Id == model.Id);
                dc.NotificationEmails.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<NotificationEmail>(new NotificationEmail { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<NotificationEmail>(new NotificationEmail { Id = 0 });
            }
        }

        public static bool DeleteAll(int objectId, int objectType)
        {
            return DeleteAll(DBUtility.GetSocialLearningDataContext, objectId, objectType);
        }

        public static bool DeleteAll(SocialLearningDataContext dc, int objectId, int objectType)
        {
            try
            {
                var obj = dc.NotificationEmails.Where(q => q.ObjectId == objectId && q.ObjectType == objectType).ToList();
                foreach (var item in obj)
                {
                    dc.NotificationEmails.DeleteOnSubmit(item);

                }
                dc.SubmitChanges();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        #endregion
    }
}
