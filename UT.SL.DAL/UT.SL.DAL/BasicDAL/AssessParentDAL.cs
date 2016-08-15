using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;

namespace UT.SL.DAL
{
    public partial class AssessParentDAL
    {

        #region Get

        public static AssessParent Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static AssessParent Get(SocialLearningDataContext dc, int Id)
        {
            return dc.AssessParents.SingleOrDefault(u => u.Id == Id);
        }

        #endregion

        #region GetAll

        public static List<AssessParent> GetAll(int objectId, int type)
        {
            return GetAll(DBUtility.GetSocialLearningDataContext, objectId, type);
        }

        public static List<AssessParent> GetAll(SocialLearningDataContext dc, int objectId, int type)
        {
            return dc.AssessParents .Where(x => x.ObjectType == type && x.ObjectId == objectId).ToList();
        }
        
        #endregion        

        #region Add

        public static DALReturnModel<AssessParent> Add(AssessParent model,  BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model,  bpr);
        }

        public static DALReturnModel<AssessParent> Add(SocialLearningDataContext dc, AssessParent model,  BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new AssessParent();
                    obj.ObjectId = model.ObjectId;
                    obj.ObjectType = model.ObjectType;
                    obj.UpvoteCount = model.UpvoteCount;
                    obj.DownvoteCount = model.DownvoteCount;
                    obj.Count = model.Count;
                    obj.AssessValueAverage  = model.AssessValueAverage ;
                    dc.AssessParents.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<AssessParent>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<AssessParent>(new AssessParent { Id = 0 }, bpr);
        }

        public static DALReturnModel<AssessParent> AddNotExist(AssessParent model)
        {
            return AddNotExist(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<AssessParent> AddNotExist(SocialLearningDataContext dc, AssessParent model)
        {
            bool noErrorFlag = true;
            var obj = new AssessParent();
            obj = dc.AssessParents.SingleOrDefault(x => x.ObjectId == model.ObjectId && x.ObjectType == model.ObjectType);
            if (obj != null)
                return new DALReturnModel<AssessParent>(obj);
            try
            {
                if (noErrorFlag)
                {
                    obj = new AssessParent();
                    obj.ObjectId = model.ObjectId;
                    obj.ObjectType = model.ObjectType;
                    obj.UpvoteCount = 0;
                    obj.DownvoteCount = 0;
                    obj.Count = 0;
                    obj.AssessValueAverage  = 0;
                    dc.AssessParents.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<AssessParent>(obj);
                }
            }
            catch
            {
            }
            return new DALReturnModel<AssessParent>(new AssessParent { Id = 0 });
        }

        #endregion

        #region Update

        public static DALReturnModel<AssessParent> Update(AssessParent model)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<AssessParent> Update(SocialLearningDataContext dc, AssessParent model)
        {
            AssessParent obj = null;
            bool noErrorFlag = true;
            obj = dc.AssessParents.SingleOrDefault(u => u.Id == model.Id);
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
            return new DALReturnModel<AssessParent>(obj);
        }

        public static DALReturnModel<AssessParent> UpdateReaction(AssessParent model)
        {
            return UpdateReaction(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<AssessParent> UpdateReaction(SocialLearningDataContext dc, AssessParent model)
        {
            AssessParent obj = null;
            bool noErrorFlag = true;
            obj = dc.AssessParents.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.AssessValueAverage  = model.AssessValueAverage ;
                        obj.Count = model.Count;
                        dc.SubmitChanges();
                    }

                }
            }
            catch
            {
            }
            return new DALReturnModel<AssessParent>(obj);
        }

        #endregion

        #region Delete

        public static DALReturnModel<AssessParent> Delete(AssessParent model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<AssessParent> Delete(SocialLearningDataContext dc, AssessParent model)
        {
            try
            {
                var obj = dc.AssessParents.Single(q => q.Id == model.Id);
                dc.AssessParents.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<AssessParent>(new AssessParent { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<AssessParent>(new AssessParent { Id = 0 });
            }
        }

        public static DALReturnModel<AssessParent> DeleteAllVote(int objectId, int objectType)
        {
            return DeleteAllVote(DBUtility.GetSocialLearningDataContext, objectId, objectType);
        }

        public static DALReturnModel<AssessParent> DeleteAllVote(SocialLearningDataContext dc, int objectId, int objectType)
        {
            try
            {
                var obj = dc.AssessParents.FirstOrDefault(q => q.ObjectId == objectId && q.ObjectType == objectType);
                var votes = obj.Assesses.ToList();
                foreach (var item in votes)
                {
                    dc.Assesses .DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }
                dc.AssessParents.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<AssessParent>(new AssessParent { Id = objectId });  ///!!!wrong
            }
            catch (System.Exception)
            {
                return new DALReturnModel<AssessParent>(new AssessParent { Id = 0 });
            }
        }

        #endregion
    }
}
