using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL {
    
    
    public partial class dddCommentLogDAL {
        
        #region Get
        public static CommentLog Get(SocialLearningDataContext dc, int Id) {
            return dc.CommentLogs.SingleOrDefault(u => u.Id == Id);
        }
        
        public static CommentLog Get(int Id) {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }
        #endregion
        
        #region GetAll
        public static List<CommentLog> GetAll(SocialLearningDataContext dc) {
            return dc.CommentLogs.ToList();
        }
        
        public static List<CommentLog> GetAll() {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }
        #endregion
        
        #region Find
        public static IQueryable<CommentLog> Find(CommentLogSearchModel model) {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }
        
        public static IQueryable<CommentLog> Find(SocialLearningDataContext dc, CommentLogSearchModel model) {
            var qry = from p in dc.CommentLogs select p;
            if (model != null) {
                if (!string.IsNullOrEmpty(model.SearchChangeText)) {
                    qry = qry.Where(u => u.ChangeText.Contains(model.SearchChangeText.StringNormalizer()));
                }
                if (model.SearchChangeDate.HasValue && model.SearchChangeDate > DateTime.MinValue) {
                    qry = qry.Where(u => u.ChangeDate == model.SearchChangeDate.WesternizeDateTime());
                }
                if (model.SearchChangedBy > 0) {
                    qry = qry.Where(u => u.App_User.Id == model.SearchChangedBy);
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
        public static CommentLog Add(CommentLog model, out BatchProcessResultModel bpr) {
            return Add(DBUtility.GetSocialLearningDataContext, model, out bpr);
        }
        
        public static CommentLog Add(SocialLearningDataContext dc, CommentLog model, out BatchProcessResultModel bpr) {
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            try {
                if (noErrorFlag) {
                    var obj = new CommentLog();
                    obj.ChangeText = model.ChangeText.StringNormalizer();
                    obj.ChangeDate = model.ChangeDate.WesternizeDateTime();
                    obj.ChangedBy = model.ChangedBy;
                    obj.CommentId = model.CommentId;
                    dc.CommentLogs.InsertOnSubmit(obj);
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
        public static CommentLog Update(CommentLog model, out BatchProcessResultModel bpr, bool InsertIfNotExist = false) {
            return Update(DBUtility.GetSocialLearningDataContext, model, out bpr ,InsertIfNotExist);
        }
        
        public static CommentLog Update(SocialLearningDataContext dc, CommentLog model, out BatchProcessResultModel bpr, bool InsertIfNotExist = false) {
            CommentLog obj = null;
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            obj = dc.CommentLogs.SingleOrDefault(u => u.Id == model.Id);
            try {
                if (noErrorFlag) {
                    if (obj != null) {
                        obj.ChangeText = model.ChangeText.StringNormalizer();
                        obj.ChangeDate = model.ChangeDate.WesternizeDateTime();
                        obj.ChangedBy = model.ChangedBy;
                        obj.CommentId = model.CommentId;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                    else {
                        if (InsertIfNotExist) {
                            dc.CommentLogs.InsertOnSubmit(obj);
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
        public static bool Delete(CommentLog model) {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }
        
        public static bool Delete(SocialLearningDataContext dc, CommentLog model) {
            try {
                var obj = dc.CommentLogs.Single(q => q.Id == model.Id);
                dc.CommentLogs.DeleteOnSubmit(obj);
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
