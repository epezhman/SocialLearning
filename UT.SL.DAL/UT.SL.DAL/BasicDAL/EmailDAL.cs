using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class EmailDAL
    {

        #region Get

        public static Email Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static Email Get(SocialLearningDataContext dc, int Id)
        {
            return dc.Emails.SingleOrDefault(u => u.Id == Id);
        }

        public static Email Get(string email)
        {
            return Get(DBUtility.GetSocialLearningDataContext, email);
        }

        public static Email Get(SocialLearningDataContext dc, string email)
        {
            return dc.Emails.SingleOrDefault(u => u.ReceiverEmail == email);
        }

        public static Email GetLastByUserId(int id)
        {
            return GetLastByUserId(DBUtility.GetSocialLearningDataContext, id);
        }

        public static Email GetLastByUserId(SocialLearningDataContext dc, int id)
        {
            return dc.Emails.Where(u => u.UserId == id).OrderByDescending(x => x.CreateDate).FirstOrDefault();
        }

        #endregion

        #region GetAll

        public static List<Email> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<Email> GetAll(SocialLearningDataContext dc)
        {
            return dc.Emails.ToList();
        }

        public static List<Email> GetByUserId(int id)
        {
            return GetByUserId(DBUtility.GetSocialLearningDataContext, id);
        }

        public static List<Email> GetByUserId(SocialLearningDataContext dc, int id)
        {
            return dc.Emails.Where(u => u.UserId == id).ToList();
        }

        public static List<Email> GetNotSent()
        {
            return GetNotSent(DBUtility.GetSocialLearningDataContext);
        }

        public static List<Email> GetNotSent(SocialLearningDataContext dc)
        {
            return dc.Emails.Where(u => u.Statuse == 0).ToList();
        }

        #endregion

        #region Find

        public static IQueryable<Email> Find(EmailSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<Email> Find(SocialLearningDataContext dc, EmailSearchModel model)
        {
            var qry = from p in dc.Emails select p;
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

        public static DALReturnModel<Email> Add(Email model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Email> Add(SocialLearningDataContext dc, Email model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new Email();
                    obj.Body = model.Body;
                    obj.CreateDate = model.CreateDate;
                    obj.ReceiverEmail = model.ReceiverEmail;
                    obj.SenderEmail = model.SenderEmail;
                    obj.Statuse = model.Statuse;
                    obj.Subject = model.Subject;
                    model.Type = model.Type;
                    if (model.UserId.HasValue)
                        obj.UserId = model.UserId;
                    dc.Emails.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<Email>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Email>(new Email { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<Email> Update(Email model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Email> Update(SocialLearningDataContext dc, Email model, BatchProcessResultModel bpr)
        {
            Email obj = null;
            bool noErrorFlag = true;
            obj = dc.Emails.SingleOrDefault(u => u.Id == model.Id);
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
            return new DALReturnModel<Email>(obj, bpr);
        }

        public static DALReturnModel<Email> UpdateSent(int id)
        {
            return UpdateSent(DBUtility.GetSocialLearningDataContext, id);
        }

        public static DALReturnModel<Email> UpdateSent(SocialLearningDataContext dc,int id)
        {
            Email obj = null;
            bool noErrorFlag = true;
            obj = dc.Emails.SingleOrDefault(u => u.Id == id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Statuse = 1;
                        dc.SubmitChanges();
                    }

                }
            }
            catch 
            {
            }
            return new DALReturnModel<Email>(obj);
        }

        #endregion

        #region Delete

        public static DALReturnModel<Email> Delete(Email model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Email> Delete(SocialLearningDataContext dc, Email model)
        {
            try
            {
                var obj = dc.Emails.Single(q => q.Id == model.Id);
                dc.Emails.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<Email>(new Email { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Email>(new Email { Id = 0 });
            }
        }

        #endregion
    }
}
