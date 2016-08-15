using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class CommentDAL
    {

        #region Get

        public static Comment Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static Comment Get(SocialLearningDataContext dc, int Id)
        {
            return dc.Comments.SingleOrDefault(u => u.Id == Id);
        }

        public static bool CommentAny(int objectId, int type)
        {
            return CommentAny(DBUtility.GetSocialLearningDataContext, objectId, type);
        }

        public static bool CommentAny(SocialLearningDataContext dc, int objectId, int type)
        {
            return dc.Comments.Any(x => x.ObjectId == objectId && x.Type == type);
        }

        #endregion

        #region GetAll

        public static List<Comment> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<Comment> GetAll(SocialLearningDataContext dc)
        {
            return dc.Comments.ToList();
        }

        public static List<Comment> GetAll(int objectId, int type)
        {
            return GetAll(DBUtility.GetSocialLearningDataContext, objectId, type);
        }

        public static List<Comment> GetAll(SocialLearningDataContext dc, int objectId, int type)
        {
            return dc.Comments.Where(x => x.Type == type && x.ObjectId == objectId).OrderBy(x => x.CreateDate).ToList();
        }

        public static List<App_User> GetAllAssociatedUser(int objectId, int type)
        {
            return GetAllAssociatedUser(DBUtility.GetSocialLearningDataContext, objectId, type);
        }

        public static List<App_User> GetAllAssociatedUser(SocialLearningDataContext dc, int objectId, int type)
        {
            return dc.Comments.Where(x => x.Type == type && x.ObjectId == objectId).Select(x => x.App_User).Distinct().ToList();
        }

        public static int GetAllCount(int objectId, int type)
        {
            return GetAllCount(DBUtility.GetSocialLearningDataContext, objectId, type);
        }

        public static int GetAllCount(SocialLearningDataContext dc, int objectId, int type)
        {
            return dc.Comments.Where(x => x.Type == type && x.ObjectId == objectId).Count();
        }

        public static List<Comment> GetAllUserAndObject(int userId, int objectId, int objectType)
        {
            return GetAllUserAndObject(DBUtility.GetSocialLearningDataContext, userId, objectId, objectType);
        }

        public static List<Comment> GetAllUserAndObject(SocialLearningDataContext dc, int userId, int objectId, int objectType)
        {
            return dc.Comments.Where(x => x.Type == objectType && x.ObjectId == objectId && x.App_User.Id == userId).ToList();
        }

        public static int CountNewComments(int objectId, int objectType, DateTime date, int userId)
        {
            return CountNewComments(DBUtility.GetSocialLearningDataContext, objectId, objectType, date, userId);
        }

        public static int CountNewComments(SocialLearningDataContext dc, int objectId, int objectType, DateTime date, int userId)
        {
            return dc.Comments.Count(x => x.Type == objectType && x.ObjectId == objectId && x.CreateDate >= date && x.OwnerId != userId);
        }

        #endregion

        #region Find

        public static IQueryable<Comment> Find(CommentSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<Comment> Find(SocialLearningDataContext dc, CommentSearchModel model)
        {
            var qry = from p in dc.Comments select p;
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.SearchTitle))
                {
                    qry = qry.Where(u => u.Title.Contains(model.SearchTitle.StringNormalizer()));
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

        public static DALReturnModel<Comment> Add(Comment model)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Comment> Add(SocialLearningDataContext dc, Comment model)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new Comment();
                    obj.Title = model.Title.StringNormalizer();
                    obj.CreateDate = DateTime.Now;
                    obj.OwnerId = model.OwnerId;
                    obj.ObjectId = model.ObjectId;
                    obj.Type = model.Type;
                    dc.Comments.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<Comment>(obj);
                }
            }
            catch
            {
            }
            return new DALReturnModel<Comment>(new Comment { Id = 0 });
        }

        #endregion

        #region Update

        public static DALReturnModel<Comment> Update(Comment model)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Comment> Update(SocialLearningDataContext dc, Comment model)
        {
            Comment obj = null;
            bool noErrorFlag = true;
            obj = dc.Comments.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Title = model.Title.StringNormalizer();
                        dc.SubmitChanges();
                    }
                }
            }
            catch
            {
            }
            return new DALReturnModel<Comment>(obj);
        }

        #endregion

        #region Delete

        public static DALReturnModel<Comment> Delete(Comment model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Comment> Delete(SocialLearningDataContext dc, Comment model)
        {
            try
            {
                var obj = dc.Comments.Single(q => q.Id == model.Id);
                VoteParentDAL.DeleteAllVote(obj.Id, (int)UT.SL.Model.Enumeration.ObjectType.Comment);
                dc.Comments.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<Comment>(new Comment { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Comment>(new Comment { Id = 0 });
            }
        }

        public static DALReturnModel<Comment> DeleteObjectComments(int objectId, int objectType)
        {
            return DeleteObjectComments(DBUtility.GetSocialLearningDataContext, objectId, objectType);
        }

        public static DALReturnModel<Comment> DeleteObjectComments(SocialLearningDataContext dc, int objectId, int objectType)
        {
            try
            {
                foreach (var item in dc.Comments.Where(q => q.ObjectId == objectId && q.Type == objectType).ToList())
                {
                    VoteParentDAL.DeleteAllVote(item.Id, (int)UT.SL.Model.Enumeration.ObjectType.Comment);
                    foreach (var log in item.CommentLogs.ToList())
                    {
                        dc.CommentLogs.DeleteOnSubmit(log);
                        dc.SubmitChanges();
                    }
                    dc.Comments.DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }
                return new DALReturnModel<Comment>(new Comment { Id = objectId }); ///!!!wrong
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Comment>(new Comment { Id = 0 });
            }
        }

        #endregion
    }
}
