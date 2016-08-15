using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;
using UT.SL.Model.Enumeration;


namespace UT.SL.DAL
{


    public partial class App_UserEnrolementDAL
    {

        #region Get

        public static App_UserEnrolement Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static App_UserEnrolement Get(SocialLearningDataContext dc, int Id)
        {
            return dc.App_UserEnrolements.SingleOrDefault(u => u.Id == Id);
        }

        public static App_UserEnrolement HaveThisMembership(string userName, int roleType)
        {
            return HaveThisMembership(DBUtility.GetSocialLearningDataContext, userName, roleType);
        }

        public static App_UserEnrolement HaveThisMembership(SocialLearningDataContext dc, string userName, int roleType)
        {
            return dc.App_UserEnrolements.FirstOrDefault(u => u.App_User.UserName == userName && u.Type == roleType);
        }

        public static List<App_UserEnrolement> IsAMember(int courseId, string userName)
        {
            return IsAMember(DBUtility.GetSocialLearningDataContext, courseId, userName);
        }

        public static List<App_UserEnrolement> IsAMember(SocialLearningDataContext dc, int courseId, string userName)
        {
            return dc.App_UserEnrolements.Where(u => u.CourseId == courseId && u.App_User.UserName == userName.StringNormalizer()).ToList();
        }

        public static bool IsAMember(int courseId, int userId)
        {
            return IsAMember(DBUtility.GetSocialLearningDataContext, courseId, userId);
        }

        public static bool IsAMember(SocialLearningDataContext dc, int courseId, int userId)
        {
            return dc.App_UserEnrolements.Any(u => u.CourseId == courseId && u.UserId == userId);
        }

        #endregion

        #region GetAll

        public static List<App_UserEnrolement> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<App_UserEnrolement> GetAll(SocialLearningDataContext dc)
        {
            return dc.App_UserEnrolements.ToList();
        }

        public static List<App_UserEnrolement> GetAllByCourse(int id)
        {
            return GetAllByCourse(DBUtility.GetSocialLearningDataContext, id);
        }

        public static List<App_UserEnrolement> GetAllByCourse(SocialLearningDataContext dc, int id)
        {
            return dc.App_UserEnrolements.Where(x => x.CourseId == id).ToList();
        }

        public static List<App_User> GetAllCourseTeachers(int id)
        {
            return GetAllCourseTeachers(DBUtility.GetSocialLearningDataContext, id);
        }

        public static List<App_User> GetAllCourseTeachers(SocialLearningDataContext dc, int id)
        {
            return dc.App_UserEnrolements.Where(x => x.CourseId == id && x.Type == (int)UT.SL.Model.Enumeration.MemebrshipType.Teacher).Select(x => x.App_User).ToList();
        }

        public static List<App_User> GetAllCourseTeachersAndTAs(int id)
        {
            return GetAllCourseTeachersAndTAs(DBUtility.GetSocialLearningDataContext, id);
        }

        public static List<App_User> GetAllCourseTeachersAndTAs(SocialLearningDataContext dc, int id)
        {
            return dc.App_UserEnrolements.Where(x => x.CourseId == id && (x.Type == (int)MemebrshipType.Teacher || x.Type == (int)MemebrshipType.TA)).Select(x => x.App_User).ToList();
        }

        public static List<App_UserEnrolement> GetAllByCourseAndUser(int courseId, int userId)
        {
            return GetAllByCourseAndUser(DBUtility.GetSocialLearningDataContext, courseId, userId);
        }

        public static List<App_UserEnrolement> GetAllByCourseAndUser(SocialLearningDataContext dc, int courseId, int userId)
        {
            return dc.App_UserEnrolements.Where(x => x.CourseId == courseId && x.UserId == userId).ToList();
        }

        public static List<App_UserEnrolement> GetAllUserEnrolemts(int id)
        {
            return GetAllUserEnrolemts(DBUtility.GetSocialLearningDataContext, id);
        }

        public static List<App_UserEnrolement> GetAllUserEnrolemts(SocialLearningDataContext dc, int id)
        {
            return dc.App_UserEnrolements.Where(x => x.UserId == id).Distinct().ToList();
        }

        public static List<App_User> GetAll(string userName, int courseId)
        {
            return GetAll(DBUtility.GetSocialLearningDataContext, userName, courseId);
        }

        public static List<App_User> GetAll(SocialLearningDataContext dc, string userName, int courseId)
        {
            return dc.App_UserEnrolements.Where(x => x.CourseId == courseId && (x.App_User.UserName.ToLower().Contains(userName) || x.App_User.FirstName.ToLower().Contains(userName) || x.App_User.LastName.ToLower().Contains(userName) || x.App_User.Email.ToLower().Contains(userName))).Select(x => x.App_User).Distinct().ToList();
        }

        #endregion

        #region Find

