using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{
    public partial class MessageThreadDAL
    {

        #region Get

        public static MessageThread Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static MessageThread Get(SocialLearningDataContext dc, int Id)
        {
            return dc.MessageThreads.SingleOrDefault(u => u.Id == Id);
        }

        public static MessageThread Get(System.Guid GuidId)
        {
            return Get(DBUtility.GetSocialLearningDataContext, GuidId);
        }

        public static MessageThread Get(SocialLearningDataContext dc, System.Guid GuidId)
        {
            return dc.MessageThreads.SingleOrDefault(u => u.GuidId == GuidId);
        }

        public static List<MessageThread> Get(int ownerId, int receiverId)
        {
            return Get(DBUtility.GetSocialLearningDataContext, ownerId, receiverId);
        }

        public static List<MessageThread> Get(SocialLearningDataContext dc, int ownerId, int receiverId)
        {
            return dc.MessageThreads.Where(u => (u.OwnerUserId == ownerId && u.AssociatedUserId == receiverId) ||
                                                         (u.OwnerUserId == receiverId && u.AssociatedUserId == ownerId)).ToList();
        }

        public static int GetUnSeenThreadCount(int Id)
        {
            return GetUnSeenThreadCount(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static int GetUnSeenThreadCount(SocialLearningDataContext dc, int Id)
        {
            return dc.MessageThreads.Where(u => u.OwnerUserId == Id && u.HasNotSeen).Count();
        }

        #endregion

        #region GetAll

        public static List<MessageThread> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<MessageThread> GetAll(SocialLearningDataContext dc)
        {
            return dc.MessageThreads.ToList();
        }

        public static List<MessageThread> GetAll(int ownerId)
        {
            return GetAll(DBUtility.GetSocialLearningDataContext, ownerId);
        }

        public static List<MessageThread> GetAll(SocialLearningDataContext dc, int ownerId)
        {
            return dc.MessageThreads.Where(x => x.OwnerUserId == ownerId).ToList();
        }

        #endregion

        #region Find

        public static IQueryable<MessageThread> Find(MessageThreadSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<MessageThread> Find(SocialLearningDataContext dc, MessageThreadSearchModel model)
        {
            var qry = from p in dc.MessageThreads select p;
            if (model != null)
            {
                if (model.SearchMessageCount.HasValue && model.SearchMessageCount > 0)
                {
                    qry = qry.Where(u => u.MessageCount == model.SearchMessageCount);
                }
                if (!string.IsNullOrEmpty(model.SearchSubject))
                {
                    qry = qry.Where(u => u.Subject.Contains(model.SearchSubject.StringNormalizer()));
                }
                if (model.SearchLastUpdate.HasValue && model.SearchLastUpdate > DateTime.MinValue)
                {
                    qry = qry.Where(u => u.LastUpdate == model.SearchLastUpdate.WesternizeDateTime());
                }
                if (model.SearchCreateDate.HasValue && model.SearchCreateDate > DateTime.MinValue)
                {
                    qry = qry.Where(u => u.CreateDate == model.SearchCreateDate.WesternizeDateTime());
                }
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
        public static DALReturnModel<MessageThread> Add(MessageThread model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<MessageThread> Add(SocialLearningDataContext dc, MessageThread model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    dc.MessageThreads.InsertOnSubmit(model);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<MessageThread>(model, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<MessageThread>(new MessageThread { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<MessageThread> Update(MessageThread model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<MessageThread> Update(SocialLearningDataContext dc, MessageThread model, BatchProcessResultModel bpr)
        {
            MessageThread obj = null;
            bool noErrorFlag = true;
            obj = dc.MessageThreads.SingleOrDefault(u => u.GuidId == model.GuidId || u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj = model;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<MessageThread>(obj, bpr);
        }

        public static DALReturnModel<MessageThread> UpdateSeen(MessageThread model)
        {
            return UpdateSeen(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<MessageThread> UpdateSeen(SocialLearningDataContext dc, MessageThread model)
        {
            MessageThread obj = null;
            bool noErrorFlag = true;
            obj = dc.MessageThreads.SingleOrDefault(u => u.GuidId == model.GuidId || u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.HasNotSeen = false;
                        obj.HasNotReaden = false; 
                        dc.SubmitChanges();
                    }
                }
            }
            catch
            {
            }
            return new DALReturnModel<MessageThread>(obj);
        }

        public static DALReturnModel<MessageThread> UpdateSnippest(MessageThread model)
        {
            return UpdateSnippest(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<MessageThread> UpdateSnippest(SocialLearningDataContext dc, MessageThread model)
        {
            MessageThread obj = null;
            bool noErrorFlag = true;
            obj = dc.MessageThreads.SingleOrDefault(u => u.GuidId == model.GuidId || u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        var lastMessage = obj.Messages.OrderBy(x => x.CreateDate).LastOrDefault();
                        if (lastMessage == null)
                            obj.Snippest = string.Empty;
                        else
                        {
                            if (lastMessage.Body.Length >= 33)
                                obj.Snippest = lastMessage.Body.Substring(0, 32) + "...";
                            else obj.Snippest = lastMessage.Body;
                        }
                        dc.SubmitChanges();

                    }
                }
            }
            catch
            {
            }
            return new DALReturnModel<MessageThread>(obj);
        }

        #endregion

        #region Delete

        public static DALReturnModel<MessageThread> Delete(MessageThread model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<MessageThread> Delete(SocialLearningDataContext dc, MessageThread model)
        {
            try
            {
                var obj = dc.MessageThreads.Single(q => q.GuidId == model.GuidId || q.Id == model.Id);
                model.Id = obj.Id;
                var messages = obj.Messages.ToList();
                foreach (var item in messages)
                {
                    dc.Messages.DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }
                dc.MessageThreads.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<MessageThread>(new MessageThread { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<MessageThread>(new MessageThread { Id = 0 });
            }
        }

        #endregion
    }
}
