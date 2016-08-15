/*
 * ****************************************************************
 * Filename:        ManageObject.cs 
 * version:         
 * Author's name:   Fatemeh orooji, Pezhman Nasirifard, Elearning lab 
 * Creation date:   
 * Purpose:         
 * ****************************************************************
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UT.SL.DAL;
using UT.SL.Data.LINQ;
using UT.SL.Helper;
using UT.SL.Model;
using System.Web.Mvc;
using System.Web;
using UT.SL.Model.Enumeration;

namespace UT.SL.BLL
{
    /// <summary>
    /// conseptual management of database tables to use in social capabilities of system
    /// it provide social useability for all objects in the system
    /// </summary>
    public static class ManageObject
    {

        /// <summary>
        /// provide ObjectViewModel of the item
        /// </summary>
        /// <param name="objectId">id of the item</param>
        /// <param name="type">type of the item</param>
        /// <returns>ObjectViewModel of the item</returns>
        public static ObjectViewModel GetSharedObject(int objectId, int type)
        {
            var objectModel = new ObjectModel();
            var res = new ObjectViewModel();
            try
            {
                if (type == (int)ObjectType.Resource)
                {
                    var resourceDB = ResourceDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = (int)ObjectType.Resource,
                            Body = resourceDB.Body,
                            Title = resourceDB.Title,
                            CreateDate = resourceDB.CreateDate,
                            CreateUser = resourceDB.App_User,
                            FileTitle = resourceDB.FileTitle,
                            FileMime = resourceDB.FileMime,
                            BadgetTitle = "DF"
                        };
                        if (resourceDB.CourseId.HasValue)
                        {
                            res.CourseId = resourceDB.CourseId.Value;
                            res.CourseTitle = resourceDB.Course.Title;
                            res.CameFromId = resourceDB.CourseId.Value;
                            res.CameFromTitle = resourceDB.Course.Title;
                        }
                        if (resourceDB.FileContent != null)
                        {
                            res.FileContent = resourceDB.FileContent.ToArray();
                        }
                    }
                }
                else if (type == (int)ObjectType.Assignment)
                {
                    var resourceDB = AssignmentDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = (int)ObjectType.Assignment,
                            Body = resourceDB.Body,
                            Title = resourceDB.Title,
                            CreateDate = resourceDB.CreateDate,
                            CreateUser = resourceDB.App_User,
                            FileTitle = resourceDB.FileTitle,
                            FileMime = resourceDB.FileMime,
                            //IsWide = true
                        };
                        res.CourseId = resourceDB.CourseId;
                        res.CourseTitle = resourceDB.Course.Title;
                        res.CameFromId = resourceDB.CourseId;
                        res.CameFromTitle = resourceDB.Course.Title;
                        if (resourceDB.FileContent != null)
                        {
                            res.FileContent = resourceDB.FileContent.ToArray();
                        }
                    }
                }
                else if (type == (int)ObjectType.AssignmentSubmission)
                {
                    var resourceDB = AssignmentSubmissionDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = (int)ObjectType.AssignmentSubmission,
                            CreateDate = resourceDB.CreateDate,
                            CreateUser = resourceDB.App_User,
                            FileTitle = resourceDB.FileTitle,
                            FileMime = resourceDB.FileMime,
                            Body = resourceDB.Body,
                            //IsWide = true

                        };
                        res.CourseId = resourceDB.Assignment.CourseId;
                        res.CourseTitle = resourceDB.Assignment.Course.Title;
                        res.CameFromId = resourceDB.Assignment.Id;
                        res.CameFromTitle = resourceDB.Assignment.Title;
                        res.CameFromType = (int)ObjectType.Assignment;
                        if (resourceDB.FileContent != null)
                        {
                            res.FileContent = resourceDB.FileContent.ToArray();
                        }
                    }
                }
                else if (type == (int)ObjectType.Forum)
                {
                    var resourceDB = ForumDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = (int)ObjectType.Forum,
                            Body = resourceDB.Body,
                            Title = resourceDB.Title,
                            CreateDate = resourceDB.CreateDate.Value,
                            CreateUser = resourceDB.App_User,
                            FileTitle = resourceDB.FileTitle,
                            FileMime = resourceDB.FileMime,
                            //IsWide = true
                        };
                        res.CourseId = resourceDB.CourseId;
                        res.CourseTitle = resourceDB.Course.Title;
                        res.CameFromId = resourceDB.CourseId;
                        res.CameFromTitle = resourceDB.Course.Title;
                        if (resourceDB.FileContent != null)
                        {
                            res.FileContent = resourceDB.FileContent.ToArray();
                        }
                    }
                }
                else if (type == (int)ObjectType.ForumDiscussion)
                {
                    var resourceDB = ForumDiscussionDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = (int)ObjectType.ForumDiscussion,
                            Body = resourceDB.Body,
                            Title = resourceDB.Title,
                            CreateDate = resourceDB.CreateDate.Value,
                            CreateUser = resourceDB.App_User,
                            FileTitle = resourceDB.FileTitle,
                            FileMime = resourceDB.FileMime,
                            //IsWide = true
                        };
                        res.CourseId = resourceDB.Forum.CourseId;
                        res.CourseTitle = resourceDB.Forum.Course.Title;

                        res.CameFromId = resourceDB.Forum.Id;
                        if (resourceDB.ParentId.HasValue)
                        {
                            var prentDiscussion = ForumDiscussionDAL.Get(resourceDB.ParentId.Value);
                            res.CameFromTitle = prentDiscussion.Title;
                            if (string.IsNullOrEmpty(res.CameFromTitle))
                                res.CameFromTitle = prentDiscussion.Body;
                            if (string.IsNullOrEmpty(res.CameFromTitle))
                                res.CameFromTitle = prentDiscussion.FileTitle;
                            if (string.IsNullOrEmpty(res.CameFromTitle))
                                res.CameFromTitle = resourceDB.Forum.Title;
                            res.CameFromType = (int)ObjectType.ForumDiscussion;
                        }
                        else
                        {
                            var forum = ForumDAL.Get(resourceDB.ForumId);
                            res.CameFromTitle = forum.Title;
                            if (string.IsNullOrEmpty(res.CameFromTitle))
                                res.CameFromTitle = forum.Body;
                            if (string.IsNullOrEmpty(res.CameFromTitle))
                                res.CameFromTitle = forum.FileTitle;
                            if (string.IsNullOrEmpty(res.CameFromTitle))
                                res.CameFromTitle = resourceDB.Forum.Title;
                            res.CameFromType = (int)ObjectType.Forum;
                        }
                        res.ExtraInfo = resourceDB.Forum.Title;
                        if (string.IsNullOrEmpty(res.ExtraInfo))
                            res.ExtraInfo = resourceDB.Forum.Body;
                        if (string.IsNullOrEmpty(res.ExtraInfo))
                            res.ExtraInfo = resourceDB.Forum.FileTitle;
                        if (resourceDB.FileContent != null)
                        {
                            res.FileContent = resourceDB.FileContent.ToArray();
                        }
                    }
                }
                else if (type == (int)ObjectType.Comment)
                {
                    var resourceDB = CommentDAL.Get(objectId);
                    var courseId = 0;
                    var cameFromTitle = string.Empty;
                    var cameFromUser = new App_User();
                    var courseTitle = string.Empty;
                    if (resourceDB.Type != type)
                    {
                        var getObject = GetSharedObject(resourceDB.ObjectId, resourceDB.Type);
                        courseId = getObject.CourseId ?? 0;
                        if (getObject.CourseId.HasValue)
                            courseTitle = getObject.CourseTitle;
                        cameFromTitle = getObject.Title;
                        cameFromTitle = getObject.Title;
                        if (string.IsNullOrEmpty(cameFromTitle))
                            cameFromTitle = getObject.Body;
                        if (string.IsNullOrEmpty(cameFromTitle))
                            cameFromTitle = getObject.FileTitle;
                        cameFromUser = getObject.CreateUser;
                    }
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = (int)ObjectType.Comment,
                            Title = resourceDB.Title,
                            CreateDate = resourceDB.CreateDate.Value,
                            CreateUser = resourceDB.App_User,
                        };
                        res.CourseId = courseId;
                        res.CourseTitle = courseTitle;
                        res.CameFromId = resourceDB.ObjectId;
                        res.CameFromType = resourceDB.Type;
                        res.CameFromTitle = cameFromTitle;
                        res.CameFromUser = cameFromUser;
                    }
                }
                else if (type == (int)ObjectType.Vote)
                {
                    var resourceDB = VoteDAL.Get(objectId);
                    var courseId = 0;
                    var cameFromTitle = string.Empty;
                    var cameFromUser = new App_User();
                    var courseTitle = string.Empty;

                    if (resourceDB.VoteParent.ObjectType != type)
                    {
                        var getObject = GetSharedObject(resourceDB.VoteParent.ObjectId, resourceDB.VoteParent.ObjectType.Value);
                        courseId = getObject.CourseId ?? 0;
                        if (getObject.CourseId.HasValue)
                            courseTitle = getObject.CourseTitle;
                        cameFromTitle = getObject.Title;
                        cameFromTitle = getObject.Title;
                        if (string.IsNullOrEmpty(cameFromTitle))
                            cameFromTitle = getObject.Body;
                        if (string.IsNullOrEmpty(cameFromTitle))
                            cameFromTitle = getObject.FileTitle;
                        cameFromUser = getObject.CreateUser;
                    }
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = (int)ObjectType.Vote,
                            Title = resourceDB.Updown == true ? "UpVote" : string.Empty,
                            CreateDate = resourceDB.CreateDate.Value,
                            CreateUser = resourceDB.App_User,
                        };
                        res.CourseId = courseId;
                        res.CourseTitle = courseTitle;

                        res.CameFromId = resourceDB.VoteParent.ObjectId;
                        res.CameFromType = resourceDB.VoteParent.ObjectType.Value;
                        res.CameFromTitle = cameFromTitle;
                        res.CameFromUser = cameFromUser;
                    }
                }
                else if (type == (int)ObjectType.VoteParent)
                {
                    var resourceDB = VoteParentDAL.Get(objectId);
                    var courseId = 0;
                    var cameFromTitle = string.Empty;
                    var courseTitle = string.Empty;

                    if (resourceDB.ObjectType != type)
                    {
                        var getObject = GetSharedObject(resourceDB.ObjectId, resourceDB.ObjectType.Value);
                        courseId = getObject.CourseId ?? 0;
                        if (getObject.CourseId.HasValue)
                            courseTitle = getObject.CourseTitle;
                        cameFromTitle = getObject.Title;
                        if (string.IsNullOrEmpty(cameFromTitle))
                            cameFromTitle = getObject.Body;
                        if (string.IsNullOrEmpty(cameFromTitle))
                            cameFromTitle = getObject.FileTitle;
                    }
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = (int)ObjectType.VoteParent,
                            Title = string.Empty,
                        };
                        res.CourseId = courseId;
                        res.CourseTitle = courseTitle;

                        res.CameFromId = resourceDB.ObjectId;
                        res.CameFromType = resourceDB.ObjectType.Value;
                        res.CameFromTitle = cameFromTitle;
                    }
                }
                else if (type == (int)ObjectType.TagMapper)
                {
                    var resourceDB = TagMapperDAL.Get(objectId);
                    var courseId = 0;
                    var cameFromTitle = string.Empty;
                    var cameFromUser = new App_User();
                    var courseTitle = string.Empty;

                    if (resourceDB.ObjectType != type)
                    {
                        var getObject = GetSharedObject(resourceDB.ObjectId, resourceDB.ObjectType);
                        courseId = getObject.CourseId ?? 0;
                        if (getObject.CourseId.HasValue)
                            courseTitle = getObject.CourseTitle;
                        cameFromTitle = getObject.Title;
                        cameFromTitle = getObject.Title;
                        if (string.IsNullOrEmpty(cameFromTitle))
                            cameFromTitle = getObject.Body;
                        if (string.IsNullOrEmpty(cameFromTitle))
                            cameFromTitle = getObject.FileTitle;
                        cameFromUser = getObject.CreateUser;
                    }
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = (int)ObjectType.TagMapper,
                            Title = resourceDB.Tag.Title,
                            CreateUser = resourceDB.App_User,
                            CreateDate = resourceDB.CreateDate.Value
                        };
                        res.CourseId = courseId;
                        res.CourseTitle = courseTitle;

                        res.CameFromId = resourceDB.ObjectId;
                        res.CameFromType = resourceDB.ObjectType;
                        res.CameFromTitle = cameFromTitle;
                        res.CameFromUser = cameFromUser;
                    }
                }
                else if (type == (int)ObjectType.Grade)
                {
                    var resourceDB = GradeDAL.Get(objectId);
                    var courseId = 0;
                    var cameFromTitle = string.Empty;
                    var cameFromUser = new App_User();
                    var courseTitle = string.Empty;

                    if (resourceDB.ObjectType != type)
                    {
                        var getObject = GetSharedObject(resourceDB.ObjectId.Value, resourceDB.ObjectType.Value);
                        courseId = getObject.CourseId ?? 0;
                        if (getObject.CourseId.HasValue)
                            courseTitle = getObject.CourseTitle;
                        cameFromTitle = getObject.Title;
                        cameFromTitle = getObject.Title;
                        if (string.IsNullOrEmpty(cameFromTitle))
                            cameFromTitle = getObject.Body;
                        if (string.IsNullOrEmpty(cameFromTitle))
                            cameFromTitle = getObject.FileTitle;
                        cameFromUser = getObject.CreateUser;
                    }
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = (int)ObjectType.Grade,
                            CreateUser = resourceDB.App_User1,
                            CreateDate = resourceDB.CreateDate,
                            Title = resourceDB.GradeValue.ToString(),
                            GradeUser = resourceDB.App_User
                        };
                        res.CourseId = courseId;
                        res.CourseTitle = courseTitle;

                        res.CameFromId = resourceDB.ObjectId.Value;
                        res.CameFromType = resourceDB.ObjectType.Value;
                        res.CameFromTitle = cameFromTitle;
                        res.CameFromUser = cameFromUser;
                    }
                }
                else if (type == (int)ObjectType.ObjectStream)
                {
                    var resourceDB = ObjectStreamDAL.Get(objectId);
                    var courseId = 0;
                    var cameFromTitle = string.Empty;
                    var cameFromUser = new App_User();
                    var courseTitle = string.Empty;

                    if (resourceDB.ObjectType != type)
                    {
                        var getObject = GetSharedObject(resourceDB.ObjectId, resourceDB.ObjectType);
                        courseId = getObject.CourseId ?? 0;
                        if (getObject.CourseId.HasValue)
                            courseTitle = getObject.CourseTitle;
                        cameFromTitle = getObject.Title;
                        cameFromTitle = getObject.Title;
                        if (string.IsNullOrEmpty(cameFromTitle))
                            cameFromTitle = getObject.Body;
                        if (string.IsNullOrEmpty(cameFromTitle))
                            cameFromTitle = getObject.FileTitle;
                        cameFromUser = getObject.CreateUser;
                    }
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = (int)ObjectType.ObjectStream,
                            CreateUser = resourceDB.App_User,
                            CreateDate = resourceDB.CreateDate
                        };
                        res.CourseId = courseId;
                        res.CourseTitle = courseTitle;

                        res.CameFromId = resourceDB.ObjectId;
                        res.CameFromType = resourceDB.ObjectType;
                        res.CameFromTitle = cameFromTitle;
                        res.CameFromUser = cameFromUser;
                    }
                }
                else if (type == (int)ObjectType.Course)
                {
                    var resourceDB = CourseDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = (int)ObjectType.Course,
                            Title = resourceDB.Title,
                        };
                        res.CourseId = resourceDB.Id;
                        res.CourseTitle = resourceDB.Title;

                        res.CameFromId = resourceDB.CourseAbstractId.Value;
                    }
                }
                else if (type == (int)ObjectType.User || type == (int)ObjectType.App_User)
                {
                    var resourceDB = App_UserDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = (int)ObjectType.Course,
                            Title = resourceDB.FirstName + " " + resourceDB.LastName,
                        };
                    }
                }
                else if (type == (int)ObjectType.ImageAnnotation)
                {
                    var resourceDB = ImageAnnotationDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = (int)ObjectType.ImageAnnotation,
                        };
                        res.CameFromId = resourceDB.ObjectId;
                        res.CameFromType = resourceDB.ObjectType;
                    }
                }
                else if (type == (int)ObjectType.Category)
                {
                    var resourceDB = CategoryDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = (int)ObjectType.Category,
                            Title = resourceDB.Title
                        };
                        if (resourceDB.ParentId.HasValue)
                            res.CameFromId = resourceDB.ParentId.Value;
                        res.CameFromType = (int)ObjectType.Category;
                    }
                }
                else if (type == (int)ObjectType.SocialGroup)
                {
                    var resourceDB = SocialGroupDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = (int)ObjectType.SocialGroup,
                        };
                    }
                }
                else if (type == (int)ObjectType.Share)
                {
                    var resourceDB = ShareDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = (int)ObjectType.Share,
                        };
                        res.CameFromId = resourceDB.ObjectId;
                        if (resourceDB.Type.HasValue)
                            res.CameFromType = resourceDB.Type.Value;
                    }
                }
                else if (type == (int)ObjectType.LearningGroup)
                {
                    var resourceDB = LearningGroupDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = (int)ObjectType.LearningGroup,
                        };
                        res.CourseId = resourceDB.CourseId;
                        res.CourseTitle = resourceDB.Course.Title;

                    }
                }
                else if (type == (int)ObjectType.Feedback)
                {
                    var resourceDB = FeedbackDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = (int)ObjectType.Feedback,
                            Title = resourceDB.PageURL
                        };
                    }
                }
                else if (type == (int)ObjectType.Email)
                {
                    var resourceDB = EmailDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = (int)ObjectType.Email,
                            Title = resourceDB.Subject,
                            Body = resourceDB.Body
                        };
                    }
                }
                else if (type == (int)ObjectType.Survey)
                {
                    var resourceDB = SurveyDAL.GetSurvey(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = (int)ObjectType.Survey,
                        };
                    }
                }
                else if (type == (int)ObjectType.UserEnrolement || type == (int)ObjectType.App_UserEnrolement)
                {
                    var resourceDB = App_UserEnrolementDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                    }
                }
                else if (type == (int)ObjectType.App_Action)
                {
                    var resourceDB = App_ActionDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                    }
                }
                else if (type == (int)ObjectType.App_UserInRole)
                {
                    var resourceDB = App_UserInRoleDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                    }
                }
                else if (type == (int)ObjectType.App_UserInfo)
                {
                    var resourceDB = App_UserInfoDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                    }
                }
                else if (type == (int)ObjectType.UserTopicMapper)
                {

                }
                else if (type == (int)ObjectType.App_ActionEvaulation)
                {
                    var resourceDB = App_ActionEvaulationDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                    }
                }
                else if (type == (int)ObjectType.App_Permission)
                {
                    var resourceDB = App_PermissionDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                    }
                }
                else if (type == (int)ObjectType.AssessParent)
                {
                    var resourceDB = AssessParentDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                        res.CameFromId = resourceDB.ObjectId;
                        res.CameFromType = resourceDB.ObjectType;
                    }
                }
                else if (type == (int)ObjectType.Assess)
                {
                    var resourceDB = AssessDAL.Get(objectId);
                    var cameFromTitle = string.Empty;
                    var cameFromUser = new App_User();
                    var courseTitle = string.Empty;
                    var courseId = 0;

                    if (resourceDB.AssessParent.ObjectType != type)
                    {
                        var getObject = GetSharedObject(resourceDB.AssessParent.ObjectId, resourceDB.AssessParent.ObjectType);
                        courseId = getObject.CourseId ?? 0;
                        if (getObject.CourseId.HasValue)
                            courseTitle = getObject.CourseTitle;
                        cameFromTitle = getObject.Title;
                        cameFromTitle = getObject.Title;
                        if (string.IsNullOrEmpty(cameFromTitle))
                            cameFromTitle = getObject.Body;
                        if (string.IsNullOrEmpty(cameFromTitle))
                            cameFromTitle = getObject.FileTitle;
                        cameFromUser = getObject.CreateUser;
                    }
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                        res.CameFromId = resourceDB.ParentId;
                        res.CameFromType = (int)ObjectType.AssessParent;

                        res.CourseId = courseId;
                        res.CourseTitle = courseTitle;

                        res.CameFromId = resourceDB.AssessParent.ObjectId;
                        res.CameFromType = resourceDB.AssessParent.ObjectType;
                        res.CameFromTitle = cameFromTitle;
                        res.CameFromUser = cameFromUser;
                    }
                }
                else if (type == (int)ObjectType.ObjectTopicMapper)
                {
                    var resourceDB = ObjectTopicMapperDAL.Get(objectId);
                    var courseId = 0;
                    var cameFromTitle = string.Empty;
                    var cameFromUser = new App_User();
                    var courseTitle = string.Empty;

                    if (resourceDB.ObjectType != type)
                    {
                        var getObject = GetSharedObject(resourceDB.ObjectId, resourceDB.ObjectType);
                        courseId = getObject.CourseId ?? 0;
                        if (getObject.CourseId.HasValue)
                            courseTitle = getObject.CourseTitle;
                        cameFromTitle = getObject.Title;
                        cameFromTitle = getObject.Title;
                        if (string.IsNullOrEmpty(cameFromTitle))
                            cameFromTitle = getObject.Body;
                        if (string.IsNullOrEmpty(cameFromTitle))
                            cameFromTitle = getObject.FileTitle;
                        cameFromUser = getObject.CreateUser;
                    }
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };

                        res.CourseId = courseId;
                        res.CourseTitle = courseTitle;

                        res.CameFromId = resourceDB.ObjectId;
                        res.CameFromType = resourceDB.ObjectType;
                        res.CameFromTitle = cameFromTitle;
                        res.CameFromUser = cameFromUser;
                    }
                }
                else if (type == (int)ObjectType.CourseAbstract)
                {
                    var resourceDB = CourseAbstractDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                    }
                }
                else if (type == (int)ObjectType.Topic)
                {
                    var resourceDB = TopicDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                    }
                }
                else if (type == (int)ObjectType.CourseTopcMapper)
                {

                }
                else if (type == (int)ObjectType.ImageAnnotation)
                {
                    var resourceDB = ImageAnnotationDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                        res.CameFromId = resourceDB.ObjectId;
                        res.CameFromType = resourceDB.ObjectType;
                    }
                }
                else if (type == (int)ObjectType.CourseSchedule)
                {
                    var resourceDB = CourseScheduleDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                        res.CourseId = resourceDB.CourseId;
                        res.CourseTitle = resourceDB.Course.Title;
                    }
                }
                else if (type == (int)ObjectType.CategoryMapper)
                {
                    var resourceDB = CategoryMapperDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                    }
                }
                else if (type == (int)ObjectType.Answer)
                {

                }
                else if (type == (int)ObjectType.Quiz)
                {

                }
                else if (type == (int)ObjectType.Wiki)
                {

                }
                else if (type == (int)ObjectType.GroupMember)
                {
                    var resourceDB = GroupMemberDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                        if (resourceDB.SocialGroupId.HasValue)
                        {
                            res.CameFromId = resourceDB.SocialGroupId.Value;
                            res.CameFromType = (int)ObjectType.SocialGroup;
                        }
                        if (resourceDB.LearningGroupId.HasValue)
                        {
                            res.CameFromId = resourceDB.LearningGroupId.Value;
                            res.CameFromType = (int)ObjectType.LearningGroup;
                        }

                    }
                }
                else if (type == (int)ObjectType.SurveyAnswer)
                {
                    var resourceDB = SurveyDAL.GetSurveyAnswer(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                        res.CameFromId = resourceDB.SurveyId.Value;
                        res.CameFromType = (int)ObjectType.Survey;
                    }
                }
                else if (type == (int)ObjectType.SurveyUserSummary)
                {
                    var resourceDB = SurveyDAL.GetSurveySummary(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                        res.CameFromId = resourceDB.SurveyId.Value;
                        res.CameFromType = (int)ObjectType.Survey;
                    }
                }
                else if (type == (int)ObjectType.ObjectStreamGroupMapper)
                {
                    var resourceDB = ObjectStreamGroupMapperDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                        if (resourceDB.SocialGroupId.HasValue)
                        {
                            res.CameFromId = resourceDB.SocialGroupId.Value;
                            res.CameFromType = (int)ObjectType.SocialGroup;
                        }
                        if (resourceDB.LearningGroupId.HasValue)
                        {
                            res.CameFromId = resourceDB.LearningGroupId.Value;
                            res.CameFromType = (int)ObjectType.LearningGroup;
                        }
                    }
                }
                else if (type == (int)ObjectType.ObjectGroupMapper)
                {
                    var resourceDB = ObjectGroupMapperDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                        res.CameFromId = resourceDB.ObjectId;
                        res.CameFromType = resourceDB.ObjectType;
                    }
                }
                else if (type == (int)ObjectType.ObjectStreamCourse)
                {
                    var resourceDB = ObjectStreamCourseDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                        res.CameFromId = resourceDB.ObjectId;
                        res.CameFromType = resourceDB.ObjectType;
                    }
                }
                else if (type == (int)ObjectType.Message)
                {
                    var resourceDB = MessageDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                        res.CameFromId = resourceDB.ThreadId.Value;
                        res.CameFromType = (int)ObjectType.MessageThread;
                        if (resourceDB.FileContent != null)
                        {
                            res.FileContent = resourceDB.FileContent.ToArray();
                            res.FileTitle = resourceDB.FileTitle;
                            res.FileMime = resourceDB.FileMime;
                        }
                    }
                }
                else if (type == (int)ObjectType.MessageThread)
                {
                    var resourceDB = MessageThreadDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                    }
                }
                else if (type == (int)ObjectType.MessageContact)
                {
                    var resourceDB = MessageContactDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                    }
                }
                else if (type == (int)ObjectType.Notification)
                {
                    var resourceDB = NotificationDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                    }
                }
                else if (type == (int)ObjectType.App_UserProfile)
                {
                    var resourceDB = App_UserProfileDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                    }
                }
                
            }
            catch
            {
            }

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ObjectViewModel GetSharedObject(Guid objectId, int type)
        {
            var objectModel = new ObjectModel();
            var res = new ObjectViewModel();
            try
            {
                if (type == (int)ObjectType.Message)
                {
                    var resourceDB = MessageDAL.Get(objectId);
                    if (resourceDB != null)
                    {
                        res = new ObjectViewModel
                        {
                            Id = resourceDB.Id,
                            Type = type,
                        };
                        res.CameFromId = resourceDB.ThreadId.Value;
                        res.CameFromType = (int)ObjectType.MessageThread;
                        if (resourceDB.FileContent != null)
                        {
                            res.FileContent = resourceDB.FileContent.ToArray();
                            res.FileTitle = resourceDB.FileTitle;
                            res.FileMime = resourceDB.FileMime;
                        }
                    }
                }
            }
            catch
            {
            }

            return res;
        }

        /// <summary>
        /// delete integrated item from database
        /// </summary>
        /// <param name="objectId">id of the item</param>
        /// <param name="objectType">type of the item</param>
        /// <param name="RDM"></param>
        /// <returns>achievement of delete operation</returns>
        public static bool DeleteObject(int objectId, int objectType, RequestDetailModel RDM)
        {
            if (objectType == (int)ObjectType.Course)
            {
                RDM.ObjectType = (int)ObjectType.Course;
                ManageAction.ProxyCall(RDM, new Func<Course, DALReturnModel<Course>>(CourseDAL.Delete), new Course { Id = objectId });
            }
            else if (objectType == (int)ObjectType.Resource)
            {
                RDM.ObjectType = (int)ObjectType.Resource;
                var resource = ResourceDAL.Get(objectId);
                //update credit value
                ModelManager.UpdateCreditValue((int)ActionType.Delete, objectId, objectType, resource.CreateUserId);
                //update credit value
                ManageAction.ProxyCall(RDM, new Func<Resource, DALReturnModel<Resource>>(ResourceDAL.Delete), new Resource { Id = objectId });
            }
            else if (objectType == (int)ObjectType.Forum)
            {
                RDM.ObjectType = (int)ObjectType.Forum;
                var forum = ForumDAL.Get(objectId);
                var discussions = forum.ForumDiscussions.ToList();
                foreach (var item in discussions)
                {
                    ManageObject.DeleteObject(item.Id, (int)ObjectType.ForumDiscussion, RDM);
                }
                //update credit value
                ModelManager.UpdateCreditValue((int)ActionType.Delete, objectId, objectType, forum.CreateUserId);
                //update credit value
                ManageAction.ProxyCall(RDM, new Func<Forum, DALReturnModel<Forum>>(ForumDAL.Delete), forum);
            }
            else if (objectType == (int)ObjectType.ForumDiscussion)
            {
                RDM.ObjectType = (int)ObjectType.ForumDiscussion;
                var forumDiscussions = ForumDiscussionDAL.Get(objectId);
                if (!forumDiscussions.ParentId.HasValue)
                {
                    var discussions = forumDiscussions.ForumDiscussions.ToList();
                    foreach (var item in discussions)
                    {
                        ManageObject.DeleteObject(item.Id, (int)ObjectType.ForumDiscussion, RDM);
                    }
                }
                var posts = forumDiscussions.ForumDiscussionPosts.ToList();
                foreach (var item in posts)
                {
                    ManageObject.DeleteObject(item.Id, (int)ObjectType.ForumDiscussionPost, RDM);
                }
                //update credit value
                ModelManager.UpdateCreditValue((int)ActionType.Delete, objectId, objectType, forumDiscussions.UserId);
                //update credit value
                ManageAction.ProxyCall(RDM, new Func<ForumDiscussion, DALReturnModel<ForumDiscussion>>(ForumDiscussionDAL.Delete), forumDiscussions);
            }
            else if (objectType == (int)ObjectType.ForumDiscussionPost)
            {
                RDM.ObjectType = (int)ObjectType.ForumDiscussionPost;
                var forumDiscussionPost = ForumDiscussionPostDAL.Get(objectId);
                //update credit value
                ModelManager.UpdateCreditValue((int)ActionType.Delete, objectId, objectType, forumDiscussionPost.UserId);
                //update credit value
                ManageAction.ProxyCall(RDM, new Func<ForumDiscussionPost, DALReturnModel<ForumDiscussionPost>>(ForumDiscussionPostDAL.Delete), forumDiscussionPost);
            }
            //yousefi
            else if (objectType == (int)ObjectType.Assignment)
            {
                RDM.ObjectType = (int)ObjectType.Assignment;
                var assignment = AssignmentDAL.Get(objectId);
                var submissions = assignment.AssignmentSubmissions.ToList();
                foreach (var item in submissions)
                {
                    ManageObject.DeleteObject(item.Id, (int)ObjectType.AssignmentSubmission, RDM);
                }
                //update credit value
                ModelManager.UpdateCreditValue((int)ActionType.Delete, objectId, objectType, assignment.CreateUserId);
                //update credit value
                ManageAction.ProxyCall(RDM, new Func<Assignment, DALReturnModel<Assignment>>(AssignmentDAL.Delete), assignment);
            }
            else if (objectType == (int)ObjectType.AssignmentSubmission)
            {
                RDM.ObjectType = (int)ObjectType.AssignmentSubmission;
                var submission = AssignmentSubmissionDAL.Get(objectId);
                //update credit value
                ModelManager.UpdateCreditValue((int)ActionType.Delete, objectId, objectType, submission.UserId);
                //update credit value
                ManageAction.ProxyCall(RDM, new Func<AssignmentSubmission, DALReturnModel<AssignmentSubmission>>(AssignmentSubmissionDAL.Delete), submission);
            }
            //yousefi

            RDM.ObjectType = (int)ObjectType.ObjectStream;
            var streams = ObjectStreamDAL.GetAll(objectId, objectType);
            foreach (var item in streams)
            {
                ManageAction.ProxyCall(RDM, new Func<ObjectStream, DALReturnModel<ObjectStream>>(ObjectStreamDAL.Delete), item);
            }            
            RDM.ObjectType = (int)ObjectType.Comment;
            var comments = CommentDAL.GetAll(objectId, objectType);
            foreach (var item in comments)
            {
                var votesC = VoteParentDAL.GetAll(item.Id, (int)ObjectType.Comment);
                foreach (var vote in votesC)
                {
                    ManageAction.ProxyCall(RDM, new Func<VoteParent, DALReturnModel<VoteParent>>(VoteParentDAL.Delete), item);
                }   
                ManageAction.ProxyCall(RDM, new Func<Comment, DALReturnModel<Comment>>(CommentDAL.Delete), item);
            }   
            RDM.ObjectType = (int)ObjectType.TagMapper;
            var tags = TagMapperDAL.GetAll(objectId, objectType);
            foreach (var item in tags)
            {
                ManageAction.ProxyCall(RDM, new Func<TagMapper, DALReturnModel<TagMapper>>(TagMapperDAL.Delete), item);
            }   
            RDM.ObjectType = (int)ObjectType.VoteParent;
            var votes = VoteParentDAL.GetAll(objectId, objectType);
            foreach (var item in votes)
            {
                ManageAction.ProxyCall(RDM, new Func<VoteParent, DALReturnModel<VoteParent>>(VoteParentDAL.Delete), item);
            }   
            RDM.ObjectType = (int)ObjectType.Share;
            var shares = ShareDAL.GetAll(objectId, objectType);
            foreach (var item in shares)
            {
                ManageAction.ProxyCall(RDM, new Func<Share, DALReturnModel<Share>>(ShareDAL.Delete), item);
            }  
            RDM.ObjectType = (int)ObjectType.ObjectTopicMapper;
            var topicMappers = ObjectTopicMapperDAL.GetAllObject(objectId, objectType);
            foreach (var item in topicMappers)
            {
                ManageAction.ProxyCall(RDM, new Func<ObjectTopicMapper, DALReturnModel<ObjectTopicMapper>>(ObjectTopicMapperDAL.Delete), item);
            }  

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toCastObject"></param>
        /// <param name="objectType"></param>
        /// <param name="generic"></param>
        /// <returns></returns>
        public static ObjectViewModel CastObjectAndGetIt(object toCastObject, int objectType, bool generic = false)
        {
            if (objectType == (int)ObjectType.Course)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<Course>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((Course)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.Resource)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<Resource>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((Resource)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.User)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<App_User>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((App_User)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.Comment)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<Comment>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((Comment)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.Forum)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<Forum>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((Forum)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.Assignment)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<Assignment>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((Assignment)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.Quiz)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<Quiz>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((Quiz)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.ForumDiscussion)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<ForumDiscussion>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((ForumDiscussion)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.ImageAnnotation)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<ImageAnnotation>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((ImageAnnotation)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.Category)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<Category>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((Category)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.Wiki)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<Wiki>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((Wiki)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.VoteParent)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<VoteParent>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((VoteParent)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.Vote)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<Vote>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((Vote)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.TagMapper)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<TagMapper>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((TagMapper)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.Tag)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<Tag>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((Tag)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.SocialGroup)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<SocialGroup>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((SocialGroup)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.Share)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<Share>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((Share)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.App_User)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<App_User>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((App_User)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.LearningGroup)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<LearningGroup>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((LearningGroup)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.ForumDiscussionPost)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<ForumDiscussionPost>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((ForumDiscussionPost)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.AssignmentSubmission)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<AssignmentSubmission>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((AssignmentSubmission)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.Grade)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<Grade>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((Grade)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.Feedback)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<Feedback>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((Feedback)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.Email)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<Email>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((Email)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.Survey)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<Survey>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((Survey)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.UserEnrolement)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<App_UserEnrolement>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((App_UserEnrolement)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.ObjectStream)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<ObjectStream>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((ObjectStream)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.App_Action)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<App_Action>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((App_Action)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.App_UserInRole)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<App_UserInRole>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((App_UserInRole)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.App_UserEnrolement)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<App_UserEnrolement>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((App_UserEnrolement)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.App_UserInfo)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<App_UserInfo>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((App_UserInfo)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.UserTopicMapper)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<UserTopicMapper>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((UserTopicMapper)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.App_ActionEvaulation)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<App_ActionEvaulation>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((App_ActionEvaulation)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.App_Permission)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<App_Permission>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((App_Permission)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.AssessParent)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<AssessParent>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((AssessParent)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.Assess)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<Assess>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((Assess)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.ObjectTopicMapper)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<ObjectTopicMapper>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((ObjectTopicMapper)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.CourseAbstract)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<CourseAbstract>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((CourseAbstract)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.Topic)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<Topic>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((Topic)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.CourseTopcMapper)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<CourseTopcMapper>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((CourseTopcMapper)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.CourseSchedule)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<CourseSchedule>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((CourseSchedule)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.CategoryMapper)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<CategoryMapper>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((CategoryMapper)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.Answer)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<Answer>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((Answer)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.GroupMember)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<GroupMember>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((GroupMember)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.SurveyAnswer)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<SurveyAnswer>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((SurveyAnswer)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.SurveyUserSummary)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<SurveyUserSummary>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((SurveyUserSummary)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.ObjectStreamGroupMapper)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<ObjectStreamGroupMapper>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((ObjectStreamGroupMapper)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.ObjectGroupMapper)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<ObjectGroupMapper>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((ObjectGroupMapper)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.ObjectStreamCourse)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<ObjectStreamCourse>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((ObjectStreamCourse)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.Message)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<Message>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((Message)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.MessageContact)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<MessageContact>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((MessageContact)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.MessageThread)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<MessageThread>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((MessageThread)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.Notification)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<Notification>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((Notification)toCastObject).Id, objectType);
            }
            else if (objectType == (int)ObjectType.App_UserProfile)
            {
                if (generic)
                    return GetSharedObject(((DALReturnModel<App_UserProfile>)toCastObject).ReturnObject.Id, objectType);
                return GetSharedObject(((App_UserProfile)toCastObject).Id, objectType);
            }
            return null;
        }

    }
}
