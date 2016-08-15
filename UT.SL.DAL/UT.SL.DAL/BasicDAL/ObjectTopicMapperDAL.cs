using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class ObjectTopicMapperDAL
    {

        #region Get

        public static ObjectTopicMapper Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static ObjectTopicMapper Get(SocialLearningDataContext dc, int Id)
        {
            return dc.ObjectTopicMappers.SingleOrDefault(u => u.Id == Id);
        }

        #endregion

        #region GetAll

        public static List<ObjectTopicMapper> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<ObjectTopicMapper> GetAll(SocialLearningDataContext dc)
        {
            return dc.ObjectTopicMappers.ToList();
        }

        public static List<Topic> GetAll(int objectId, int type)
        {
            return GetAll(DBUtility.GetSocialLearningDataContext, objectId, type);
        }

        public static List<Topic> GetAll(SocialLearningDataContext dc, int objectId, int type)
        {
            return dc.ObjectTopicMappers.Where(x => x.ObjectId == objectId && x.ObjectType == type).Select(x => x.Topic).ToList();
        }

        public static List<ObjectTopicMapper> GetAllObject(int objectId, int type)
        {
            return GetAllObject(DBUtility.GetSocialLearningDataContext, objectId, type);
        }

        public static List<ObjectTopicMapper> GetAllObject(SocialLearningDataContext dc, int objectId, int type)
        {
            return dc.ObjectTopicMappers.Where(x => x.ObjectId == objectId && x.ObjectType == type).ToList();
        }

        public static int GetAllCount(int objectId, int type)
        {
            return GetAllCount(DBUtility.GetSocialLearningDataContext, objectId, type);
        }

        public static int GetAllCount(SocialLearningDataContext dc, int objectId, int type)
        {
            return dc.ObjectTopicMappers.Count(x => x.ObjectId == objectId && x.ObjectType == type);
        }

        #endregion

        #region Add

        public static DALReturnModel<ObjectTopicMapper> Add(ObjectTopicMapper model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<ObjectTopicMapper> Add(SocialLearningDataContext dc, ObjectTopicMapper model, BatchProcessResultModel bpr)
        {
            var obj = dc.ObjectTopicMappers.SingleOrDefault(x => x.ObjectId == model.ObjectId && x.ObjectType == model.ObjectType && x.TopicId == model.TopicId);
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag && obj == null)
                {
                    dc.ObjectTopicMappers.InsertOnSubmit(model);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<ObjectTopicMapper>(model, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<ObjectTopicMapper>(new ObjectTopicMapper { Id = 0 }, bpr);
        }
        #endregion

        #region Update

        public static DALReturnModel<ObjectTopicMapper> Update(ObjectTopicMapper model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<ObjectTopicMapper> Update(SocialLearningDataContext dc, ObjectTopicMapper model, BatchProcessResultModel bpr)
        {
            ObjectTopicMapper obj = null;
            bool noErrorFlag = true;
            obj = dc.ObjectTopicMappers.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<ObjectTopicMapper>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<ObjectTopicMapper> Delete(ObjectTopicMapper model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<ObjectTopicMapper> Delete(SocialLearningDataContext dc, ObjectTopicMapper model)
        {
            try
            {
                var obj = dc.ObjectTopicMappers.Single(q => q.Id == model.Id);
                dc.ObjectTopicMappers.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<ObjectTopicMapper>(new ObjectTopicMapper { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<ObjectTopicMapper>(new ObjectTopicMapper { Id = 0 });
            }
        }

        public static DALReturnModel<ObjectTopicMapper> DeleteObjectTopics(int objectId, int objectType)
        {
            return DeleteObjectTopics(DBUtility.GetSocialLearningDataContext, objectId, objectType);
        }

        public static DALReturnModel<ObjectTopicMapper> DeleteObjectTopics(SocialLearningDataContext dc, int objectId, int objectType)
        {
            try
            {
                foreach (var item in dc.ObjectTopicMappers.Where(x => x.ObjectId == objectId && x.ObjectType == objectType))
                {
                    dc.ObjectTopicMappers.DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }
                return new DALReturnModel<ObjectTopicMapper>(new ObjectTopicMapper { Id = objectId }); ///!!!wrong
            }
            catch
            {
                return new DALReturnModel<ObjectTopicMapper>(new ObjectTopicMapper { Id = 0 });
            }
        }

        #endregion
    }
}
