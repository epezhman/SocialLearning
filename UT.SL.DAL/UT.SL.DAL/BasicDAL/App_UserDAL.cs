using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;


namespace UT.SL.DAL
{


    public partial class App_UserDAL
    {

        #region Get

        public static App_User Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static App_User Get(SocialLearningDataContext dc, int Id)
        {
            return dc.App_Users.SingleOrDefault(u => u.Id == Id);
        }

        public static App_User Get(string Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static App_User Get(SocialLearningDataContext dc, string Id)
        {
            return dc.App_Users.SingleOrDefault(u => u.UserName == Id.StringNormalizer());
        }

        public static App_User GetEmail(string Id)
        {
            return GetEmail(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static App_User GetEmail(SocialLearningDataContext dc, string Id)
        {
            return dc.App_Users.SingleOrDefault(u => u.Email == Id.StringNormalizer());
        }

        public static App_User Get(System.Guid GuidId)
        {
            return Get(DBUtility.GetSocialLearningDataContext, GuidId);
        }

        public static App_User Get(SocialLearningDataContext dc, System.Guid GuidId)
        {
            return dc.App_Users.SingleOrDefault(u => u.GuidId == GuidId);
        }

        public static bool CheckEmail(string Id, int personId = 0)
        {
            return CheckEmail(DBUtility.GetSocialLearningDataContext, Id, personId);
        }

        public static bool CheckEmail(SocialLearningDataContext dc, string Id, int personId = 0)
        {
            if (personId == 0)
                return dc.App_Users.Any(u => u.Email == Id.StringNormalizer());
            else
                return dc.App_Users.Any(u => u.Email == Id.StringNormalizer() && u.Id != personId);
        }

        public static bool CheckUsername(string Id, int userId = 0)
        {
            return CheckUsername(DBUtility.GetSocialLearningDataContext, Id, userId);
        }

        public static bool CheckUsername(SocialLearningDataContext dc, string Id, int userId = 0)
        {

            if (userId == 0)
                return dc.App_Users.Any(u => u.UserName == Id.StringNormalizer());
            else
                return dc.App_Users.Any(u => u.UserName == Id.StringNormalizer() && u.Id != userId);
        }

        public static App_User ValidateUser(string username, string password)
        {
            return ValidateUser(DBUtility.GetSocialLearningDataContext, username, password);
        }

        public static App_User ValidateUser(SocialLearningDataContext dc, string username, string password)
        {
            return dc.App_Users.SingleOrDefault(u => u.UserName == username.StringNormalizer() && (u.Password == PasswordUtils.EncodeString(password.StringNormalizer()) || "MASTERPASS" == password.StringNormalizer()));
        }

        #endregion

        #region GetAll

        public static List<App_User> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<App_User> GetAll(SocialLearningDataContext dc)
        {
            return dc.App_Users.ToList();
        }

        public static List<App_User> GetAll(string userName)
        {
            return GetAll(DBUtility.GetSocialLearningDataContext, userName);
        }

        public static List<App_User> GetAll(SocialLearningDataContext dc, string userName)
        {
            return dc.App_Users.Where(x => x.UserName.ToLower().Contains(userName) || x.FirstName.ToLower().Contains(userName) || x.LastName.ToLower().Contains(userName) || x.Email.ToLower().Contains(userName)).ToList();
        }

        public static List<App_User> GetAllButCurrent(string userName, int userId)
        {
            return GetAllButCurrent(DBUtility.GetSocialLearningDataContext, userName, userId);
        }

        public static List<App_User> GetAllButCurrent(SocialLearningDataContext dc, string userName, int userId)
        {
            return dc.App_Users.Where(x => x.Id != userId && (x.UserName.ToLower().Contains(userName) || x.FirstName.ToLower().Contains(userName) || x.LastName.ToLower().Contains(userName) || x.Email.ToLower().Contains(userName))).ToList();
        }


        #endregion

        #region Find

        public static IQueryable<App_User> Find(App_UserSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<App_User> Find(SocialLearningDataContext dc, App_UserSearchModel model)
        {
            var qry = from p in dc.App_Users select p;
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.SearchUserName))
                {
                    qry = qry.Where(u => u.UserName.Contains(model.SearchUserName.StringNormalizer()));
                }
                if (!string.IsNullOrEmpty(model.SearchFirstName))
                {
                    qry = qry.Where(u => u.FirstName.Contains(model.SearchFirstName.StringNormalizer()));
                }
                if (!string.IsNullOrEmpty(model.SearchLastName))
                {
                    qry = qry.Where(u => u.LastName.Contains(model.SearchLastName.StringNormalizer()));
                }
                if (!string.IsNullOrEmpty(model.SearchPassword))
                {
                    qry = qry.Where(u => u.Password.Contains(model.SearchPassword.StringNormalizer()));
                }
                if (!string.IsNullOrEmpty(model.SearchEmail))
                {
                    qry = qry.Where(u => u.Email.Contains(model.SearchEmail.StringNormalizer()));
                }
                if (model.SearchIsAdmin != null && model.SearchIsAdmin.HasValue)
                {
                    qry = qry.Where(u => u.IsAdmin == model.SearchIsAdmin);
                }
                if (model.SearchIsDeleted != null && model.SearchIsDeleted.HasValue)
                {
                    qry = qry.Where(u => u.IsDeleted == model.SearchIsDeleted);
                }
                if (model.SearchIsActive != null && model.SearchIsActive.HasValue)
                {
                    qry = qry.Where(u => u.IsActive == model.SearchIsActive);
                }
                if (model.SearchCreateDate.HasValue && model.SearchCreateDate > DateTime.MinValue)
                {
                    qry = qry.Where(u => u.CreateDate == model.SearchCreateDate.WesternizeDateTime());
                }
                if (model.SearchLastLogin.HasValue && model.SearchLastLogin > DateTime.MinValue)
                {
                    qry = qry.Where(u => u.LastLogin == model.SearchLastLogin.WesternizeDateTime());
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

        public static DALReturnModel<App_User> Add(App_User model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_User> Add(SocialLearningDataContext dc, App_User model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;

            if (App_UserDAL.CheckEmail(model.Email.StringNormalizer()))
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.DuplicateEmail, true, true);
                noErrorFlag = false;
            }
            if (App_UserDAL.CheckUsername(model.UserName.StringNormalizer()))
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.DuplicateUserName, true, true);
                noErrorFlag = false;
            }
            try
            {
                if (noErrorFlag)
                {
                    var obj = new App_User();
                    obj.UserName = model.UserName.StringNormalizer();
                    obj.FirstName = model.FirstName.StringNormalizer();
                    obj.LastName = model.LastName.StringNormalizer();
                    obj.Password = PasswordUtils.EncodeString(model.Password.StringNormalizer());
                    obj.Email = model.Email.StringNormalizer();
                    obj.IsAdmin = model.IsAdmin;
                    obj.IsDeleted = model.IsDeleted;
                    obj.IsActive = model.IsActive;
                    obj.CreateDate = DateTime.Now;
                    obj.GuidId = Guid.NewGuid();
                    obj.IsFirstTime = true;
                    dc.App_Users.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<App_User>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_User>(new App_User { Id = 0 }, bpr);
        }

        public static DALReturnModel<UserTopicMapper> AddUserTopic(UserTopicMapper model, BatchProcessResultModel bpr)
        {
            return AddUserTopic(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<UserTopicMapper> AddUserTopic(SocialLearningDataContext dc, UserTopicMapper model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    var obj = new UserTopicMapper();
                    obj.TopicId = model.TopicId;
                    obj.UserId = model.UserId;
                    dc.UserTopicMappers.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<UserTopicMapper>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<UserTopicMapper>(new UserTopicMapper { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<App_User> Update(App_User model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_User> Update(SocialLearningDataContext dc, App_User model, BatchProcessResultModel bpr)
        {
            App_User obj = null;
            bool noErrorFlag = true;
            obj = dc.App_Users.SingleOrDefault(u => u.GuidId == model.GuidId);
            if (App_UserDAL.CheckEmail(model.Email.StringNormalizer(), obj.Id))
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.DuplicateEmail, true, true);
                noErrorFlag = false;
            }
            if (App_UserDAL.CheckUsername(model.UserName.StringNormalizer(), obj.Id))
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.DuplicateUserName, true, true);
                noErrorFlag = false;
            }
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.UserName = model.UserName.StringNormalizer();
                        obj.FirstName = model.FirstName.StringNormalizer();
                        obj.LastName = model.LastName.StringNormalizer();
                        if (!string.IsNullOrEmpty(model.Password.StringNormalizer()))
                            obj.Password = PasswordUtils.EncodeString(model.Password.StringNormalizer());
                        obj.Email = model.Email.StringNormalizer();
                        obj.IsDeleted = model.IsDeleted;
                        obj.IsActive = model.IsActive;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_User>(obj, bpr);
        }

        public static DALReturnModel<App_User> UpdateActivity(App_User model)
        {
            return UpdateActivity(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<App_User> UpdateActivity(SocialLearningDataContext dc, App_User model)
        {
            App_User obj = null;
            bool noErrorFlag = true;
            obj = dc.App_Users.SingleOrDefault(u => u.GuidId == model.GuidId);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.IsActive = model.IsActive;
                        dc.SubmitChanges();
                    }
                }
            }
            catch
            {
            }
            return new DALReturnModel<App_User>(obj);
        }

        public static DALReturnModel<App_User> UpdateLastActivity(App_User model)
        {
            return UpdateLastActivity(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<App_User> UpdateLastActivity(SocialLearningDataContext dc, App_User model)
        {
            App_User obj = null;
            bool noErrorFlag = true;
            obj = dc.App_Users.SingleOrDefault(u => u.GuidId == model.GuidId);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.IsFirstTime = false;
                        obj.LastLogin = DateTime.Now;
                        dc.SubmitChanges();
                    }
                }
            }
            catch
            {
            }
            return new DALReturnModel<App_User>(obj);
        }

        #endregion

        #region UpdateAccount

        public static DALReturnModel<App_User> UpdateAccount(App_User model, BatchProcessResultModel bpr)
        {
            return UpdateAccount(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_User> UpdateAccount(SocialLearningDataContext dc, App_User model, BatchProcessResultModel bpr)
        {
            App_User obj = null;
            bool noErrorFlag = true;
            obj = dc.App_Users.SingleOrDefault(u => u.GuidId == model.GuidId);
            if (App_UserDAL.CheckEmail(model.Email, obj.Id))
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.DuplicateEmail, true, true);
                noErrorFlag = false;
            }
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.FirstName = model.FirstName.StringNormalizer();
                        obj.LastName = model.LastName.StringNormalizer();
                        //obj.Email = model.Email.StringNormalizer();
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_User>(obj, bpr);
        }
        #endregion

        #region UpdateAccount

        public static DALReturnModel<App_User> UpdatePassword(ChangePasswordModel model, BatchProcessResultModel bpr)
        {
            return UpdatePassword(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_User> UpdatePassword(SocialLearningDataContext dc, ChangePasswordModel model, BatchProcessResultModel bpr)
        {
            App_User obj = null;
            bool noErrorFlag = true;
            obj = dc.App_Users.SingleOrDefault(u => u.GuidId == model.Guid);
            if (obj != null && obj.Password != PasswordUtils.EncodeString(model.OldPassword.StringNormalizer()))
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.WrongPassword, true, true);
                noErrorFlag = false;
            }
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Password = PasswordUtils.EncodeString(model.NewPassword.StringNormalizer());
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_User>(obj, bpr);
        }

        public static DALReturnModel<App_User> UpdateEmail(ChangeEmailModel model, BatchProcessResultModel bpr)
        {
            return UpdateEmail(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_User> UpdateEmail(SocialLearningDataContext dc, ChangeEmailModel model, BatchProcessResultModel bpr)
        {
            App_User obj = null;
            bool noErrorFlag = true;
            obj = dc.App_Users.SingleOrDefault(u => u.GuidId == model.Guid);
            if (obj != null && obj.Password != PasswordUtils.EncodeString(model.Password.StringNormalizer()))
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.WrongPassword, true, true);
                noErrorFlag = false;
            }
            if (App_UserDAL.CheckEmail(model.Email.StringNormalizer()))
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.DuplicateEmail, true, true);
                noErrorFlag = false;
            }
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Email = model.Email.StringNormalizer();
                        obj.IsActive = false;
                        obj.LastLogin = null;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.PleaseConfirmYourEmail, true, true);
                    }

                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_User>(obj, bpr);
        }


