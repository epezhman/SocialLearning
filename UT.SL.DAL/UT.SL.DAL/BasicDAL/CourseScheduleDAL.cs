using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class CourseScheduleDAL
    {

        #region Get

        public static CourseSchedule Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static CourseSchedule Get(SocialLearningDataContext dc, int Id)
        {
            return dc.CourseSchedules.SingleOrDefault(u => u.Id == Id);
        }

        #endregion

        #region GetAll

        public static List<CourseSchedule> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<CourseSchedule> GetAll(SocialLearningDataContext dc)
        {
            return dc.CourseSchedules.ToList();
        }

        #endregion

        #region Add

        public static DALReturnModel<CourseSchedule> Add(CourseSchedule model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<CourseSchedule> Add(SocialLearningDataContext dc, CourseSchedule model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            if (model.BeginDate >= model.EndDate)
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.BprDateSoon, true, true);
                noErrorFlag = false;
            }
            try
            {
                if (noErrorFlag)
                {
                    var obj = new CourseSchedule();
                    obj.Summary = model.Summary;
                    obj.Rank = model.Rank;
                    obj.BeginDate = model.BeginDate.WesternizeDateTime();
                    obj.EndDate = model.EndDate.WesternizeDateTime();
                    obj.CourseId = model.CourseId;
                    dc.CourseSchedules.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<CourseSchedule>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<CourseSchedule>(new CourseSchedule { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<CourseSchedule> Update(CourseSchedule model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<CourseSchedule> Update(SocialLearningDataContext dc, CourseSchedule model, BatchProcessResultModel bpr)
        {
            CourseSchedule obj = null;
            bool noErrorFlag = true;
            //if (model.BeginDate >= model.EndDate)
            //{
            //    bpr.AddError(UT.SL.Model.Resource.App_Errors.BprDateSoon, true, true);
            //    noErrorFlag = false;
            //}
            obj = dc.CourseSchedules.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Summary = model.Summary;
                        //obj.Rank = model.Rank;
                        //obj.BeginDate = model.BeginDate.WesternizeDateTime();
                        //obj.EndDate = model.EndDate.WesternizeDateTime();
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<CourseSchedule>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<CourseSchedule> Delete(CourseSchedule model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<CourseSchedule> Delete(SocialLearningDataContext dc, CourseSchedule model)
        {
            try
            {
                var obj = dc.CourseSchedules.Single(q => q.Id == model.Id);
                dc.CourseSchedules.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<CourseSchedule>(new CourseSchedule { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<CourseSchedule>(new CourseSchedule { Id = 0 });
            }
        }

        public static DALReturnModel<CourseSchedule> DeleteAll(int Id)
        {
            return DeleteAll(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static DALReturnModel<CourseSchedule> DeleteAll(SocialLearningDataContext dc, int Id)
        {
            try
            {
                foreach (var obj in dc.CourseSchedules.Where(q => q.CourseId == Id).ToList())
                {
                    dc.CourseSchedules.DeleteOnSubmit(obj);
                    dc.SubmitChanges();
                }

                return new DALReturnModel<CourseSchedule>(new CourseSchedule { Id = Id }); ///!!!wrong
            }
            catch (System.Exception)
            {
                return new DALReturnModel<CourseSchedule>(new CourseSchedule { Id = 0 });
            }
        }

        #endregion
    }
}
