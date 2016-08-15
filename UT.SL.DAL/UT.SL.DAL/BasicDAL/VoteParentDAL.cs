using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class VoteParentDAL
    {

        #region Get

        public static VoteParent Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static VoteParent Get(SocialLearningDataContext dc, int Id)
        {
            return dc.VoteParents.SingleOrDefault(u => u.Id == Id);
        }

        public static VoteParent Get(int objectId, int objectType)
        {
            return Get(DBUtility.GetSocialLearningDataContext, objectId, objectType);
        }

        public static VoteParent Get(SocialLearningDataContext dc, int objectId, int objectType)
        {
            return dc.VoteParents.SingleOrDefault(u => u.ObjectId == objectId && u.ObjectType == objectType && u.Count != 0);
        }

        #endregion

        #region GetAll

        public static List<VoteParent> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<VoteParent> GetAll(SocialLearningDataContext dc)
        {
            return dc.VoteParents.ToList();
        }

        public static List<VoteParent> GetAllUserAndObject(int userId, int objectId, int objectType)
        {
            return GetAllUserAndObject(DBUtility.GetSocialLearningDataContext, userId, objectId, objectType);
        }

        public static List<VoteParent> GetAllUserAndObject(SocialLearningDataContext dc, int userId, int objectId, int objectType)
        {
            return dc.VoteParents.Where(x => x.ObjectId == objectId && x.ObjectType == objectType && x.Votes.Any(u => u.UserId == userId)).ToList();
        }

        public static List<VoteParent> GetAll(int objectId, int objectType)
        {
            return GetAll(DBUtility.GetSocialLearningDataContext, objectId, objectType);
        }

        public static List<VoteParent> GetAll(SocialLearningDataContext dc, int objectId, int objectType)
        {
            return dc.VoteParents.Where(x => x.ObjectId == objectId && x.ObjectType == objectType).ToList();
        }

        #endregion

        #region Find

        public static IQueryable<VoteParent> Find(VoteParentSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<VoteParent> Find(SocialLearningDataContext dc, VoteParentSearchModel model)
        {
            var qry = from p in dc.VoteParents select p;
            if (model != null)
            {
                if (model.SearchObjectType.HasValue && model.SearchObjectType > 0)
                {
                    qry = qry.Where(u => u.ObjectType == model.SearchObjectType);
                }
                if (model.SearchUpvoteCount.HasValue && model.SearchUpvoteCount > 0)
                {
                    qry = qry.Where(u => u.UpvoteCount == model.SearchUpvoteCount);
                }
                if (model.SearchDownvoteCount.HasValue && model.SearchDownvoteCount > 0)
                {
                    qry = qry.Where(u => u.DownvoteCount == model.SearchDownvoteCount);
                }
                if (model.SearchCount.HasValue && model.SearchCount > 0)
                {
                    qry = qry.Where(u => u.Count == model.SearchCount);
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

        public static DALReturnModel<VoteParent> Add(VoteParent model, out BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, out bpr);
        }

        public static DALReturnModel<VoteParent> Add(SocialLearningDataContext dc, VoteParent model, out BatchProcessResultModel bpr)
        {
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new VoteParent();
                    obj.ObjectId = model.ObjectId;
                    obj.ObjectType = model.ObjectType;
                    obj.UpvoteCount = model.UpvoteCount;
                    obj.DownvoteCount = model.DownvoteCount;
                    obj.Count = model.Count;
                    obj.VoteValueAverage = model.VoteValueAverage;
                    dc.VoteParents.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<VoteParent>(obj);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<VoteParent>(new VoteParent { Id = 0 });
        }

        public static DALReturnModel<VoteParent> AddNotExist(VoteParent model)
        {
            return AddNotExist(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<VoteParent> AddNotExist(SocialLearningDataContext dc, VoteParent model)
        {
            bool noErrorFlag = true;
            var obj = new VoteParent();
            obj = dc.VoteParents.FirstOrDefault(x => x.ObjectId == model.ObjectId && x.ObjectType == model.ObjectType);
            if (obj != null)
                return new DALReturnModel<VoteParent>(obj);
            try
            {
                if (noErrorFlag)
                {
                    obj = new VoteParent();
                    obj.ObjectId = model.ObjectId;
                    obj.ObjectType = model.ObjectType;
                    obj.UpvoteCount = 0;
                    obj.DownvoteCount = 0;
                    obj.Count = 0;
                    obj.VoteValueAverage = 0;
                    dc.VoteParents.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<VoteParent>(obj);
                }
            }
            catch
            {
            }
            return new DALReturnModel<VoteParent>(new VoteParent { Id = 0 });
        }

        #endregion

        #region Update

        public static DALReturnModel<VoteParent> Update(VoteParent model)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<VoteParent> Update(SocialLearningDataContext dc, VoteParent model)
        {
            VoteParent obj = null;
            bool noErrorFlag = true;
            obj = dc.VoteParents.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.UpvoteCount = model.UpvoteCount;
                        obj.DownvoteCount = model.DownvoteCount;
                        obj.Count = model.Count;
                        dc.SubmitChanges();
                    }
                }
            }
            catch
            {
            }
            return new DALReturnModel<VoteParent>(obj);
        }

        public static DALReturnModel<VoteParent> UpdateReaction(VoteParent model)
        {
            return UpdateReaction(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<VoteParent> UpdateReaction(SocialLearningDataContext dc, VoteParent model)
        {
            VoteParent obj = null;
            bool noErrorFlag = true;
            obj = dc.VoteParents.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.VoteValueAverage = model.VoteValueAverage;
                        obj.Count = model.Count;
                        dc.SubmitChanges();
                    }
                }
            }
            catch
            {
            }
            return new DALReturnModel<VoteParent>(obj);
        }

        #endregion

        #region Delete

        public static DALReturnModel<VoteParent> Delete(VoteParent model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<VoteParent> Delete(SocialLearningDataContext dc, VoteParent model)
        {
            try
            {
                var obj = dc.VoteParents.Single(q => q.Id == model.Id);
                var votes = obj.Votes.ToList();
                foreach (var item in votes)
                {
                    dc.Votes.DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }
                dc.VoteParents.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<VoteParent>(new VoteParent { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<VoteParent>(new VoteParent { Id = 0 });
            }
        }

        public static DALReturnModel<VoteParent> DeleteAllVote(int objectId, int objectType)
        {
            return DeleteAllVote(DBUtility.GetSocialLearningDataContext, objectId, objectType);
        }

        public static DALReturnModel<VoteParent> DeleteAllVote(SocialLearningDataContext dc, int objectId, int objectType)
        {
            try
            {
                var obj = dc.VoteParents.FirstOrDefault(q => q.ObjectId == objectId && q.ObjectType == objectType);
                var votes = obj.Votes.ToList();
                foreach (var item in votes)
                {
                    dc.Votes.DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }
                dc.VoteParents.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<VoteParent>(new VoteParent { Id = objectId }); ///!!!wrong
            }
            catch (System.Exception)
            {
                return new DALReturnModel<VoteParent>(new VoteParent { Id = 0 });
            }
        }

        #endregion
    }
}
