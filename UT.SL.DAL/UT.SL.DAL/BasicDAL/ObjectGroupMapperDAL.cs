using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class ObjectGroupMapperDAL
    {

        #region Get

        public static ObjectGroupMapper Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static ObjectGroupMapper Get(SocialLearningDataContext dc, int Id)
        {
            return dc.ObjectGroupMappers.SingleOrDefault(u => u.Id == Id);
        }

        #endregion

        #region GetAll

        public static List<ObjectGroupMapper> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<ObjectGroupMapper> GetAll(SocialLearningDataContext dc)
        {
            return dc.ObjectGroupMappers.ToList();
        }

        public static List<int> GetAllLearningWithObjectIdAndType(int objectId, int objectType)
        {
            return GetAllLearningWithObjectIdAndType(DBUtility.GetSocialLearningDataContext, objectId, objectType);
        }

        public static List<int> GetAllLearningWithObjectIdAndType(SocialLearningDataContext dc, int objectId, int objectType)
        {
            return dc.ObjectGroupMappers.Where(x => x.ObjectId == objectId && x.ObjectType == objectType && x.LearningGroupId.HasValue).Select(x => x.LearningGroupId.Value).ToList();
        }

        public static List<int> GetAllSocialWithObjectIdAndType(int objectId, int objectType)
        {
            return GetAllSocialWithObjectIdAndType(DBUtility.GetSocialLearningDataContext, objectId, objectType);
        }

        public static List<int> GetAllSocialWithObjectIdAndType(SocialLearningDataContext dc, int objectId, int objectType)
        {
            return dc.ObjectGroupMappers.Where(x => x.ObjectId == objectId && x.ObjectType == objectType && x.SocialGroupId.HasValue).Select(x => x.SocialGroupId.Value).ToList();
        }

        #endregion

        #region Add

        public static DALReturnModel<ObjectGroupMapper> Add(ObjectGroupMapper model)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<ObjectGroupMapper> Add(SocialLearningDataContext dc, ObjectGroupMapper model)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    dc.ObjectGroupMappers.InsertOnSubmit(model);
                    dc.SubmitChanges();
                    return new DALReturnModel<ObjectGroupMapper>(model);
                }
            }
            catch
            {
            }
            return new DALReturnModel<ObjectGroupMapper>(new ObjectGroupMapper { Id = 0 });
        }

        #endregion

        #region Update

        public static DALReturnModel<ObjectGroupMapper> Update(ObjectGroupMapper model, bool InsertIfNotExist = false)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, InsertIfNotExist);
        }

        public static DALReturnModel<ObjectGroupMapper> Update(SocialLearningDataContext dc, ObjectGroupMapper model, bool InsertIfNotExist = false)
        {
            ObjectGroupMapper obj = null;
            bool noErrorFlag = true;
            obj = dc.ObjectGroupMappers.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        dc.SubmitChanges();
                    }
                    else
                    {
                        if (InsertIfNotExist)
                        {
                            dc.ObjectGroupMappers.InsertOnSubmit(obj);
                            dc.SubmitChanges();
                        }
                    }
                }
            }
            catch
            {
            }
            return new DALReturnModel<ObjectGroupMapper>(obj);
        }

        #endregion

        #region Delete

        public static DALReturnModel<ObjectGroupMapper> Delete(ObjectGroupMapper model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<ObjectGroupMapper> Delete(SocialLearningDataContext dc, ObjectGroupMapper model)
        {
            try
            {
                var obj = dc.ObjectGroupMappers.Single(q => q.Id == model.Id);
                dc.ObjectGroupMappers.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<ObjectGroupMapper>(new ObjectGroupMapper { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<ObjectGroupMapper>(new ObjectGroupMapper { Id = 0 });
            }
        }

        #endregion
    }
}
