using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class App_UserProfileDAL
    {

        #region Get

        public static App_UserProfile Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static App_UserProfile Get(SocialLearningDataContext dc, int Id)
        {
            return dc.App_UserProfiles.SingleOrDefault(u => u.Id == Id);
        }

        #endregion

        #region GetAll

        public static List<App_UserProfile> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<App_UserProfile> GetAll(SocialLearningDataContext dc)
        {
            return dc.App_UserProfiles.ToList();
        }

        #endregion

        #region Find

        public static IQueryable<App_UserProfile> Find(App_UserProfileSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<App_UserProfile> Find(SocialLearningDataContext dc, App_UserProfileSearchModel model)
        {
            var qry = from p in dc.App_UserProfiles select p;
            if (model != null)
            {
            }
            if (!string.IsNullOrEmpty(model.SortExpression))
            {
                qry = qry.OrderBy(model.SortExpression);
            }
            qry = qry.OrderBy(u => u.Id);
            return qry;
        }

        #endregion

        #region Add

        public static DALReturnModel<App_UserProfile> Add(App_UserProfile model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_UserProfile> Add(SocialLearningDataContext dc, App_UserProfile model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new App_UserProfile();
                    obj.UserId = model.UserId;
                    dc.App_UserProfiles.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<App_UserProfile>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_UserProfile>(new App_UserProfile { Id = 0 }, bpr);
        }

        public static DALReturnModel<App_UserProfile> GetOrAddIfNotExist(int userId)
        {
            return GetOrAddIfNotExist(DBUtility.GetSocialLearningDataContext, userId);
        }

        public static DALReturnModel<App_UserProfile> GetOrAddIfNotExist(SocialLearningDataContext dc, int userId)
        {
            bool noErrorFlag = true;
            var obj = dc.App_UserProfiles.SingleOrDefault(x => x.UserId == userId);
            if (obj != null)
                return new DALReturnModel<App_UserProfile>(obj);
            try
            {
                if (noErrorFlag)
                {
                    obj = new App_UserProfile();
                    obj.UserId = userId;
                    obj.GetNotificationEmails = true;
                    obj.GetUnreadenMessagesEmail = true;
                    obj.Guid = Guid.NewGuid();
                    obj.LastUnreadenMessagesEmailSent = DateTime.Now;
                    dc.App_UserProfiles.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<App_UserProfile>(obj);
                }
            }
            catch
            {
            }
            return new DALReturnModel<App_UserProfile>(new App_UserProfile { Id = 0 });
        }

        #endregion

        #region Update

        public static DALReturnModel<App_UserProfile> Update(App_UserProfile model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_UserProfile> Update(SocialLearningDataContext dc, App_UserProfile model, BatchProcessResultModel bpr)
        {
            App_UserProfile obj = null;
            bool noErrorFlag = true;
            obj = dc.App_UserProfiles.SingleOrDefault(u => u.Guid == model.Guid);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.GetNotificationEmails = model.GetNotificationEmails;
                        obj.GetUnreadenMessagesEmail = model.GetUnreadenMessagesEmail;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_UserProfile>(obj, bpr);
        }

        public static DALReturnModel<App_UserProfile> UpdateSentEmailDate(int userId)
        {
            return UpdateSentEmailDate(DBUtility.GetSocialLearningDataContext, userId);
        }

        public static DALReturnModel<App_UserProfile> UpdateSentEmailDate(SocialLearningDataContext dc, int userId)
        {
            bool noErrorFlag = true;
            var profiles = dc.App_UserProfiles.Where(u => u.Id == userId).ToList();
            try
            {
                if (noErrorFlag && profiles.Any())
                {

                    foreach (var item in profiles)
                    {
                        item.LastUnreadenMessagesEmailSent = DateTime.Now;
                        dc.SubmitChanges();

                    }

                }
            }
            catch
            {
            }
            return new DALReturnModel<App_UserProfile>(new App_UserProfile());
        }


        #endregion

        #region Delete

        public static DALReturnModel<App_UserProfile> Delete(App_UserProfile model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<App_UserProfile> Delete(SocialLearningDataContext dc, App_UserProfile model)
        {
            try
            {
                var obj = dc.App_UserProfiles.Single(q => q.Id == model.Id);
                dc.App_UserProfiles.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<App_UserProfile>(new App_UserProfile { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<App_UserProfile>(new App_UserProfile { Id = 0 });
            }
        }

        #endregion
    }
}