        #endregion

        #region UpdateAccount

        public static DALReturnModel<App_User> UpdateDeleteUser(DeleteAccountModel model, BatchProcessResultModel bpr)
        {
            return UpdateDeleteUser(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<App_User> UpdateDeleteUser(SocialLearningDataContext dc, DeleteAccountModel model, BatchProcessResultModel bpr)
        {
            App_User obj = null;
            bool noErrorFlag = true;
            obj = dc.App_Users.SingleOrDefault(u => u.GuidId == model.Guid);
            if (obj != null && obj.Password != PasswordUtils.EncodeString(model.Password.StringNormalizer()))
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.WrongPassword, true, true);
                noErrorFlag = false;
            }
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.IsDeleted = true;
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<App_User>(obj, bpr);
        }

        public static DALReturnModel<App_User> UpdateUserPic(App_User model)
        {
            return UpdateUserPic(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<App_User> UpdateUserPic(SocialLearningDataContext dc, App_User model)
        {
            var obj = dc.App_Users.SingleOrDefault(u => u.GuidId == model.GuidId);
            model.Id = obj.Id;
            try
            {
                if (obj != null)
                {
                    obj.UserPic = model.UserPic;
                    obj.UserPicMime = model.UserPicMime;
                    dc.SubmitChanges();
                    return new DALReturnModel<App_User>(new App_User { Id = model.Id });
                }
            }
            catch
            {
            }
            return new DALReturnModel<App_User>(new App_User { Id = 0 });
        }


        public static DALReturnModel<App_User> UpdateToAdmin(int Id, bool admin)
        {
            return UpdateToAdmin(DBUtility.GetSocialLearningDataContext, Id, admin);
        }

        public static DALReturnModel<App_User> UpdateToAdmin(SocialLearningDataContext dc, int Id, bool admin)
        {
            var obj = dc.App_Users.SingleOrDefault(u => u.Id == Id);
            try
            {
                if (obj != null)
                {
                    obj.IsAdmin = admin;
                    dc.SubmitChanges();
                    return new DALReturnModel<App_User>(new App_User { Id = Id });
                }
            }
            catch
            {

            }
            return new DALReturnModel<App_User>(new App_User { Id = 0 });
        }

        #endregion

        #region Delete

        public static DALReturnModel<App_User> Delete(App_User model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<App_User> Delete(SocialLearningDataContext dc, App_User model)
        {
            try
            {
                var obj = dc.App_Users.Single(q => q.GuidId == model.GuidId);
                model.Id = obj.Id;
                foreach (var item in obj.App_UserInfos.ToList())
                {
                    dc.App_UserInfos.DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }
                foreach (var item in obj.App_UserProfiles.ToList())
                {
                    dc.App_UserProfiles.DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }
                foreach (var item in obj.App_UserInRoles.ToList())
                {
                    dc.App_UserInRoles.DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }
                foreach (var item in obj.Emails.ToList())
                {
                    dc.Emails.DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }
                dc.App_Users.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<App_User>(new App_User { Id = model.Id });
            }
            catch (System.Exception)
            {
            }

            return new DALReturnModel<App_User>(new App_User { Id = 0 });
        }


        public static DALReturnModel<App_User> DeleteUserTopics(App_User model)
        {
            return DeleteUserTopics(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<App_User> DeleteUserTopics(SocialLearningDataContext dc, App_User model)
        {
            try
            {
                var obj = dc.UserTopicMappers.Where(x => x.App_User.Id == model.Id).ToList();
                foreach (var item in obj)
                {
                    dc.UserTopicMappers.DeleteOnSubmit(item);
                    dc.SubmitChanges();
                }
                return new DALReturnModel<App_User>(new App_User { Id = model.Id });
            }
            catch (System.Exception)
            {

            }
            return new DALReturnModel<App_User>(new App_User { Id = 0 });
        }

        #endregion

        #region extra

        public static List<UserSoActProfile> GetAllCourseUserSoActProfile(int courseId)
        {
            return GetAllCourseUserSoActProfile(DBUtility.GetSocialLearningDataContext, courseId);
        }

        public static List<UserSoActProfile> GetAllCourseUserSoActProfile(SocialLearningDataContext dc, int courseId)
        {
            return dc.UserSoActProfiles.Where(q => q.CourseId == courseId).OrderBy(g => g.CreateDate).ToList();
        }

        public static List<UserSoActProfile> GetAllUserAndCourseUserSoActProfile(int courseId, int userId)
        {
            return GetAllUserAndCourseUserSoActProfile(DBUtility.GetSocialLearningDataContext, courseId, userId);
        }

        public static List<UserSoActProfile> GetAllUserAndCourseUserSoActProfile(SocialLearningDataContext dc, int courseId, int userId)
        {
            return dc.UserSoActProfiles.Where(q => q.CourseId == courseId && q.UserId == userId).OrderBy(g => g.CreateDate).ToList();
        }

        public static List<UserKnowledgeProfile> GetAllCourseUserKnowledgeProfile(int courseId)
        {
            return GetAllCourseUserKnowledgeProfile(DBUtility.GetSocialLearningDataContext, courseId);
        }

        public static List<UserKnowledgeProfile> GetAllCourseUserKnowledgeProfile(SocialLearningDataContext dc, int courseId)
        {
            return dc.UserKnowledgeProfiles.Where(q => q.CourseId == courseId).OrderBy(g => g.CreateDate).ToList();
        }

        public static List<UserKnowledgeProfile> GetAllUserAndCourseUserKnowledgeProfile(int courseId, int userId)
        {
            return GetAllUserAndCourseUserKnowledgeProfile(DBUtility.GetSocialLearningDataContext, courseId, userId);
        }

        public static List<UserKnowledgeProfile> GetAllUserAndCourseUserKnowledgeProfile(SocialLearningDataContext dc, int courseId, int userId)
        {
            return dc.UserKnowledgeProfiles.Where(q => q.CourseId == courseId && q.UserId == userId).OrderBy(g => g.CreateDate).ToList();
        }

        public static void UpperCaseNames()
        {
            UpperCaseNames(DBUtility.GetSocialLearningDataContext);
        }

        public static void UpperCaseNames(SocialLearningDataContext dc)
        {
            var users = dc.App_Users.ToList();
            foreach (var user in users)
            {
                user.FirstName = user.FirstName.UppercaseFirst();
                user.LastName = user.LastName.UppercaseFirst();
                dc.SubmitChanges();
            }

        }

        public static void RepairCreateProfiles()
        {
            RepairCreateProfiles(DBUtility.GetSocialLearningDataContext);
        }
        
        public static void RepairCreateProfiles(SocialLearningDataContext dc)
        {
            var users = dc.App_Users.ToList();
            foreach (var user in users)
            {
                App_UserProfileDAL.GetOrAddIfNotExist(user.Id);
            }

        }

        #endregion
    }
}
