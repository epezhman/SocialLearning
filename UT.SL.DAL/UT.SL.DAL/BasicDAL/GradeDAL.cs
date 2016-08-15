using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UT.SL.Data.LINQ;
using UT.SL.Helper;

namespace UT.SL.DAL
{
    public partial class GradeDAL
    {
        #region Get

        public static Grade Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static Grade Get(SocialLearningDataContext dc, int Id)
        {
            return dc.Grades.SingleOrDefault(u => u.Id == Id);
        }

        public static int Get(int objectId, int objectType)
        {
            return Get(DBUtility.GetSocialLearningDataContext, objectId, objectType);
        }

        public static int Get(SocialLearningDataContext dc, int objectId, int objectType)
        {
            var grade = dc.Grades.FirstOrDefault(u => u.ObjectId == objectId && u.ObjectType == objectType);
            if (grade == null)
                return 0;
            else return grade.GradeValue ?? 0;
        }

        #endregion

        #region GetAll

        public static List<Grade> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<Grade> GetAll(SocialLearningDataContext dc)
        {
            return dc.Grades.ToList();
        }

        public static List<Grade> GetAll(int objectId, int type)
        {
            return GetAll(DBUtility.GetSocialLearningDataContext, objectId, type);
        }

        public static List<Grade> GetAll(SocialLearningDataContext dc, int objectId, int type)
        {
            return dc.Grades.Where(g => g.ObjectId == objectId && g.ObjectType == type).OrderBy(g => g.CreateDate).ToList();
        }

        public static List<Grade> GetAllCourse(int courseId)
        {
            return GetAllCourse(DBUtility.GetSocialLearningDataContext, courseId);
        }

        public static List<Grade> GetAllCourse(SocialLearningDataContext dc, int courseId)
        {
            return dc.Grades.Where(q => q.CourseId == courseId).OrderBy(g => g.CreateDate).ToList();
        }

        public static List<Grade> GetAllUserAndCourse(int courseId, int userId)
        {
            return GetAllUserAndCourse(DBUtility.GetSocialLearningDataContext, courseId, userId);
        }

        public static List<Grade> GetAllUserAndCourse(SocialLearningDataContext dc, int courseId, int userId)
        {
            return dc.Grades.Where(q => q.CourseId == courseId && q.GradeOwnerId == userId).OrderBy(g => g.CreateDate).ToList();
        }

        public static List<Grade> GetAllByCourse(int id)
        {
            return GetAllByCourse(DBUtility.GetSocialLearningDataContext, id);
        }

        public static List<Grade> GetAllByCourse(SocialLearningDataContext dc, int id)
        {
            return dc.Grades.Where(x => x.CourseId == id).ToList();
        }


        #endregion

        #region Add

        public static DALReturnModel<Grade> Add(Grade model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Grade> Add(SocialLearningDataContext dc, Grade model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            var any = dc.Grades.Any(u => u.GradeOwnerId == model.GradeOwnerId && u.ParentObjectId == model.ParentObjectId &&
                u.ParentObjectType == model.ParentObjectType && u.ObjectType == model.ObjectType &&
                u.ObjectId == model.ObjectId);

            try
            {
                if (noErrorFlag && !any)
                {
                    var obj = new Grade();
                    obj.CreateDate = model.CreateDate;
                    obj.ObjectId = model.ObjectId;
                    obj.ObjectType = model.ObjectType;
                    obj.GradeValue = model.GradeValue;
                    obj.UserId = model.UserId;
                    obj.Body = model.Body;
                    obj.CourseId = model.CourseId;
                    if (model.GradeOwnerId > 0)
                        obj.GradeOwnerId = model.GradeOwnerId;
                    obj.ParentObjectId = model.ParentObjectId;
                    obj.ParentObjectType = model.ParentObjectType;
                    dc.Grades.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<Grade>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Grade>(new Grade { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<Grade> Update(Grade model, BatchProcessResultModel bpr, bool InsertIfNotExist = false)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr, InsertIfNotExist);
        }

        public static DALReturnModel<Grade> Update(SocialLearningDataContext dc, Grade model, BatchProcessResultModel bpr, bool InsertIfNotExist = false)
        {
            Grade obj = null;
            bool noErrorFlag = true;
            obj = dc.Grades.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.CreateDate = model.CreateDate;
                        obj.GradeValue = model.GradeValue;
                        obj.UserId = model.UserId;
                        obj.Body = model.Body;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                    else
                    {
                        if (InsertIfNotExist)
                        {
                            obj = new Grade();
                            obj.CreateDate = model.CreateDate;
                            obj.GradeValue = model.GradeValue;
                            obj.UserId = model.UserId;
                            obj.Body = model.Body;
                            dc.Grades.InsertOnSubmit(obj);
                            dc.SubmitChanges();
                            bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                        }
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Grade>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<Grade> Delete(Grade model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Grade> Delete(SocialLearningDataContext dc, Grade model)
        {
            try
            {
                var obj = dc.Grades.Single(q => q.Id == model.Id);
                dc.Grades.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<Grade>(new Grade { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Grade>(new Grade { Id = 0 });
            }
        }
        #endregion

        #region Extra

        public static void UpdateGradeOwner(int gradeId, int userId)
        {
            UpdateGradeOwner(DBUtility.GetSocialLearningDataContext, gradeId, userId);
        }

        public static void UpdateGradeOwner(SocialLearningDataContext dc, int gradeId, int userId)
        {
            var grade = dc.Grades.SingleOrDefault(x => x.Id == gradeId);
            grade.GradeOwnerId = userId;
            dc.SubmitChanges();
        }

        public static void UpdateGradeParent(int gradeId, int parentId, int parentType, int? courseId)
        {
            UpdateGradeParent(DBUtility.GetSocialLearningDataContext, gradeId, parentId, parentType, courseId);
        }

        public static void UpdateGradeParent(SocialLearningDataContext dc, int gradeId, int parentId, int parentType, int? courseId)
        {
            var grade = dc.Grades.SingleOrDefault(x => x.Id == gradeId);
            grade.ParentObjectId = parentId;
            grade.ParentObjectType = parentType;
            grade.CourseId = courseId ?? 0;
            dc.SubmitChanges();
        }

        #endregion
    }
}
