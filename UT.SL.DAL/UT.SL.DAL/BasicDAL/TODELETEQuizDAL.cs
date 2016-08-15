using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL {
    
    
    public partial class xxxQuizDAL {
        
        #region Get
        public static Quiz Get(SocialLearningDataContext dc, int Id) {
            return dc.Quizs.SingleOrDefault(u => u.Id == Id);
        }
        
        public static Quiz Get(int Id) {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }
        
        public static Quiz Get(SocialLearningDataContext dc, System.Guid GuidId) {
            return dc.Quizs.SingleOrDefault(u => u.GuidId == GuidId);
        }
        
        public static Quiz Get(System.Guid GuidId) {
            return Get(DBUtility.GetSocialLearningDataContext, GuidId);
        }
        #endregion
        
        #region GetAll
        public static List<Quiz> GetAll(SocialLearningDataContext dc) {
            return dc.Quizs.ToList();
        }
        
        public static List<Quiz> GetAll() {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }
        #endregion
        
        #region Find
        public static IQueryable<Quiz> Find(QuizSearchModel model) {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }
        
        public static IQueryable<Quiz> Find(SocialLearningDataContext dc, QuizSearchModel model) {
            var qry = from p in dc.Quizs select p;
            if (model != null) {
                if (model.SearchDueDate.HasValue && model.SearchDueDate > DateTime.MinValue) {
                    qry = qry.Where(u => u.DueDate == model.SearchDueDate.WesternizeDateTime());
                }
                if (model.SearchDeadline.HasValue && model.SearchDeadline > DateTime.MinValue) {
                    qry = qry.Where(u => u.Deadline == model.SearchDeadline.WesternizeDateTime());
                }
                if (model.SearchDuration.HasValue && model.SearchDuration > 0) {
                    qry = qry.Where(u => u.Duration == model.SearchDuration);
                }
                if (model.SearchEffectiveScore != null && model.SearchEffectiveScore.HasValue) {
                    qry = qry.Where(u => u.EffectiveScore == model.SearchEffectiveScore);
                }
                if (model.SearchType.HasValue && model.SearchType > 0) {
                    qry = qry.Where(u => u.Type == model.SearchType);
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
        public static Quiz Add(Quiz model, out BatchProcessResultModel bpr) {
            return Add(DBUtility.GetSocialLearningDataContext, model, out bpr);
        }
        
        public static Quiz Add(SocialLearningDataContext dc, Quiz model, out BatchProcessResultModel bpr) {
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            try {
                if (noErrorFlag) {
                    var obj = new Quiz();
                    obj.DueDate = model.DueDate.WesternizeDateTime();
                    obj.Deadline = model.Deadline.WesternizeDateTime();
                    obj.Duration = model.Duration;
                    obj.EffectiveScore = model.EffectiveScore;
                    obj.Type = model.Type;
                    obj.GuidId = Guid.NewGuid();
                    obj.TopicId = model.TopicId;
                    obj.CourseId = model.CourseId;
                    dc.Quizs.InsertOnSubmit(obj);
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
        public static Quiz Update(Quiz model, out BatchProcessResultModel bpr, bool InsertIfNotExist = false) {
            return Update(DBUtility.GetSocialLearningDataContext, model, out bpr ,InsertIfNotExist);
        }
        
        public static Quiz Update(SocialLearningDataContext dc, Quiz model, out BatchProcessResultModel bpr, bool InsertIfNotExist = false) {
            Quiz obj = null;
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            obj = dc.Quizs.SingleOrDefault(u => u.GuidId == model.GuidId);
            try {
                if (noErrorFlag) {
                    if (obj != null) {
                        obj.DueDate = model.DueDate.WesternizeDateTime();
                        obj.Deadline = model.Deadline.WesternizeDateTime();
                        obj.Duration = model.Duration;
                        obj.EffectiveScore = model.EffectiveScore;
                        obj.Type = model.Type;
                        obj.TopicId = model.TopicId;
                        obj.CourseId = model.CourseId;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                    else {
                        if (InsertIfNotExist) {
                            dc.Quizs.InsertOnSubmit(obj);
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
        public static bool Delete(Quiz model) {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }
        
        public static bool Delete(SocialLearningDataContext dc, Quiz model) {
            try {
                var obj = dc.Quizs.Single(q => q.GuidId == model.GuidId);
                dc.Quizs.DeleteOnSubmit(obj);
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
