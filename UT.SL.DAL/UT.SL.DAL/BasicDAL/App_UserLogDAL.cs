using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class App_UserLogDAL
    {

        #region Get

        public static App_UserLog Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static App_UserLog Get(SocialLearningDataContext dc, int Id)
        {
            return dc.App_UserLogs.SingleOrDefault(u => u.Id == Id);
        }

        #endregion

        #region GetAll

        public static List<App_UserLog> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<App_UserLog> GetAll(SocialLearningDataContext dc)
        {
            return dc.App_UserLogs.ToList();
        }

        #endregion

        #region Find

        public static IQueryable<App_ActionEvaulation> Find(App_UserLogSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<App_ActionEvaulation> Find(SocialLearningDataContext dc, App_UserLogSearchModel model)
        {
            var qry = from p in dc.App_ActionEvaulations select p;
            if (model != null)
            {
                //if (!string.IsNullOrEmpty(model.SearchAbout)) {
                //    qry = qry.Where(u => u.About.Contains(model.SearchAbout.StringNormalizer()));
                //}
                //if (model.SearchType.HasValue && model.SearchType > 0) {
                //    qry = qry.Where(u => u.Type == model.SearchType);
                //}
                //if (model.SearchCreateDate.HasValue && model.SearchCreateDate > DateTime.MinValue) {
                //    qry = qry.Where(u => u.CreateDate == model.SearchCreateDate.WesternizeDateTime());
                //}
            }
            if (!string.IsNullOrEmpty(model.SortExpression))
            {
                qry = qry.OrderBy(model.SortExpression);
            }
            qry = qry.OrderByDescending(u => u.CreateDate);
            return qry;
        }

        #endregion

        #region Add

        public static DALReturnModel<App_UserLog> Add(App_UserLog model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_UserLog> Add(SocialLearningDataContext dc, App_UserLog model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new App_UserLog();
                    obj.About = model.About.StringNormalizer();
                    obj.Type = model.Type;
                    obj.CreateDate = DateTime.Now;
                    obj.UserId = model.UserId;
                    obj.CourseId = model.CourseId;
                    dc.App_UserLogs.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<App_UserLog>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_UserLog>(new App_UserLog { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<App_UserLog> Update(App_UserLog model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_UserLog> Update(SocialLearningDataContext dc, App_UserLog model, BatchProcessResultModel bpr)
        {
            App_UserLog obj = null;
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            obj = dc.App_UserLogs.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.About = model.About.StringNormalizer();
                        obj.Type = model.Type;
                        obj.UserId = model.UserId;
                        obj.CourseId = model.CourseId;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_UserLog>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<App_UserLog> Delete(App_UserLog model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<App_UserLog> Delete(SocialLearningDataContext dc, App_UserLog model)
        {
            try
            {
                var obj = dc.App_UserLogs.Single(q => q.Id == model.Id);
                dc.App_UserLogs.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<App_UserLog>(new App_UserLog { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<App_UserLog>(new App_UserLog { Id = 0 });
            }
        }

        #endregion
    }
}