        public static IQueryable<App_UserEnrolement> Find(App_UserEnrolementSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<App_UserEnrolement> Find(SocialLearningDataContext dc, App_UserEnrolementSearchModel model)
        {
            var qry = from p in dc.App_UserEnrolements select p;
            if (model != null)
            {
                if (model.SearchType.HasValue && model.SearchType > 0)
                {
                    qry = qry.Where(u => u.Type == model.SearchType);
                }
                if (model.SearchCreateDate.HasValue && model.SearchCreateDate > DateTime.MinValue)
                {
                    qry = qry.Where(u => u.CreateDate == model.SearchCreateDate.WesternizeDateTime());
                }
                if (model.SearchStatus.HasValue && model.SearchStatus > 0)
                {
                    qry = qry.Where(u => u.Status == model.SearchStatus);
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

        public static DALReturnModel<App_UserEnrolement> Add(App_UserEnrolement model, out BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, out bpr);
        }

        public static DALReturnModel<App_UserEnrolement> Add(SocialLearningDataContext dc, App_UserEnrolement model, out BatchProcessResultModel bpr)
        {
            bpr = new BatchProcessResultModel();
            bool noErrorFlag = true;

            if (dc.App_UserEnrolements.Any(x => x.CourseId == model.CourseId & x.UserId == model.UserId && x.Type == model.Type))
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.BprDupInput, true, true);
                noErrorFlag = false;
            }
            try
            {
                if (noErrorFlag)
                {
                    var obj = new App_UserEnrolement();
                    obj.Type = model.Type;
                    obj.CreateDate = DateTime.Now;
                    obj.UserId = model.UserId;
                    obj.CourseId = model.CourseId;
                    dc.App_UserEnrolements.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<App_UserEnrolement>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_UserEnrolement>(new App_UserEnrolement { Id = 0 }, bpr);
        }

        public static DALReturnModel<App_UserEnrolement> AddCourseMember(AddCourseMembershipModel model, BatchProcessResultModel bpr)
        {
            return AddCourseMember(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_UserEnrolement> AddCourseMember(SocialLearningDataContext dc, AddCourseMembershipModel model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            if (App_UserDAL.Get(model.Member) == null)
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.BprMainUnknownError, true, true);
                noErrorFlag = false;
            }
            if (noErrorFlag && dc.App_UserEnrolements.Any(x => x.CourseId == model.CourseId && x.UserId == App_UserDAL.Get(model.Member).Id))
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.BprDupInput, true, true);
                noErrorFlag = false;
            }
            try
            {
                if (noErrorFlag)
                {
                    var obj = new App_UserEnrolement();
                    obj.CourseId = model.CourseId;
                    obj.UserId = App_UserDAL.Get(model.Member).Id;
                    obj.Type = model.MembershipType;
                    obj.CreateDate = DateTime.Now;
                    dc.App_UserEnrolements.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<App_UserEnrolement>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_UserEnrolement>(new App_UserEnrolement { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<App_UserEnrolement> Update(App_UserEnrolement model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_UserEnrolement> Update(SocialLearningDataContext dc, App_UserEnrolement model, BatchProcessResultModel bpr)
        {
            App_UserEnrolement obj = null;
            bool noErrorFlag = true;
            obj = dc.App_UserEnrolements.SingleOrDefault(u => u.Id == model.Id);
            if (dc.App_UserEnrolements.Any(x => x.CourseId == obj.CourseId & x.UserId == obj.UserId && x.Type == model.Type && x.Id != model.Id))
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.BprDupInput, true, true);
                noErrorFlag = false;
            }
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Type = model.Type;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_UserEnrolement>(obj, bpr);
        }

        #endregion

        #region Delete

        public static DALReturnModel<App_UserEnrolement> Delete(App_UserEnrolement model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<App_UserEnrolement> Delete(SocialLearningDataContext dc, App_UserEnrolement model)
        {
            try
            {
                var obj = dc.App_UserEnrolements.Single(q => q.Id == model.Id);
                dc.App_UserEnrolements.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<App_UserEnrolement>(new App_UserEnrolement { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<App_UserEnrolement>(new App_UserEnrolement { Id = 0 });
            }
        }

        public static DALReturnModel<App_UserEnrolement> DeleteMember(int cId, Guid userId)
        {
            return DeleteMember(DBUtility.GetSocialLearningDataContext, cId, userId);
        }

        public static DALReturnModel<App_UserEnrolement> DeleteMember(SocialLearningDataContext dc, int cId, Guid userId)
        {
            try
            {
                var obj = dc.App_UserEnrolements.FirstOrDefault(q => q.App_User.GuidId == userId && q.CourseId == cId);
                if (obj != null)
                    dc.App_UserEnrolements.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<App_UserEnrolement>(new App_UserEnrolement { Id = cId }); //!!!!wrong
            }
            catch (System.Exception)
            {
                return new DALReturnModel<App_UserEnrolement>(new App_UserEnrolement { Id = 0 });
            }
        }

        #endregion
    }
}
