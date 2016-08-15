using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UT.SL.Data.LINQ;
using UT.SL.Helper;
using UT.SL.Model;

namespace UT.SL.DAL
{
    public partial class AssignmentSubmissionDAL
    {

        #region Get

        public static AssignmentSubmission Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static AssignmentSubmission Get(SocialLearningDataContext dc, int Id)
        {
            return dc.AssignmentSubmissions.SingleOrDefault(u => u.Id == Id);
        }

        public static AssignmentSubmission Get(int Id, int UserId)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id, UserId);
        }

        public static AssignmentSubmission Get(SocialLearningDataContext dc, int id, int UserId)
        {
            return dc.AssignmentSubmissions.SingleOrDefault(x => (x.AssignmentId == id && x.UserId == UserId) && (x.IsPublishd && x.IsValid));
        }

        #endregion

        #region GetAll

        public static List<AssignmentSubmission> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<AssignmentSubmission> GetAll(SocialLearningDataContext dc)
        {
            return dc.AssignmentSubmissions.Where(x => x.IsPublishd && x.IsValid).ToList();
        }

        public static List<AssignmentSubmission> GetAllSubmissions(int id)
        {
            return GetAllSubmissions(DBUtility.GetSocialLearningDataContext, id);
        }

        public static List<AssignmentSubmission> GetAllSubmissions(SocialLearningDataContext dc, int id)
        {
            return dc.AssignmentSubmissions.Where(x => (x.Id == id) && (x.IsPublishd && x.IsValid)).ToList();
        }

        public static List<AssignmentSubmission> GetAllSubmissions(int id, int UserId)
        {
            return GetAllSubmissions(DBUtility.GetSocialLearningDataContext, id, UserId);
        }

        public static List<AssignmentSubmission> GetAllSubmissions(SocialLearningDataContext dc, int id, int UserId)
        {
            return dc.AssignmentSubmissions.Where(x => (x.AssignmentId == id && x.UserId == UserId) && (x.IsPublishd && x.IsValid)).ToList();
        }

        public static List<AssignmentSubmission> GetAllByCourse(int id)
        {
            return GetAllByCourse(DBUtility.GetSocialLearningDataContext, id);
        }

        public static List<AssignmentSubmission> GetAllByCourse(SocialLearningDataContext dc, int id)
        {
            return dc.AssignmentSubmissions.Where(x => x.Assignment.CourseId == id).ToList();
        }


        #endregion

        #region Add

        public static DALReturnModel<AssignmentSubmission> Add(AssignmentSubmission model)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<AssignmentSubmission> Add(SocialLearningDataContext dc, AssignmentSubmission model)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new AssignmentSubmission();
                    obj.Body = model.Body.StringNormalizer();
                    obj.CreateDate = DateTime.Now;

                    obj.AssignmentId = model.AssignmentId;
                    obj.UserId = model.UserId;
                    dc.AssignmentSubmissions.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<AssignmentSubmission>(obj);
                }
            }
            catch
            {
            }
            return new DALReturnModel<AssignmentSubmission>(new AssignmentSubmission { Id = 0 });
        }

        public static DALReturnModel<AssignmentSubmission> Add(AssignmentSubmission model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<AssignmentSubmission> Add(SocialLearningDataContext dc, AssignmentSubmission model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    model.Body = model.Body.StringNormalizer();
                    dc.AssignmentSubmissions.InsertOnSubmit(model);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<AssignmentSubmission>(model);
                }
            }
            catch
            {
                return new DALReturnModel<AssignmentSubmission>(new AssignmentSubmission { Id = 0 });
            }
            return null;
        }

        #endregion

        #region Update

        public static DALReturnModel<AssignmentSubmission> Update(AssignmentSubmission model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<AssignmentSubmission> Update(SocialLearningDataContext dc, AssignmentSubmission model, BatchProcessResultModel bpr)
        {
            AssignmentSubmission obj = null;
            bool noErrorFlag = true;
            obj = dc.AssignmentSubmissions.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Body = model.Body.StringNormalizer();
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<AssignmentSubmission>(obj);
        }

        public static DALReturnModel<AssignmentSubmission> UpdateFile(AssignmentSubmission model, BatchProcessResultModel bpr)
        {
            return UpdateFile(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<AssignmentSubmission> UpdateFile(SocialLearningDataContext dc, AssignmentSubmission model, BatchProcessResultModel bpr)
        {
            AssignmentSubmission obj = null;
            bool noErrorFlag = true;
            obj = dc.AssignmentSubmissions.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.FileContent = model.FileContent;
                        obj.FileMime = model.FileMime;
                        obj.FileSize = model.FileSize;
                        obj.FileTitle = model.FileTitle;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<AssignmentSubmission>(obj);
        }

        #endregion

        #region Delete

        public static DALReturnModel<AssignmentSubmission> Delete(AssignmentSubmission model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<AssignmentSubmission> Delete(SocialLearningDataContext dc, AssignmentSubmission model)
        {
            try
            {
                var obj = dc.AssignmentSubmissions.Single(q => q.Id == model.Id);
                dc.AssignmentSubmissions.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<AssignmentSubmission>(new AssignmentSubmission { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<AssignmentSubmission>(new AssignmentSubmission { Id = 0 });
            }
        }

        public static DALReturnModel<AssignmentSubmission> DeleteFile(AssignmentSubmission model)
        {
            return DeleteFile(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<AssignmentSubmission> DeleteFile(SocialLearningDataContext dc, AssignmentSubmission model)
        {
            try
            {
                var obj = dc.AssignmentSubmissions.Single(q => q.Id == model.Id);
                obj.FileContent = null;
                obj.FileMime = null;
                obj.FileSize = null;
                obj.FileTitle = null;
                dc.SubmitChanges();
                return new DALReturnModel<AssignmentSubmission>(new AssignmentSubmission { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<AssignmentSubmission>(new AssignmentSubmission { Id = 0 });
            }
        }

        #endregion


    }
}
