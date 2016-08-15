using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Data.LINQ;
using UT.SL.Helper;


namespace UT.SL.DAL
{
    public partial class App_ActionDAL
    {

        #region Repair

        public static void Repair()
        {
            Repair(DBUtility.GetSocialLearningDataContext);
        }

        public static void Repair(SocialLearningDataContext dc)
        {
            var permssions = dc.App_Permissions.ToList();
            foreach (var item in permssions)
            {
                var accId = item.ActionId;
                var action = dc.App_Actions.SingleOrDefault(u => u.Id == accId);
                if (action == null)
                {
                    dc.App_Permissions.DeleteOnSubmit(item);
                }
            }
            dc.SubmitChanges();
        }

        public static void MoveToBookmark()
        {
            MoveToBookmark(DBUtility.GetSocialLearningDataContext);
        }

        public static void MoveToBookmark(SocialLearningDataContext dc)
        {
            var actions = dc.App_Actions.Where(x => x.IsCredit).ToList();
            foreach (var item in actions)
            {
                item.Bookmark = true;
                dc.SubmitChanges();
            }

        }

        #endregion

        #region Get

        public static App_Action Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static App_Action Get(SocialLearningDataContext dc, int Id)
        {
            return dc.App_Actions.SingleOrDefault(u => u.Id == Id);
        }

        public static bool CheckAvailability(string area, string controller, string action)
        {
            return CheckAvailability(DBUtility.GetSocialLearningDataContext, area, controller, action);
        }

        public static bool CheckAvailability(SocialLearningDataContext dc, string area, string controller, string action)
        {
            return dc.App_Actions.Any(x => x.AreaName.ToLower() == area.ToLower() && x.ControllerName.ToLower() == controller.ToLower() && x.ActionName.ToLower() == action.ToLower());
        }

        public static App_Action Get(string area, string controller, string action)
        {
            return Get(DBUtility.GetSocialLearningDataContext, area, controller, action);
        }

        public static App_Action Get(SocialLearningDataContext dc, string area, string controller, string action)
        {
            if (string.IsNullOrEmpty(action))
                action = "Index";
            if (string.IsNullOrEmpty(controller))
                controller = "Home";
            return dc.App_Actions.SingleOrDefault(x => x.AreaName.ToLower() == area.ToLower() && x.ControllerName.ToLower() == controller.ToLower() && x.ActionName.ToLower() == action.ToLower());
        }

        #endregion

        #region GetAll

        public static List<App_Action> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<App_Action> GetAll(SocialLearningDataContext dc)
        {
            return dc.App_Actions.ToList();
        }

        public static List<App_Action> GetAllBookmarks()
        {
            return GetAllBookmarks(DBUtility.GetSocialLearningDataContext);
        }

        public static List<App_Action> GetAllBookmarks(SocialLearningDataContext dc)
        {
            return dc.App_Actions.Where(x => x.Bookmark).ToList();
        }

        public static List<int> GetAllAdmin()
        {
            return GetAllAdmin(DBUtility.GetSocialLearningDataContext);
        }

        public static List<int> GetAllAdmin(SocialLearningDataContext dc)
        {
            return dc.App_Actions.Where(u => u.App_Permissions.Any(x => x.App_Role.Title == "Admin")).Select(u => u.Id).Distinct().ToList();
        }

        public static List<int> GetAllByRole(string roleName)
        {
            return GetAllByRole(DBUtility.GetSocialLearningDataContext, roleName);
        }

        public static List<int> GetAllByRole(SocialLearningDataContext dc, string roleName)
        {
            return dc.App_Actions.Where(u => u.App_Permissions.Any(x => x.App_Role.Title.ToLower() == roleName.ToLower())).Select(u => u.Id).Distinct().ToList();
        }

        public static IQueryable<string> GetAllAreas()
        {
            return GetAllAreas(DBUtility.GetSocialLearningDataContext);
        }

        public static IQueryable<string> GetAllAreas(SocialLearningDataContext dc)
        {
            return dc.App_Actions.Select(u => u.AreaName).Distinct();
        }

