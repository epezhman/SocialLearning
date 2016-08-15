using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;



namespace UT.SL.DAL
{


    public partial class ForumDAL
    {

        #region Get

        public static Forum Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static Forum Get(SocialLearningDataContext dc, int Id)
        {
            return dc.Forums.SingleOrDefault(u => u.Id == Id);
        }

        public static ForumDiscussion GetDiscussion(int Id)
        {
            return GetDiscussion(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static ForumDiscussion GetDiscussion(SocialLearningDataContext dc, int Id)
        {
            return dc.ForumDiscussions.SingleOrDefault(u => u.Id == Id);
        }

        #endregion

        #region GetAll

        public static List<Forum> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<Forum> GetAll(SocialLearningDataContext dc)
        {
            return dc.Forums.ToList();
        }

        public static List<Forum> GetAllByCourse(int id)
        {
            return GetAllByCourse(DBUtility.GetSocialLearningDataContext, id);
        }

        public static List<Forum> GetAllByCourse(SocialLearningDataContext dc, int id)
        {
            return dc.Forums.Where(x => x.CourseId == id).ToList();
        }



        #endregion

        #region Find

        public static IQueryable<Forum> Find(ForumsearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<Forum> Find(SocialLearningDataContext dc, ForumsearchModel model)
        {
            var qry = from p in dc.Forums select p;

            if (model != null)
            {
                if (model.CourseId.HasValue)
                {
                    qry = qry.Where(u => u.CourseId == model.CourseId);
                }
                if (!string.IsNullOrEmpty(model.SearchTitle))
                {
                    qry = qry.Where(u => u.Title.Contains(model.SearchTitle.StringNormalizer()));
                }
                if (model.SearchType.HasValue && model.SearchType > 0)
                {
                    qry = qry.Where(u => u.Type == model.SearchType);
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

        public static DALReturnModel<Forum> Add(Forum model)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Forum> Add(SocialLearningDataContext dc, Forum model)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new Forum();
                    obj.Title = model.Title.StringNormalizer();
                    obj.Type = model.Type;
                    obj.CreateDate = DateTime.Now;
                    obj.CourseId = model.CourseId;
                    dc.Forums.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<Forum>(obj);
                }
            }
            catch
            {
            }
            return new DALReturnModel<Forum>(new Forum { Id = 0 });
        }


        public static DALReturnModel<Forum> Add(Forum model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Forum> Add(SocialLearningDataContext dc, Forum model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    model.Title = model.Title.StringNormalizer();
                    dc.Forums.InsertOnSubmit(model);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<Forum>(model);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Forum>(new Forum { Id = 0 });
        }

        #endregion

        #region Update

        public static DALReturnModel<Forum> Update(Forum model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Forum> Update(SocialLearningDataContext dc, Forum model, BatchProcessResultModel bpr)
        {
            Forum obj = null;
            bool noErrorFlag = true;
            obj = dc.Forums.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Title = model.Title.StringNormalizer();
                        obj.Body = model.Body.StringNormalizer();
                        obj.GradeFrom = model.GradeFrom;
                        obj.GetLockedAfterExpiration = model.GetLockedAfterExpiration;
                        obj.DueDate = model.DueDate;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Forum>(obj, bpr);
        }

        public static DALReturnModel<Forum> UpdateFile(Forum model, BatchProcessResultModel bpr)
        {
            return UpdateFile(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Forum> UpdateFile(SocialLearningDataContext dc, Forum model, BatchProcessResultModel bpr)
        {
            Forum obj = null;
            bool noErrorFlag = true;
            obj = dc.Forums.SingleOrDefault(u => u.Id == model.Id);
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
            return new DALReturnModel<Forum>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<Forum> Delete(Forum model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Forum> Delete(SocialLearningDataContext dc, Forum model)
        {
            try
            {
                var obj = dc.Forums.Single(q => q.Id == model.Id);
                dc.Forums.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<Forum>(new Forum { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Forum>(new Forum { Id = 0 });
            }
        }

        public static DALReturnModel<Forum> DeleteFile(Forum model)
        {
            return DeleteFile(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Forum> DeleteFile(SocialLearningDataContext dc, Forum model)
        {
            try
            {
                var obj = dc.Forums.Single(q => q.Id == model.Id);
                obj.FileContent = null;
                obj.FileMime = null;
                obj.FileSize = null;
                obj.FileTitle = null;
                dc.SubmitChanges();
                return new DALReturnModel<Forum>(new Forum { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Forum>(new Forum { Id = 0 });
            }
        }

        #endregion

    }
}
