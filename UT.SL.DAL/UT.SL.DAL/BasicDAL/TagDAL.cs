using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class TagDAL
    {

        #region Get

        public static Tag Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static Tag Get(SocialLearningDataContext dc, int Id)
        {
            return dc.Tags.SingleOrDefault(u => u.Id == Id);
        }

        public static Tag Get(string Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static Tag Get(SocialLearningDataContext dc, string Id)
        {
            return dc.Tags.SingleOrDefault(u => u.Title == Id);
        }

        #endregion

        #region GetAll

        public static List<Tag> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<Tag> GetAll(SocialLearningDataContext dc)
        {
            return dc.Tags.ToList();
        }

        public static List<Tag> GetAll(string title)
        {
            return GetAll(DBUtility.GetSocialLearningDataContext, title);
        }

        public static List<Tag> GetAll(SocialLearningDataContext dc, string title)
        {
            return dc.Tags.Where(x => x.Title.ToLower().Contains(title)).ToList();
        }

        public static List<Tag> GetAll(int Id)
        {
            return GetAll(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static List<Tag> GetAll(SocialLearningDataContext dc, int Id)
        {
            return dc.Tags.Where(x => x.CategoryId == Id).ToList();
        }

        public static List<Tag> GetAll(int Id, string title)
        {
            return GetAll(DBUtility.GetSocialLearningDataContext, Id, title);
        }

        public static List<Tag> GetAll(SocialLearningDataContext dc, int Id, string title)
        {
            return dc.Tags.Where(x => x.CategoryId == Id && x.Title.ToLower().Contains(title.StringNormalizer())).ToList();
        }

        #endregion

        #region Find

        public static IQueryable<Tag> Find(TagSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<Tag> Find(SocialLearningDataContext dc, TagSearchModel model)
        {
            var qry = from p in dc.Tags select p;
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.SearchTitile))
                {
                    qry = qry.Where(u => u.Title.Contains(model.SearchTitile.StringNormalizer()));
                }
                if (model.SearchIsValid.HasValue && model.SearchIsValid.HasValue)
                {
                    qry = qry.Where(u => !u.IsValid);
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
            qry = qry.OrderBy(u => u.CategoryId).ThenBy(x => x.Id);
            return qry;
        }

        #endregion

        #region Add

        public static DALReturnModel<Tag> Add(Tag model)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Tag> Add(SocialLearningDataContext dc, Tag model)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new Tag();
                    obj.Title = model.Title.StringNormalizer();
                    obj.CreateDate = DateTime.Now;
                    if (model.UserId.HasValue && model.UserId != 0)
                        obj.UserId = model.UserId;
                    if (model.CategoryId.HasValue && model.CategoryId != 0)
                        obj.CategoryId = model.CategoryId.Value;
                    obj.IsValid = model.IsValid;
                    dc.Tags.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<Tag>(obj);
                }
            }
            catch
            {
            }
            return new DALReturnModel<Tag>(new Tag { Id = 0 });
        }

        #endregion

        #region Update

        public static DALReturnModel<Tag> Update(Tag model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Tag> Update(SocialLearningDataContext dc, Tag model, BatchProcessResultModel bpr)
        {
            Tag obj = null;
            bool noErrorFlag = true;
            obj = dc.Tags.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Title = model.Title.StringNormalizer();
                        if (model.CategoryId.HasValue && model.CategoryId != 0)
                            obj.CategoryId = model.CategoryId.Value;
                        else
                            obj.CategoryId = null;
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
            return new DALReturnModel<Tag>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<Tag> Delete(Tag model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Tag> Delete(SocialLearningDataContext dc, Tag model)
        {
            try
            {
                var obj = dc.Tags.Single(q => q.Id == model.Id);
                foreach (var item in obj.TagMappers)
                {
                    TagMapperDAL.Delete(item);
                }

                dc.Tags.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<Tag>(new Tag { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Tag>(new Tag { Id = 0 });
            }
        }

        #endregion
    }
}
