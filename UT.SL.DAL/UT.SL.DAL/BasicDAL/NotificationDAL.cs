using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class NotificationDAL
    {

        #region Get

        public static Notification Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static Notification Get(SocialLearningDataContext dc, int Id)
        {
            return dc.Notifications.SingleOrDefault(u => u.Id == Id);
        }

        public static int GetUnSeenCount(int Id)
        {
            return GetUnSeenCount(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static int GetUnSeenCount(SocialLearningDataContext dc, int Id)
        {
            return dc.Notifications.Where(u => u.UserId == Id && !u.Seen).Count();
        }

        #endregion

        #region GetAll

        public static List<Notification> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<Notification> GetAll(SocialLearningDataContext dc)
        {
            return dc.Notifications.ToList();
        }

        public static List<Notification> GetAllUser(int userId)
        {
            return GetAllUser(DBUtility.GetSocialLearningDataContext, userId);
        }

        public static List<Notification> GetAllUser(SocialLearningDataContext dc, int userId)
        {
            return dc.Notifications.Where(x => x.UserId == userId).ToList();
        }

        public static List<Notification> GetAllUserUnRead(int userId)
        {
            return GetAllUserUnRead(DBUtility.GetSocialLearningDataContext, userId);
        }

        public static List<Notification> GetAllUserUnRead(SocialLearningDataContext dc, int userId)
        {
            return dc.Notifications.Where(x => x.UserId == userId && !x.Readen).ToList();
        }

        public static List<Notification> GetTop100UserUnRead(int userId)
        {
            return GetTop100UserUnRead(DBUtility.GetSocialLearningDataContext, userId);
        }

        public static List<Notification> GetTop100UserUnRead(SocialLearningDataContext dc, int userId)
        {
            return dc.Notifications.Where(x => x.UserId == userId && !x.Readen).Take(100).ToList();
        }


        public static List<Notification> GetAllUserAndDate(int userId, DateTime date)
        {
            return GetAllUserAndDate(DBUtility.GetSocialLearningDataContext, userId, date);
        }

        public static List<Notification> GetAllUserAndDate(SocialLearningDataContext dc, int userId, DateTime date)
        {
            return dc.Notifications.Where(x => x.UserId == userId && x.CreateDate.Date == date.Date).ToList();
        }

        public static List<Notification> IsThereAny(int objectId, int objectType)
        {
            return IsThereAny(DBUtility.GetSocialLearningDataContext, objectId, objectType);
        }

        public static List<Notification> IsThereAny(SocialLearningDataContext dc, int objectId, int objectType)
        {
            return dc.Notifications.Where(x => x.ObjectId == objectId && x.ObjectType == objectType).ToList();
        }

        #endregion

        #region Add

        public static DALReturnModel<Notification> Add(Notification model)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Notification> Add(SocialLearningDataContext dc, Notification model)
        {
            bool noErrorFlag = true;
            if (dc.Notifications.Any(x => x.UserId == model.UserId && x.ObjectId == model.ObjectId && x.ObjectType == x.ObjectType))
                noErrorFlag = false;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new Notification();
                    obj.UserId = model.UserId;
                    obj.CreateDate = model.CreateDate;
                    obj.ObjectId = model.ObjectId;
                    obj.ObjectType = model.ObjectType;
                    obj.CourseId = model.CourseId;
                    obj.Seen = obj.Seen;
                    obj.Readen = obj.Readen;
                    dc.Notifications.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<Notification>(obj);
                }
            }
            catch
            {
            }
            return new DALReturnModel<Notification>(new Notification { Id = 0 });
        }

        #endregion

        #region Update

        public static DALReturnModel<Notification> Update(Notification model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Notification> Update(SocialLearningDataContext dc, Notification model, BatchProcessResultModel bpr)
        {
            Notification obj = null;
            bool noErrorFlag = true;
            obj = dc.Notifications.SingleOrDefault(u => u.Id == model.Id);
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
            return new DALReturnModel<Notification>(obj, bpr);
        }

        public static DALReturnModel<Notification> UpdateAsNew(int id)
        {
            return UpdateAsNew(DBUtility.GetSocialLearningDataContext, id);
        }

        public static DALReturnModel<Notification> UpdateAsNew(SocialLearningDataContext dc, int id)
        {
            Notification obj = null;
            bool noErrorFlag = true;
            obj = dc.Notifications.SingleOrDefault(u => u.Id == id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.CreateDate = DateTime.Now;
                        obj.Seen = false;
                        obj.Readen = false;
                        obj.SeenDate = null;
                        dc.SubmitChanges();
                    }
                }
            }
            catch 
            {
            }
            return new DALReturnModel<Notification>(obj);
        }

        public static DALReturnModel<Notification> UpdateSeenDate(int id, BatchProcessResultModel bpr)
        {
            return UpdateSeenDate(DBUtility.GetSocialLearningDataContext, id, bpr);
        }

        public static DALReturnModel<Notification> UpdateSeenDate(SocialLearningDataContext dc, int id, BatchProcessResultModel bpr)
        {
            Notification obj = null;
            bool noErrorFlag = true;
            obj = dc.Notifications.SingleOrDefault(u => u.Id == id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Seen = true;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Notification>(obj, bpr);
        }

        public static DALReturnModel<Notification> UpdateReaden(int id, BatchProcessResultModel bpr)
        {
            return UpdateReaden(DBUtility.GetSocialLearningDataContext, id, bpr);
        }

        public static DALReturnModel<Notification> UpdateReaden(SocialLearningDataContext dc, int id, BatchProcessResultModel bpr)
        {
            Notification obj = null;
            bool noErrorFlag = true;
            obj = dc.Notifications.SingleOrDefault(u => u.Id == id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.SeenDate = DateTime.Now;
                        obj.Readen = true;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Notification>(obj, bpr);
        }



        #endregion

        #region Delete

        public static DALReturnModel<Notification> Delete(Notification model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Notification> Delete(SocialLearningDataContext dc, Notification model)
        {
            try
            {
                var obj = dc.Notifications.Single(q => q.Id == model.Id);
                dc.Notifications.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<Notification>(new Notification { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Notification>(new Notification { Id = 0 });
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
                var obj = dc.Notifications.Where(q => q.ObjectId == objectId && q.ObjectType == objectType).ToList();
                foreach (var item in obj)
                {
                    dc.Notifications.DeleteOnSubmit(item);

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
