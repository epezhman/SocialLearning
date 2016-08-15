using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class FileAnnotationDAL
    {

        #region Get

        public static FileAnnotation Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static FileAnnotation Get(SocialLearningDataContext dc, int Id)
        {
            return dc.FileAnnotations.SingleOrDefault(u => u.Id == Id);
        }

        #endregion

        #region GetAll

        public static List<FileAnnotation> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<FileAnnotation> GetAll(SocialLearningDataContext dc)
        {
            return dc.FileAnnotations.ToList();
        }

        #endregion

        #region Find

        public static IQueryable<FileAnnotation> Find(FileAnnotationSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<FileAnnotation> Find(SocialLearningDataContext dc, FileAnnotationSearchModel model)
        {
            var qry = from p in dc.FileAnnotations select p;
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.SearchText))
                {
                    qry = qry.Where(u => u.Text.Contains(model.SearchText.StringNormalizer()));
                }
                if (model.SearchObjectType.HasValue && model.SearchObjectType > 0)
                {
                    qry = qry.Where(u => u.ObjectType == model.SearchObjectType);
                }
                if (model.SearchCreateDate.HasValue && model.SearchCreateDate > DateTime.MinValue)
                {
                    qry = qry.Where(u => u.CreateDate == model.SearchCreateDate.WesternizeDateTime());
                }
                if (model.SearchFromPage.HasValue && model.SearchFromPage > 0)
                {
                    qry = qry.Where(u => u.FromPage == model.SearchFromPage);
                }
                if (model.SearchFromeLine.HasValue && model.SearchFromeLine > 0)
                {
                    qry = qry.Where(u => u.FromeLine == model.SearchFromeLine);
                }
                if (model.SearchFromLetter.HasValue && model.SearchFromLetter > 0)
                {
                    qry = qry.Where(u => u.FromLetter == model.SearchFromLetter);
                }
                if (model.SearchToPage.HasValue && model.SearchToPage > 0)
                {
                    qry = qry.Where(u => u.ToPage == model.SearchToPage);
                }
                if (model.SearchToLine.HasValue && model.SearchToLine > 0)
                {
                    qry = qry.Where(u => u.ToLine == model.SearchToLine);
                }
                if (model.SearchToLetter.HasValue && model.SearchToLetter > 0)
                {
                    qry = qry.Where(u => u.ToLetter == model.SearchToLetter);
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

        public static DALReturnModel<FileAnnotation> Add(FileAnnotation model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<FileAnnotation> Add(SocialLearningDataContext dc, FileAnnotation model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new FileAnnotation();
                    obj.Text = model.Text.StringNormalizer();
                    obj.ObjectId = model.ObjectId;
                    obj.ObjectType = model.ObjectType;
                    obj.CreateDate = DateTime.Now;
                    obj.FromPage = model.FromPage;
                    obj.FromeLine = model.FromeLine;
                    obj.FromLetter = model.FromLetter;
                    obj.ToPage = model.ToPage;
                    obj.ToLine = model.ToLine;
                    obj.ToLetter = model.ToLetter;
                    obj.UserId = model.UserId;
                    dc.FileAnnotations.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<FileAnnotation>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<FileAnnotation>(new FileAnnotation { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<FileAnnotation> Update(FileAnnotation model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<FileAnnotation> Update(SocialLearningDataContext dc, FileAnnotation model, BatchProcessResultModel bpr)
        {
            FileAnnotation obj = null;
            bool noErrorFlag = true;
            obj = dc.FileAnnotations.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Text = model.Text.StringNormalizer();
                        obj.ObjectId = model.ObjectId;
                        obj.ObjectType = model.ObjectType;
                        obj.FromPage = model.FromPage;
                        obj.FromeLine = model.FromeLine;
                        obj.FromLetter = model.FromLetter;
                        obj.ToPage = model.ToPage;
                        obj.ToLine = model.ToLine;
                        obj.ToLetter = model.ToLetter;
                        obj.UserId = model.UserId;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<FileAnnotation>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<FileAnnotation> Delete(FileAnnotation model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<FileAnnotation> Delete(SocialLearningDataContext dc, FileAnnotation model)
        {
            try
            {
                var obj = dc.FileAnnotations.Single(q => q.Id == model.Id);
                dc.FileAnnotations.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<FileAnnotation>(new FileAnnotation { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<FileAnnotation>(new FileAnnotation { Id = 0 });
            }
        }

        #endregion
    }
}
