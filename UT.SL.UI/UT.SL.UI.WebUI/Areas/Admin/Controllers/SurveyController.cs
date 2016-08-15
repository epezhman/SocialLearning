using System;
using System.Linq;
using System.Web.Mvc;
using UT.SL.Helper;
using UT.SL.UI.WebUI.Controllers;
using UT.SL.Model;
using UT.SL.Data.LINQ;
using UT.SL.DAL;
using System.Xml;
using UT.SL.BLL;
using UT.SL.Model.Enumeration;

namespace UT.SL.UI.WebUI.Areas.Admin.Controllers
{
    [Authorize()]
    public class SurveyController : BaseController
    {

        public ActionResult Index(SurveySearchModel model)
        {
            model.Area = "Admin";
            return View(model);
        }

        public ActionResult SurveySearchModelView(SurveySearchModel model)
        {
            model.Area = "Admin";
            return PartialView(model);
        }

        public PagedList<App_User> SearchFilters(SurveySearchModel model)
        {
            model.Area = "Admin";
            var qry = SurveyDAL.Find(model);
            model.Update(model.PageSize, qry.Count());
            var ls = qry.Skip(model.PageSize * model.PageIndex).Take(model.PageSize).ToList();
            var ql = new PagedList<App_User>(ls, model);
            return ql;
        }

        public ActionResult IX(SurveySearchModel model)
        {
            model.Area = "Admin";
            return PartialView(SearchFilters(model));
        }

        public ActionResult Harter(int? rank)
        {
            if (rank.HasValue)
                ViewBag.LinkRank = rank.Value;
            var surveySummary = SurveyDAL.GetSurveySummary(1, CurrentUser.Id);
            if (surveySummary.Statuse == 3)
                return View("AlreadyDone");
            var survey = SurveyDAL.GetSurvey(1);
            ViewBag.QuestionCount = survey.CountOfQuestions;
            var surveyAnsweredCount = SurveyDAL.GetAllForUserAndSurvey(CurrentUser.Id, 1);
            ViewBag.QuestionCountLeft = survey.CountOfQuestions.Value - surveyAnsweredCount;
            return View();
        }

        public ActionResult Hermans(int? rank)
        {
            if (rank.HasValue)
                ViewBag.LinkRank = rank.Value;
            var surveySummary = SurveyDAL.GetSurveySummary(2, CurrentUser.Id);
            if (surveySummary.Statuse == 3)
                return View("AlreadyDone");
            var survey = SurveyDAL.GetSurvey(2);
            ViewBag.QuestionCount = survey.CountOfQuestions;
            var surveyAnsweredCount = SurveyDAL.GetAllForUserAndSurvey(CurrentUser.Id, 2);
            ViewBag.QuestionCountLeft = survey.CountOfQuestions.Value - surveyAnsweredCount;
            return View();
        }

        public ActionResult AGQ_R(int? rank)
        {
            if (rank.HasValue)
                ViewBag.LinkRank = rank.Value;
            var surveySummary = SurveyDAL.GetSurveySummary(3, CurrentUser.Id);
            if (surveySummary.Statuse == 3)
                return View("AlreadyDone");
            var survey = SurveyDAL.GetSurvey(3);
            ViewBag.QuestionCount = survey.CountOfQuestions;
            var surveyAnsweredCount = SurveyDAL.GetAllForUserAndSurvey(CurrentUser.Id, 3);
            ViewBag.QuestionCountLeft = survey.CountOfQuestions.Value - surveyAnsweredCount;
            return View();
        }

        public ActionResult Feedback(int? rank)
        {
            if (rank.HasValue)
                ViewBag.LinkRank = rank.Value;
            var surveySummary = SurveyDAL.GetSurveySummary(4, CurrentUser.Id);
            if (surveySummary.Statuse == 3)
                return View("AlreadyDone");
            var survey = SurveyDAL.GetSurvey(4);
            ViewBag.QuestionCount = survey.CountOfQuestions;
            var surveyAnsweredCount = SurveyDAL.GetAllForUserAndSurvey(CurrentUser.Id, 4);
            ViewBag.QuestionCountLeft = survey.CountOfQuestions.Value - surveyAnsweredCount;
            return View();
        }

