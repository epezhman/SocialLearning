using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class CategoryMapperDAL
    {

        #region Get

        public static CategoryMapper Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static CategoryMapper Get(SocialLearningDataContext dc, int Id)
        {
            return dc.CategoryMappers.SingleOrDefault(u => u.Id == Id);
        }

        #endregion

        #region GetAll

        public static List<CategoryMapper> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<CategoryMapper> GetAll(SocialLearningDataContext dc)
        {
            return dc.CategoryMappers.ToList();
        }

        #endregion

        #region Find

        public static IQueryable<CategoryMapper> Find(CategoryMapperSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<CategoryMapper> Find(SocialLearningDataContext dc, CategoryMapperSearchModel model)
        {
            var qry = from p in dc.CategoryMappers select p;
            if (model != null)
            {
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

        public static DALReturnModel<CategoryMapper> Add(CategoryMapper model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<CategoryMapper> Add(SocialLearningDataContext dc, CategoryMapper model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new CategoryMapper();
                    obj.CourseId = model.CourseId;
                    obj.CategoryId = model.CategoryId;
                    dc.CategoryMappers.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<CategoryMapper>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<CategoryMapper>(new CategoryMapper { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<CategoryMapper> Update(CategoryMapper model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<CategoryMapper> Update(SocialLearningDataContext dc, CategoryMapper model, BatchProcessResultModel bpr)
        {
            CategoryMapper obj = null;
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            obj = dc.CategoryMappers.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.CourseId = model.CourseId;
                        obj.CategoryId = model.CategoryId;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<CategoryMapper>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<CategoryMapper> Delete(CategoryMapper model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<CategoryMapper> Delete(SocialLearningDataContext dc, CategoryMapper model)
        {
            try
            {
                var obj = dc.CategoryMappers.Single(q => q.Id == model.Id);
                dc.CategoryMappers.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<CategoryMapper>(new CategoryMapper { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<CategoryMapper>(new CategoryMapper { Id = 0 });
            }
        }

        public static DALReturnModel<CategoryMapper> DeleteAll(int courseId)
        {
            return DeleteAll(DBUtility.GetSocialLearningDataContext, courseId);
        }

        public static DALReturnModel<CategoryMapper> DeleteAll(SocialLearningDataContext dc, int courseId)
        {
            try
            {
                foreach (var item in dc.CategoryMappers.Where(q => q.CourseId == courseId))
                {
                    dc.CategoryMappers.DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }
                return new DALReturnModel<CategoryMapper>(new CategoryMapper { Id = courseId }); ///!!!Wrong
            }
            catch (System.Exception)
            {
                return new DALReturnModel<CategoryMapper>(new CategoryMapper { Id = 0 });
            }
        }

        #endregion
    }
}
