using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{
    public partial class MessageDAL
    {

        #region Get

        public static Message Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static Message Get(SocialLearningDataContext dc, int Id)
        {
            return dc.Messages.SingleOrDefault(u => u.Id == Id);
        }

        public static Message Get(System.Guid GuidId)
        {
            return Get(DBUtility.GetSocialLearningDataContext, GuidId);
        }

        public static Message Get(SocialLearningDataContext dc, System.Guid GuidId)
        {
            return dc.Messages.SingleOrDefault(u => u.GuidId == GuidId);
        }

        #endregion

        #region GetAll

        public static List<Message> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<Message> GetAll(SocialLearningDataContext dc)
        {
            return dc.Messages.ToList();
        }

        public static List<Message> GetLongUnreadenMessages()
        {
            return GetLongUnreadenMessages(DBUtility.GetSocialLearningDataContext);
        }

        public static List<Message> GetLongUnreadenMessages(SocialLearningDataContext dc)
        {
            return dc.Messages.Where(x => !x.Seen.Value && (x.App_User1.App_UserProfiles.Any(q => q.GetUnreadenMessagesEmail) || x.App_User1.App_UserProfiles.Count() == 0)
                /*&& x.App_User1.App_UserProfile.LastUnreadenMessagesEmailSent.Value.Date < DateTime.Now.AddDays(-7).Date */).ToList();

        }

        #endregion

        #region Find

        public static IQueryable<Message> Find(MessageSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<Message> Find(SocialLearningDataContext dc, MessageSearchModel model)
        {
            var qry = from p in dc.Messages select p;
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.SearchSubject))
                {
                    qry = qry.Where(u => u.Subject.Contains(model.SearchSubject.StringNormalizer()));
                }
                if (!string.IsNullOrEmpty(model.SearchBody))
                {
                    qry = qry.Where(u => u.Body.Contains(model.SearchBody.StringNormalizer()));
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

        public static DALReturnModel<Message> Add(Message model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Message> Add(SocialLearningDataContext dc, Message model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    if (model.Id > 0)
                    {
                        var msg = dc.Messages.SingleOrDefault(x => x.Id == model.Id);
                        if (msg != null)
                        {
                            msg = model;
                        }
                        else
                            dc.Messages.InsertOnSubmit(model);
                    }
                    else
                        dc.Messages.InsertOnSubmit(model);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<Message>(model, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Message>(new Message { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<Message> Update(Message model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Message> Update(SocialLearningDataContext dc, Message model, BatchProcessResultModel bpr)
        {
            Message obj = null;
            bool noErrorFlag = true;
            obj = dc.Messages.SingleOrDefault(u => u.GuidId == model.GuidId);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Subject = model.Subject.StringNormalizer();

                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Message>(obj, bpr);
        }

        public static DALReturnModel<Message> UpdateFile(Message model, BatchProcessResultModel bpr)
        {
            return UpdateFile(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Message> UpdateFile(SocialLearningDataContext dc, Message model, BatchProcessResultModel bpr)
        {
            Message obj = null;
            bool noErrorFlag = true;
            obj = dc.Messages.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.FileContent = model.FileContent;
                        obj.FileMime = model.FileMime;
                        obj.FileSize = model.FileSize;
                        obj.FileTitle = model.FileTitle;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Message>(obj);
        }

        public static DALReturnModel<Message> UpdateSeen(Message model)
        {
            return UpdateSeen(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Message> UpdateSeen(SocialLearningDataContext dc, Message model)
        {
            Message obj = null;
            bool noErrorFlag = true;
            obj = dc.Messages.SingleOrDefault(u => u.GuidId == model.GuidId);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Seen = true;
                        obj.SeenDate = DateTime.Now;
                        dc.SubmitChanges();
                    }
                }
            }
            catch
            {
            }
            return new DALReturnModel<Message>(obj);
        }

        #endregion

        #region Delete

        public static DALReturnModel<Message> Delete(Message model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Message> Delete(SocialLearningDataContext dc, Message model)
        {
            try
            {
                var obj = dc.Messages.Single(q => q.GuidId == model.GuidId || q.Id == model.Id);
                model.Id = obj.Id;
                var threadId = obj.ThreadId.Value;
                obj.MessageThread.MessageCount = obj.MessageThread.MessageCount - 1;
                dc.Messages.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                MessageThreadDAL.UpdateSnippest(new MessageThread { Id = threadId });
                return new DALReturnModel<Message>(new Message { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Message>(new Message { Id = 0 });
            }
        }

        public static DALReturnModel<Message> DeleteFile(Message model)
        {
            return DeleteFile(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Message> DeleteFile(SocialLearningDataContext dc, Message model)
        {
            try
            {
                var obj = dc.Assignments.Single(q => q.Id == model.Id);
                obj.FileContent = null;
                obj.FileMime = null;
                obj.FileSize = null;
                obj.FileTitle = null;
                dc.SubmitChanges();
                return new DALReturnModel<Message>(new Message { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Message>(new Message { Id = 0 });
            }
        }

        #endregion
    }
}
