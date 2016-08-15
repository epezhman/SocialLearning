using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{

    public partial class App_ActionEvaulationDAL
    {

        #region Get

        public static App_ActionEvaulation Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static App_ActionEvaulation Get(SocialLearningDataContext dc, int Id)
        {
            return dc.App_ActionEvaulations.SingleOrDefault(u => u.Id == Id);
        }

        #endregion

        #region GetAll

        public static List<App_ActionEvaulation> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<App_ActionEvaulation> GetAll(SocialLearningDataContext dc)
        {
            return dc.App_ActionEvaulations.ToList();
        }

        #endregion

        #region Add

        public static DALReturnModel<App_ActionEvaulation> Add(App_ActionEvaulation model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_ActionEvaulation> Add(SocialLearningDataContext dc, App_ActionEvaulation model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new App_ActionEvaulation();
                    obj.ActionId = model.ActionId;
                    obj.CreateDate = DateTime.Now;
                    obj.Credit = model.Credit;
                    obj.ObjectId = model.ObjectId;
                    obj.ObjectType = model.ObjectType;
                    obj.RoleId = model.RoleId;
                    obj.IP = model.IP;
                    obj.Browser = model.Browser;
                    obj.OS = model.OS;
                    obj.IsMobile = model.IsMobile;
                    obj.ScreenRes = model.ScreenRes;
                    obj.UserId = model.UserId;
                    dc.App_ActionEvaulations.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<App_ActionEvaulation>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_ActionEvaulation>(new App_ActionEvaulation { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<App_ActionEvaulation> Update(App_ActionEvaulation model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_ActionEvaulation> Update(SocialLearningDataContext dc, App_ActionEvaulation model, BatchProcessResultModel bpr)
        {
            App_ActionEvaulation obj = null;
            bool noErrorFlag = true;
            obj = dc.App_ActionEvaulations.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_ActionEvaulation>(obj, bpr);
        }
        #endregion

        #region Delete

        public static DALReturnModel<App_ActionEvaulation> Delete(App_ActionEvaulation model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<App_ActionEvaulation> Delete(SocialLearningDataContext dc, App_ActionEvaulation model)
        {
            try
            {
                var obj = dc.App_ActionEvaulations.Single(q => q.Id == model.Id);
                dc.App_ActionEvaulations.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<App_ActionEvaulation>(new App_ActionEvaulation { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<App_ActionEvaulation>(new App_ActionEvaulation { Id = 0 });
            }
        }

        #endregion
    }
}
