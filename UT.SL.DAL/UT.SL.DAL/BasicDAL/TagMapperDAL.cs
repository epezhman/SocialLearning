using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class TagMapperDAL
    {

        #region Get

        public static TagMapper Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static TagMapper Get(SocialLearningDataContext dc, int Id)
        {
            return dc.TagMappers.SingleOrDefault(u => u.Id == Id);
        }

        public static TagMapper GetByDetail(int objectId, int type, int tagId)
        {
            return GetByDetail(DBUtility.GetSocialLearningDataContext, objectId, type, tagId);
        }

        public static TagMapper GetByDetail(SocialLearningDataContext dc, int objectId, int type, int tagId)
        {
            return dc.TagMappers.SingleOrDefault(u => u.ObjectId == objectId && u.ObjectType == type && u.TagId == tagId);
        }

        public static int GetAssingedTagCount(int objectId, int type)
        {
            return GetAssingedTagCount(DBUtility.GetSocialLearningDataContext, objectId, type);
        }

        public static int GetAssingedTagCount(SocialLearningDataContext dc, int objectId, int type)
        {
            return dc.TagMappers.Where(u => u.ObjectId == objectId && u.ObjectType == type).Count();
        }

        #endregion

        #region GetAll

        public static List<TagMapper> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<TagMapper> GetAll(SocialLearningDataContext dc)
        {
            return dc.TagMappers.ToList();
        }

        public static List<TagMapper> GetAll(int objectId, int type)
        {
            return GetAll(DBUtility.GetSocialLearningDataContext, objectId, type);
        }

        public static List<TagMapper> GetAll(SocialLearningDataContext dc, int objectId, int type)
        {
            return dc.TagMappers.Where(x => x.ObjectId == objectId && x.ObjectType == type).ToList();
        }

        public static bool TagAny(int objectId, int type)
        {
            return TagAny(DBUtility.GetSocialLearningDataContext, objectId, type);
        }

        public static bool TagAny(SocialLearningDataContext dc, int objectId, int type)
        {
            return dc.TagMappers.Any(x => x.ObjectId == objectId && x.ObjectType == type);
        }

        public static int GetAllCount(int objectId, int type, int userId)
        {
            return GetAllCount(DBUtility.GetSocialLearningDataContext, objectId, type, userId);
        }

        public static int GetAllCount(SocialLearningDataContext dc, int objectId, int type, int userId)
        {
            return dc.TagMappers.Where(x => x.ObjectId == objectId && x.ObjectType == type).Count();
        }

        public static int GetAllCount(int objectId, int type)
        {
            return GetAllCount(DBUtility.GetSocialLearningDataContext, objectId, type);
        }

        public static int GetAllCount(SocialLearningDataContext dc, int objectId, int type)
        {
            return dc.TagMappers.Where(x => x.ObjectId == objectId && x.ObjectType == type).Count();
        }

        public static List<TagMapper> GetAllUserAndObject(int userId, int objectId, int objectType)
        {
            return GetAllUserAndObject(DBUtility.GetSocialLearningDataContext, userId, objectId, objectType);
        }

        public static List<TagMapper> GetAllUserAndObject(SocialLearningDataContext dc, int userId, int objectId, int objectType)
        {
            return dc.TagMappers.Where(x => x.ObjectId == objectId && x.ObjectType == objectType && x.UserId == userId).ToList();
        }

        public static List<App_User> GetAllAssociatedUser(int objectId, int type)
        {
            return GetAllAssociatedUser(DBUtility.GetSocialLearningDataContext, objectId, type);
        }

        public static List<App_User> GetAllAssociatedUser(SocialLearningDataContext dc, int objectId, int type)
        {
            return dc.TagMappers.Where(x => x.ObjectType == type && x.ObjectId == objectId).Select(x => x.App_User).Distinct().ToList();
        }

        public static int CountNewTags(int objectId, int objectType, DateTime date, int userId)
        {
            return CountNewTags(DBUtility.GetSocialLearningDataContext, objectId, objectType, date,  userId);
        }

        public static int CountNewTags(SocialLearningDataContext dc, int objectId, int objectType, DateTime date, int userId)
        {
            return dc.TagMappers.Count(x => x.ObjectType == objectType && x.ObjectId == objectId && x.CreateDate >= date && x.UserId != userId);
        }

        #endregion

        #region Find

        public static IQueryable<TagMapper> Find(TagMapperSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<TagMapper> Find(SocialLearningDataContext dc, TagMapperSearchModel model)
        {
            var qry = from p in dc.TagMappers select p;
            if (model != null)
            {
                if (model.SearchObjectType.HasValue && model.SearchObjectType > 0)
                {
                    qry = qry.Where(u => u.ObjectType == model.SearchObjectType);
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

        public static DALReturnModel<TagMapper> Add(TagMapper model)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<TagMapper> Add(SocialLearningDataContext dc, TagMapper model)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new TagMapper();
                    obj.ObjectId = model.ObjectId;
                    obj.ObjectType = model.ObjectType;
                    obj.CreateDate = DateTime.Now;
                    obj.TagId = model.TagId;
                    if (model.UserId.HasValue && model.UserId != 0)
                        obj.UserId = model.UserId;
                    dc.TagMappers.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<TagMapper>(obj);
                }
            }
            catch
            {
            }
            return new DALReturnModel<TagMapper>(new TagMapper { Id = 0 });
        }

        #endregion

        #region Update

        public static DALReturnModel<TagMapper> Update(TagMapper model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<TagMapper> Update(SocialLearningDataContext dc, TagMapper model, BatchProcessResultModel bpr)
        {
            TagMapper obj = null;
            bool noErrorFlag = true;
            obj = dc.TagMappers.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.ObjectId = model.ObjectId;
                        obj.ObjectType = model.ObjectType;
                        obj.TagId = model.TagId;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<TagMapper>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<TagMapper> Delete(TagMapper model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<TagMapper> Delete(SocialLearningDataContext dc, TagMapper model)
        {
            try
            {
                var obj = dc.TagMappers.Single(q => q.Id == model.Id);
                dc.TagMappers.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<TagMapper>(new TagMapper { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<TagMapper>(new TagMapper { Id = 0 });
            }
        }

        public static DALReturnModel<TagMapper> DeleteAll(int objectId, int type)
        {
            return DeleteAll(DBUtility.GetSocialLearningDataContext, objectId, type);
        }

        public static DALReturnModel<TagMapper> DeleteAll(SocialLearningDataContext dc, int objectId, int type)
        {
            try
            {

                foreach (var obj in dc.TagMappers.Where(q => q.ObjectId == objectId && q.ObjectType == type))
                {
                    dc.TagMappers.DeleteOnSubmit(obj);
                    dc.SubmitChanges();
                }
                return new DALReturnModel<TagMapper>(new TagMapper { Id = objectId }); ///!!!wrong
            }
            catch (System.Exception)
            {
                return new DALReturnModel<TagMapper>(new TagMapper { Id = 0 });
            }
        }

        #endregion
    }
}
