using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class FeedbackDAL
    {

        #region Get

        public static Feedback Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static Feedback Get(SocialLearningDataContext dc, int Id)
        {
            return dc.Feedbacks.SingleOrDefault(u => u.Id == Id);
        }

        #endregion

        #region GetAll

        public static List<Feedback> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<Feedback> GetAll(SocialLearningDataContext dc)
        {
            return dc.Feedbacks.ToList();
        }

        #endregion

        #region Find

        public static IQueryable<Feedback> Find(FeedbackSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<Feedback> Find(SocialLearningDataContext dc, FeedbackSearchModel model)
        {
            var qry = from p in dc.Feedbacks.OrderByDescending(x => x.CreateDate) select p;
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.SearchTitle))
                {
                    qry = qry.Where(u => u.Feedback1.Contains(model.SearchTitle.StringNormalizer()));
                }

            }
            if (!string.IsNullOrEmpty(model.SortExpression))
            {
                qry = qry.OrderBy(model.SortExpression);
            }
            return qry;
        }

        #endregion

        #region Add

        public static DALReturnModel<Feedback> Add(Feedback model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Feedback> Add(SocialLearningDataContext dc, Feedback model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new Feedback();
                    obj.Feedback1 = model.Feedback1.StringNormalizer();
                    obj.PageURL = model.PageURL.StringNormalizer();
                    obj.CreateDate = model.CreateDate;
                    if (model.UserId.HasValue)
                        obj.UserId = model.UserId;
                    dc.Feedbacks.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<Feedback>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Feedback>(new Feedback { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<Feedback> Update(Feedback model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Feedback> Update(SocialLearningDataContext dc, Feedback model, BatchProcessResultModel bpr)
        {
            Feedback obj = null;
            bool noErrorFlag = true;
            obj = dc.Feedbacks.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Feedback1 = model.Feedback1.StringNormalizer();
                        obj.PageURL = model.PageURL.StringNormalizer();
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Feedback>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<Feedback> Delete(Feedback model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Feedback> Delete(SocialLearningDataContext dc, Feedback model)
        {
            try
            {
                var obj = dc.Feedbacks.Single(q => q.Id == model.Id);
                dc.Feedbacks.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<Feedback>(new Feedback { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Feedback>(new Feedback { Id = 0 });
            }
        } 

        #endregion
    }
}
