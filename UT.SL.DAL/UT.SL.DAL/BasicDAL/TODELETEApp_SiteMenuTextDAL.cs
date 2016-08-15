using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Data.LINQ;
using UT.SL.Helper;


namespace UT.SL.DAL
{

    public partial class dddApp_SiteMenuTextDAL
    {

        #region Get
        public static App_SiteMenuText Get(SocialLearningDataContext dc, int Id)
        {
            return dc.App_SiteMenuTexts.SingleOrDefault(u => u.Id == Id);
        }

        public static App_SiteMenuText Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static App_SiteMenuText GetWithContentType(SocialLearningDataContext dc, int Id, byte ctype)
        {
            return dc.App_SiteMenuTexts.SingleOrDefault(q => q.ContentType == ctype && q.App_SiteMenu.Id == Id); ;
        }

        public static App_SiteMenuText GetWithContentType(int Id, byte ctype)
        {
            return GetWithContentType(DBUtility.GetSocialLearningDataContext, Id, ctype);
        }

        #endregion

        #region GetAll
        public static List<App_SiteMenuText> GetAll(SocialLearningDataContext dc)
        {
            return dc.App_SiteMenuTexts.ToList();
        }

        public static List<App_SiteMenuText> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static int CountAll(SocialLearningDataContext dc, int Id)
        {
            return dc.App_SiteMenuTexts.Count(x => x.App_SiteMenu.Id == Id);
        }

        public static int CountAll(int Id)
        {
            return CountAll(DBUtility.GetSocialLearningDataContext, Id);
        }
        #endregion

        #region Add
        public static App_SiteMenuText Add(App_SiteMenuText model, out BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, out bpr);
        }

        public static App_SiteMenuText Add(SocialLearningDataContext dc, App_SiteMenuText model, out BatchProcessResultModel bpr)
        {
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new App_SiteMenuText();
                    obj.ContentText = model.ContentText.StringNormalizer();
                    obj.ContentType = model.ContentType;
                    obj.MenuId = model.MenuId;
                    dc.App_SiteMenuTexts.InsertOnSubmit(obj);
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
        public static App_SiteMenuText Update(App_SiteMenuText model, out BatchProcessResultModel bpr, bool InsertIfNotExist = false)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, out bpr, InsertIfNotExist);
        }

        public static App_SiteMenuText Update(SocialLearningDataContext dc, App_SiteMenuText model, out BatchProcessResultModel bpr, bool InsertIfNotExist = false)
        {
            App_SiteMenuText obj = null;
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;
            obj = dc.App_SiteMenuTexts.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.ContentText = model.ContentText.StringNormalizer();
                        obj.ContentType = model.ContentType;
                        obj.MenuId = model.MenuId;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                    else
                    {
                        if (InsertIfNotExist)
                        {
                            dc.App_SiteMenuTexts.InsertOnSubmit(obj);
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
        public static bool Delete(App_SiteMenuText model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static bool Delete(SocialLearningDataContext dc, App_SiteMenuText model)
        {
            try
            {
                var obj = dc.App_SiteMenuTexts.Single(q => q.Id == model.Id);
                dc.App_SiteMenuTexts.DeleteOnSubmit(obj);
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