        public static IQueryable<string> GetAllAreaControllers(string Id)
        {
            return GetAllAreaControllers(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static IQueryable<string> GetAllAreaControllers(SocialLearningDataContext dc, string Id)
        {
            return dc.App_Actions.Where(u => u.AreaName.ToLower() == Id.ToLower()).Select(u => u.ControllerName).Distinct();
        }

        public static IQueryable<App_Action> GetAllControllers(string Id)
        {
            return GetAllControllers(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static IQueryable<App_Action> GetAllControllers(SocialLearningDataContext dc, string Id)
        {
            return dc.App_Actions.Where(u => u.ControllerName.ToLower() == Id.ToLower()).Distinct();
        }

        #endregion

        #region Add

        public static DALReturnModel<App_Action> Add(App_Action model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_Action> Add(SocialLearningDataContext dc, App_Action model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new App_Action();
                    obj.AreaName = model.AreaName.StringNormalizer();
                    obj.ControllerName = model.ControllerName.StringNormalizer();
                    obj.ActionName = model.ActionName.StringNormalizer();
                    obj.Title = model.Title.StringNormalizer();
                    obj.RequireAuthorization = model.RequireAuthorization;
                    obj.RequireAuthentication = model.RequireAuthentication;
                    obj.IsActive = model.IsActive;
                    obj.ReturnBlankIfNotAllowed = false;
                    obj.IsCredit = false;
                    obj.CreditOnlyPost = false;
                    obj.InNotification = false;
                    obj.Bookmark = false;
                    obj.InEmail = false;
                    obj.CreateDate = DateTime.Now;
                    dc.App_Actions.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    var permission = new App_Permission
                    {
                        ActionId = obj.Id,
                        RoleId = App_RoleDAL.GetRole("admin").Id
                    };
                    App_PermissionDAL.Add(permission, bpr);
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<App_Action>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }

            return new DALReturnModel<App_Action>(new App_Action { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<App_Action> Update(App_Action model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_Action> Update(SocialLearningDataContext dc, App_Action model, BatchProcessResultModel bpr)
        {
            App_Action obj = null;
            bool noErrorFlag = true;
            obj = dc.App_Actions.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.AreaName = model.AreaName.StringNormalizer();
                        obj.ControllerName = model.ControllerName.StringNormalizer();
                        obj.ActionName = model.ActionName.StringNormalizer();
                        obj.Title = model.Title.StringNormalizer();
                        obj.RequireAuthorization = model.RequireAuthorization;
                        obj.RequireAuthentication = model.RequireAuthentication;
                        obj.IsActive = model.IsActive;
                        obj.IgnoreOwner = model.IgnoreOwner;
                        obj.ReturnBlankIfNotAllowed = model.ReturnBlankIfNotAllowed;
                        obj.Bookmark = model.Bookmark;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_Action>(obj, bpr);
        }

        public static DALReturnModel<App_Action> UpdateBookmarked(App_Action model, BatchProcessResultModel bpr)
        {
            return UpdateBookmarked(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_Action> UpdateBookmarked(SocialLearningDataContext dc, App_Action model, BatchProcessResultModel bpr)
        {
            App_Action obj = null;
            bool noErrorFlag = true;
            obj = dc.App_Actions.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.AreaName = model.AreaName.StringNormalizer();
                        obj.ControllerName = model.ControllerName.StringNormalizer();
                        obj.ActionName = model.ActionName.StringNormalizer();
                        obj.Title = model.Title.StringNormalizer();
                        obj.RequireAuthorization = model.RequireAuthorization;
                        obj.RequireAuthentication = model.RequireAuthentication;
                        obj.IsActive = model.IsActive;
                        obj.IgnoreOwner = model.IgnoreOwner;
                        obj.ReturnBlankIfNotAllowed = model.ReturnBlankIfNotAllowed;
                        obj.Bookmark = model.Bookmark;
                        obj.IsCredit = model.IsCredit;
                        obj.StudentCredit = model.StudentCredit;
                        obj.TACredit = model.TACredit;
                        obj.TeacherCredit = model.TeacherCredit;
                        obj.CreditOnlyPost = model.CreditOnlyPost;
                        obj.InNotification = model.InNotification;
                        obj.DoesNotHaveDAL = model.DoesNotHaveDAL;

                        //obj.AssociatedObjectId = model.AssociatedObjectId;
                        obj.InEmail = model.InEmail;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_Action>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<App_Action> Delete(App_Action model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<App_Action> Delete(SocialLearningDataContext dc, App_Action model)
        {
            try
            {
                var obj = dc.App_Actions.Single(q => q.Id == model.Id);

                //var permissions = obj.App_Permissions.ToList();
                //foreach (var item in permissions)
                //{
                //    dc.App_Permissions.DeleteOnSubmit(item);
                //    dc.SubmitChanges();
                //}
                //var menues = obj.App_SiteMenus.ToList();
                //foreach (var item in permissions)
                //{
                //    App_SiteMenuDAL.Delete(new App_SiteMenu { Id = item.Id });
                //}

                dc.App_Actions.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<App_Action>(new App_Action { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<App_Action>(new App_Action { Id = 0 });
            }
        }

        #endregion
    }
}
