using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{

    public partial class dddApp_UserGradeDAL
    {

        #region Get

        public static App_UserGrade Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static App_UserGrade Get(SocialLearningDataContext dc, int Id)
        {
            return dc.App_UserGrades.SingleOrDefault(u => u.Id == Id);
        }

        public static App_UserGrade Get(System.Guid GuidId)
        {
            return Get(DBUtility.GetSocialLearningDataContext, GuidId);
        }

        public static App_UserGrade Get(SocialLearningDataContext dc, System.Guid GuidId)
        {
            return dc.App_UserGrades.SingleOrDefault(u => u.GuidId == GuidId);
        }
       
        #endregion

        #region GetAll

        public static List<App_UserGrade> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<App_UserGrade> GetAll(SocialLearningDataContext dc)
        {
            return dc.App_UserGrades.ToList();
        }

       
        #endregion

        #region Find
        public static IQueryable<App_UserGrade> Find(App_UserGradeSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<App_UserGrade> Find(SocialLearningDataContext dc, App_UserGradeSearchModel model)
        {
            var qry = from p in dc.App_UserGrades select p;
            if (model != null)
            {
                if (model.SearchGrade.HasValue && model.SearchGrade > 0)
                {
                    qry = qry.Where(u => u.Grade == model.SearchGrade);
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
        public static App_UserGrade Add(App_UserGrade model, out BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, out bpr);
        }

        public static App_UserGrade Add(SocialLearningDataContext dc, App_UserGrade model, out BatchProcessResultModel bpr)
        {
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new App_UserGrade();
                    obj.Grade = model.Grade;
                    obj.CreateDate = DateTime.Now;
                    obj.GuidId = Guid.NewGuid();
                    obj.UserId = model.UserId;
                    dc.App_UserGrades.InsertOnSubmit(obj);
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
        public static App_UserGrade Update(App_UserGrade model, out BatchProcessResultModel bpr, bool InsertIfNotExist = false)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, out bpr, InsertIfNotExist);
        }

        public static App_UserGrade Update(SocialLearningDataContext dc, App_UserGrade model, out BatchProcessResultModel bpr, bool InsertIfNotExist = false)
        {
            App_UserGrade obj = null;
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            obj = dc.App_UserGrades.SingleOrDefault(u => u.GuidId == model.GuidId);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Grade = model.Grade;
                        obj.UserId = model.UserId;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                    else
                    {
                        if (InsertIfNotExist)
                        {
                            dc.App_UserGrades.InsertOnSubmit(obj);
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
        public static bool Delete(App_UserGrade model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static bool Delete(SocialLearningDataContext dc, App_UserGrade model)
        {
            try
            {
                var obj = dc.App_UserGrades.Single(q => q.GuidId == model.GuidId);
                dc.App_UserGrades.DeleteOnSubmit(obj);
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
