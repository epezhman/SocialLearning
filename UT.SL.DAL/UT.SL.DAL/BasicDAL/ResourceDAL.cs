using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{

    public partial class ResourceDAL
    {

        #region Get

        public static Resource Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static Resource Get(SocialLearningDataContext dc, int Id)
        {
            return dc.Resources.SingleOrDefault(u => u.Id == Id);
        }

        public static Resource Get(System.Guid GuidId)
        {
            return Get(DBUtility.GetSocialLearningDataContext, GuidId);
        }

        public static Resource Get(SocialLearningDataContext dc, System.Guid GuidId)
        {
            return dc.Resources.SingleOrDefault(u => u.GuidId == GuidId);
        }

        #endregion

        #region GetAll

        public static List<Resource> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<Resource> GetAll(SocialLearningDataContext dc)
        {
            return dc.Resources.ToList();
        }

        public static List<Resource> GetAllByCourse(int Id)
        {
            return GetAllByCourse(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static List<Resource> GetAllByCourse(SocialLearningDataContext dc, int Id)
        {
            return dc.Resources.Where(x=>x.CourseId == Id).ToList();
        }


        public static IEnumerable<ResourceBackbone> GetAllBackbone(int Id)
        {
            return GetAllBackbone(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static IEnumerable<ResourceBackbone> GetAllBackbone(SocialLearningDataContext dc, int Id)
        {
            if (Id == 0)
                return new List<ResourceBackbone>();
            var result = dc.Resources.Where(x => x.CourseId == Id && x.IsValid && x.IsPublishd).OrderByDescending(x => x.CreateDate).Select(x => new ResourceBackbone { Id = x.Id, Type = Model.Enumeration.SharedType.FromCourse, CreateDate = x.CreateDate });
            return result;
        }

        public static IEnumerable<ResourceBackbone> GetNullBackbone()
        {
            return GetNullBackbone(DBUtility.GetSocialLearningDataContext);
        }

        public static IEnumerable<ResourceBackbone> GetNullBackbone(SocialLearningDataContext dc)
        {
            return new List<ResourceBackbone>();
        }

        public static IEnumerable<ResourceBackbone> GetAllNewBackbone(int Id, DateTime date, int userId)
        {
            return GetAllNewBackbone(DBUtility.GetSocialLearningDataContext, Id, date, userId);
        }

        public static IEnumerable<ResourceBackbone> GetAllNewBackbone(SocialLearningDataContext dc, int Id, DateTime date, int userId)
        {
            if (Id == 0)
                return new List<ResourceBackbone>();
            return dc.Resources.Where(x => x.CourseId == Id && x.IsValid && x.IsPublishd && x.CreateDate >= date && x.CreateUserId != userId).OrderByDescending(x => x.CreateDate).Select(x => new ResourceBackbone { Id = x.Id, Type = Model.Enumeration.SharedType.FromCourse, CreateDate = x.CreateDate });
        }

        public static List<Resource> GetAllForView(int courseId, int page, int take, DateTime? dateAfter)
        {
            return GetAllForView(DBUtility.GetSocialLearningDataContext, courseId, page, take, dateAfter);
        }

        public static List<Resource> GetAllForView(SocialLearningDataContext dc, int courseId, int page, int take, DateTime? dateAfter)
        {
            if (!dateAfter.HasValue)
                return dc.Resources.Where(x => x.CourseId == courseId && x.IsValid && x.IsPublishd).OrderByDescending(x => x.CreateDate).Skip(page * take).Take(take).ToList();
            else
                return dc.Resources.Where(x => x.CourseId == courseId && x.CreateDate < dateAfter.Value && x.IsValid && x.IsPublishd).OrderByDescending(x => x.CreateDate).Skip(page * take).Take(take).ToList();
        }

        public static List<Resource> GetAllNew(int courseId, DateTime dateAfter, int userId)
        {
            return GetAllNew(DBUtility.GetSocialLearningDataContext, courseId, dateAfter, userId);
        }

        public static List<Resource> GetAllNew(SocialLearningDataContext dc, int courseId, DateTime dateAfter, int userId)
        {
            return dc.Resources.Where(x => x.CourseId == courseId && x.CreateDate >= dateAfter && x.CreateUserId != userId && x.IsValid && x.IsPublishd).OrderByDescending(x => x.CreateDate).ToList();
        }

        #endregion

        #region Find

        public static IQueryable<Resource> Find(ResourceSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<Resource> Find(SocialLearningDataContext dc, ResourceSearchModel model)
        {
            var qry = from p in dc.Resources select p;
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
                if (!string.IsNullOrEmpty(model.SearchBody))
                {
                    qry = qry.Where(u => u.Body.Contains(model.SearchBody.StringNormalizer()));
                }
                if (model.SearchType.HasValue && model.SearchType > 0)
                {
                    qry = qry.Where(u => u.Type == model.SearchType);
                }
                if (model.SearchFileContent != null)
                {
                    qry = qry.Where(u => u.FileContent == model.SearchFileContent);
                }

            }
            if (!string.IsNullOrEmpty(model.SortExpression))
            {
                qry = qry.OrderBy(model.SortExpression);
            }
            qry = qry.OrderByDescending(u => u.CreateDate);
            return qry;
        }

        #endregion

        #region Add

        public static DALReturnModel<Resource> Add(Resource model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Resource> Add(SocialLearningDataContext dc, Resource model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    dc.Resources.InsertOnSubmit(model);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<Resource>(model, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Resource>(new Resource { Id = 0 }, bpr);
        }
        #endregion

        #region Update

        public static DALReturnModel<Resource> Update(Resource model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Resource> Update(SocialLearningDataContext dc, Resource model, BatchProcessResultModel bpr)
        {
            Resource obj = null;
            bool noErrorFlag = true;
            obj = dc.Resources.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Title = model.Title.StringNormalizer();
                        obj.Body = model.Body.StringNormalizer();
                        obj.IsPublishd = model.IsPublishd;
                        obj.IsValid = model.IsValid;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Resource>(obj, bpr);
        }

        public static DALReturnModel<Resource> UpdateFile(Resource model, BatchProcessResultModel bpr)
        {
            return UpdateFile(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Resource> UpdateFile(SocialLearningDataContext dc, Resource model, BatchProcessResultModel bpr)
        {
            Resource obj = null;
            bool noErrorFlag = true;
            obj = dc.Resources.SingleOrDefault(u => u.Id == model.Id);
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
            return new DALReturnModel<Resource>(obj, bpr);
        }

        public static DALReturnModel<Resource> UpdateBody(string newBody, int id)
        {
            return UpdateBody(DBUtility.GetSocialLearningDataContext, newBody, id);
        }

        public static DALReturnModel<Resource> UpdateBody(SocialLearningDataContext dc, string newBody, int id)
        {
            Resource obj = null;
            bool noErrorFlag = true;
            obj = dc.Resources.SingleOrDefault(u => u.Id == id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Body = newBody.StringNormalizer();
                        dc.SubmitChanges();
                    }
                }
            }
            catch
            {
            }
            return new DALReturnModel<Resource>(obj);
        }


        #endregion

        #region Delete

        public static DALReturnModel<Resource> Delete(Resource model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Resource> Delete(SocialLearningDataContext dc, Resource model)
        {
            try
            {
                var obj = dc.Resources.Single(q => q.GuidId == model.GuidId || q.Id == model.Id);                
                dc.Resources.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<Resource>(new Resource { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Resource>(new Resource { Id = 0 });
            }
        }

        public static DALReturnModel<Resource> DeleteFile(Resource model)
        {
            return DeleteFile(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Resource> DeleteFile(SocialLearningDataContext dc, Resource model)
        {
            try
            {
                var obj = dc.Resources.Single(q => q.GuidId == model.GuidId || q.Id == model.Id);
                obj.FileContent = null;
                obj.FileMime = null;
                obj.FileSize = null;
                obj.FileTitle = null;
                dc.SubmitChanges();
                return new DALReturnModel<Resource>(new Resource { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Resource>(new Resource { Id = 0 });
            }
        }

        #endregion
    }
}
