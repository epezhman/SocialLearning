using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL {
    
    
    public partial class dddGradeTypeDAL {
        
        #region Get
        public static GradeType Get(SocialLearningDataContext dc, int Id) {
            return dc.GradeTypes.SingleOrDefault(u => u.Id == Id);
        }

        public static GradeType Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }
        #endregion
        
        #region GetAll
        public static List<GradeType> GetAll(SocialLearningDataContext dc)
        {
            return dc.GradeTypes.ToList();
        }

        public static List<GradeType> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }
        #endregion
        
        #region Find
        public static IQueryable<GradeType> Find(GradeTypeSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<GradeType> Find(SocialLearningDataContext dc, GradeTypeSearchModel model)
        {
            var qry = from p in dc.GradeTypes select p;
            if (model != null) {
                if (model.SearchMinGrade != null && model.SearchMinGrade.HasValue) {
                    qry = qry.Where(u => u.MinGrade == model.SearchMinGrade);
                }
                if (model.SearchType.HasValue && model.SearchType > 0) {
                    qry = qry.Where(u => u.Type == model.SearchType);
                }
                if (model.SearchMaxGrade != null && model.SearchMaxGrade.HasValue) {
                    qry = qry.Where(u => u.MaxGrade == model.SearchMaxGrade);
                }
            }
            if (!string.IsNullOrEmpty(model.SortExpression)) {
                qry = qry.OrderBy(model.SortExpression);
            }
            qry = qry.OrderBy(u => u.Id);
            return qry;
        }
        #endregion
        
        #region Add
        public static GradeType Add(GradeType model, out BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, out bpr);
        }

        public static GradeType Add(SocialLearningDataContext dc, GradeType model, out BatchProcessResultModel bpr)
        {
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            try {
                if (noErrorFlag) {
                    var obj = new GradeType();
                    obj.CourseId = model.CourseId;
                    obj.MinGrade = model.MinGrade;
                    obj.Type = model.Type;
                    obj.MaxGrade = model.MaxGrade;
                    dc.GradeTypes.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return obj;
                }
            }
            catch (System.Exception ex) {
                bpr.AddError(ex.Message, true, true);
            }
            return null;
        }
        #endregion
        
        #region Update
        public static GradeType Update(GradeType model, out BatchProcessResultModel bpr, bool InsertIfNotExist = false)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, out bpr ,InsertIfNotExist);
        }

        public static GradeType Update(SocialLearningDataContext dc, GradeType model, out BatchProcessResultModel bpr, bool InsertIfNotExist = false)
        {
            GradeType obj = null;
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            obj = dc.GradeTypes.SingleOrDefault(u => u.Id == model.Id);
            try {
                if (noErrorFlag) {
                    if (obj != null) {
                        obj.CourseId = model.CourseId;
                        obj.MinGrade = model.MinGrade;
                        obj.Type = model.Type;
                        obj.MaxGrade = model.MaxGrade;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                    else {
                        if (InsertIfNotExist) {
                            dc.GradeTypes.InsertOnSubmit(obj);
                            dc.SubmitChanges();
                            bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                        }
                    }
                }
            }
            catch (System.Exception ex) {
                bpr.AddError(ex.Message, true, true);
            }
            return obj;
        }
        #endregion
        
        #region Delete
        public static bool Delete(GradeType model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static bool Delete(SocialLearningDataContext dc, GradeType model)
        {
            try {
                var obj = dc.GradeTypes.Single(q => q.Id == model.Id);
                dc.GradeTypes.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return true;
            }
            catch (System.Exception ) {
                return false;
            }
        }
        #endregion
    }
}
