using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;
using System.Web;


namespace UT.SL.DAL
{


    public partial class CategoryDAL
    {

        #region Get

        public static Category Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);

        }

        public static Category Get(SocialLearningDataContext dc, int Id)
        {
            return dc.Categories.SingleOrDefault(u => u.Id == Id);
        }

        public static Category Get(string Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static Category Get(SocialLearningDataContext dc, string Id)
        {
            return dc.Categories.SingleOrDefault(u => u.Title == Id);
        }

        public static Category GetFirstWithCourse(string Id, int courseId)
        {
            return GetFirstWithCourse(DBUtility.GetSocialLearningDataContext, Id, courseId);
        }

        public static Category GetFirstWithCourse(SocialLearningDataContext dc, string Id, int courseId)
        {
            var result = dc.Categories.FirstOrDefault(u => u.Title.ToLower().Contains(Id) && u.Categories.Any(x => x.Tags.Any()) && u.CourseAbstracts.Any(p => p.Courses.Any(c => c.Id == courseId)));
            if (result != null)
                return result;
            result = dc.Categories.FirstOrDefault(u => u.Title.ToLower().Contains(Id) && u.Tags.Any() && u.CourseAbstracts.Any(p => p.Courses.Any(c => c.Id == courseId)));
            if (result != null)
                return result;
            return dc.Categories.FirstOrDefault(u => u.Title.ToLower().Contains(Id) && u.CourseAbstracts.Any(p => p.Courses.Any(c => c.Id == courseId)));
        }

        public static Category GetFirst(string Id)
        {
            return GetFirst(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static Category GetFirst(SocialLearningDataContext dc, string Id)
        {
            var result = dc.Categories.FirstOrDefault(u => u.Title.ToLower().Contains(Id) && u.Categories.Any(x => x.Tags.Any()));
            if (result != null)
                return result;
            result = dc.Categories.FirstOrDefault(u => u.Title.ToLower().Contains(Id) && u.Tags.Any());
            if (result != null)
                return result;
            return dc.Categories.FirstOrDefault(u => u.Title.ToLower().Contains(Id));
        }

        public static Category GetFirst(string Id, int parentId)
        {
            return GetFirst(DBUtility.GetSocialLearningDataContext, Id, parentId);
        }

        public static Category GetFirst(SocialLearningDataContext dc, string Id, int parentId)
        {
            return dc.Categories.FirstOrDefault(u => u.Title.ToLower().Contains(Id) && u.ParentId == parentId && u.Tags.Any());
        }

        #endregion

        #region GetAll

        public static List<Category> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<Category> GetAll(SocialLearningDataContext dc)
        {
            var cts = dc.Categories.ToList();
            var temp = new List<Category>();
            foreach (var item in cts.Where(x => x.ParentId == null))
            {
                temp.Add(item);
                temp.AddRange(cts.Where(x => x.ParentId == item.Id));
            }
            return temp;
        }

        public static List<Category> GetAllWithoutParent()
        {
            return GetAllWithoutParent(DBUtility.GetSocialLearningDataContext);
        }

        public static List<Category> GetAllWithoutParent(SocialLearningDataContext dc)
        {
            return dc.Categories.Where(x => x.ParentId == null).ToList();
        }

        public static List<Category> GetAll(int Id)
        {
            return GetAll(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static List<Category> GetAll(SocialLearningDataContext dc, int Id)
        {
            return dc.Categories.Where(x => x.ParentId == Id).ToList();
        }

        public static List<CourseAbstract> GetAllCources(int Id)
        {
            return GetAllCources(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static List<CourseAbstract> GetAllCources(SocialLearningDataContext dc, int Id)
        {
            return dc.CourseAbstracts.Where(x => x.SubCategoryId == Id).ToList();
        }

        #endregion

        #region Find

        public static IQueryable<Category> Find(CategorySearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<Category> Find(SocialLearningDataContext dc, CategorySearchModel model)
        {
            var qry = from p in dc.Categories select p;
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.SearchTitle))
                {
                    qry = qry.Where(u => u.Title.Contains(model.SearchTitle.StringNormalizer()));
                }
            }
            if (!string.IsNullOrEmpty(model.SortExpression))
            {
                qry = qry.OrderBy(model.SortExpression);
            }
            qry = qry.OrderBy(u => u.Id).OrderBy(x => x.ParentId);
            return qry;
        }

        #endregion

        #region Add

        public static DALReturnModel<Category> Add(Category model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Category> Add(SocialLearningDataContext dc, Category model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            if (dc.Categories.Any(x => x.Title == model.Title.StringNormalizer()))
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.BprDupInput, true, true);
                noErrorFlag = false;
            }
            try
            {
                if (noErrorFlag)
                {
                    var obj = new Category();
                    obj.Title = model.Title.StringNormalizer();
                    if (model.ParentId.HasValue && model.ParentId > 0)
                        obj.ParentId = model.ParentId;
                    dc.Categories.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<Category>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Category>(new Category { Id = 0 }, bpr);
        }

        public static DALReturnModel<Category> SaveImage(int Id, HttpPostedFileBase image, BatchProcessResultModel bpr)
        {
            return SaveImage(DBUtility.GetSocialLearningDataContext, Id, image, bpr);
        }

        public static DALReturnModel<Category> SaveImage(SocialLearningDataContext dc, int Id, HttpPostedFileBase image, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            var obj = dc.Categories.SingleOrDefault(u => u.Id == Id);
            try
            {
                if (noErrorFlag)
                {
                    if (image != null)
                    {
                        byte[] tempFile = null;
                        tempFile = new byte[image.ContentLength];
                        image.InputStream.Read(tempFile, 0, image.ContentLength);
                        obj.ImageData = tempFile;
                        obj.ImageMIME = image.ContentType;
                    }
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<Category>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Category>(new Category { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<Category> Update(Category model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Category> Update(SocialLearningDataContext dc, Category model, BatchProcessResultModel bpr)
        {
            Category obj = null;
            bool noErrorFlag = true;
            if (dc.Categories.Any(x => x.Title == model.Title.StringNormalizer() && x.Id != model.Id))
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.BprDupInput, true, true);
                noErrorFlag = false;
            }
            obj = dc.Categories.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Title = model.Title.StringNormalizer();
                        if (model.ParentId.HasValue && model.ParentId > 0)
                            obj.ParentId = model.ParentId;
                        else
                            obj.ParentId = null;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Category>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<Category> Delete(Category model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Category> Delete(SocialLearningDataContext dc, Category model)
        {
            try
            {
                var obj = dc.Categories.Single(q => q.Id == model.Id);
                foreach (var item in dc.Categories.Where(q => q.ParentId == model.Id).ToList())
                {
                    foreach (var mapper in item.CategoryMappers.ToList())
                    {
                        dc.CategoryMappers.DeleteOnSubmit(mapper);
                        dc.SubmitChanges();
                    }
                    foreach (var abstrct in item.CourseAbstracts.ToList())
                    {
                        foreach (var crs in abstrct.Courses.ToList())
                        {
                            crs.CourseAbstractId = null;
                            dc.SubmitChanges();
                        }
                        foreach (var topic in abstrct.CourseTopcMappers.ToList())
                        {
                            dc.CourseTopcMappers.DeleteOnSubmit(topic);
                            dc.SubmitChanges();
                        }
                        dc.CourseAbstracts.DeleteOnSubmit(abstrct);
                        dc.SubmitChanges();
                    }
                    dc.Categories.DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }
                foreach (var mapper in obj.CategoryMappers.ToList())
                {
                    dc.CategoryMappers.DeleteOnSubmit(mapper);
                    dc.SubmitChanges();
                }
                foreach (var abstrct in obj.CourseAbstracts.ToList())
                {
                    foreach (var crs in abstrct.Courses.ToList())
                    {
                        crs.CourseAbstractId = null;
                        dc.SubmitChanges();
                    }
                    foreach (var topic in abstrct.CourseTopcMappers.ToList())
                    {
                        dc.CourseTopcMappers.DeleteOnSubmit(topic);
                        dc.SubmitChanges();
                    }
                    dc.CourseAbstracts.DeleteOnSubmit(abstrct);
                    dc.SubmitChanges();
                }
                dc.Categories.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<Category>(new Category { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Category>(new Category { Id = 0 });
            }
        }

        public static DALReturnModel<Category> DeletePic(Category model)
        {
            return DeletePic(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Category> DeletePic(SocialLearningDataContext dc, Category model)
        {
            try
            {
                var obj = dc.Categories.Single(q => q.Id == model.Id);
                obj.ImageData = null;
                obj.ImageMIME = string.Empty;
                dc.SubmitChanges();
                return new DALReturnModel<Category>(new Category { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Category>(new Category { Id = 0 });
            }
        }

        #endregion
    }
}