        public ActionResult SurveyReport()
        {
            var allSum = SurveyDAL.GetAllForUser(CurrentUser.Id);
            var allDone = 0;
            foreach (var item in allSum.Where(x => x.Statuse == 3))
            {
                allDone++;
            }
            ViewBag.CountStat = allDone;
            return View();
        }

        public ActionResult QuestionsLeft(int? surveyId)
        {
            if (surveyId.HasValue)
            {
                var survey = SurveyDAL.GetSurvey(surveyId.Value);
                var surveyAnsweredCount = SurveyDAL.GetAllForUserAndSurvey(CurrentUser.Id, surveyId.Value);
                if (survey.CountOfQuestions.Value == surveyAnsweredCount)
                    return Content("D");
                return Content((survey.CountOfQuestions.Value - surveyAnsweredCount).ToString());
            }
            return Content("E");
        }

        public ActionResult SurveyStatuse()
        {
            var surveys = SurveyDAL.GetAll();
            var userSurveys = SurveyDAL.GetAllForUser(CurrentUser.Id);
            if (surveys.Count != userSurveys.Count)
            {
                ViewBag.Statuse = 1;
                var bpr = new BatchProcessResultModel();
                foreach (var item in surveys)
                {
                    var surveySummary = new SurveyUserSummary
                    {
                        SurveyId = item.Id,
                        UserId = CurrentUser.Id
                    };
                    var drm = (DALReturnModel<SurveyUserSummary>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.SurveyUserSummary),
                                       new Func<SurveyUserSummary, BatchProcessResultModel, DALReturnModel<SurveyUserSummary>>(SurveyDAL.Add), surveySummary, bpr);
                    bpr = drm.BPR;
                }
            }
            else
            {
                if (userSurveys.Any(x => x.Statuse == 2))
                {
                    ViewBag.Statuse = 2;
                }
                if (userSurveys.Any(x => x.Statuse == 0))
                {
                    ViewBag.Statuse = 2;
                }
                if (userSurveys.All(x => x.Statuse == 0))
                {
                    ViewBag.Statuse = 1;
                }
            }
            return PartialView();
        }

        public ActionResult RatingContent(int surveyId, int questionId)
        {
            var surveyAsnwer = SurveyDAL.GetSurveyAnswer(surveyId, questionId, CurrentUser.Id);
            if (surveyAsnwer == null)
            {
                surveyAsnwer = new SurveyAnswer
                {
                    SurveyId = surveyId,
                    QuestionId = questionId
                };
            }
            return PartialView(surveyAsnwer);
        }

        public ActionResult Rating(int surveyId, int questionId)
        {
            var surveyAsnwer = SurveyDAL.GetSurveyAnswer(surveyId, questionId, CurrentUser.Id);
            if (surveyAsnwer == null)
            {
                surveyAsnwer = new SurveyAnswer
                {
                    SurveyId = surveyId,
                    QuestionId = questionId
                };
            }
            return PartialView(surveyAsnwer);
        }

        public ActionResult CircleRatingContent(int surveyId, int questionId)
        {
            var surveyAsnwer = SurveyDAL.GetSurveyAnswer(surveyId, questionId, CurrentUser.Id);
            if (surveyAsnwer == null)
            {
                surveyAsnwer = new SurveyAnswer
                {
                    SurveyId = surveyId,
                    QuestionId = questionId
                };
            }
            return PartialView(surveyAsnwer);
        }

        public ActionResult CircleRating(int surveyId, int questionId)
        {
            var surveyAsnwer = SurveyDAL.GetSurveyAnswer(surveyId, questionId, CurrentUser.Id);
            if (surveyAsnwer == null)
            {
                surveyAsnwer = new SurveyAnswer
                {
                    SurveyId = surveyId,
                    QuestionId = questionId
                };
            }
            return PartialView(surveyAsnwer);
        }

