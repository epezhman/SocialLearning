using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class CourseAbstractDAL
    {

        #region Get

        public static CourseAbstract Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static CourseAbstract Get(SocialLearningDataContext dc, int Id)
        {
            return dc.CourseAbstracts.SingleOrDefault(u => u.Id == Id);
        }

        #endregion

        #region GetAll

        public static List<CourseAbstract> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<CourseAbstract> GetAll(SocialLearningDataContext dc)
        {
            return dc.CourseAbstracts.OrderBy(x => x.Title).ToList();
        }

        #endregion

        #region Add

        public static DALReturnModel<CourseAbstract> Add(CourseAbstract model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<CourseAbstract> Add(SocialLearningDataContext dc, CourseAbstract model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new CourseAbstract();
                    obj.Title = model.Title.StringNormalizer();
                    obj.About = model.About;
                    obj.SubCategoryId = model.SubCategoryId;
                    dc.CourseAbstracts.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<CourseAbstract>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<CourseAbstract>(new CourseAbstract { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<CourseAbstract> Update(CourseAbstract model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<CourseAbstract> Update(SocialLearningDataContext dc, CourseAbstract model, BatchProcessResultModel bpr)
        {
            CourseAbstract obj = null;
            bool noErrorFlag = true;
            obj = dc.CourseAbstracts.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Title = model.Title.StringNormalizer();
                        obj.About = model.About;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<CourseAbstract>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<CourseAbstract> Delete(CourseAbstract model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<CourseAbstract> Delete(SocialLearningDataContext dc, CourseAbstract model)
        {
            try
            {
                var obj = dc.CourseAbstracts.Single(q => q.Id == model.Id);
                dc.CourseAbstracts.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<CourseAbstract>(new CourseAbstract { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<CourseAbstract>(new CourseAbstract { Id = 0 });
            }
        }

        #endregion
    }
}
