using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class App_UserInfoDAL
    {

        #region Get

        public static App_UserInfo Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static App_UserInfo Get(SocialLearningDataContext dc, int Id)
        {
            return dc.App_UserInfos.SingleOrDefault(u => u.Id == Id);
        }

        public static App_UserInfo Get(Guid Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static App_UserInfo Get(SocialLearningDataContext dc, Guid Id)
        {
            return dc.App_UserInfos.SingleOrDefault(u => u.App_User.GuidId == Id);
        }

        #endregion

        #region GetAll

        public static List<App_UserInfo> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<App_UserInfo> GetAll(SocialLearningDataContext dc)
        {
            return dc.App_UserInfos.ToList();
        }

        #endregion

        #region Find

        public static IQueryable<App_UserInfo> Find(App_UserInfoSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<App_UserInfo> Find(SocialLearningDataContext dc, App_UserInfoSearchModel model)
        {
            var qry = from p in dc.App_UserInfos select p;
            if (model != null)
            {

                if (model.SearchBirsthDate.HasValue && model.SearchBirsthDate > DateTime.MinValue)
                {
                    qry = qry.Where(u => u.BirsthDate == model.SearchBirsthDate.WesternizeDateTime());
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

        public static DALReturnModel<App_UserInfo> Add(App_UserInfo model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_UserInfo> Add(SocialLearningDataContext dc, App_UserInfo model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new App_UserInfo();

                    obj.NationalId = model.NationalId.StringNormalizer();
                    obj.BirsthDate = model.BirsthDate.WesternizeDateTime();
                    obj.UserId = model.UserId;
                    dc.App_UserInfos.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<App_UserInfo>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_UserInfo>(new App_UserInfo { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<App_UserInfo> Update(App_UserInfo model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_UserInfo> Update(SocialLearningDataContext dc, App_UserInfo model, BatchProcessResultModel bpr)
        {
            App_UserInfo obj = null;
            bool noErrorFlag = true;
            var InsertIfNotExist = false;
            obj = dc.App_UserInfos.SingleOrDefault(u => u.UserId == App_UserDAL.Get(model.App_User.GuidId).Id);
            if (obj == null)
            {
                InsertIfNotExist = true;
            }
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.NationalId = model.NationalId.StringNormalizer();
                        obj.BirsthDate = model.BirsthDate.WesternizeDateTime();
                        obj.About = model.About.StringNormalizer();
                        obj.BSin = model.BSin.StringNormalizer();
                        obj.MSin = model.MSin.StringNormalizer();
                        obj.PHDin = model.PHDin.StringNormalizer();
                        obj.Title = model.Title;
                        obj.Occupation = model.Occupation.StringNormalizer();
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                    else
                    {
                        if (InsertIfNotExist)
                        {
                            obj = new App_UserInfo
                            {
                                UserId = App_UserDAL.Get(model.App_User.GuidId).Id,
                                NationalId = model.NationalId.StringNormalizer(),
                                BirsthDate = model.BirsthDate,
                                About = model.About.StringNormalizer(),
                                BSin = model.BSin.StringNormalizer(),
                                MSin = model.MSin.StringNormalizer(),
                                PHDin = model.PHDin.StringNormalizer(),
                                Title = model.Title,
                                Occupation = model.Occupation.StringNormalizer()
                            };
                            dc.App_UserInfos.InsertOnSubmit(obj);
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
            return new DALReturnModel<App_UserInfo>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<App_UserInfo> Delete(App_UserInfo model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<App_UserInfo> Delete(SocialLearningDataContext dc, App_UserInfo model)
        {
            try
            {
                var obj = dc.App_UserInfos.Single(q => q.Id == model.Id);
                dc.App_UserInfos.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<App_UserInfo>(new App_UserInfo { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<App_UserInfo>(new App_UserInfo { Id = 0 });
            }
        }

        #endregion
    }
}
