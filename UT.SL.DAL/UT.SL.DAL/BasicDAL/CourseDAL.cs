using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;
using System.Web;


namespace UT.SL.DAL
{

    public partial class CourseDAL
    {

        #region Get

        public static Course Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static Course Get(SocialLearningDataContext dc, int Id)
        {
            return dc.Courses.SingleOrDefault(u => u.Id == Id);
        }

        #endregion

        #region GetAll

        public static List<Course> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }

        public static List<Course> GetAll(SocialLearningDataContext dc)
        {
            return dc.Courses.ToList();
        }

        public static List<Course> GetAll(int userId)
        {
            return GetAll(DBUtility.GetSocialLearningDataContext, userId);
        }

        public static List<Course> GetAll(SocialLearningDataContext dc, int userId)
        {
            return dc.Courses.Where(x => x.App_UserEnrolements.Any(p => p.UserId == userId)).ToList();
        }

        public static List<Course> GetAllNotForThisUser(int userId)
        {
            return GetAllNotForThisUser(DBUtility.GetSocialLearningDataContext, userId);
        }

        public static List<Course> GetAllNotForThisUser(SocialLearningDataContext dc, int userId)
        {
            return dc.Courses.Where(x => x.App_UserEnrolements.All(p => p.UserId != userId)).ToList();
        }

        public static List<Course> GetAllWithCategory(int id)
        {
            return GetAllWithCategory(DBUtility.GetSocialLearningDataContext, id);
        }

        public static List<Course> GetAllWithCategory(SocialLearningDataContext dc, int id)
        {
            return dc.Courses.Where(x => x.CategoryMappers.Any(p => p.CategoryId == id)).ToList();
        }

        public static List<Resource> GetAllResources(int id)
        {
            return GetAllResources(DBUtility.GetSocialLearningDataContext, id);
        }

        public static List<Resource> GetAllResources(SocialLearningDataContext dc, int id)
        {
            return dc.Courses.Single(x => x.Id == id).Resources.Where(x => x.IsValid && x.IsPublishd).ToList();
        }

        public static int GetActivitesCount(int id)
        {
            return GetActivitesCount(DBUtility.GetSocialLearningDataContext, id);
        }

        public static int GetActivitesCount(SocialLearningDataContext dc, int id)
        {
            return dc.Courses.Single(x => x.Id == id).Forums.Count()
                + dc.Courses.Single(x => x.Id == id).Assignments.Count();
        }

        public static int GetForumCount(int id)
        {
            return GetForumCount(DBUtility.GetSocialLearningDataContext, id);
        }

        public static int GetForumCount(SocialLearningDataContext dc, int id)
        {
            return dc.Courses.Single(x => x.Id == id).Forums.Count();
        }

        public static int GetAssigmentCount(int id)
        {
            return GetAssigmentCount(DBUtility.GetSocialLearningDataContext, id);
        }

        public static int GetAssigmentCount(SocialLearningDataContext dc, int id)
        {
            return dc.Courses.Single(x => x.Id == id).Assignments.Count();
        }

        public static int GetLearningGroupsCount(int id)
        {
            return GetLearningGroupsCount(DBUtility.GetSocialLearningDataContext, id);
        }

        public static int GetLearningGroupsCount(SocialLearningDataContext dc, int id)
        {
            return dc.Courses.Single(x => x.Id == id).LearningGroups.Count();
        }

        public static List<ObjectStreamCourse> GetAllMaterials(int id)
        {
            return GetAllMaterials(DBUtility.GetSocialLearningDataContext, id);
        }

        public static List<ObjectStreamCourse> GetAllMaterials(SocialLearningDataContext dc, int id)
        {
            return dc.ObjectStreamCourses.Where(x => x.CourseId == id).ToList();
        }

        #endregion

        #region Find

