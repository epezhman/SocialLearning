using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Data.LINQ;
using UT.SL.Helper;


namespace UT.SL.DAL
{
    public partial class dddApp_SiteMenuDAL
    {

        #region Get
        public static App_SiteMenu Get(SocialLearningDataContext dc, int Id)
        {
            return dc.App_SiteMenus.SingleOrDefault(u => u.Id == Id);
        }

        public static App_SiteMenu Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }
        #endregion

        #region GetAll
        public static List<App_SiteMenu> GetAll(SocialLearningDataContext dc)
        {
            return dc.App_SiteMenus.ToList();
        }

        public static List<App_SiteMenu> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static int CountAll(SocialLearningDataContext dc, int Id)
        {
            return dc.App_SiteMenus.Count(u => u.App_SiteMenu1.Id == Id);
        }

        public static int CountAll(int Id)
        {
            return CountAll(DBUtility.GetSocialLearningDataContext, Id);
        }

        #endregion

        #region Add
        public static App_SiteMenu Add(App_SiteMenu model, out BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, out bpr);
        }

        public static App_SiteMenu Add(SocialLearningDataContext dc, App_SiteMenu model, out BatchProcessResultModel bpr)
        {
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new App_SiteMenu();
                    obj.MenuTitle = model.MenuTitle.StringNormalizer();
                    obj.BrifDescription = model.BrifDescription.StringNormalizer();
                    obj.ExternalLink = model.ExternalLink.StringNormalizer();
                    obj.PageTitle = model.PageTitle.StringNormalizer();
                    obj.CreateDate = DateTime.Now;
                    obj.Rank = model.Rank;
                    obj.MenuPosition = model.MenuPosition;
                    obj.IsActive = model.IsActive;
                    obj.RequireAuthentication = model.RequireAuthentication;
                    obj.MenuId = model.MenuId;
                    obj.ActionId = model.ActionId;
                    if (model.ParentId > 0)
                    {
                        obj.ParentId = model.ParentId;
                        //obj.MenuPosition = App_SiteMenuDAL.Get(model.ParentId.Value).MenuPosition;
                    }
                    dc.App_SiteMenus.InsertOnSubmit(obj);
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
        public static App_SiteMenu Update(App_SiteMenu model, out BatchProcessResultModel bpr, bool InsertIfNotExist = false)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, out bpr, InsertIfNotExist);
        }

        public static App_SiteMenu Update(SocialLearningDataContext dc, App_SiteMenu model, out BatchProcessResultModel bpr, bool InsertIfNotExist = false)
        {
            App_SiteMenu obj = null;
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            obj = dc.App_SiteMenus.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.MenuTitle = model.MenuTitle.StringNormalizer();
                        obj.BrifDescription = model.BrifDescription.StringNormalizer();
                        obj.ExternalLink = model.ExternalLink.StringNormalizer();
                        obj.PageTitle = model.PageTitle.StringNormalizer();
                        obj.Rank = model.Rank;
                        obj.MenuPosition = model.MenuPosition;
                        obj.IsActive = model.IsActive;
                        obj.RequireAuthentication = model.RequireAuthentication;
                        obj.MenuId = model.MenuId;
                        obj.ActionId = model.ActionId;
                        if (model.ParentId > 0)
                        {
                            obj.ParentId = model.ParentId;
                            //obj.MenuPosition = App_SiteMenuDAL.Get(model.ParentId.Value).MenuPosition;
                        }
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                    else
                    {
                        if (InsertIfNotExist)
                        {
                            dc.App_SiteMenus.InsertOnSubmit(obj);
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
        public static bool Delete(App_SiteMenu model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static bool Delete(SocialLearningDataContext dc, App_SiteMenu model)
        {
            try
            {
                var obj = dc.App_SiteMenus.Single(q => q.Id == model.Id);
                foreach (var item in obj.App_SiteMenuTexts)
                {
                    dc.App_SiteMenuTexts.DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }        
                dc.App_SiteMenus.DeleteOnSubmit(obj);
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
