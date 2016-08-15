using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class AnswerDAL
    {

        #region Get

        public static Answer Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static Answer Get(SocialLearningDataContext dc, int Id)
        {
            return dc.Answers.SingleOrDefault(u => u.Id == Id);
        }

        #endregion

        #region GetAll

        public static List<Answer> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<Answer> GetAll(SocialLearningDataContext dc)
        {
            return dc.Answers.ToList();
        }

        #endregion

        #region Find

        public static IQueryable<Answer> Find(AnswerSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<Answer> Find(SocialLearningDataContext dc, AnswerSearchModel model)
        {
            var qry = from p in dc.Answers select p;
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.SearchTitle))
                {
                    qry = qry.Where(u => u.Title.Contains(model.SearchTitle.StringNormalizer()));
                }
                if (!string.IsNullOrEmpty(model.SearchCorrectAnsver))
                {
                    qry = qry.Where(u => u.CorrectAnsver.Contains(model.SearchCorrectAnsver.StringNormalizer()));
                }
                if (model.SearchIsEffective != null && model.SearchIsEffective.HasValue)
                {
                    qry = qry.Where(u => u.IsEffective == model.SearchIsEffective);
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

        public static DALReturnModel<Answer> Add(Answer model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Answer> Add(SocialLearningDataContext dc, Answer model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new Answer();
                    obj.Title = model.Title.StringNormalizer();
                    obj.CorrectAnsver = model.CorrectAnsver.StringNormalizer();
                    obj.IsEffective = model.IsEffective;
                    obj.GuidId = Guid.NewGuid();
                    obj.QuestionId = model.QuestionId;
                    obj.QuestionId = model.QuestionId;
                    dc.Answers.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<Answer>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Answer>(new Answer { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<Answer> Update(Answer model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Answer> Update(SocialLearningDataContext dc, Answer model, BatchProcessResultModel bpr)
        {
            Answer obj = null;
            bool noErrorFlag = true;
            obj = dc.Answers.SingleOrDefault(u => u.GuidId == model.GuidId);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Title = model.Title.StringNormalizer();
                        obj.CorrectAnsver = model.CorrectAnsver.StringNormalizer();
                        obj.IsEffective = model.IsEffective;
                        obj.QuestionId = model.QuestionId;
                        obj.QuestionId = model.QuestionId;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Answer>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<Answer> Delete(Answer model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Answer> Delete(SocialLearningDataContext dc, Answer model)
        {
            try
            {
                var obj = dc.Answers.Single(q => q.GuidId == model.GuidId);
                dc.Answers.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<Answer>(new Answer { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Answer>(new Answer { Id = 0 });
            }
        }

        #endregion
    }
}
