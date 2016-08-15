using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class ShareDAL
    {

        #region Get

        public static Share Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static Share Get(SocialLearningDataContext dc, int Id)
        {
            return dc.Shares.SingleOrDefault(u => u.Id == Id);
        }

        #endregion

        #region GetAll

        public static List<Share> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<Share> GetAll(SocialLearningDataContext dc)
        {
            return dc.Shares.ToList();
        }

        public static List<Share> GetAll(int objectId, int type)
        {
            return GetAll(DBUtility.GetSocialLearningDataContext, objectId, type);
        }

        public static List<Share> GetAll(SocialLearningDataContext dc, int objectId, int type)
        {
            return dc.Shares.Where(x => x.ObjectId == objectId && x.Type == type).ToList();
        }

        public static List<Share> GetAllObject(int objectId, int type, int userId)
        {
            return GetAllObject(DBUtility.GetSocialLearningDataContext, objectId, type, userId);
        }

        public static List<Share> GetAllObject(SocialLearningDataContext dc, int objectId, int type, int userId)
        {
            return dc.Shares.Where(x => x.ObjectId == objectId && x.Type == type && x.UserId == userId).ToList();
        }

        public static IEnumerable<ResourceBackbone> GetAllSocialGroupShared(SocialGroup sl)
        {
            return GetAllSocialGroupShared(DBUtility.GetSocialLearningDataContext, sl);
        }

        public static IEnumerable<ResourceBackbone> GetAllLearningGroupShared(SocialLearningDataContext dc, LearningGroup lg)
        {
            return dc.Shares.Where(x => x.LearningGroupId == lg.Id).OrderByDescending(x => x.CreateDate).Select(x => new ResourceBackbone { Id = x.Id, Type = Model.Enumeration.SharedType.FromLearningGroup, CreateDate = x.CreateDate.Value });
        }

        public static IEnumerable<ResourceBackbone> GetAllLearningGroupShared(LearningGroup lg)
        {
            return GetAllLearningGroupShared(DBUtility.GetSocialLearningDataContext, lg);
        }

        public static IEnumerable<ResourceBackbone> GetAllSocialGroupShared(SocialLearningDataContext dc, SocialGroup sl)
        {
            return dc.Shares.Where(x => x.SocialGroupId == sl.Id).OrderByDescending(x => x.CreateDate).Select(x => new ResourceBackbone { Id = x.Id, Type = Model.Enumeration.SharedType.FromSocialGroup, CreateDate = x.CreateDate.Value });
        }

        public static IEnumerable<ResourceBackbone> GetAllNewLearningGroupShared(LearningGroup lg, DateTime date)
        {
            return GetAllNewLearningGroupShared(DBUtility.GetSocialLearningDataContext, lg, date);
        }

        public static IEnumerable<ResourceBackbone> GetAllNewLearningGroupShared(SocialLearningDataContext dc, LearningGroup lg, DateTime date)
        {
            return dc.Shares.Where(x => x.LearningGroupId == lg.Id && x.CreateDate >= date).OrderByDescending(x => x.CreateDate).Select(x => new ResourceBackbone { Id = x.Id, Type = Model.Enumeration.SharedType.FromLearningGroup, CreateDate = x.CreateDate.Value });
        }

        public static IEnumerable<ResourceBackbone> GetAllNewSocialGroupShared(SocialGroup sl, DateTime date)
        {
            return GetAllNewSocialGroupShared(DBUtility.GetSocialLearningDataContext, sl, date);
        }

        public static IEnumerable<ResourceBackbone> GetAllNewSocialGroupShared(SocialLearningDataContext dc, SocialGroup sl, DateTime date)
        {
            return dc.Shares.Where(x => x.SocialGroupId == sl.Id && x.CreateDate >= date).OrderByDescending(x => x.CreateDate).Select(x => new ResourceBackbone { Id = x.Id, Type = Model.Enumeration.SharedType.FromLearningGroup, CreateDate = x.CreateDate.Value });
        }

        public static IEnumerable<ResourceBackbone> GetAllShredWithUser(int userId)
        {
            return GetAllShredWithUser(DBUtility.GetSocialLearningDataContext, userId);
        }

        public static IEnumerable<ResourceBackbone> GetAllShredWithUser(SocialLearningDataContext dc, int userId)
        {
            return dc.Shares.Where(x => x.UserShareId == userId).OrderByDescending(x => x.CreateDate).Select(x => new ResourceBackbone { Id = x.Id, Type = Model.Enumeration.SharedType.FromUser, CreateDate = x.CreateDate.Value });
        }

        public static IEnumerable<ResourceBackbone> GetAllNewShredWithUser(int userId, DateTime date)
        {
            return GetAllNewShredWithUser(DBUtility.GetSocialLearningDataContext, userId, date);
        }

        public static IEnumerable<ResourceBackbone> GetAllNewShredWithUser(SocialLearningDataContext dc, int userId, DateTime date)
        {
            return dc.Shares.Where(x => x.UserShareId == userId && x.CreateDate >= date).OrderByDescending(x => x.CreateDate).Select(x => new ResourceBackbone { Id = x.Id, Type = Model.Enumeration.SharedType.FromUser, CreateDate = x.CreateDate.Value });
        }

        #endregion

        #region Find

        public static IQueryable<Share> Find(ShareSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<Share> Find(SocialLearningDataContext dc, ShareSearchModel model)
        {
            var qry = from p in dc.Shares select p;
            if (model != null)
            {
                if (model.SearchCreateDate.HasValue && model.SearchCreateDate > DateTime.MinValue)
                {
                    qry = qry.Where(u => u.CreateDate == model.SearchCreateDate.WesternizeDateTime());
                }
                if (model.SearchType.HasValue && model.SearchType > 0)
                {
                    qry = qry.Where(u => u.Type == model.SearchType);
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

        public static DALReturnModel<Share> Add(Share model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Share> Add(SocialLearningDataContext dc, Share model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new Share();
                    obj.ObjectId = model.ObjectId;
                    obj.CreateDate = DateTime.Now;
                    obj.Type = model.Type;
                    obj.UserId = model.UserId;
                    if (model.SocialGroupId.HasValue && model.SocialGroupId > 0)
                        obj.SocialGroupId = model.SocialGroupId;
                    if (model.UserShareId.HasValue && model.UserShareId > 0)
                        obj.UserShareId = model.UserShareId;
                    if (model.LearningGroupId.HasValue && model.LearningGroupId > 0)
                        obj.LearningGroupId = model.LearningGroupId;
                    dc.Shares.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<Share>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Share>(new Share { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<Share> Update(Share model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Share> Update(SocialLearningDataContext dc, Share model, BatchProcessResultModel bpr)
        {
            Share obj = null;
            bool noErrorFlag = true;
            obj = dc.Shares.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        if (model.SocialGroupId.HasValue && model.SocialGroupId > 0)
                            obj.SocialGroupId = model.SocialGroupId;
                        if (model.UserShareId.HasValue && model.UserShareId > 0)
                            obj.UserShareId = model.UserShareId;
                        if (model.LearningGroupId.HasValue && model.LearningGroupId > 0)
                            obj.LearningGroupId = model.LearningGroupId;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Share>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<Share> Delete(Share model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Share> Delete(SocialLearningDataContext dc, Share model)
        {
            try
            {
                var obj = dc.Shares.Single(q => q.Id == model.Id);
                dc.Shares.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<Share>(new Share { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Share>(new Share { Id = 0 });
            }
        }

        public static DALReturnModel<Share> DeleteAllObject(int objectId, int type, int userId)
        {
            return DeleteAllObject(DBUtility.GetSocialLearningDataContext, objectId, type, userId);
        }

        public static DALReturnModel<Share> DeleteAllObject(SocialLearningDataContext dc, int objectId, int type, int userId)
        {
            try
            {
                foreach (var item in dc.Shares.Where(x => x.ObjectId == objectId && x.Type == type && x.UserId == userId))
                {
                    dc.Shares.DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }
                return new DALReturnModel<Share>(new Share { Id = objectId }); ///!!!wrong
            }
            catch
            {
                return new DALReturnModel<Share>(new Share { Id = 0 });
            }
        }

        public static DALReturnModel<Share> DeleteObjectShare(int objectId, int objectType)
        {
            return DeleteObjectShare(DBUtility.GetSocialLearningDataContext, objectId, objectType);
        }

        public static DALReturnModel<Share> DeleteObjectShare(SocialLearningDataContext dc, int objectId, int objectType)
        {
            try
            {
                foreach (var item in dc.Shares.Where(x => x.ObjectId == objectId && x.Type == objectType))
                {
                    dc.Shares.DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }
                return new DALReturnModel<Share>(new Share { Id = objectId }); ///!!!wrong
            }
            catch
            {
                return new DALReturnModel<Share>(new Share { Id = 0 });
            }
        }

        #endregion
    }
}
