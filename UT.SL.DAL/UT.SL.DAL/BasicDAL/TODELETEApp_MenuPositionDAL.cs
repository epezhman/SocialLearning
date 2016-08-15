using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Data.LINQ;
using UT.SL.Helper;


namespace UT.SL.DAL
{
    public partial class dddApp_MenuPositionDAL
    {

        #region Get
        public static App_MenuPosition Get(SocialLearningDataContext dc, int Id)
        {
            return dc.App_MenuPositions.SingleOrDefault(u => u.Id == Id);
        }

        public static App_MenuPosition Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }
        #endregion

        #region GetAll
        public static List<App_MenuPosition> GetAll(SocialLearningDataContext dc)
        {
            return dc.App_MenuPositions.ToList();
        }

        public static List<App_MenuPosition> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }
        #endregion

        #region Add
        public static App_MenuPosition Add(App_MenuPosition model, out BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, out bpr);
        }

        public static App_MenuPosition Add(SocialLearningDataContext dc, App_MenuPosition model, out BatchProcessResultModel bpr)
        {
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;

            try
            {
                if (noErrorFlag)
                {
                    var obj = new App_MenuPosition();
                    obj.PositionId = model.PositionId;
                    obj.MenuId = model.MenuId;
                    dc.App_MenuPositions.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return obj;
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }

            return null;
        }
        #endregion

        #region Update
        public static App_MenuPosition Update(App_MenuPosition model, out BatchProcessResultModel bpr, bool InsertIfNotExist = false)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, out bpr, InsertIfNotExist);
        }

        public static App_MenuPosition Update(SocialLearningDataContext dc, App_MenuPosition model, out BatchProcessResultModel bpr, bool InsertIfNotExist = false)
        {
            App_MenuPosition obj = null;
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            obj = dc.App_MenuPositions.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.PositionId = model.PositionId;
                        obj.MenuId = model.MenuId;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                    else
                    {
                        if (InsertIfNotExist)
                        {
                            dc.App_MenuPositions.InsertOnSubmit(obj);
                            dc.SubmitChanges();
                            bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return obj;
        }
        #endregion

        #region Delete
        public static bool Delete(App_MenuPosition model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static bool Delete(SocialLearningDataContext dc, App_MenuPosition model)
        {
            try
            {
                var obj = dc.App_MenuPositions.Single(q => q.Id == model.Id);
                dc.App_MenuPositions.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        #endregion
    }
}
