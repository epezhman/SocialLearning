using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class App_RoleDAL
    {

        #region Get

        public static App_Role Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static App_Role Get(SocialLearningDataContext dc, int Id)
        {
            return dc.App_Roles.SingleOrDefault(u => u.Id == Id);
        }

        public static bool Get(string Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static bool Get(SocialLearningDataContext dc, string Id)
        {
            return dc.App_Roles.Any(u => u.Title.ToLower() == Id.StringNormalizer().ToLower());
        }

        public static App_Role GetRole(string Id)
        {
            return GetRole(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static App_Role GetRole(SocialLearningDataContext dc, string Id)
        {
            return dc.App_Roles.SingleOrDefault(u => u.Title.ToLower() == Id.StringNormalizer().ToLower());
        }

        public static bool IsUserInRole(string username, string roleName)
        {
            return IsUserInRole(DBUtility.GetSocialLearningDataContext, username, roleName);
        }

        public static bool IsUserInRole(SocialLearningDataContext dc, string username, string roleName)
        {
            return dc.App_UserInRoles.Any(u => u.App_Role.Title == roleName.StringNormalizer() && u.App_User.UserName == username.StringNormalizer());
        }

        #endregion

        #region GetAll

        public static List<App_Role> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<App_Role> GetAll(SocialLearningDataContext dc)
        {
            return dc.App_Roles.ToList();
        }

        public static List<App_Role> GetAll(int id)
        {
            return GetAll(DBUtility.GetSocialLearningDataContext, id);
        }

        public static List<App_Role> GetAll(SocialLearningDataContext dc, int id)
        {
            return dc.App_Roles.Where(u => u.App_UserInRoles.Any(x => x.App_User.Id == id)).Distinct().ToList();
        }

        #endregion

        #region Find

        public static IQueryable<App_Role> Find(App_RoleSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<App_Role> Find(SocialLearningDataContext dc, App_RoleSearchModel model)
        {
            var qry = from p in dc.App_Roles select p;
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.SearchTitle))
                {
                    qry = qry.Where(u => u.Title.Contains(model.SearchTitle.StringNormalizer()));
                }
                if (!string.IsNullOrEmpty(model.SearchDescription))
                {
                    qry = qry.Where(u => u.Description.Contains(model.SearchDescription.StringNormalizer()));
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

        public static DALReturnModel<App_Role> Add(App_Role model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_Role> Add(SocialLearningDataContext dc, App_Role model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new App_Role();
                    obj.Title = model.Title.StringNormalizer();
                    obj.Description = model.Description.StringNormalizer();
                    dc.App_Roles.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<App_Role>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_Role>(new App_Role { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<App_Role> Update(App_Role model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_Role> Update(SocialLearningDataContext dc, App_Role model, BatchProcessResultModel bpr)
        {
            App_Role obj = null;
            bool noErrorFlag = true;
            obj = dc.App_Roles.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Title = model.Title.StringNormalizer();
                        obj.Description = model.Description.StringNormalizer();
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_Role>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<App_Role> Delete(App_Role model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<App_Role> Delete(SocialLearningDataContext dc, App_Role model)
        {
            try
            {
                var obj = dc.App_Roles.Single(q => q.Id == model.Id);
                dc.App_Roles.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<App_Role>(new App_Role { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<App_Role>(new App_Role { Id = 0 });
            }
        }

        #endregion
    }
}
