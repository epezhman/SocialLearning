using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class ForumDiscussionDAL
    {

        #region Get

        public static ForumDiscussion Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static ForumDiscussion Get(SocialLearningDataContext dc, int Id)
        {
            return dc.ForumDiscussions.SingleOrDefault(u => u.Id == Id);
        }

        #endregion

        #region GetAll

        public static List<ForumDiscussion> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<ForumDiscussion> GetAll(SocialLearningDataContext dc)
        {
            return dc.ForumDiscussions.ToList();
        }

        public static List<ForumDiscussion> GetAllDiscussions(int id)
        {
            return GetAllDiscussions(DBUtility.GetSocialLearningDataContext, id);
        }

        public static List<ForumDiscussion> GetAllDiscussions(SocialLearningDataContext dc, int id)
        {
            return dc.ForumDiscussions.Where(x => (x.Id == id || x.ParentId == id) && (x.IsPublishd && x.IsValid)).ToList();
        }

        public static List<ForumDiscussion> GetDiscussionReplies(int Id)
        {
            return GetDiscussionReplies(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static List<ForumDiscussion> GetDiscussionReplies(SocialLearningDataContext dc, int Id)
        {
            return dc.ForumDiscussions.Where(x => x.ParentId == Id).ToList();
        }

        public static List<ForumDiscussion> GetAllDiscussionsByForumAndUser(int forumId, int userId)
        {
            return GetAllDiscussionsByForumAndUser(DBUtility.GetSocialLearningDataContext, forumId, userId);
        }

        public static List<ForumDiscussion> GetAllDiscussionsByForumAndUser(SocialLearningDataContext dc, int forumId, int userId)
        {
            return dc.ForumDiscussions.Where(x => (x.ForumId == forumId && x.UserId == userId) && (x.IsPublishd && x.IsValid)).ToList();
        }

        public static List<App_User> GetAllAssociatedUser(int forumId, int parentId)
        {
            return GetAllAssociatedUser(DBUtility.GetSocialLearningDataContext, forumId, parentId);
        }

        public static List<App_User> GetAllAssociatedUser(SocialLearningDataContext dc, int forumId, int parentId)
        {
            return dc.ForumDiscussions.Where(x => x.ForumId == forumId && (x.ParentId == parentId || x.Id == parentId)).Select(x => x.App_User).Distinct().ToList();
        }

        public static int CountNewDiscussions(int objectId, int objectType, DateTime date, int userId)
        {
            return CountNewDiscussions(DBUtility.GetSocialLearningDataContext, objectId, objectType, date, userId);
        }

        public static int CountNewDiscussions(SocialLearningDataContext dc, int objectId, int objectType, DateTime date, int userId)
        {
            return dc.ForumDiscussions.Count(x => x.ForumId == objectId && x.CreateDate >= date && x.UserId != userId && !x.ParentId.HasValue);
        }

        public static int CountNewReplies(int objectId, int objectType, DateTime date, int userId)
        {
            return CountNewReplies(DBUtility.GetSocialLearningDataContext, objectId, objectType, date, userId);
        }

        public static int CountNewReplies(SocialLearningDataContext dc, int objectId, int objectType, DateTime date, int userId)
        {
            return dc.ForumDiscussions.Count(x => x.ForumId == objectId && x.CreateDate >= date && x.UserId != userId && x.ParentId.HasValue);
        }

        public static int CountNewRepliesForOneDiscussion(int parentId, DateTime date, int userId)
        {
            return CountNewRepliesForOneDiscussion(DBUtility.GetSocialLearningDataContext, parentId, date, userId);
        }

        public static int CountNewRepliesForOneDiscussion(SocialLearningDataContext dc, int parentId, DateTime date, int userId)
        {
            return dc.ForumDiscussions.Count(x => x.ParentId == parentId && x.CreateDate >= date && x.UserId != userId);
        }

        public static List<ForumDiscussion> GetAllByCourseNew(int id)
        {
            return GetAllByCourseNew(DBUtility.GetSocialLearningDataContext, id);
        }

        public static List<ForumDiscussion> GetAllByCourseNew(SocialLearningDataContext dc, int id)
        {
            return dc.ForumDiscussions.Where(x => x.Forum.CourseId == id && !x.ParentId.HasValue).ToList();
        }

        public static List<ForumDiscussion> GetAllByCourseReplies(int id)
        {
            return GetAllByCourseReplies(DBUtility.GetSocialLearningDataContext, id);
        }

        public static List<ForumDiscussion> GetAllByCourseReplies(SocialLearningDataContext dc, int id)
        {
            return dc.ForumDiscussions.Where(x => x.Forum.CourseId == id && x.ParentId.HasValue).ToList();
        }

        public static List<ForumDiscussion> GetAllByCourse(int id)
        {
            return GetAllByCourse(DBUtility.GetSocialLearningDataContext, id);
        }

        public static List<ForumDiscussion> GetAllByCourse(SocialLearningDataContext dc, int id)
        {
            return dc.ForumDiscussions.Where(x => x.Forum.CourseId == id ).ToList();
        }

        public static int GetRepliesCount(int id)
        {
            return GetRepliesCount(DBUtility.GetSocialLearningDataContext, id);
        }

        public static int GetRepliesCount(SocialLearningDataContext dc, int id)
        {
            return dc.ForumDiscussions.Count(x => x.ParentId == id);
        }

        public static int GetDiscussionsCount(int id)
        {
            return GetDiscussionsCount(DBUtility.GetSocialLearningDataContext, id);
        }

        public static int GetDiscussionsCount(SocialLearningDataContext dc, int id)
        {
            return dc.ForumDiscussions.Count(x => x.ForumId == id && !x.ParentId.HasValue);
        }

        #endregion

        #region Find

        public static IQueryable<ForumDiscussion> Find(ForumDiscussionsearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<ForumDiscussion> Find(SocialLearningDataContext dc, ForumDiscussionsearchModel model)
        {
            var qry = from p in dc.ForumDiscussions select p;
            if (model != null)
            {

                if (!string.IsNullOrEmpty(model.SearchBody))
                {
                    qry = qry.Where(u => u.Body.Contains(model.SearchBody.StringNormalizer()));
                }
                if (model.SearchCreateDate.HasValue && model.SearchCreateDate > DateTime.MinValue)
                {
                    qry = qry.Where(u => u.CreateDate == model.SearchCreateDate.WesternizeDateTime());
                }
                if (model.SearchViewCount.HasValue && model.SearchViewCount > 0)
                {
                    qry = qry.Where(u => u.ViewCount == model.SearchViewCount);
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

        public static DALReturnModel<ForumDiscussion> Add(ForumDiscussion model)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<ForumDiscussion> Add(SocialLearningDataContext dc, ForumDiscussion model)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new ForumDiscussion();
                    obj.Body = model.Body.StringNormalizer();
                    obj.CreateDate = DateTime.Now;
                    obj.ViewCount = model.ViewCount;
                    obj.ForumId = model.ForumId;
                    obj.UserId = model.UserId;
                    dc.ForumDiscussions.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<ForumDiscussion>(obj);
                }
            }
            catch
            {
            }
            return new DALReturnModel<ForumDiscussion>(new ForumDiscussion { Id = 0 });
        }

        public static DALReturnModel<ForumDiscussion> Add(ForumDiscussion model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<ForumDiscussion> Add(SocialLearningDataContext dc, ForumDiscussion model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    model.Title = model.Title.StringNormalizer();
                    dc.ForumDiscussions.InsertOnSubmit(model);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<ForumDiscussion>(model);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<ForumDiscussion>(new ForumDiscussion { Id = 0 });
        }

        #endregion

        #region Update

        public static DALReturnModel<ForumDiscussion> Update(ForumDiscussion model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<ForumDiscussion> Update(SocialLearningDataContext dc, ForumDiscussion model, BatchProcessResultModel bpr)
        {
            ForumDiscussion obj = null;
            bool noErrorFlag = true;
            obj = dc.ForumDiscussions.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Title = model.Title.StringNormalizer();
                        obj.Body = model.Body.StringNormalizer();
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<ForumDiscussion>(obj, bpr);
        }

        public static DALReturnModel<ForumDiscussion> UpdateFile(ForumDiscussion model, BatchProcessResultModel bpr)
        {
            return UpdateFile(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<ForumDiscussion> UpdateFile(SocialLearningDataContext dc, ForumDiscussion model, BatchProcessResultModel bpr)
        {
            ForumDiscussion obj = null;
            bool noErrorFlag = true;
            obj = dc.ForumDiscussions.SingleOrDefault(u => u.Id == model.Id);
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
            return new DALReturnModel<ForumDiscussion>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<ForumDiscussion> Delete(ForumDiscussion model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<ForumDiscussion> Delete(SocialLearningDataContext dc, ForumDiscussion model)
        {
            try
            {
                var obj = dc.ForumDiscussions.Single(q => q.Id == model.Id);
                dc.ForumDiscussions.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<ForumDiscussion>(new ForumDiscussion { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<ForumDiscussion>(new ForumDiscussion { Id = 0 });
            }
        }

        public static DALReturnModel<ForumDiscussion> DeleteFile(ForumDiscussion model)
        {
            return DeleteFile(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<ForumDiscussion> DeleteFile(SocialLearningDataContext dc, ForumDiscussion model)
        {
            try
            {
                var obj = dc.ForumDiscussions.Single(q => q.Id == model.Id);
                obj.FileContent = null;
                obj.FileMime = null;
                obj.FileSize = null;
                obj.FileTitle = null;
                dc.SubmitChanges();
                return new DALReturnModel<ForumDiscussion>(new ForumDiscussion { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<ForumDiscussion>(new ForumDiscussion { Id = 0 });
            }
        }

        #endregion

    }
}
