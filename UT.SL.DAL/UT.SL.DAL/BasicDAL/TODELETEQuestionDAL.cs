using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL {
    
    
    public partial class xxxQuestionDAL {
        
        #region Get
        public static Question Get(SocialLearningDataContext dc, int Id) {
            return dc.Questions.SingleOrDefault(u => u.Id == Id);
        }
        
        public static Question Get(int Id) {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }
        #endregion
        
        #region GetAll
        public static List<Question> GetAll(SocialLearningDataContext dc) {
            return dc.Questions.ToList();
        }
        
        public static List<Question> GetAll() {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }
        #endregion
        
        #region Find
        public static IQueryable<Question> Find(QuestionSearchModel model) {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }
        
        public static IQueryable<Question> Find(SocialLearningDataContext dc, QuestionSearchModel model) {
            var qry = from p in dc.Questions select p;
            if (model != null) {
                if (!string.IsNullOrEmpty(model.SearchTitle)) {
                    qry = qry.Where(u => u.Title.Contains(model.SearchTitle.StringNormalizer()));
                }
                if (!string.IsNullOrEmpty(model.SearchBody)) {
                    qry = qry.Where(u => u.Body.Contains(model.SearchBody.StringNormalizer()));
                }
                if (model.SearchCreateDate.HasValue && model.SearchCreateDate > DateTime.MinValue) {
                    qry = qry.Where(u => u.CreateDate == model.SearchCreateDate.WesternizeDateTime());
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
        public static Question Add(Question model, out BatchProcessResultModel bpr) {
            return Add(DBUtility.GetSocialLearningDataContext, model, out bpr);
        }
        
        public static Question Add(SocialLearningDataContext dc, Question model, out BatchProcessResultModel bpr) {
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            try {
                if (noErrorFlag) {
                    var obj = new Question();
                    obj.Title = model.Title.StringNormalizer();
                    obj.Body = model.Body.StringNormalizer();
                    obj.CreateDate = DateTime.Now;
                    obj.Type = model.Type;
                    obj.QuizId = model.QuizId;
                    dc.Questions.InsertOnSubmit(obj);
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
        public static Question Update(Question model, out BatchProcessResultModel bpr, bool InsertIfNotExist = false) {
            return Update(DBUtility.GetSocialLearningDataContext, model, out bpr ,InsertIfNotExist);
        }
        
        public static Question Update(SocialLearningDataContext dc, Question model, out BatchProcessResultModel bpr, bool InsertIfNotExist = false) {
            Question obj = null;
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            obj = dc.Questions.SingleOrDefault(u => u.Id == model.Id);
            try {
                if (noErrorFlag) {
                    if (obj != null) {
                        obj.Title = model.Title.StringNormalizer();
                        obj.Body = model.Body.StringNormalizer();
                        obj.Type = model.Type;
                        obj.QuizId = model.QuizId;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                    else {
                        if (InsertIfNotExist) {
                            dc.Questions.InsertOnSubmit(obj);
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
        public static bool Delete(Question model) {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }
        
        public static bool Delete(SocialLearningDataContext dc, Question model) {
            try {
                var obj = dc.Questions.Single(q => q.Id == model.Id);
                dc.Questions.DeleteOnSubmit(obj);
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
