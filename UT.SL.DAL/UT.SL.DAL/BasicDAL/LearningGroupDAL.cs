using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class LearningGroupDAL
    {

        #region Get

        public static LearningGroup Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static LearningGroup Get(SocialLearningDataContext dc, int Id)
        {
            return dc.LearningGroups.SingleOrDefault(u => u.Id == Id);
        }

        public static int GetCount(int Id)
        {
            return GetCount(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static int GetCount(SocialLearningDataContext dc, int Id)
        {
            return dc.LearningGroups.SingleOrDefault(u => u.Id == Id).GroupMembers.Count();
        }

        #endregion

        #region GetAll

        public static List<LearningGroup> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<LearningGroup> GetAll(SocialLearningDataContext dc)
        {
            return dc.LearningGroups.ToList();
        }

        public static List<LearningGroup> GetAllByCourse(int courseId)
        {
            return GetAllByCourse(DBUtility.GetSocialLearningDataContext, courseId);
        }

        public static List<LearningGroup> GetAllByCourse(SocialLearningDataContext dc, int courseId)
        {
            return dc.LearningGroups.Where(x => x.CourseId == courseId).ToList();
        }

        public static List<LearningGroup> GetAllByUserId(int userId, int courseId)
        {
            return GetAllByUserId(DBUtility.GetSocialLearningDataContext, userId, courseId);
        }

        public static List<LearningGroup> GetAllByUserId(SocialLearningDataContext dc, int userId, int courseId)
        {
            if (userId == 0)
                return new List<LearningGroup>();
            return dc.LearningGroups.Where(x => (x.CreateUserId == userId || x.GroupMembers.Any(p => p.UserId == userId)) && x.CourseId == courseId).ToList();
        }

        public static List<LearningGroup> GetAllByUserId(int id)
        {
            return GetAllByUserId(DBUtility.GetSocialLearningDataContext, id);
        }

        public static List<LearningGroup> GetAllByUserId(SocialLearningDataContext dc, int id)
        {
            return dc.LearningGroups.Where(x => x.CreateUserId == id || x.GroupMembers.Any(p => p.UserId == id)).ToList();
        }

        public static IEnumerable<ObjectStreamGroupMapper> GetAllStreamMapper(int groupId)
        {
            return GetAllStreamMapper(DBUtility.GetSocialLearningDataContext, groupId);
        }

        public static IEnumerable<ObjectStreamGroupMapper> GetAllStreamMapper(SocialLearningDataContext dc, int groupId)
        {
            return dc.ObjectStreamGroupMappers.Where(x => x.LearningGroupId == groupId).ToList().Distinct(new ObjectStreamGroupMapperComparer()).AsQueryable();
        }

        #endregion

        #region Find

        public static IQueryable<LearningGroup> Find(LearningGroupSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<LearningGroup> Find(SocialLearningDataContext dc, LearningGroupSearchModel model)
        {
            var qry = from p in dc.LearningGroups select p;
            if (model != null)
            {

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

        public static DALReturnModel<LearningGroup> Add(LearningGroup model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<LearningGroup> Add(SocialLearningDataContext dc, LearningGroup model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new LearningGroup();
                    obj.Title = model.Title.StringNormalizer();
                    obj.CreateDate = DateTime.Now;
                    obj.CourseId = model.CourseId;
                    obj.CreateUserId = model.CreateUserId;
                    dc.LearningGroups.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<LearningGroup>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<LearningGroup>(new LearningGroup { Id = 0 }, bpr);
        }

        public static DALReturnModel<ObjectStreamGroupMapper> AddObjectStreamGroup(int streamId, int gId)
        {
            return AddObjectStreamGroup(DBUtility.GetSocialLearningDataContext, streamId, gId);
        }

        public static DALReturnModel<ObjectStreamGroupMapper> AddObjectStreamGroup(SocialLearningDataContext dc, int streamId, int gId)
        {
            bool noErrorFlag = true;
            var objectStreamMapper = dc.ObjectStreamGroupMappers.SingleOrDefault(x => x.ObjectStreamId == streamId && x.LearningGroupId == gId);
            try
            {
                if (noErrorFlag && objectStreamMapper == null)
                {
                    objectStreamMapper = new ObjectStreamGroupMapper
                    {
                        ObjectStreamId = streamId,
                        LearningGroupId = gId
                    };
                    dc.ObjectStreamGroupMappers.InsertOnSubmit(objectStreamMapper);
                    dc.SubmitChanges();
                    return new DALReturnModel<ObjectStreamGroupMapper>(objectStreamMapper);
                }
            }
            catch
            {
            }
            return new DALReturnModel<ObjectStreamGroupMapper>(new ObjectStreamGroupMapper { Id = 0 });
        }

        #endregion

        #region Update

        public static DALReturnModel<LearningGroup> Update(LearningGroup model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<LearningGroup> Update(SocialLearningDataContext dc, LearningGroup model, BatchProcessResultModel bpr)
        {
            LearningGroup obj = null;
            bool noErrorFlag = true;
            obj = dc.LearningGroups.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Title = model.Title.StringNormalizer();
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<LearningGroup>(obj, bpr);
        }
        #endregion

        #region Delete

        public static DALReturnModel<LearningGroup> Delete(LearningGroup model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<LearningGroup> Delete(SocialLearningDataContext dc, LearningGroup model)
        {
            try
            {
                var obj = dc.LearningGroups.Single(q => q.Id == model.Id);
                dc.LearningGroups.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<LearningGroup>(new LearningGroup { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<LearningGroup>(new LearningGroup { Id = 0 });
            }
        }

        public static DALReturnModel<LearningGroup> DeleteGroupMember(int gId, int userId)
        {
            return DeleteGroupMember(DBUtility.GetSocialLearningDataContext, gId, userId);
        }

        public static DALReturnModel<LearningGroup> DeleteGroupMember(SocialLearningDataContext dc, int gId, int userId)
        {
            try
            {
                var obj = dc.ObjectStreamGroupMappers.Where(q => q.LearningGroupId == gId && q.ObjectStream.UserId == userId);
                var toBeDeleted = obj.ToList();
                var streamIds = new List<int>();
                foreach (var item in toBeDeleted)
                {
                    streamIds.Add(item.ObjectStreamId);
                    dc.ObjectStreamGroupMappers.DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }
                foreach (var item in streamIds)
                {
                    ObjectStreamDAL.Delete(new ObjectStream { Id = item });
                }
                return new DALReturnModel<LearningGroup>(new LearningGroup { Id = gId }); ///!!!wrong
            }
            catch (System.Exception)
            {
                return new DALReturnModel<LearningGroup>(new LearningGroup { Id = 0 });
            }
        }

        #endregion
    }
}

class ObjectStreamGroupMapperComparer : IEqualityComparer<ObjectStreamGroupMapper>
{
    #region  IEqualityComparer<ObjectStreamGroupMapper> Members

    public bool Equals(ObjectStreamGroupMapper x, ObjectStreamGroupMapper y)
    {
        return x.ObjectStream.ObjectId.Equals(y.ObjectStream.ObjectId);
    }

    public int GetHashCode(ObjectStreamGroupMapper obj)
    {
        return obj.ObjectStream.ObjectId.GetHashCode();
    }

    #endregion
}