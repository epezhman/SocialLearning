using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{
    public partial class MessageContactDAL
    {

        #region Get

        public static MessageContact Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static MessageContact Get(SocialLearningDataContext dc, int Id)
        {
            return dc.MessageContacts.SingleOrDefault(u => u.Id == Id);
        }        

        #endregion

        #region GetAll

        public static List<MessageContact> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<MessageContact> GetAll(SocialLearningDataContext dc)
        {
            return dc.MessageContacts.ToList();
        }

        public static List<MessageContact> GetContacts(int ownerId, int contactId)
        {
            return GetContacts(DBUtility.GetSocialLearningDataContext, ownerId, contactId);
        }

        public static List<MessageContact> GetContacts(SocialLearningDataContext dc, int ownerId, int contactId)
        {
            return dc.MessageContacts.Where(u => (u.OwnerUserId == ownerId && u.ContactUserId == contactId) ||
                                                         (u.OwnerUserId == contactId && u.ContactUserId == ownerId)).ToList();
        }
        #endregion

        #region Add

        public static DALReturnModel<MessageContact> Add(MessageContact model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<MessageContact> Add(SocialLearningDataContext dc, MessageContact model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new MessageContact();
                    obj.OwnerUserId = model.OwnerUserId;
                    obj.ContactUserId = model.ContactUserId;
                    obj.CreateDate = DateTime.Now;
                    dc.MessageContacts.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<MessageContact>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<MessageContact>(new MessageContact { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<MessageContact> Update(MessageContact model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<MessageContact> Update(SocialLearningDataContext dc, MessageContact model, BatchProcessResultModel bpr)
        {
            MessageContact obj = null;
            bool noErrorFlag = true;
            obj = dc.MessageContacts.SingleOrDefault(u => u.Id == model.Id);
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
            return new DALReturnModel<MessageContact>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<MessageContact> Delete(Answer model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<MessageContact> Delete(SocialLearningDataContext dc, Answer model)
        {
            try
            {
                var obj = dc.Answers.Single(q => q.GuidId == model.GuidId);
                dc.Answers.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<MessageContact>(new MessageContact { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<MessageContact>(new MessageContact { Id = 0 });
            }
        }

        #endregion
    }
}
