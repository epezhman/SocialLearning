using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;



namespace UT.SL.DAL
{


    public partial class ForumDiscussionPostDAL
    {

        #region Get

        public static ForumDiscussionPost Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static ForumDiscussionPost Get(SocialLearningDataContext dc, int Id)
        {
            return dc.ForumDiscussionPosts.SingleOrDefault(u => u.Id == Id);
        }

        #endregion

        #region GetAll

        public static List<ForumDiscussionPost> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<ForumDiscussionPost> GetAll(SocialLearningDataContext dc)
        {
            return dc.ForumDiscussionPosts.ToList();

        }

        #endregion

        #region Find

        public static IQueryable<ForumDiscussionPost> Find(ForumDiscussionPostSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<ForumDiscussionPost> Find(SocialLearningDataContext dc, ForumDiscussionPostSearchModel model)
        {
            var qry = from p in dc.ForumDiscussionPosts select p;
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.SearchText))
                {
                    qry = qry.Where(u => u.Text.Contains(model.SearchText.StringNormalizer()));
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

        public static DALReturnModel<ForumDiscussionPost> Add(ForumDiscussionPost model)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<ForumDiscussionPost> Add(SocialLearningDataContext dc, ForumDiscussionPost model)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new ForumDiscussionPost();
                    obj.Text = model.Text.StringNormalizer();
                    obj.CreateDate = DateTime.Now;
                    obj.UserId = model.UserId;
                    obj.ParentId = model.ParentId;
                    dc.ForumDiscussionPosts.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<ForumDiscussionPost>(obj);
                }
            }
            catch
            {

            }
            return new DALReturnModel<ForumDiscussionPost>(new ForumDiscussionPost { Id = 0 });
        }

        #endregion

        #region Update

        public static DALReturnModel<ForumDiscussionPost> Update(ForumDiscussionPost model)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<ForumDiscussionPost> Update(SocialLearningDataContext dc, ForumDiscussionPost model)
        {
            ForumDiscussionPost obj = null;
            bool noErrorFlag = true;
            obj = dc.ForumDiscussionPosts.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Text = model.Text.StringNormalizer();
                        dc.SubmitChanges();
                    }
                }
            }
            catch
            {
            }
            return new DALReturnModel<ForumDiscussionPost>(obj);
        }

        #endregion

        #region Delete

        public static DALReturnModel<ForumDiscussionPost> Delete(ForumDiscussionPost model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<ForumDiscussionPost> Delete(SocialLearningDataContext dc, ForumDiscussionPost model)
        {
            try
            {
                var obj = dc.ForumDiscussionPosts.Single(q => q.Id == model.Id);
                dc.ForumDiscussionPosts.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<ForumDiscussionPost>(new ForumDiscussionPost { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<ForumDiscussionPost>(new ForumDiscussionPost { Id = 0 });
            }
        }

        #endregion

    }
}
