using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class dddAverageGradeDAL
    {

        #region Get
        public static AverageGrade Get(SocialLearningDataContext dc, int Id)
        {
            return dc.AverageGrades.SingleOrDefault(u => u.Id == Id);
        }

        public static AverageGrade Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        #endregion

        #region GetAll
        public static List<AverageGrade> GetAll(SocialLearningDataContext dc)
        {
            return dc.AverageGrades.ToList();
        }

        public static List<AverageGrade> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<AverageGrade> GetAllUserGrades(SocialLearningDataContext dc, Guid userId, int courseId)
        {
            return dc.AverageGrades.Where(x => x.CourseId == courseId && x.App_UserGrades.Any(p => p.App_User.GuidId == userId)).OrderBy(x => x.CreateDate).ToList();
        }

        public static List<AverageGrade> GetAllUserGrades(Guid userId, int courseId)
        {
            return GetAllUserGrades(DBUtility.GetSocialLearningDataContext, userId, courseId);
        }

        #endregion


        #region Add
        public static AverageGrade Add(AverageGrade model, out BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, out bpr);
        }

        public static AverageGrade Add(SocialLearningDataContext dc, AverageGrade model, out BatchProcessResultModel bpr)
        {
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new AverageGrade();
                    dc.AverageGrades.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return obj;
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return null;
        }
        #endregion

        #region Update
        public static AverageGrade Update(AverageGrade model, out BatchProcessResultModel bpr, bool InsertIfNotExist = false)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, out bpr, InsertIfNotExist);
        }

        public static AverageGrade Update(SocialLearningDataContext dc, AverageGrade model, out BatchProcessResultModel bpr, bool InsertIfNotExist = false)
        {
            AverageGrade obj = null;
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            obj = dc.AverageGrades.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                    else
                    {
                        if (InsertIfNotExist)
                        {
                            dc.AverageGrades.InsertOnSubmit(obj);
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
            return obj;
        }
        #endregion

        #region Delete
        public static bool Delete(AverageGrade model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static bool Delete(SocialLearningDataContext dc, AverageGrade model)
        {
            try
            {
                var obj = dc.AverageGrades.Single(q => q.Id == model.Id);
                dc.AverageGrades.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }

        }
        #endregion
    }
}
