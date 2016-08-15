using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{

    public partial class TopicDAL
    {

        #region Get

        public static Topic Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static Topic Get(SocialLearningDataContext dc, int Id)
        {
            return dc.Topics.SingleOrDefault(u => u.Id == Id);
        }

        public static Topic Get(string title)
        {
            return Get(DBUtility.GetSocialLearningDataContext, title);
        }

        public static Topic Get(SocialLearningDataContext dc, string title)
        {
            return dc.Topics.SingleOrDefault(u => u.Title == title.StringNormalizer());
        }        

        #endregion

        #region GetAll

        public static List<Topic> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<Topic> GetAll(SocialLearningDataContext dc)
        {
            return dc.Topics.OrderBy(x => x.Title).ToList();
        }

        public static List<Topic> GetAllSelectedTopics(int Id)
        {
            return GetAllSelectedTopics(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static List<Topic> GetAllSelectedTopics(SocialLearningDataContext dc, int Id)
        {
            return dc.CourseTopcMappers.Where(x => x.CourseAbstractId == Id).Select(x => x.Topic).OrderBy(x => x.Title).ToList();
        }

        #endregion

        #region Add

        public static DALReturnModel<Topic> Add(Topic model,  BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model,  bpr);
        }

        public static DALReturnModel<Topic> Add(SocialLearningDataContext dc, Topic model,  BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            if (dc.Topics.Any(x => x.Title == model.Title.StringNormalizer()))
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.BprDupInput, true, true);
                noErrorFlag = false;
            }
            try
            {
                if (noErrorFlag)
                {
                    var obj = new Topic();
                    obj.Title = model.Title.StringNormalizer();
                    dc.Topics.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<Topic>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Topic>(new Topic { Id = 0 }, bpr);
        }

        public static DALReturnModel<CourseTopcMapper> AddTopicMapper(CourseTopcMapper model,  BatchProcessResultModel bpr)
        {
            return AddTopicMapper(DBUtility.GetSocialLearningDataContext, model,  bpr);
        }

        public static DALReturnModel<CourseTopcMapper> AddTopicMapper(SocialLearningDataContext dc, CourseTopcMapper model,  BatchProcessResultModel bpr)
        {
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new CourseTopcMapper();
                    obj.CourseAbstractId = model.CourseAbstractId;
                    obj.TopicId = model.TopicId;
                    dc.CourseTopcMappers.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<CourseTopcMapper>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<CourseTopcMapper>(new CourseTopcMapper { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<Topic> Update(Topic model,  BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model,  bpr);
        }

        public static DALReturnModel<Topic> Update(SocialLearningDataContext dc, Topic model,  BatchProcessResultModel bpr)
        {
            Topic obj = null;
            bool noErrorFlag = true;
            obj = dc.Topics.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Title = model.Title.StringNormalizer();
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                    
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Topic>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<Topic> Delete(Topic model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Topic> Delete(SocialLearningDataContext dc, Topic model)
        {
            try
            {
                var obj = dc.Topics.Single(q => q.Id == model.Id);
                dc.Topics.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<Topic>(new Topic { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Topic>(new Topic { Id = 0 });
            }
        }

        public static DALReturnModel<Topic> DeleteAllMappers(int id)
        {
            return DeleteAllMappers(DBUtility.GetSocialLearningDataContext, id);
        }

        public static DALReturnModel<Topic> DeleteAllMappers(SocialLearningDataContext dc, int id)
        {
            try
            {
                foreach (var item in dc.CourseTopcMappers.Where(q => q.CourseAbstractId == id))
                {
                    dc.CourseTopcMappers.DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }

                return new DALReturnModel<Topic>(new Topic { Id = id}); ///!!!wrong
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Topic>(new Topic { Id = 0 });
            }
        }

        #endregion
    }
}
