using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;

namespace UT.SL.DAL
{
    public partial class AssessDAL
    {

        #region Get

        public static Assess Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static Assess Get(SocialLearningDataContext dc, int Id)
        {
            return dc.Assesses.SingleOrDefault(u => u.Id == Id);
        }

        public static List<Assess> GetListIfExist(int parentId)
        {
            return GetListIfExist(DBUtility.GetSocialLearningDataContext, parentId);
        }

        public static List<Assess> GetListIfExist(SocialLearningDataContext dc, int parentId)
        {
            return dc.Assesses.Where(u => u.ParentId == parentId).ToList();
        }

        public static List<Assess> GetListIfExist(int parentId, int userId)
        {
            return GetListIfExist(DBUtility.GetSocialLearningDataContext, parentId, userId);
        }

        public static List<Assess> GetListIfExist(SocialLearningDataContext dc, int parentId, int userId)
        {
            return dc.Assesses.Where(u => u.UserId == userId && u.ParentId == parentId).ToList();
        }

        public static Assess GetIfExist(int parentId, int userId)
        {
            return GetIfExist(DBUtility.GetSocialLearningDataContext, parentId, userId);
        }

        public static Assess GetIfExist(SocialLearningDataContext dc, int parentId, int userId)
        {
            return dc.Assesses.FirstOrDefault(u => u.UserId == userId && u.ParentId == parentId);
        }

        public static List<Assess> GetIfExist(int parentId, int userId, int assessType)
        {
            return GetIfExist(DBUtility.GetSocialLearningDataContext, parentId, userId, assessType);
        }

        public static List<Assess> GetIfExist(SocialLearningDataContext dc, int parentId, int userId, int assessType)
        {
            return dc.Assesses.Where(u => u.UserId == userId && u.ParentId == parentId && u.AssessType == assessType).ToList();
        }

        #endregion

        #region GetAll

        public static List<Assess> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<Assess> GetAll(SocialLearningDataContext dc)
        {
            return dc.Assesses.ToList();
        }

        public static List<Assess> GetAllByCourse(int id)
        {
            return GetAllByCourse(DBUtility.GetSocialLearningDataContext, id);
        }

        public static List<Assess> GetAllByCourse(SocialLearningDataContext dc, int id)
        {
            return dc.Assesses.ToList();
        }


        #endregion

        #region Add

        public static DALReturnModel<Assess> AddUpDownAssess(Assess model)
        {
            return AddUpDownAssess(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Assess> AddUpDownAssess(SocialLearningDataContext dc, Assess model)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new Assess();
                    obj.Updown = model.Updown;
                    obj.CreateDate = DateTime.Now;
                    obj.UserId = model.UserId;
                    obj.ParentId = model.ParentId;
                    obj.AssessValue = 0;
                    dc.Assesses.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<Assess>(obj);
                }
            }
            catch
            {
            }
            return new DALReturnModel<Assess>(new Assess { Id = 0 });
        }

        public static DALReturnModel<Assess> AddReaction(Assess model)
        {
            return AddReaction(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Assess> AddReaction(SocialLearningDataContext dc, Assess model)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new Assess();
                    obj.Updown = false;
                    obj.CreateDate = DateTime.Now;
                    obj.UserId = model.UserId;
                    obj.ParentId = model.ParentId;
                    obj.AssessValue = model.AssessValue;
                    obj.AssessType = model.AssessType;
                    dc.Assesses.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<Assess>(obj);
                }
            }
            catch
            {
            }
            return new DALReturnModel<Assess>(new Assess { Id = 0 });
        }

        #endregion

        #region Update

        public static DALReturnModel<Assess> UpdateUpDownAssess(Assess model)
        {
            return UpdateUpDownAssess(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Assess> UpdateUpDownAssess(SocialLearningDataContext dc, Assess model)
        {
            Assess obj = null;
            bool noErrorFlag = true;
            obj = dc.Assesses.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Updown = model.Updown;
                        dc.SubmitChanges();
                    }
                }
            }
            catch
            {
            }
            return new DALReturnModel<Assess>(obj);
        }

        public static DALReturnModel<Assess> UpdateReaction(Assess model)
        {
            return UpdateReaction(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Assess> UpdateReaction(SocialLearningDataContext dc, Assess model)
        {
            Assess obj = null;
            bool noErrorFlag = true;
            obj = dc.Assesses.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.AssessValue = model.AssessValue;
                        dc.SubmitChanges();
                    }

                }
            }
            catch
            {
            }
            return new DALReturnModel<Assess>(obj);
        }

        #endregion

        #region Delete

        public static DALReturnModel<Assess> Delete(Assess model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Assess> Delete(SocialLearningDataContext dc, Assess model)
        {
            try
            {
                var obj = dc.Assesses.Single(q => q.Id == model.Id);
                dc.Assesses.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<Assess>(new Assess { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Assess>(new Assess { Id = 0 });
            }
        }

        #endregion
    }
}
