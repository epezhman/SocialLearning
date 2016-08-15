using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class VoteDAL
    {

        #region Get

        public static Vote Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static Vote Get(SocialLearningDataContext dc, int Id)
        {
            return dc.Votes.SingleOrDefault(u => u.Id == Id);
        }

        public static Vote GetIfExist(int parentId, int userId)
        {
            return GetIfExist(DBUtility.GetSocialLearningDataContext, parentId, userId);
        }

        public static Vote GetIfExist(SocialLearningDataContext dc, int parentId, int userId)
        {
            return dc.Votes.SingleOrDefault(u => u.UserId == userId && u.ParentId == parentId);
        }

        #endregion

        #region GetAll

        public static List<Vote> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<Vote> GetAll(SocialLearningDataContext dc)
        {
            return dc.Votes.ToList();
        }

        public static List<Vote> GetByParent(int parentId)
        {
            return GetByParent(DBUtility.GetSocialLearningDataContext, parentId);
        }

        public static List<Vote> GetByParent(SocialLearningDataContext dc, int parentId)
        {
            return dc.Votes.Where(u => u.ParentId == parentId).ToList();
        }

        public static int GetAllCount(int objectId, int type)
        {
            return GetAllCount(DBUtility.GetSocialLearningDataContext, objectId, type);
        }

        public static int GetAllCount(SocialLearningDataContext dc, int objectId, int type)
        {
            return dc.Votes.Where(x => x.VoteParent.ObjectType == type && x.VoteParent.ObjectId == objectId).Count();
        }

        public static int CountNewVotes(int objectId, int objectType, DateTime date, int userId)
        {
            return CountNewVotes(DBUtility.GetSocialLearningDataContext, objectId, objectType, date,  userId);
        }

        public static int CountNewVotes(SocialLearningDataContext dc, int objectId, int objectType, DateTime date, int userId)
        {
            return dc.Votes.Count(x => x.VoteParent.ObjectType == objectType && x.VoteParent.ObjectId == objectId && x.CreateDate >= date && x.UserId != userId);
        }

        #endregion

        #region Find

        public static IQueryable<Vote> Find(VoteSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<Vote> Find(SocialLearningDataContext dc, VoteSearchModel model)
        {
            var qry = from p in dc.Votes select p;
            if (model != null)
            {
                if (model.SearchUpdown != null && model.SearchUpdown.HasValue)
                {
                    qry = qry.Where(u => u.Updown == model.SearchUpdown);
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

        public static DALReturnModel<Vote> AddUpDownVote(Vote model)
        {
            return AddUpDownVote(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Vote> AddUpDownVote(SocialLearningDataContext dc, Vote model)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new Vote();
                    obj.Updown = model.Updown;
                    obj.CreateDate = DateTime.Now;
                    obj.UserId = model.UserId;
                    obj.ParentId = model.ParentId;
                    obj.VoteValue = 0;
                    dc.Votes.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<Vote>(obj);
                }
            }
            catch
            {
            }
            return new DALReturnModel<Vote>(new Vote { Id = 0 });
        }
        public static DALReturnModel<Vote> AddReaction(Vote model)
        {
            return AddReaction(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Vote> AddReaction(SocialLearningDataContext dc, Vote model)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new Vote();
                    obj.Updown = false;
                    obj.CreateDate = DateTime.Now;
                    obj.UserId = model.UserId;
                    obj.ParentId = model.ParentId;
                    obj.VoteValue = model.VoteValue;
                    dc.Votes.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<Vote>(obj);
                }
            }
            catch
            {
            }
            return new DALReturnModel<Vote>(new Vote { Id = 0 });
        }

        #endregion

        #region Update

        public static DALReturnModel<Vote> UpdateUpDownVote(Vote model)
        {
            return UpdateUpDownVote(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Vote> UpdateUpDownVote(SocialLearningDataContext dc, Vote model)
        {
            Vote obj = null;
            bool noErrorFlag = true;
            obj = dc.Votes.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Updown = model.Updown;
                        dc.SubmitChanges();
                    }
                }
            }
            catch
            {
            }
            return new DALReturnModel<Vote>(obj);
        }

        public static DALReturnModel<Vote> UpdateReaction(Vote model)
        {
            return UpdateReaction(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Vote> UpdateReaction(SocialLearningDataContext dc, Vote model)
        {
            Vote obj = null;
            bool noErrorFlag = true;
            obj = dc.Votes.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.VoteValue = model.VoteValue;
                        dc.SubmitChanges();
                    }
                }
            }
            catch
            {
            }
            return new DALReturnModel<Vote>(obj);
        }

        #endregion

        #region Delete

        public static DALReturnModel<Vote> Delete(Vote model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Vote> Delete(SocialLearningDataContext dc, Vote model)
        {
            try
            {
                var obj = dc.Votes.Single(q => q.Id == model.Id);
                dc.Votes.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<Vote>(new Vote { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Vote>(new Vote { Id = 0 });
            }
        }

        #endregion
    }
}
