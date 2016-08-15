using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class App_UserInRoleDAL
    {

        #region Get

        public static App_UserInRole Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static App_UserInRole Get(SocialLearningDataContext dc, int Id)
        {
            return dc.App_UserInRoles.SingleOrDefault(u => u.Id == Id);
        }

        public static App_UserInRole Get(System.Guid GuidId)
        {
            return Get(DBUtility.GetSocialLearningDataContext, GuidId);
        }

        public static App_UserInRole Get(SocialLearningDataContext dc, System.Guid GuidId)
        {
            return dc.App_UserInRoles.SingleOrDefault(u => u.GuidId == GuidId);
        }

        public static App_UserInRole IsInRole(string userName, string roleName)
        {
            return IsInRole(DBUtility.GetSocialLearningDataContext, userName, roleName);
        }

        public static App_UserInRole IsInRole(SocialLearningDataContext dc, string userName, string roleName)
        {
            return dc.App_UserInRoles.FirstOrDefault(u => u.App_User.UserName == userName && u.App_Role.Title.ToLower() == roleName.ToLower());
        }

        public static App_UserInRole GetFirstOrDefault(int userId)
        {
            return GetFirstOrDefault(DBUtility.GetSocialLearningDataContext, userId);
        }

        public static App_UserInRole GetFirstOrDefault(SocialLearningDataContext dc, int userId)
        {
            return dc.App_UserInRoles.FirstOrDefault(u => u.App_User.Id == userId);
        }

        #endregion

        #region GetAll

        public static List<App_UserInRole> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<App_UserInRole> GetAll(SocialLearningDataContext dc)
        {
            return dc.App_UserInRoles.ToList();
        }

        #endregion

        #region Find

        public static IQueryable<App_UserInRole> Find(App_UserInRoleSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<App_UserInRole> Find(SocialLearningDataContext dc, App_UserInRoleSearchModel model)
        {
            var qry = from p in dc.App_UserInRoles select p;
            if (model != null)
            {
                if (model.SearchCreateDate.HasValue && model.SearchCreateDate > DateTime.MinValue)
                {
                    qry = qry.Where(u => u.CreateDate == model.SearchCreateDate);
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

        public static DALReturnModel<App_UserInRole> Add(App_UserInRole model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_UserInRole> Add(SocialLearningDataContext dc, App_UserInRole model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new App_UserInRole();
                    obj.CreateDate = DateTime.Now;
                    obj.GuidId = Guid.NewGuid();
                    obj.UserId = model.UserId;
                    obj.RoleId = model.RoleId;
                    dc.App_UserInRoles.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<App_UserInRole>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_UserInRole>(new App_UserInRole { Id = 0 }, bpr);
        }

        public static DALReturnModel<App_UserInRole> AddRole(int userId, int roleId, BatchProcessResultModel bpr)
        {
            return AddRole(DBUtility.GetSocialLearningDataContext, userId, roleId, bpr);
        }

        public static DALReturnModel<App_UserInRole> AddRole(SocialLearningDataContext dc, int userId, int roleId, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            var obj = App_UserInRoleDAL.GetFirstOrDefault(userId);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.RoleId = roleId;
                        dc.SubmitChanges();
                    }
                    else
                    {
                        obj = new App_UserInRole();
                        obj.CreateDate = DateTime.Now;
                        obj.GuidId = Guid.NewGuid();
                        obj.UserId = userId;
                        obj.RoleId = roleId;
                        dc.App_UserInRoles.InsertOnSubmit(obj);
                        dc.SubmitChanges();
                    }
                    if (roleId == dc.App_Roles.SingleOrDefault(x => x.Title.ToLower() == "admin").Id)
                    {
                        App_UserDAL.UpdateToAdmin(userId, true);
                    }
                    else
                    {
                        App_UserDAL.UpdateToAdmin(userId, false);
                    }
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<App_UserInRole>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_UserInRole>(new App_UserInRole { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<App_UserInRole> Update(App_UserInRole model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_UserInRole> Update(SocialLearningDataContext dc, App_UserInRole model, BatchProcessResultModel bpr)
        {
            App_UserInRole obj = null;
            bool noErrorFlag = true;
            obj = dc.App_UserInRoles.SingleOrDefault(u => u.GuidId == model.GuidId);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.UserId = model.UserId;
                        obj.RoleId = model.RoleId;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_UserInRole>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<App_UserInRole> Delete(App_UserInRole model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<App_UserInRole> Delete(SocialLearningDataContext dc, App_UserInRole model)
        {
            try
            {
                var obj = dc.App_UserInRoles.Single(q => q.GuidId == model.GuidId);
                dc.App_UserInRoles.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<App_UserInRole>(new App_UserInRole { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<App_UserInRole>(new App_UserInRole { Id = 0 });
            }
        }

        #endregion
    }
}
