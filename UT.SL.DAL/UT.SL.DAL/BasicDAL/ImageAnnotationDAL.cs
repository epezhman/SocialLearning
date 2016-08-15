using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class ImageAnnotationDAL
    {

        #region Get

        public static ImageAnnotation Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static ImageAnnotation Get(SocialLearningDataContext dc, int Id)
        {
            return dc.ImageAnnotations.SingleOrDefault(u => u.Id == Id);
        }

        #endregion

        #region GetAll

        public static List<ImageAnnotation> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<ImageAnnotation> GetAll(SocialLearningDataContext dc)
        {
            return dc.ImageAnnotations.ToList();
        }

        public static List<ImageAnnotation> GetAll(int objectId, int objectType)
        {
            return GetAll(DBUtility.GetSocialLearningDataContext, objectId, objectType);
        }

        public static List<ImageAnnotation> GetAll(SocialLearningDataContext dc, int objectId, int objectType)
        {
            return dc.ImageAnnotations.Where(x => x.ObjectId == objectId && x.ObjectType == objectType).ToList();
        }

        #endregion

        #region Find

        public static IQueryable<ImageAnnotation> Find(ImageAnnotationSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<ImageAnnotation> Find(SocialLearningDataContext dc, ImageAnnotationSearchModel model)
        {
            var qry = from p in dc.ImageAnnotations select p;
            if (model != null)
            {
                if (model.SearchTopCoord.HasValue && model.SearchTopCoord > 0)
                {
                    qry = qry.Where(u => u.TopCoord == model.SearchTopCoord);
                }
                if (model.SearchLeftCoord.HasValue && model.SearchLeftCoord > 0)
                {
                    qry = qry.Where(u => u.LeftCoord == model.SearchLeftCoord);
                }
                if (model.SearchWidth.HasValue && model.SearchWidth > 0)
                {
                    qry = qry.Where(u => u.Width == model.SearchWidth);
                }
                if (model.SearchHeight.HasValue && model.SearchHeight > 0)
                {
                    qry = qry.Where(u => u.Height == model.SearchHeight);
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

        public static DALReturnModel<ImageAnnotation> Add(ImageAnnotation model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<ImageAnnotation> Add(SocialLearningDataContext dc, ImageAnnotation model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new ImageAnnotation();
                    obj.TopCoord = model.TopCoord;
                    obj.LeftCoord = model.LeftCoord;
                    obj.Height = model.Height;
                    obj.Width = model.Width;
                    obj.ObjectType = model.ObjectType;
                    obj.ObjectId = model.ObjectId;
                    dc.ImageAnnotations.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<ImageAnnotation>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<ImageAnnotation>(new ImageAnnotation { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<ImageAnnotation> Update(ImageAnnotation model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<ImageAnnotation> Update(SocialLearningDataContext dc, ImageAnnotation model, BatchProcessResultModel bpr)
        {
            ImageAnnotation obj = null;
            bool noErrorFlag = true;
            obj = dc.ImageAnnotations.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.TopCoord = model.TopCoord;
                        obj.LeftCoord = model.LeftCoord;
                        obj.Height = model.Height;
                        obj.Width = model.Width;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<ImageAnnotation>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<ImageAnnotation> Delete(ImageAnnotation model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<ImageAnnotation> Delete(SocialLearningDataContext dc, ImageAnnotation model)
        {
            try
            {
                var obj = dc.ImageAnnotations.Single(q => q.Id == model.Id);
                TagMapperDAL.DeleteAll(obj.ObjectId, obj.ObjectType);
                dc.ImageAnnotations.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<ImageAnnotation>(new ImageAnnotation { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<ImageAnnotation>(new ImageAnnotation { Id = 0 });
            }
        }

        #endregion
    }
}