        public static IQueryable<ObjectViewModel> FindActivities(CourseSearchModel model)
        {
            return FindActivities(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<ObjectViewModel> FindActivities(SocialLearningDataContext dc, CourseSearchModel model)
        {
            var forums = from p in dc.Forums.Where(x => x.CourseId == model.CourseId) select p;
            var assignments = from p in dc.Assignments.Where(x => x.CourseId == model.CourseId) select p;
            var objectList = new List<ObjectViewModel>();
            foreach (var item in forums)
            {
                objectList.Add(new ObjectViewModel
                {
                    Id = item.Id,
                    CreateUser = item.App_User,
                    CreateDate = item.CreateDate ?? DateTime.Now,
                    Title = item.Title,
                    Body = item.Body,
                    FileTitle = string.Empty,
                    Type = (int)UT.SL.Model.Enumeration.ObjectType.Forum,
                    AAssignmentScore = item.ForumDiscussions.Where(x => !x.ParentId.HasValue).Count()
                });
            }
            foreach (var item in assignments)
            {
                objectList.Add(new ObjectViewModel
                {
                    Id = item.Id,
                    CreateUser = item.App_User,
                    CreateDate = item.CreateDate,
                    Title = item.Title,
                    Body = item.Body,
                    FileTitle = string.Empty,
                    Type = (int)UT.SL.Model.Enumeration.ObjectType.Assignment,
                    AAssignmentScore = item.AssignmentSubmissions.Count
                });
            }
            objectList = objectList.OrderByDescending(u => u.CreateDate).ToList();
            return objectList.AsQueryable();
        }

        public static IQueryable<Course> Find(CourseSearchModel model)
        {
            return Find(DBUtility.GetSocialLearningDataContext, model);
        }

        public static IQueryable<Course> Find(SocialLearningDataContext dc, CourseSearchModel model)
        {
            var qry = from p in dc.Courses select p;
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.SearchTitle))
                {
                    qry = qry.Where(u => u.Title.Contains(model.SearchTitle.StringNormalizer()));
                }
                if (!string.IsNullOrEmpty(model.SearchAbout))
                {
                    qry = qry.Where(u => u.About.Contains(model.SearchAbout.StringNormalizer()));
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

        public static DALReturnModel<Course> Add(Course model, BatchProcessResultModel bpr)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Course> Add(SocialLearningDataContext dc, Course model, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            if (model.StartDate >= model.EndDate)
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.BprDateSoon, true, true);
                noErrorFlag = false;
            }
            try
            {
                if (noErrorFlag)
                {
                    var obj = new Course();
                    obj.Title = model.Title.StringNormalizer();
                    obj.UniversityTitle = model.UniversityTitle.StringNormalizer();
                    obj.About = model.About;
                    obj.StartDate = model.StartDate.WesternizeDateTime();
                    obj.EndDate = model.EndDate.WesternizeDateTime();
                    obj.CourseAbstractId = model.CourseAbstractId;
                    dc.Courses.InsertOnSubmit(obj);
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<Course>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Course>(new Course { Id = 0 }, bpr);
        }

        #endregion

        #region Update

        public static DALReturnModel<Course> Update(Course model, BatchProcessResultModel bpr)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, bpr);
        }

        public static DALReturnModel<Course> Update(SocialLearningDataContext dc, Course model, BatchProcessResultModel bpr)
        {
            Course obj = null;
            bool noErrorFlag = true;
            if (model.StartDate >= model.EndDate)
            {
                bpr.AddError(UT.SL.Model.Resource.App_Errors.BprDateSoon, true, true);
                noErrorFlag = false;
            }
            obj = dc.Courses.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.Title = model.Title.StringNormalizer();
                        obj.UniversityTitle = model.UniversityTitle.StringNormalizer();
                        obj.About = model.About;
                        obj.StartDate = model.StartDate.WesternizeDateTime();
                        obj.EndDate = model.EndDate.WesternizeDateTime();
                        dc.SubmitChanges();
                        bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Course>(obj, bpr);
        }

        public static DALReturnModel<Course> SaveImage(int Id, HttpPostedFileBase image, BatchProcessResultModel bpr)
        {
            return SaveImage(DBUtility.GetSocialLearningDataContext, Id, image, bpr);
        }

        public static DALReturnModel<Course> SaveImage(SocialLearningDataContext dc, int Id, HttpPostedFileBase image, BatchProcessResultModel bpr)
        {
            bool noErrorFlag = true;
            var obj = dc.Courses.SingleOrDefault(u => u.Id == Id);
            try
            {
                if (noErrorFlag)
                {
                    if (image != null)
                    {
                        byte[] tempFile = null;
                        tempFile = new byte[image.ContentLength];
                        image.InputStream.Read(tempFile, 0, image.ContentLength);
                        obj.ImageData = tempFile;
                        obj.ImageMIME = image.ContentType;
                    }
                    dc.SubmitChanges();
                    bpr.AddSuccess(UT.SL.Model.Resource.App_Common.BprAddSuccess, true, true);
                    return new DALReturnModel<Course>(obj, bpr);
                }
            }
            catch (System.Exception ex)
            {
                bpr.AddError(ex.Message, true, true);
            }
            return new DALReturnModel<Course>(new Course { Id = 0 }, bpr);
        }


        #endregion

        #region Delete

        public static DALReturnModel<Course> Delete(Course model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Course> Delete(SocialLearningDataContext dc, Course model)
        {
            try
            {
                var obj = dc.Courses.Single(q => q.Id == model.Id);                
                dc.Courses.DeleteOnSubmit(obj);
                dc.SubmitChanges();
                return new DALReturnModel<Course>(new Course { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Course>(new Course { Id = 0 });
            }
        }

        public static DALReturnModel<Course> DeletePic(Course model)
        {
            return DeletePic(DBUtility.GetSocialLearningDataContext, model);
        }

        public static DALReturnModel<Course> DeletePic(SocialLearningDataContext dc, Course model)
        {
            try
            {
                var obj = dc.Courses.Single(q => q.Id == model.Id);
                obj.ImageData = null;
                obj.ImageMIME = string.Empty;
                dc.SubmitChanges();
                return new DALReturnModel<Course>(new Course { Id = model.Id });
            }
            catch (System.Exception)
            {
                return new DALReturnModel<Course>(new Course { Id = 0 });
            }
        }

       

        #endregion
    }
}
