using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Data.LINQ;
using UT.SL.Helper;


namespace UT.SL.DAL
{
    public partial class App_PermissionDAL
    {

        #region Get

        public static App_Permission Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static App_Permission Get(SocialLearningDataContext dc, int Id)
        {
            return dc.App_Permissions.SingleOrDefault(u => u.Id == Id);
        }

        public static bool GetByRole(int Id, int roleId)
        {
            return GetByRole(DBUtility.GetSocialLearningDataContext, Id, roleId);
        }

        public static bool GetByRole(SocialLearningDataContext dc, int Id, int roleId)
        {
            return dc.App_Permissions.Any(x => x.ActionId == Id && x.App_Role.Id == roleId);
        }

        #endregion

        #region GetAll

        public static List<App_Permission> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<App_Permission> GetAll(SocialLearningDataContext dc)
        {
            return dc.App_Permissions.ToList();
        }

        #endregion

        #region Add

        public static DALReturnModel<App_Permission> Add(App_Permission model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_Permission> Add(SocialLearningDataContext dc, App_Permission model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new App_Permission();
                    obj.RoleId = model.RoleId;
                    obj.CreateDate = DateTime.Now;
                    obj.ActionId = model.ActionId;
                    dc.App_Permissions.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    //bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<App_Permission>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_Permission>(new App_Permission { Id = 0 }, bpr);
        }
        #endregion

        #region Update

        public static DALReturnModel<App_Permission> Update(App_Permission model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_Permission> Update(SocialLearningDataContext dc, App_Permission model, BatchProcessResultModel bpr)
        {
            App_Permission obj = null;
            bool noErrorFlag = true;
            obj = dc.App_Permissions.SingleOrDefault(u => u.Id == model.Id);

            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.RoleId = model.RoleId;
                        obj.ActionId = model.ActionId;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_Permission>(obj, bpr);

        }
        #endregion

        #region Delete

        public static DALReturnModel<App_Permission> Delete(App_Permission model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<App_Permission> Delete(SocialLearningDataContext dc, App_Permission model)
        {
            try
            {
                var obj = dc.App_Permissions.Single(q => q.Id == model.Id);
                dc.App_Permissions.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<App_Permission>(new App_Permission { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<App_Permission>(new App_Permission { Id = 0 });
            }
        }

        public static DALReturnModel<App_Permission> DeleteAll(int id)
        {
            return DeleteAll(DBUtility.GetSocialLearningDataContext, id);
        }

        public static DALReturnModel<App_Permission> DeleteAll(SocialLearningDataContext dc, int id)
        {
            try
            {
                foreach (var item in dc.App_Permissions.Where(u => u.ActionId == id))
                {
                    dc.App_Permissions.DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }
                return new DALReturnModel<App_Permission>(new App_Permission { Id = id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<App_Permission>(new App_Permission { Id = 0 });
            }
        }

        #endregion
    }
}
