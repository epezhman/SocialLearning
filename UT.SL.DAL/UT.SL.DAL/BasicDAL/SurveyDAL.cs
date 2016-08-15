using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{
    public partial class SurveyDAL
    {

        #region Get

        public static Survey GetSurvey(int Id)
        {
            return GetSurvey(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static Survey GetSurvey(SocialLearningDataContext dc, int Id)
        {
            return dc.Surveys.SingleOrDefault(u => u.Id == Id);
        }

        public static SurveyAnswer GetSurveyAnswer(int Id)
        {
            return GetSurveyAnswer(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static SurveyAnswer GetSurveyAnswer(SocialLearningDataContext dc, int Id)
        {
            return dc.SurveyAnswers.SingleOrDefault(u => u.Id == Id);
        }

        public static SurveyAnswer GetSurveyAnswer(int surveyId, int questionId, int userId)
        {
            return GetSurveyAnswer(DBUtility.GetSocialLearningDataContext, surveyId, questionId, userId);
        }

        public static SurveyAnswer GetSurveyAnswer(SocialLearningDataContext dc, int surveyId, int questionId, int userId)
        {
            return dc.SurveyAnswers.FirstOrDefault(u => u.SurveyId == surveyId && u.QuestionId == questionId && u.UserId == userId);
        }

        public static List<SurveyAnswer> GetSurveyAnswer(int surveyId, int userId)
        {
            return GetSurveyAnswer(DBUtility.GetSocialLearningDataContext, surveyId, userId);
        }

        public static List<SurveyAnswer> GetSurveyAnswer(SocialLearningDataContext dc, int surveyId, int userId)
        {
            return dc.SurveyAnswers.Where(u => u.SurveyId == surveyId && u.UserId == userId).ToList();
        }

        public static SurveyUserSummary GetSurveySummary(int surveyId, int userId)
        {
            return GetSurveySummary(DBUtility.GetSocialLearningDataContext, surveyId, userId);
        }

        public static SurveyUserSummary GetSurveySummary(SocialLearningDataContext dc, int surveyId, int userId)
        {
            return dc.SurveyUserSummaries.FirstOrDefault(u => u.SurveyId == surveyId && u.UserId == userId);
        }

        public static SurveyUserSummary GetSurveySummary(int id)
        {
            return GetSurveySummary(DBUtility.GetSocialLearningDataContext, id);
        }

        public static SurveyUserSummary GetSurveySummary(SocialLearningDataContext dc, int id)
        {
            return dc.SurveyUserSummaries.FirstOrDefault(u => u.Id == id);
        }

        #endregion

        #region GetAll

        public static List<Survey> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<Survey> GetAll(SocialLearningDataContext dc)
        {
            return dc.Surveys.ToList();
        }

        public static int GetAllForUserAndSurvey(int userId, int surveyId)
        {
            return GetAllForUserAndSurvey(DBUtility.GetSocialLearningDataContext, userId, surveyId);
        }

        public static int GetAllForUserAndSurvey(SocialLearningDataContext dc, int userId, int surveyId)
        {
            return dc.SurveyAnswers.Where(x => x.UserId == userId && x.SurveyId == surveyId).Count();
        }

        public static List<SurveyUserSummary> GetAllForUser(int userId)
        {
            return GetAllForUser(DBUtility.GetSocialLearningDataContext, userId);
        }

        public static List<SurveyUserSummary> GetAllForUser(SocialLearningDataContext dc, int userId)
        {
            return dc.SurveyUserSummaries.Where(x => x.UserId == userId).ToList();
        }

        public static int GetAnswersForUserCount(int surveyId, int userId)
        {
            return GetAnswersForUserCount(DBUtility.GetSocialLearningDataContext, surveyId, userId);
        }

        public static int GetAnswersForUserCount(SocialLearningDataContext dc, int surveyId, int userId)
        {
            return dc.SurveyAnswers.Where(x => x.UserId == userId && x.SurveyId == surveyId).Count();
        }

        #endregion

        #region Find

        public static IQueryable<App_User> Find(SurveySearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<App_User> Find(SocialLearningDataContext dc, SurveySearchModel model)
        {
            var qry = from p in dc.App_Users select p;
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.SearchTitile))
                {
                    qry = qry.Where(u => u.UserName.Contains(model.SearchTitile.StringNormalizer()) || u.FirstName.Contains(model.SearchTitile.StringNormalizer()) || u.LastName.Contains(model.SearchTitile.StringNormalizer()));
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

        public static DALReturnModel<SurveyAnswer> Add(SurveyAnswer model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<SurveyAnswer> Add(SocialLearningDataContext dc, SurveyAnswer model, BatchProcessResultModel bpr)
        {
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            if (dc.SurveyAnswers.Any(u => u.SurveyId == model.SurveyId && u.QuestionId == model.QuestionId && u.UserId == model.UserId))
            {
                noErrorFlag = false;
                bpr.AddError(string.Empty, true, true);
            }
            try
            {
                if (noErrorFlag)
                {
                    var obj = new SurveyAnswer();
                    obj.CreateDate = DateTime.Now;
                    obj.SurveyId = model.SurveyId;
                    obj.UserId = model.UserId;
                    obj.QuestionId = model.QuestionId;
                    obj.AnswerValue = model.AnswerValue;
                    dc.SurveyAnswers.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<SurveyAnswer>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<SurveyAnswer>(new SurveyAnswer { Id = 0 }, bpr);
        }

        public static DALReturnModel<SurveyUserSummary> Add(SurveyUserSummary model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<SurveyUserSummary> Add(SocialLearningDataContext dc, SurveyUserSummary model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            if (dc.SurveyUserSummaries.Any(u => u.SurveyId == model.SurveyId && u.UserId == model.UserId))
            {
                noErrorFlag = false;
                bpr.AddError(string.Empty, true, true);
            }
            try
            {
                if (noErrorFlag)
                {
                    var obj = new SurveyUserSummary();
                    obj.SurveyId = model.SurveyId;
                    obj.UserId = model.UserId;
                    obj.Statuse = 0;
                    dc.SurveyUserSummaries.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<SurveyUserSummary>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<SurveyUserSummary>(new SurveyUserSummary { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<SurveyAnswer> Update(SurveyAnswer model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<SurveyAnswer> Update(SocialLearningDataContext dc, SurveyAnswer model, BatchProcessResultModel bpr)
        {
            SurveyAnswer obj = null;
            bool noErrorFlag = true;
            obj = dc.SurveyAnswers.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.AnswerValue = model.AnswerValue;

                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<SurveyAnswer>(obj, bpr);
        }

        public static DALReturnModel<SurveyUserSummary> Update(SurveyUserSummary model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<SurveyUserSummary> Update(SocialLearningDataContext dc, SurveyUserSummary model, BatchProcessResultModel bpr)
        {
            SurveyUserSummary obj = null;
            bool noErrorFlag = true;
            obj = dc.SurveyUserSummaries.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Statuse = model.Statuse;

                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<SurveyUserSummary>(obj, bpr);
        }


        #endregion

        #region Delete

        public static DALReturnModel<SurveyAnswer> Delete(SurveyAnswer model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<SurveyAnswer> Delete(SocialLearningDataContext dc, SurveyAnswer model)
        {
            try
            {
                var obj = dc.SurveyAnswers.Single(q => q.Id == model.Id);
                dc.SurveyAnswers.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<SurveyAnswer>(new SurveyAnswer { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<SurveyAnswer>(new SurveyAnswer { Id = 0 });
            }
        }

        #endregion
    }
}
