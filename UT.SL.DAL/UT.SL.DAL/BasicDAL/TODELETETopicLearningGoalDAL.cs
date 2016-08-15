using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL {
    
    
    public partial class dddTopicLearningGoalDAL {
        
        #region Get
        public static TopicLearningGoal Get(SocialLearningDataContext dc, int Id) {
            return dc.TopicLearningGoals.SingleOrDefault(u => u.Id == Id);
        }
        
        public static TopicLearningGoal Get(int Id) {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }
        #endregion
        
        #region GetAll
        public static List<TopicLearningGoal> GetAll(SocialLearningDataContext dc) {
            return dc.TopicLearningGoals.ToList();
        }
        
        public static List<TopicLearningGoal> GetAll() {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }
        #endregion
        
        #region Find
        public static IQueryable<TopicLearningGoal> Find(TopicLearningGoalSearchModel model) {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }
        
        public static IQueryable<TopicLearningGoal> Find(SocialLearningDataContext dc, TopicLearningGoalSearchModel model) {
            var qry = from p in dc.TopicLearningGoals select p;
            if (model != null) {
            }
            if (!string.IsNullOrEmpty(model.SortExpression)) {
                qry = qry.OrderBy(model.SortExpression);
            }
            qry = qry.OrderBy(u => u.Id);
            return qry;
        }
        #endregion
        
        #region Add
        public static TopicLearningGoal Add(TopicLearningGoal model, out BatchProcessResultModel bpr) {
            return Add(DBUtility.GetSocialLearningDataContext, model, out bpr);
        }
        
        public static TopicLearningGoal Add(SocialLearningDataContext dc, TopicLearningGoal model, out BatchProcessResultModel bpr) {
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            try {
                if (noErrorFlag) {
                    var obj = new TopicLearningGoal();
                    obj.LevelId = model.LevelId;
                    //obj.QuizId = model.QuizId;
                    obj.TopicId = model.TopicId;
                    obj.ImpactId = model.ImpactId;
                    dc.TopicLearningGoals.InsertOnSubmit(obj);
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
        public static TopicLearningGoal Update(TopicLearningGoal model, out BatchProcessResultModel bpr, bool InsertIfNotExist = false) {
            return Update(DBUtility.GetSocialLearningDataContext, model, out bpr ,InsertIfNotExist);
        }
        
        public static TopicLearningGoal Update(SocialLearningDataContext dc, TopicLearningGoal model, out BatchProcessResultModel bpr, bool InsertIfNotExist = false) {
            TopicLearningGoal obj = null;
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            obj = dc.TopicLearningGoals.SingleOrDefault(u => u.Id == model.Id);
            try {
                if (noErrorFlag) {
                    if (obj != null) {
                        obj.LevelId = model.LevelId;
                        //obj.QuizId = model.QuizId;
                        obj.TopicId = model.TopicId;
                        obj.ImpactId = model.ImpactId;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                    else {
                        if (InsertIfNotExist) {
                            dc.TopicLearningGoals.InsertOnSubmit(obj);
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
        public static bool Delete(TopicLearningGoal model) {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }
        
        public static bool Delete(SocialLearningDataContext dc, TopicLearningGoal model) {
            try {
                var obj = dc.TopicLearningGoals.Single(q => q.Id == model.Id);
                dc.TopicLearningGoals.DeleteOnSubmit(obj);
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
