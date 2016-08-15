using System;
using System.Collections.Generic;
using System.Linq;
using UT.SL.DAL;
using UT.SL.Data.LINQ;
using UT.SL.Model;
using UT.SL.Helper;

namespace UT.SL.BLL
{
    /// <summary>
    /// 
    /// </summary>
    public class UserInfoManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public static List<UserActivityRecordModel> GetUserCourseSummary(int userId, int courseId)
        {
            var records = new List<UserActivityRecordModel>();
            try
            {
                var courseObjects = ObjectStreamDAL.GetAllForCourseAndUser(courseId, userId);
                foreach (var item in courseObjects)
                {
                    var sharedObject = ManageObject.GetSharedObject(item.ObjectId, item.ObjectType);
                    if (sharedObject != null && sharedObject.Id > 0)
                    {
                        if (item.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Forum)
                        {
                            var forumDiscussions = ForumDiscussionDAL.GetAllDiscussionsByForumAndUser(item.ObjectId, userId);
                            foreach (var disc in forumDiscussions)
                            {
                                if (records.Any(x => x.Date.Date == disc.CreateDate.Value.Date))
                                {
                                    var index = records.FindLastIndex(x => x.Date.Date == disc.CreateDate.Value.Date);
                                    records[index].Score++;
                                    records[index].CountOfForumDisscussion++;
                                    records[index].Objects.Add(ManageObject.GetSharedObject(disc.Id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion));
                                }
                                else
                                {
                                    var tempRecord = new UserActivityRecordModel();
                                    tempRecord.Date = disc.CreateDate.Value.Date;
                                    tempRecord.Score++;
                                    tempRecord.CountOfForumDisscussion++;
                                    tempRecord.Objects.Add(ManageObject.GetSharedObject(disc.Id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion));
                                    records.Add(tempRecord);
                                }

                                if (disc.ParentId == null)
                                {
                                    var discTags = TagMapperDAL.GetAllUserAndObject(userId, disc.Id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);
                                    foreach (var tag in discTags.Where(x => x.CreateDate.HasValue))
                                    {
                                        if (records.Any(x => x.Date.Date == tag.CreateDate.Value.Date))
                                        {
                                            var index = records.FindLastIndex(x => x.Date.Date == tag.CreateDate.Value.Date);
                                            records[index].Score++;
                                            records[index].CountOfTags++;
                                            records[index].Objects.Add(ManageObject.GetSharedObject(tag.Id, (int)UT.SL.Model.Enumeration.ObjectType.TagMapper));
                                        }
                                        else
                                        {
                                            var tempRecord = new UserActivityRecordModel();
                                            tempRecord.Date = tag.CreateDate.Value.Date;
                                            tempRecord.Score++;
                                            tempRecord.CountOfTags++;
                                            tempRecord.Objects.Add(ManageObject.GetSharedObject(tag.Id, (int)UT.SL.Model.Enumeration.ObjectType.TagMapper));
                                            records.Add(tempRecord);
                                        }
                                    }
                                }

                                var discVoteParents = VoteParentDAL.GetAllUserAndObject(userId, disc.Id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion);
                                foreach (var votes in discVoteParents)
                                {
                                    foreach (var vote in votes.Votes.Where(x => x.UserId == userId))
                                    {
                                        if (records.Any(x => x.Date.Date == vote.CreateDate.Value.Date))
                                        {
                                            var index = records.FindLastIndex(x => x.Date.Date == vote.CreateDate.Value.Date);
                                            records[index].Score++;
                                            records[index].CountOfVote++;
                                            records[index].Objects.Add(ManageObject.GetSharedObject(vote.Id, (int)UT.SL.Model.Enumeration.ObjectType.Vote));
                                        }
                                        else
                                        {
                                            var tempRecord = new UserActivityRecordModel();
                                            tempRecord.Date = vote.CreateDate.Value.Date;
                                            tempRecord.Score++;
                                            tempRecord.CountOfVote++;
                                            tempRecord.Objects.Add(ManageObject.GetSharedObject(vote.Id, (int)UT.SL.Model.Enumeration.ObjectType.Vote));
                                            records.Add(tempRecord);
                                        }
                                    }

                                }
                            }
                        }

                        if (item.ObjectType == (int)UT.SL.Model.Enumeration.ObjectType.Assignment)
                        {
                            var assignmentSubmission = AssignmentSubmissionDAL.GetAllSubmissions(item.ObjectId, userId);
                            foreach (var assignmentSubmit in assignmentSubmission)
                            {
                                if (records.Any(x => x.Date.Date == assignmentSubmit.CreateDate.Date))
                                {
                                    var index = records.FindLastIndex(x => x.Date.Date == assignmentSubmit.CreateDate.Date);
                                    records[index].Score++;
                                    records[index].CountOfAssignmentsubmissionSubmit++;
                                    records[index].Objects.Add(ManageObject.GetSharedObject(assignmentSubmit.Id, (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission));
                                }
                                else
                                {
                                    var tempRecord = new UserActivityRecordModel();
                                    tempRecord.Date = assignmentSubmit.CreateDate.Date;
                                    tempRecord.Score++;
                                    tempRecord.CountOfAssignmentsubmissionSubmit++;
                                    tempRecord.Objects.Add(ManageObject.GetSharedObject(assignmentSubmit.Id, (int)UT.SL.Model.Enumeration.ObjectType.AssignmentSubmission));
                                    records.Add(tempRecord);
                                }
                            }
                        }
                        if (item.ReadDate.HasValue && sharedObject.CreateUser.Id != userId)
                        {
                            if (records.Any(x => x.Date.Date == item.ReadDate.Value.Date))
                            {
                                var index = records.FindLastIndex(x => x.Date.Date == item.ReadDate.Value.Date);
                                if (item.IsRead.HasValue && item.IsRead.Value)
                                {
                                    records[index].Score++;
                                    records[index].CountOfReadenObjects++;
                                    records[index].Objects.Add(ManageObject.GetSharedObject(item.ObjectId, item.ObjectType));
                                }
                            }
                            else
                            {
                                var tempRecord = new UserActivityRecordModel();
                                tempRecord.Date = item.ReadDate.Value.Date;
                                if (item.IsRead.HasValue && item.IsRead.Value)
                                {
                                    tempRecord.Score++;
                                    tempRecord.CountOfReadenObjects++;
                                    tempRecord.Objects.Add(ManageObject.GetSharedObject(item.ObjectId, item.ObjectType));
                                }
                                records.Add(tempRecord);
                            }
                        }

                        if (sharedObject.CreateUser.Id == item.UserId)
                        {
                            if (records.Any(x => x.Date.Date == item.CreateDate.Date))
                            {
                                var index = records.FindLastIndex(x => x.Date.Date == item.CreateDate.Date);
                                if (sharedObject.CreateUser.Id == item.UserId)
                                {
                                    records[index].Score++;
                                    records[index].CountOfCreatedObjects++;
                                    records[index].Objects.Add(ManageObject.GetSharedObject(item.ObjectId, item.ObjectType));
                                }
                            }
                            else
                            {
                                var tempRecord = new UserActivityRecordModel();
                                tempRecord.Date = item.CreateDate.Date;
                                if (sharedObject.CreateUser.Id == item.UserId)
                                {
                                    tempRecord.Score++;
                                    tempRecord.CountOfCreatedObjects++;
                                    tempRecord.Objects.Add(ManageObject.GetSharedObject(item.ObjectId, item.ObjectType));
                                }
                                records.Add(tempRecord);
                            }
                        }

                        var comments = CommentDAL.GetAllUserAndObject(userId, item.ObjectId, item.ObjectType);
                        foreach (var comment in comments.Where(x => x.CreateDate.HasValue))
                        {
                            if (records.Any(x => x.Date.Date == comment.CreateDate.Value.Date))
                            {
                                var index = records.FindLastIndex(x => x.Date.Date == comment.CreateDate.Value.Date);
                                records[index].Score++;
                                records[index].CountOfComments++;
                                records[index].Objects.Add(ManageObject.GetSharedObject(comment.Id, (int)UT.SL.Model.Enumeration.ObjectType.Comment));
                            }
                            else
                            {
                                var tempRecord = new UserActivityRecordModel();
                                tempRecord.Date = comment.CreateDate.Value.Date;
                                tempRecord.Score++;
                                tempRecord.CountOfComments++;
                                tempRecord.Objects.Add(ManageObject.GetSharedObject(comment.Id, (int)UT.SL.Model.Enumeration.ObjectType.Comment));
                                records.Add(tempRecord);
                            }

                            var commentVoteParents = VoteParentDAL.GetAllUserAndObject(userId, comment.Id, (int)UT.SL.Model.Enumeration.ObjectType.Comment);
                            foreach (var votes in commentVoteParents)
                            {
                                foreach (var vote in votes.Votes.Where(x => x.UserId == userId))
                                {
                                    if (records.Any(x => x.Date.Date == vote.CreateDate.Value.Date))
                                    {
                                        var index = records.FindLastIndex(x => x.Date.Date == vote.CreateDate.Value.Date);
                                        records[index].Score++;
                                        records[index].CountOfVote++;
                                        records[index].Objects.Add(ManageObject.GetSharedObject(vote.Id, (int)UT.SL.Model.Enumeration.ObjectType.Vote));
                                    }
                                    else
                                    {
                                        var tempRecord = new UserActivityRecordModel();
                                        tempRecord.Date = vote.CreateDate.Value.Date;
                                        tempRecord.Score++;
                                        tempRecord.CountOfVote++;
                                        tempRecord.Objects.Add(ManageObject.GetSharedObject(vote.Id, (int)UT.SL.Model.Enumeration.ObjectType.Vote));
                                        records.Add(tempRecord);
                                    }
                                }
                            }
                        }

                        var tags = TagMapperDAL.GetAllUserAndObject(userId, item.ObjectId, item.ObjectType);
                        foreach (var tag in tags.Where(x => x.CreateDate.HasValue))
                        {
                            if (records.Any(x => x.Date.Date == tag.CreateDate.Value.Date))
                            {
                                var index = records.FindLastIndex(x => x.Date.Date == tag.CreateDate.Value.Date);
                                records[index].Score++;
                                records[index].CountOfTags++;
                                records[index].Objects.Add(ManageObject.GetSharedObject(tag.Id, (int)UT.SL.Model.Enumeration.ObjectType.TagMapper));
                            }
                            else
                            {
                                var tempRecord = new UserActivityRecordModel();
                                tempRecord.Date = tag.CreateDate.Value.Date;
                                tempRecord.Score++;
                                tempRecord.CountOfTags++;
                                tempRecord.Objects.Add(ManageObject.GetSharedObject(tag.Id, (int)UT.SL.Model.Enumeration.ObjectType.TagMapper));
                                records.Add(tempRecord);
                            }
                        }

                        var voteParents = VoteParentDAL.GetAllUserAndObject(userId, item.ObjectId, item.ObjectType);
                        foreach (var votes in voteParents)
                        {
                            foreach (var vote in votes.Votes.Where(x => x.UserId == userId))
                            {
                                if (records.Any(x => x.Date.Date == vote.CreateDate.Value.Date))
                                {
                                    var index = records.FindLastIndex(x => x.Date.Date == vote.CreateDate.Value.Date);
                                    records[index].Score++;
                                    records[index].CountOfVote++;
                                    records[index].Objects.Add(ManageObject.GetSharedObject(vote.Id, (int)UT.SL.Model.Enumeration.ObjectType.Vote));
                                }
                                else
                                {
                                    var tempRecord = new UserActivityRecordModel();
                                    tempRecord.Date = vote.CreateDate.Value.Date;
                                    tempRecord.Score++;
                                    tempRecord.CountOfVote++;
                                    tempRecord.Objects.Add(ManageObject.GetSharedObject(vote.Id, (int)UT.SL.Model.Enumeration.ObjectType.Vote));
                                    records.Add(tempRecord);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            return records.OrderBy(x => x.Date).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <param name="actType"></param>
        public static void UpdateUserActSummary(int userId, int objectId, int objectType, int actType)
        {
            var sharedObject = ManageObject.GetSharedObject(objectId, objectType);
            if (sharedObject.CourseId.HasValue && sharedObject.CourseId > 0)
            {
                var sum = UserActSummaryDAL.Get(userId, sharedObject.CourseId.Value, DateTime.Now.Date);
                if (sum == null)
                {
                    sum = new UserActSummary
                    {
                        CourseId = sharedObject.CourseId.Value,
                        UserId = userId,
                        BeginDate = DateTime.Now.GetFirstDateOfWeek(DayOfWeek.Saturday).Date,
                        EndDate = DateTime.Now.GetLastDateOfWeek(DayOfWeek.Friday).Date
                    };
                    sum = UserActSummaryDAL.Add(sum).ReturnObject;
                }
                sum.CreateCount = sum.CreateCount.HasValue ? sum.CreateCount : 0;
                sum.CommentCount = sum.CommentCount.HasValue ? sum.CommentCount : 0;
                sum.TagCount = sum.TagCount.HasValue ? sum.TagCount : 0;
                sum.VoteCount = sum.VoteCount.HasValue ? sum.VoteCount : 0;
                sum.TotalScore = sum.TotalScore.HasValue ? sum.TotalScore : 0;
                if (actType == (int)UT.SL.Model.Enumeration.ActivityType.Create)
                {
                    sum.CreateCount++;
                    sum.TotalScore = sum.TotalScore + 4;
                }
                else if (actType == (int)UT.SL.Model.Enumeration.ActivityType.Comment)
                {
                    sum.CommentCount++;
                    sum.TotalScore = sum.TotalScore + 3;
                }
                else if (actType == (int)UT.SL.Model.Enumeration.ActivityType.Tag)
                {
                    sum.TagCount++;
                    sum.TotalScore = sum.TotalScore + 2;
                }
                else if (actType == (int)UT.SL.Model.Enumeration.ActivityType.Vote)
                {
                    sum.VoteCount++;
                    sum.TotalScore = sum.TotalScore + 1;
                }
                UserActSummaryDAL.Update(sum);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <param name="actType"></param>
        /// <param name="date"></param>
        public static void UpdateUserActSummary(int userId, int objectId, int objectType, int actType, DateTime date)
        {
            var sharedObject = ManageObject.GetSharedObject(objectId, objectType);
            if (sharedObject.CourseId.HasValue && sharedObject.CourseId > 0)
            {
                var sum = UserActSummaryDAL.Get(userId, sharedObject.CourseId.Value, date.Date);
                if (sum == null)
                {
                    sum = new UserActSummary
                    {
                        CourseId = sharedObject.CourseId.Value,
                        UserId = userId,
                        BeginDate = date.GetFirstDateOfWeek(DayOfWeek.Saturday).Date,
                        EndDate = date.GetLastDateOfWeek(DayOfWeek.Friday).Date
                    };
                    sum = UserActSummaryDAL.Add(sum).ReturnObject;
                }
                sum.CreateCount = sum.CreateCount.HasValue ? sum.CreateCount : 0;
                sum.CommentCount = sum.CommentCount.HasValue ? sum.CommentCount : 0;
                sum.TagCount = sum.TagCount.HasValue ? sum.TagCount : 0;
                sum.VoteCount = sum.VoteCount.HasValue ? sum.VoteCount : 0;
                sum.TotalScore = sum.TotalScore.HasValue ? sum.TotalScore : 0;
                if (actType == (int)UT.SL.Model.Enumeration.ActivityType.Create)
                {
                    sum.CreateCount++;
                    sum.TotalScore = sum.TotalScore + 4;
                }
                else if (actType == (int)UT.SL.Model.Enumeration.ActivityType.Comment)
                {
                    sum.CommentCount++;
                    sum.TotalScore = sum.TotalScore + 3;
                }
                else if (actType == (int)UT.SL.Model.Enumeration.ActivityType.Tag)
                {
                    sum.TagCount++;
                    sum.TotalScore = sum.TotalScore + 2;
                }
                else if (actType == (int)UT.SL.Model.Enumeration.ActivityType.Vote)
                {
                    sum.VoteCount++;
                    sum.TotalScore = sum.TotalScore + 1;
                }
                UserActSummaryDAL.Update(sum);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        /// <param name="actType"></param>
        public static void DeleteUserActSummary(int userId, int objectId, int objectType, int actType)
        {
            var sharedObject = ManageObject.GetSharedObject(objectId, objectType);
            if (sharedObject.CourseId.HasValue && sharedObject.CourseId > 0)
            {
                var sum = UserActSummaryDAL.Get(userId, sharedObject.CourseId.Value, DateTime.Now.Date);
                if (sum == null)
                {
                    sum = new UserActSummary
                    {
                        CourseId = sharedObject.CourseId.Value,
                        UserId = userId,
                        BeginDate = DateTime.Now.GetFirstDateOfWeek(DayOfWeek.Saturday).Date,
                        EndDate = DateTime.Now.GetLastDateOfWeek(DayOfWeek.Friday).Date
                    };
                    sum = UserActSummaryDAL.Add(sum).ReturnObject;
                }
                sum.CreateCount = sum.CreateCount.HasValue ? sum.CreateCount : 0;
                sum.CommentCount = sum.CommentCount.HasValue ? sum.CommentCount : 0;
                sum.TagCount = sum.TagCount.HasValue ? sum.TagCount : 0;
                sum.VoteCount = sum.VoteCount.HasValue ? sum.VoteCount : 0;
                sum.TotalScore = sum.TotalScore.HasValue ? sum.TotalScore : 0;
                if (actType == (int)UT.SL.Model.Enumeration.ActivityType.Create)
                {
                    sum.CreateCount--;
                    //if (sum.CreateCount <= 0)
                    //    sum.CreateCount = 0;
                    sum.TotalScore = sum.TotalScore - 4;
                }
                else if (actType == (int)UT.SL.Model.Enumeration.ActivityType.Comment)
                {
                    sum.CommentCount--;
                    //if (sum.CommentCount <= 0)
                    //    sum.CommentCount = 0;
                    sum.TotalScore = sum.TotalScore - 3;
                }
                else if (actType == (int)UT.SL.Model.Enumeration.ActivityType.Tag)
                {
                    sum.TagCount--;
                    //if (sum.TagCount <= 0)
                    //    sum.TagCount = 0;
                    sum.TotalScore = sum.TotalScore - 2;
                }
                else if (actType == (int)UT.SL.Model.Enumeration.ActivityType.Vote)
                {
                    sum.VoteCount--;
                    //if (sum.VoteCount <= 0)
                    //    sum.VoteCount = 0;
                    sum.TotalScore = sum.TotalScore - 1;
                }
                //if (sum.TotalScore <= 0)
                //    sum.TotalScore = 0;
                UserActSummaryDAL.Update(sum);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Repair(int? userId)
        {
            if (userId.HasValue)
            {
                var user = App_UserDAL.Get(userId.Value);
                UserActSummaryDAL.DeleteUserActs(userId.Value);
                foreach (var resource in ResourceDAL.GetAll().Where(x => x.App_User.Id == user.Id && x.IsPublishd && x.IsValid))
                {
                    UserInfoManager.UpdateUserActSummary(user.Id, resource.Id, (int)UT.SL.Model.Enumeration.ObjectType.Resource, (int)UT.SL.Model.Enumeration.ActivityType.Create, resource.CreateDate);
                }
                foreach (var comment in CommentDAL.GetAll().Where(x => x.App_User.Id == user.Id))
                {
                    UserInfoManager.UpdateUserActSummary(user.Id, comment.ObjectId, comment.Type, (int)UT.SL.Model.Enumeration.ActivityType.Comment, comment.CreateDate ?? DateTime.Now);
                }
                foreach (var forum in ForumDAL.GetAll().Where(x => x.App_User.Id == user.Id && x.IsPublishd && x.IsValid))
                {
                    UserInfoManager.UpdateUserActSummary(user.Id, forum.Id, (int)UT.SL.Model.Enumeration.ObjectType.Forum, (int)UT.SL.Model.Enumeration.ActivityType.Create, forum.CreateDate ?? DateTime.Now);
                }
                foreach (var forumDiscussion in ForumDiscussionDAL.GetAll().Where(x => x.App_User.Id == user.Id && x.IsPublishd && x.IsValid))
                {
                    UserInfoManager.UpdateUserActSummary(user.Id, forumDiscussion.Id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion, (int)UT.SL.Model.Enumeration.ActivityType.Create, forumDiscussion.CreateDate ?? DateTime.Now);
                }
                foreach (var forumDiscussionPost in ForumDiscussionPostDAL.GetAll().Where(x => x.App_User.Id == user.Id))
                {
                    UserInfoManager.UpdateUserActSummary(user.Id, forumDiscussionPost.Id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussionPost, (int)UT.SL.Model.Enumeration.ActivityType.Create, forumDiscussionPost.CreateDate ?? DateTime.Now);
                }
                foreach (var tag in TagMapperDAL.GetAll().Where(x => x.App_User.Id == user.Id))
                {
                    UserInfoManager.UpdateUserActSummary(user.Id, tag.ObjectId, tag.ObjectType, (int)UT.SL.Model.Enumeration.ActivityType.Tag, tag.CreateDate ?? DateTime.Now);
                }
                foreach (var vote in VoteDAL.GetAll().Where(x => x.App_User.Id == user.Id))
                {
                    UserInfoManager.UpdateUserActSummary(user.Id, vote.VoteParent.ObjectId, vote.VoteParent.ObjectType ?? 0, (int)UT.SL.Model.Enumeration.ActivityType.Vote, vote.CreateDate ?? DateTime.Now);
                }

            }
            else
            {
                foreach (var user in App_UserDAL.GetAll())
                {
                    foreach (var resource in ResourceDAL.GetAll().Where(x => x.App_User.Id == user.Id && x.IsPublishd && x.IsValid))
                    {
                        UserInfoManager.UpdateUserActSummary(user.Id, resource.Id, (int)UT.SL.Model.Enumeration.ObjectType.Resource, (int)UT.SL.Model.Enumeration.ActivityType.Create, resource.CreateDate);
                    }
                    foreach (var comment in CommentDAL.GetAll().Where(x => x.App_User.Id == user.Id))
                    {
                        UserInfoManager.UpdateUserActSummary(user.Id, comment.ObjectId, comment.Type, (int)UT.SL.Model.Enumeration.ActivityType.Comment, comment.CreateDate ?? DateTime.Now);
                    }
                    foreach (var forum in ForumDAL.GetAll().Where(x => x.App_User.Id == user.Id && x.IsPublishd && x.IsValid))
                    {
                        UserInfoManager.UpdateUserActSummary(user.Id, forum.Id, (int)UT.SL.Model.Enumeration.ObjectType.Forum, (int)UT.SL.Model.Enumeration.ActivityType.Create, forum.CreateDate ?? DateTime.Now);
                    }
                    foreach (var forumDiscussion in ForumDiscussionDAL.GetAll().Where(x => x.App_User.Id == user.Id && x.IsPublishd && x.IsValid))
                    {
                        UserInfoManager.UpdateUserActSummary(user.Id, forumDiscussion.Id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussion, (int)UT.SL.Model.Enumeration.ActivityType.Create, forumDiscussion.CreateDate ?? DateTime.Now);
                    }
                    foreach (var forumDiscussionPost in ForumDiscussionPostDAL.GetAll().Where(x => x.App_User.Id == user.Id))
                    {
                        UserInfoManager.UpdateUserActSummary(user.Id, forumDiscussionPost.Id, (int)UT.SL.Model.Enumeration.ObjectType.ForumDiscussionPost, (int)UT.SL.Model.Enumeration.ActivityType.Create, forumDiscussionPost.CreateDate ?? DateTime.Now);
                    }
                    foreach (var tag in TagMapperDAL.GetAll().Where(x => x.App_User.Id == user.Id))
                    {
                        UserInfoManager.UpdateUserActSummary(user.Id, tag.ObjectId, tag.ObjectType, (int)UT.SL.Model.Enumeration.ActivityType.Tag, tag.CreateDate ?? DateTime.Now);
                    }
                    foreach (var vote in VoteDAL.GetAll().Where(x => x.App_User.Id == user.Id))
                    {
                        UserInfoManager.UpdateUserActSummary(user.Id, vote.VoteParent.ObjectId, vote.VoteParent.ObjectType ?? 0, (int)UT.SL.Model.Enumeration.ActivityType.Vote, vote.CreateDate ?? DateTime.Now);
                    }
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public static void UpdateGradeOwnerId()
        {
            var grades = GradeDAL.GetAll();
            foreach (var item in grades)
            {
                var ob = ManageObject.GetSharedObject(item.ObjectId.Value, item.ObjectType.Value);
                if (ob.CreateUser != null)
                    GradeDAL.UpdateGradeOwner(item.Id, ob.CreateUser.Id);
                var sharedObject = ManageObject.GetSharedObject(item.ObjectId.Value, item.ObjectType.Value);
                GradeDAL.UpdateGradeParent(item.Id, sharedObject.CameFromId, sharedObject.CameFromType, sharedObject.CourseId);
            }
        }
    }
}
