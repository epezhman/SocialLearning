using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{
    public partial class ObjectStreamCourseDAL
    {
        #region Get

        public static ObjectStreamCourse Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static ObjectStreamCourse Get(SocialLearningDataContext dc, int Id)
        {
            return dc.ObjectStreamCourses.SingleOrDefault(u => u.Id == Id);
        }

        #endregion

        #region GetAll

        public static List<ObjectStreamCourse> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<ObjectStreamCourse> GetAll(SocialLearningDataContext dc)
        {
            return dc.ObjectStreamCourses.ToList();
        }

        public static IEnumerable<ObjectStreamCourse> GetAllForCourse(int courseId)
        {
            return GetAllForCourse(DBUtility.GetSocialLearningDataContext, courseId);
        }

        public static IEnumerable<ObjectStreamCourse> GetAllForCourse(SocialLearningDataContext dc, int courseId)
        {
            return dc.ObjectStreamCourses.OrderByDescending(x => x.CreateDate).Where(x => x.CourseId == courseId);
        }

        public static List<int> GetAllWithIdAndType(int objectId, int objectType)
        {
            return GetAllWithIdAndType(DBUtility.GetSocialLearningDataContext, objectId, objectType);
        }

        public static List<int> GetAllWithIdAndType(SocialLearningDataContext dc, int objectId, int objectType)
        {
            var result = dc.ObjectStreamCourses.OrderByDescending(x => x.CreateDate).Where(x => x.ObjectId == objectId && x.ObjectType == objectType).Select(x => x.CourseId).ToList();
            return result;
        }

        #endregion

        #region Add

        public static DALReturnModel<ObjectStreamCourse> Add(ObjectStreamCourse model)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<ObjectStreamCourse> Add(SocialLearningDataContext dc, ObjectStreamCourse model)
        {
            bool noErrorFlag = true;
            var obj = dc.ObjectStreamCourses.SingleOrDefault(x => x.ObjectId == model.ObjectId && x.ObjectType == model.ObjectType);
            try
            {
                if (noErrorFlag && obj == null)
                {
                    obj = new ObjectStreamCourse();
                    obj.CourseId = model.CourseId;
                    obj.ObjectId = model.ObjectId;
                    obj.ObjectType = model.ObjectType;
                    obj.CreateDate = DateTime.Now;
                    dc.ObjectStreamCourses.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<ObjectStreamCourse>(obj);
                }
            }
            catch
            {
            }
            return new DALReturnModel<ObjectStreamCourse>(obj);
        }

        #endregion

        #region Update

        public static DALReturnModel<ObjectStreamCourse> Update(ObjectStreamCourse model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<ObjectStreamCourse> Update(SocialLearningDataContext dc, ObjectStreamCourse model, BatchProcessResultModel bpr)
        {
            ObjectStreamCourse obj = null;
            bool noErrorFlag = true;
            obj = dc.ObjectStreamCourses.SingleOrDefault(u => u.Id == model.Id);
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
            return new DALReturnModel<ObjectStreamCourse>(obj);

        }

        #endregion

        #region Delete

        public static DALReturnModel<ObjectStreamCourse> Delete(ObjectStreamCourse model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<ObjectStreamCourse> Delete(SocialLearningDataContext dc, ObjectStreamCourse model)
        {
            try
            {
                var obj = dc.ObjectStreamCourses.Single(q => q.Id == model.Id);
                dc.ObjectStreamCourses.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<ObjectStreamCourse>(new ObjectStreamCourse { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<ObjectStreamCourse>(new ObjectStreamCourse { Id = 0 });
            }
        }

        #endregion

    }
}
