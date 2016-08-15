using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class AssignmentDAL
    {

        #region Get

        public static Assignment Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static Assignment Get(SocialLearningDataContext dc, int Id)
        {
            return dc.Assignments.SingleOrDefault(u => u.Id == Id);
        }

        #endregion

        #region GetAll

        public static List<Assignment> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<Assignment> GetAll(SocialLearningDataContext dc)
        {
            return dc.Assignments.ToList();
        }

        public static List<Assignment> GetAllByCourse(int id)
        {
            return GetAllByCourse(DBUtility.GetSocialLearningDataContext, id);
        }

        public static List<Assignment> GetAllByCourse(SocialLearningDataContext dc, int id)
        {
            return dc.Assignments.Where(x => x.CourseId == id).ToList();
        }

        #endregion

        #region Find

        public static IQueryable<Assignment> Find(AssignmentSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<Assignment> Find(SocialLearningDataContext dc, AssignmentSearchModel model)
        {
            var qry = from p in dc.Assignments select p;

            if (model != null)
            {
                if (model.CourseId.HasValue)
                {
                    qry = qry.Where(x => x.CourseId == model.CourseId);
                }
                if (!string.IsNullOrEmpty(model.SearchTitle))
                {
                    qry = qry.Where(u => u.Title.Contains(model.SearchTitle.StringNormalizer()));
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

        public static DALReturnModel<Assignment> Add(Assignment model)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Assignment> Add(SocialLearningDataContext dc, Assignment model)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new Assignment();
                    obj.Title = model.Title.StringNormalizer();
                    obj.Type = model.Type;
                    obj.CreateDate = DateTime.Now;
                    obj.CourseId = model.CourseId;
                    dc.Assignments.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    return new DALReturnModel<Assignment>(obj);
                }
            }
            catch
            {
            }
            return new DALReturnModel<Assignment>(new Assignment { Id = 0 });
        }

        public static DALReturnModel<Assignment> Add(Assignment model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Assignment> Add(SocialLearningDataContext dc, Assignment model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    model.Title = model.Title.StringNormalizer();
                    dc.Assignments.InsertOnSubmit(model);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<Assignment>(model, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Assignment>(new Assignment { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<Assignment> Update(Assignment model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Assignment> Update(SocialLearningDataContext dc, Assignment model, BatchProcessResultModel bpr)
        {
            Assignment obj = null;
            bool noErrorFlag = true;
            obj = dc.Assignments.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Title = model.Title.StringNormalizer();
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Assignment>(obj);
        }

        public static DALReturnModel<Assignment> UpdateFile(Assignment model, BatchProcessResultModel bpr)
        {
            return UpdateFile(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Assignment> UpdateFile(SocialLearningDataContext dc, Assignment model, BatchProcessResultModel bpr)
        {
            Assignment obj = null;
            bool noErrorFlag = true;
            obj = dc.Assignments.SingleOrDefault(u => u.Id == model.Id);
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
            return new DALReturnModel<Assignment>(obj);
        }

        #endregion

        #region Delete

        public static DALReturnModel<Assignment> Delete(Assignment model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Assignment> Delete(SocialLearningDataContext dc, Assignment model)
        {
            try
            {
                var obj = dc.Assignments.Single(q => q.Id == model.Id);
                dc.Assignments.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<Assignment>(new Assignment { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Assignment>(new Assignment { Id = 0 });
            }
        }

        public static DALReturnModel<Assignment> DeleteFile(Assignment model)
        {
            return DeleteFile(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Assignment> DeleteFile(SocialLearningDataContext dc, Assignment model)
        {
            try
            {
                var obj = dc.Assignments.Single(q => q.Id == model.Id);
                obj.FileContent = null;
                obj.FileMime = null;
                obj.FileSize = null;
                obj.FileTitle = null;
                dc.SubmitChanges();
                return new DALReturnModel<Assignment>(new Assignment { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Assignment>(new Assignment { Id = 0 });
            }
        }

        #endregion
    }
}
