using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class EmailRecordDAL
    {

        #region Get

        public static EmailRecord Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static EmailRecord Get(SocialLearningDataContext dc, int Id)
        {
            return dc.EmailRecords.SingleOrDefault(u => u.Id == Id);
        }

        public static int CheckDoosMoocHourlyLimit(int mailServer)
        {
            return CheckDoosMoocHourlyLimit(DBUtility.GetSocialLearningDataContext, mailServer);
        }

        public static int CheckDoosMoocHourlyLimit(SocialLearningDataContext dc, int mailServer)
        {
            return dc.EmailRecords.Count(x => x.CreateDate >= DateTime.Now.AddHours(-1) && x.CreateDate <= DateTime.Now && x.MailServer == mailServer);
        }

        public static int CheckDoosMoocDailyLimit(int mailServer)
        {
            return CheckDoosMoocDailyLimit(DBUtility.GetSocialLearningDataContext, mailServer);
        }

        public static int CheckDoosMoocDailyLimit(SocialLearningDataContext dc, int mailServer)
        {
            return dc.EmailRecords.Count(x => x.CreateDate >= DateTime.Now.AddHours(-24) && x.CreateDate <= DateTime.Now && x.MailServer == mailServer);
        }

        public static int CheckUserDailyLimit(int mailType, int userId)
        {
            return CheckUserDailyLimit(DBUtility.GetSocialLearningDataContext, mailType, userId);
        }

        public static int CheckUserDailyLimit(SocialLearningDataContext dc, int mailType, int userId)
        {
            return dc.EmailRecords.Count(x => x.CreateDate >= DateTime.Now.AddHours(-24) && x.CreateDate <= DateTime.Now && x.MailServer == mailType && x.UserId == userId);
        }

        #endregion

        #region GetAll

        public static List<EmailRecord> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<EmailRecord> GetAll(SocialLearningDataContext dc)
        {
            return dc.EmailRecords.ToList();
        }

        #endregion

        #region Add

        public static DALReturnModel<EmailRecord> Add(EmailRecord model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<EmailRecord> Add(SocialLearningDataContext dc, EmailRecord model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new EmailRecord();
                    obj.CreateDate = DateTime.Now;
                    if (model.UserId.HasValue)
                        obj.UserId = model.UserId;
                    if (model.EmailType.HasValue)
                        obj.EmailType = model.EmailType;
                    if (model.MailServer.HasValue)
                        obj.MailServer = model.MailServer;
                    dc.EmailRecords.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<EmailRecord>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<EmailRecord>(new EmailRecord { Id = 0 }, bpr);
        }

        public static bool Add(EmailRecord model)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model);
        }

        public static bool Add(SocialLearningDataContext dc, EmailRecord model)
        {
            try
            {
                var obj = new EmailRecord();
                obj.CreateDate = model.CreateDate;
                if (model.UserId.HasValue)
                    obj.UserId = model.UserId;
                if (model.EmailType.HasValue)
                    obj.EmailType = model.EmailType;
                if (model.MailServer.HasValue)
                    obj.MailServer = model.MailServer;
                dc.EmailRecords.InsertOnSubmit(obj);
                dc.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }


        #endregion

        #region Update

        public static DALReturnModel<EmailRecord> Update(EmailRecord model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<EmailRecord> Update(SocialLearningDataContext dc, EmailRecord model, BatchProcessResultModel bpr)
        {
            EmailRecord obj = null;
            bool noErrorFlag = true;
            obj = dc.EmailRecords.SingleOrDefault(u => u.Id == model.Id);
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
            return new DALReturnModel<EmailRecord>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<EmailRecord> Delete(EmailRecord model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<EmailRecord> Delete(SocialLearningDataContext dc, EmailRecord model)
        {
            try
            {
                var obj = dc.EmailRecords.Single(q => q.Id == model.Id);
                dc.EmailRecords.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<EmailRecord>(new EmailRecord { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<EmailRecord>(new EmailRecord { Id = 0 });
            }
        }

        #endregion
    }
}
