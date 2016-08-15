using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.Helper;
using UT.SL.Model;
using UT.SL.Data.LINQ;



namespace UT.SL.DAL
{


    public partial class dddForumDiscussionPostGradeDAL
    {

        #region Get

        public static ForumDiscussionPostsGrade Get(SocialLearningDataContext dc, int Id)
        {
            return dc.ForumDiscussionPostsGrades.SingleOrDefault(u => u.Id == Id);
        }

        public static ForumDiscussionPostsGrade Get(int Id)
        {
            return Get(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static ForumDiscussionPostsGrade GetWithDiscussion(SocialLearningDataContext dc, int Id)
        {
            return dc.ForumDiscussionPostsGrades.SingleOrDefault(u => u.discussionId == Id);
        }

        public static ForumDiscussionPostsGrade GetWithDiscussion(int Id)
        {
            return GetWithDiscussion(DBUtility.GetSocialLearningDataContext, Id);
        }

        public static ForumDiscussionPostsGrade GetWithDiscussionReply(SocialLearningDataContext dc, int Id)
        {
            return dc.ForumDiscussionPostsGrades.SingleOrDefault(u => u.postId == Id);
        }

        public static ForumDiscussionPostsGrade GetWithDiscussionReply(int Id)
        {
            return GetWithDiscussionReply(DBUtility.GetSocialLearningDataContext, Id);
        }

        #endregion

        #region GetAll

        public static List<ForumDiscussionPostsGrade> GetAll(SocialLearningDataContext dc)
        {
            return dc.ForumDiscussionPostsGrades.ToList();

        }

        public static List<ForumDiscussionPostsGrade> GetAll()
        {
            return GetAll(DBUtility.GetSocialLearningDataContext);
        }
        #endregion

        #region Add

        public static ForumDiscussionPostsGrade Add(ForumDiscussionPostsGrade model)
        {
            return Add(DBUtility.GetSocialLearningDataContext, model);
        }

        public static ForumDiscussionPostsGrade Add(SocialLearningDataContext dc, ForumDiscussionPostsGrade model)
        {

            bool noErrorFlag = true;
            try
            {
                if (noErrorFlag)
                {
                    dc.ForumDiscussionPostsGrades.InsertOnSubmit(model);
                    dc.SubmitChanges();
                    return model;
                }
            }
            catch
            {

            }
            return null;
        }
        #endregion

        #region Update

        public static ForumDiscussionPostsGrade Update(ForumDiscussionPostsGrade model, bool InsertIfNotExist = false)
        {
            return Update(DBUtility.GetSocialLearningDataContext, model, InsertIfNotExist);
        }


        public static ForumDiscussionPostsGrade Update(SocialLearningDataContext dc, ForumDiscussionPostsGrade model, bool InsertIfNotExist = false)
        {
            ForumDiscussionPostsGrade obj = null;
            bool noErrorFlag = true;
            obj = dc.ForumDiscussionPostsGrades.SingleOrDefault(u => u.Id == model.Id);
            try
            {
                if (noErrorFlag)
                {
                    if (obj != null)
                    {
                        obj.grade = model.grade;
                        dc.SubmitChanges();
                    }
                    else
                    {
                        if (InsertIfNotExist)
                        {
                            dc.ForumDiscussionPostsGrades.InsertOnSubmit(obj);
                            dc.SubmitChanges();
                        }
                    }
                }
            }
            catch
            {
            }
            return obj;
        }
        #endregion

        #region Delete

        public static bool Delete(ForumDiscussionPostsGrade model)
        {
            return Delete(DBUtility.GetSocialLearningDataContext, model);
        }


        public static bool Delete(SocialLearningDataContext dc, ForumDiscussionPostsGrade model)
        {
            try
            {
                var obj = dc.ForumDiscussionPostsGrades.Single(q => q.Id == model.Id);
                dc.ForumDiscussionPostsGrades.DeleteOnSubmit(obj);
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