        public ActionResult AddAnswer(int surveyId, int questionId, int answerVal)
        {
            var bpr = new BatchProcessResultModel();
            var surveyAsnwer = SurveyDAL.GetSurveyAnswer(surveyId, questionId, CurrentUser.Id);
            if (surveyAsnwer == null)
            {
                surveyAsnwer = new SurveyAnswer
                {
                    SurveyId = surveyId,
                    QuestionId = questionId,
                    UserId = CurrentUser.Id,
                    AnswerValue = answerVal
                };
                var drm = (DALReturnModel<SurveyAnswer>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.SurveyAnswer),
                                      new Func<SurveyAnswer, BatchProcessResultModel, DALReturnModel<SurveyAnswer>>(SurveyDAL.Add), surveyAsnwer, bpr);
                bpr = drm.BPR;
            }
            else
            {
                surveyAsnwer.AnswerValue = answerVal;
                var drm = (DALReturnModel<SurveyAnswer>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.SurveyAnswer),
                                     new Func<SurveyAnswer, BatchProcessResultModel, DALReturnModel<SurveyAnswer>>(SurveyDAL.Update), surveyAsnwer, bpr);
                bpr = drm.BPR;
            }
            var ansersCount = SurveyDAL.GetAnswersForUserCount(surveyId, CurrentUser.Id);
            var surveySummary = SurveyDAL.GetSurveySummary(surveyId, CurrentUser.Id);
            if (ansersCount == 33)
            {
                surveySummary.Statuse = 1;
            }
            else
            {
                surveySummary.Statuse = 2;
            }
            var drm2 = (DALReturnModel<SurveyUserSummary>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.SurveyUserSummary),
                                    new Func<SurveyUserSummary, BatchProcessResultModel, DALReturnModel<SurveyUserSummary>>(SurveyDAL.Update), surveySummary, bpr);
            bpr = drm2.BPR;
            return PartialView("Rating", surveyAsnwer);
        }

        public ActionResult AddAnswerCircle(int surveyId, int questionId, int answerVal)
        {
            var bpr = new BatchProcessResultModel();
            var surveyAsnwer = SurveyDAL.GetSurveyAnswer(surveyId, questionId, CurrentUser.Id);
            if (surveyAsnwer == null)
            {
                surveyAsnwer = new SurveyAnswer
                {
                    SurveyId = surveyId,
                    QuestionId = questionId,
                    UserId = CurrentUser.Id,
                    AnswerValue = answerVal
                };
                var drm = (DALReturnModel<SurveyAnswer>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.SurveyAnswer),
                                     new Func<SurveyAnswer, BatchProcessResultModel, DALReturnModel<SurveyAnswer>>(SurveyDAL.Add), surveyAsnwer, bpr);
                bpr = drm.BPR;
            }
            else
            {
                surveyAsnwer.AnswerValue = answerVal;
                var drm = (DALReturnModel<SurveyAnswer>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.SurveyAnswer),
                                    new Func<SurveyAnswer, BatchProcessResultModel, DALReturnModel<SurveyAnswer>>(SurveyDAL.Update), surveyAsnwer, bpr);
                bpr = drm.BPR;
            }
            var ansersCount = SurveyDAL.GetAnswersForUserCount(surveyId, CurrentUser.Id);
            var surveySummary = SurveyDAL.GetSurveySummary(surveyId, CurrentUser.Id);
            if (ansersCount == 12)
            {
                surveySummary.Statuse = 1;
            }
            else
            {
                surveySummary.Statuse = 2;
            }
            var drm2 = (DALReturnModel<SurveyUserSummary>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.SurveyUserSummary),
                                  new Func<SurveyUserSummary, BatchProcessResultModel, DALReturnModel<SurveyUserSummary>>(SurveyDAL.Update), surveySummary, bpr);
            bpr = drm2.BPR;
            return PartialView("CircleRating", surveyAsnwer);
        }

        public ActionResult Radios(int surveyId, int questionId)
        {
            var surveyAsnwer = SurveyDAL.GetSurveyAnswer(surveyId, questionId, CurrentUser.Id);
            if (surveyAsnwer == null)
            {
                surveyAsnwer = new SurveyAnswer
                {
                    SurveyId = surveyId,
                    QuestionId = questionId
                };
            }
            return PartialView(surveyAsnwer);
        }

        public ActionResult AddAnswerRadio(int surveyId, int questionId, int answerVal)
        {
            var bpr = new BatchProcessResultModel();
            var surveyAsnwer = SurveyDAL.GetSurveyAnswer(surveyId, questionId, CurrentUser.Id);
            if (surveyAsnwer == null)
            {
                surveyAsnwer = new SurveyAnswer
                {
                    SurveyId = surveyId,
                    QuestionId = questionId,
                    UserId = CurrentUser.Id,
                    AnswerValue = answerVal
                };
                var drm = (DALReturnModel<SurveyAnswer>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.SurveyAnswer),
                                      new Func<SurveyAnswer, BatchProcessResultModel, DALReturnModel<SurveyAnswer>>(SurveyDAL.Add), surveyAsnwer, bpr);
                bpr = drm.BPR;
            }
            else
            {
                surveyAsnwer.AnswerValue = answerVal;
                var drm = (DALReturnModel<SurveyAnswer>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.SurveyAnswer),
                                   new Func<SurveyAnswer, BatchProcessResultModel, DALReturnModel<SurveyAnswer>>(SurveyDAL.Update), surveyAsnwer, bpr);
                bpr = drm.BPR;
            }
            var ansersCount = SurveyDAL.GetAnswersForUserCount(surveyId, CurrentUser.Id);
            var surveySummary = SurveyDAL.GetSurveySummary(surveyId, CurrentUser.Id);
            if (ansersCount == 21)
            {
                surveySummary.Statuse = 1;
            }
            else
            {
                surveySummary.Statuse = 2;
            }
            var drm2 = (DALReturnModel<SurveyUserSummary>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.SurveyUserSummary),
                                 new Func<SurveyUserSummary, BatchProcessResultModel, DALReturnModel<SurveyUserSummary>>(SurveyDAL.Update), surveySummary, bpr);
            bpr = drm2.BPR;
            return Content("200");
        }

        public ActionResult FinalSubmit(int surveyId)
        {
            var bpr = new BatchProcessResultModel();
            var surveySummary = SurveyDAL.GetSurveySummary(surveyId, CurrentUser.Id);
            var survey = SurveyDAL.GetSurvey(surveyId);
            var surveyAnsweredCount = SurveyDAL.GetAllForUserAndSurvey(CurrentUser.Id, surveyId);
            if (survey.CountOfQuestions.Value == surveyAnsweredCount)
            {
                surveySummary.Statuse = 3;
                surveySummary.SubmitDate = DateTime.Now;
            }
            var drm = (DALReturnModel<SurveyUserSummary>)ManageAction.ProxyCall(ManageAction.RequestDetail(CurrentUser, Request.RequestContext, (int)ObjectType.SurveyUserSummary),
                                 new Func<SurveyUserSummary, BatchProcessResultModel, DALReturnModel<SurveyUserSummary>>(SurveyDAL.Update), surveySummary, bpr);
            bpr = drm.BPR;
            return Content("OK");
        }

        public ActionResult GetMenuIfPossible()
        {
            var surveys = SurveyDAL.GetAll();
            var allSum = SurveyDAL.GetAllForUser(CurrentUser.Id);            
            if (allSum.Any() && allSum.All(x => x.Statuse == 3))
                ViewBag.Show = false;
            else
                ViewBag.Show = true;
            if (surveys.Count != allSum.Count)
            {
                ViewBag.Show = true;
            }
            return PartialView();
        }

        public ActionResult GetUserSyrveySummaries(int id)
        {
            var model = SurveyDAL.GetAllForUser(id);
            return PartialView(model);
        }

        public ActionResult ViewOneSurvey(int id)
        {
            var surveySummary = SurveyDAL.GetSurveySummary(id);
            var model = SurveyDAL.GetSurveyAnswer(surveySummary.SurveyId.Value, surveySummary.UserId);
            if (surveySummary.SubmitDate.HasValue)
                ViewBag.SubmitDate = surveySummary.SubmitDate.Value;
            return PartialView(model.OrderBy(x => x.QuestionId).ToList());
        }
    
    }
}
