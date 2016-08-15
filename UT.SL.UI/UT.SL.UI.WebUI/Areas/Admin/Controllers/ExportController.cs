using System;
using System.Linq;
using System.Web.Mvc;
using UT.SL.Helper;
using UT.SL.UI.WebUI.Controllers;
using UT.SL.Model;
using UT.SL.Data.LINQ;
using UT.SL.DAL;
using System.Xml;
using UT.SL.BLL;
using UT.SL.Model.Enumeration;
using System.Collections.Generic;
namespace UT.SL.UI.WebUI.Areas.Admin.Controllers
{
    [Authorize()]
    public class ExportController : BaseController
    {

        public ActionResult ExportSurvey()
        {
            XmlTextWriter writer = new XmlTextWriter(Server.MapPath("~/Exports/SurveyResults.xml"), System.Text.Encoding.UTF8);
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.WriteStartElement("SurveyResults");
            foreach (var user in App_UserDAL.GetAll())
            {
                var summries = SurveyDAL.GetAllForUser(user.Id).Where(x => x.Statuse == 3);
                if (summries.Any())
                {
                    writer.WriteStartElement("UserSurveyResults");
                    writer.WriteStartElement("User");
                    writer.WriteString(string.Format("{0} {1}", user.FirstName, user.LastName));
                    writer.WriteEndElement();
                    foreach (var surveySummary in summries)
                    {
                        writer.WriteStartElement("Survey");
                        writer.WriteStartElement("SurveyName");
                        if (surveySummary.SurveyId == 1)
                        {
                            writer.WriteString("Harter");
                        }
                        else if (surveySummary.SurveyId == 2)
                        {
                            writer.WriteString("Hermans");
                        }
                        else if (surveySummary.SurveyId == 3)
                        {
                            writer.WriteString("AGQ-R");
                        }
                        else if (surveySummary.SurveyId == 4)
                        {
                            writer.WriteString("Feedback");
                        }
                        writer.WriteEndElement();
                        writer.WriteStartElement("Answers");
                        foreach (var answer in SurveyDAL.GetSurveyAnswer(surveySummary.SurveyId.Value, surveySummary.UserId).OrderBy(x => x.QuestionId).ToList())
                        {
                            writer.WriteStartElement("Answer");
                            writer.WriteStartElement("QuestionNumber");
                            writer.WriteString(answer.QuestionId.ToString());
                            writer.WriteEndElement();
                            writer.WriteStartElement("AnsverValue");
                            writer.WriteString(answer.AnswerValue.ToString());
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            return File("~/Exports/SurveyResults.xml", "text/xml", "SurveyResults.xml");

        }

        public ActionResult ExportCourseMaterials(int courseId)
        {
            XmlTextWriter writer = new XmlTextWriter(Server.MapPath("~/Exports/CourseMaterials.xml"), System.Text.Encoding.UTF8);
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.WriteStartElement("CourseMaterials");
            var materials = CourseDAL.GetAllMaterials(courseId);
            writer.WriteStartElement("Course");
            writer.WriteString(string.Format("{0}", CourseDAL.Get(courseId).Title));
            writer.WriteEndElement();
            foreach (var group in materials.GroupBy(x => x.ObjectType))
            {
                //writer.WriteStartElement(((ObjectType)group.First().ObjectType).ToString() + "s");
                foreach (var item in group)
                {
                    var material = ManageObject.GetSharedObject(item.ObjectId, item.ObjectType);
                    if (material.CreateUser != null)
                    {
                        writer.WriteStartElement("Material");

                        writer.WriteStartElement("MaterialType");
                        writer.WriteString(string.Format("{0}", ((ObjectType)item.ObjectType).ToString()));
                        writer.WriteEndElement();

                        //writer.WriteStartElement("Metadata");
                        #region  metadata
                        writer.WriteStartElement("ObjectId");
                        writer.WriteString(string.Format("{0}", item.ObjectId));
                        writer.WriteEndElement();
                        writer.WriteStartElement("ObjectType");
                        writer.WriteString(string.Format("{0}", item.ObjectType));
                        writer.WriteEndElement();
                        writer.WriteStartElement("Popularity");
                        writer.WriteString(string.Format("{0}", ContentPopularityModelDAL.GetScore(item.ObjectId, item.ObjectType, courseId)));
                        writer.WriteEndElement();
                        writer.WriteStartElement("CreateUser");
                        writer.WriteString(string.Format("{0} {1}", material.CreateUser.FirstName, material.CreateUser.LastName));
                        writer.WriteEndElement();
                        writer.WriteStartElement("CreateDate");
                        writer.WriteString(string.Format("{0}", material.CreateDate.ToString("MM/dd/yyyy HH:mm:ss")));
                        writer.WriteEndElement();

                        var topics = ObjectTopicMapperDAL.GetAll(item.ObjectId, item.ObjectType);
                        var topicString = string.Empty;
                        foreach (var topic in topics)
                        {
                            topicString += topic.Title + ", ";
                        }
                        writer.WriteStartElement("Topics");
                        writer.WriteString(string.Format("{0}", topicString));
                        writer.WriteEndElement();

                        var tags = TagMapperDAL.GetAll(item.ObjectId, item.ObjectType);
                        var tagString = string.Empty;
                        foreach (var tag in tags)
                        {
                            tagString += tag.Tag.Title + ", ";
                        }
                        writer.WriteStartElement("Tags");
                        writer.WriteString(string.Format("{0}", tagString));
                        writer.WriteEndElement();

                        writer.WriteStartElement("Vote");
                        writer.WriteString(string.Format("{0}", VoteDAL.GetAllCount(item.ObjectId, item.ObjectType)));
                        writer.WriteEndElement();

                        writer.WriteStartElement("Comment");
                        writer.WriteString(string.Format("{0}", CommentDAL.GetAllCount(item.ObjectId, item.ObjectType)));
                        writer.WriteEndElement();

                        writer.WriteStartElement("Share");
                        writer.WriteString(string.Format("{0}", "0"));
                        writer.WriteEndElement();


                        writer.WriteStartElement("Title");
                        writer.WriteString(string.Format("{0}", material.Title));
                        writer.WriteEndElement();
                        writer.WriteStartElement("Body");
                        writer.WriteString(string.Format("{0}", material.Body));
                        writer.WriteEndElement();
                        writer.WriteStartElement("File");
                        writer.WriteString(string.Format("{0}", material.FileTitle));
                        writer.WriteEndElement();
                        #endregion

                        if (item.ObjectType == (int)ObjectType.Forum)
                        {
                            var parentDiscussions = ForumDAL.Get(item.ObjectId).ForumDiscussions.Where(x => !x.ParentId.HasValue).ToList();
                            foreach (var discParent in parentDiscussions)
                            {
                                writer.WriteStartElement("FroumDisccusions");
                                foreach (var disc in ForumDiscussionDAL.GetAllDiscussions(discParent.Id).OrderBy(x => x.CreateDate))
                                {
                                    writer.WriteStartElement("Discussion");

                                    writer.WriteStartElement("CreateUser");
                                    writer.WriteString(string.Format("{0} {1}", disc.App_User.FirstName, disc.App_User.LastName));
                                    writer.WriteEndElement();
                                    writer.WriteStartElement("CreateDate");
                                    writer.WriteString(string.Format("{0}", disc.CreateDate.Value.ToString("MM/dd/yyyy HH:mm:ss")));
                                    writer.WriteEndElement();
                                    writer.WriteStartElement("Vote");
                                    writer.WriteString(string.Format("{0}", VoteDAL.GetAllCount(disc.Id, (int)ObjectType.ForumDiscussion)));
                                    writer.WriteEndElement();

                                    if (disc.ParentId.HasValue)
                                    {
                                        tags = TagMapperDAL.GetAll(disc.Id, (int)ObjectType.ForumDiscussion);
                                        tagString = string.Empty;
                                        foreach (var tag in tags)
                                        {
                                            tagString += tag.Tag.Title + ", ";
                                        }
                                        writer.WriteStartElement("Tags");
                                        writer.WriteString(string.Format("{0}", tagString));
                                        writer.WriteEndElement();
                                    }

                                    writer.WriteStartElement("Body");
                                    writer.WriteString(string.Format("{0}", disc.Body));
                                    writer.WriteEndElement();
                                    writer.WriteStartElement("File");
                                    writer.WriteString(string.Format("{0}", disc.FileTitle));
                                    writer.WriteEndElement();

                                    writer.WriteEndElement();
                                }
                                writer.WriteEndElement();
                            }

                        }

                        //writer.WriteEndElement();
                        writer.WriteEndElement();
                    }
                }
                // writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            return File("~/Exports/CourseMaterials.xml", "text/xml", "CourseMaterials.xml");
        }

        public ActionResult ExportCourseSocialActions(int courseId)
        {
            XmlTextWriter writer = new XmlTextWriter(Server.MapPath("~/Exports/CourseSocialActions.xml"), System.Text.Encoding.UTF8);
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.WriteStartElement("CourseSocialActions");

            writer.WriteStartElement("Course");
            writer.WriteString(string.Format("{0}", CourseDAL.Get(courseId).Title));
            writer.WriteEndElement();

            #region Comment

            var comments = CommentDAL.GetAll();
            //writer.WriteStartElement("Comments");
            foreach (var comment in comments)
            {
                var sharedObject = ManageObject.GetSharedObject(comment.ObjectId, comment.Type);
                if (sharedObject.CourseId == courseId)
                {
                    writer.WriteStartElement("SocialMaterial");

                    writer.WriteStartElement("MaterialType");
                    writer.WriteString("Comment");
                    writer.WriteEndElement();

                    writer.WriteStartElement("CreateUser");
                    writer.WriteString(string.Format("{0} {1}", comment.App_User.FirstName, comment.App_User.LastName));
                    writer.WriteEndElement();

                    writer.WriteStartElement("CreateDate");
                    writer.WriteString(string.Format("{0}", comment.CreateDate.Value.ToString("MM/dd/yyyy HH:mm:ss")));
                    writer.WriteEndElement();

                    writer.WriteStartElement("CommentBody");
                    writer.WriteString(string.Format("{0}", comment.Title));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedTo");
                    writer.WriteString(string.Format("{0}", ((ObjectType)comment.Type).ToString()));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToBody");
                    writer.WriteString(string.Format("{0}", sharedObject.Title ?? sharedObject.Body ?? sharedObject.FileTitle));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToType");
                    writer.WriteString(string.Format("{0}", sharedObject.Type));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToId");
                    writer.WriteString(string.Format("{0}", sharedObject.Id));
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
            }
            //writer.WriteEndElement();

            #endregion

            #region Tag

            var tags = TagMapperDAL.GetAll();
            //writer.WriteStartElement("Tags");
            foreach (var tag in tags)
            {
                var sharedObject = ManageObject.GetSharedObject(tag.ObjectId, tag.ObjectType);
                if (sharedObject.CourseId == courseId)
                {
                    writer.WriteStartElement("SocialMaterial");

                    writer.WriteStartElement("MaterialType");
                    writer.WriteString("Tag");
                    writer.WriteEndElement();

                    writer.WriteStartElement("CreateUser");
                    writer.WriteString(string.Format("{0} {1}", tag.App_User.FirstName, tag.App_User.LastName));
                    writer.WriteEndElement();

                    writer.WriteStartElement("CreateDate");
                    writer.WriteString(string.Format("{0}", tag.CreateDate.Value.ToString("MM/dd/yyyy HH:mm:ss")));
                    writer.WriteEndElement();

                    writer.WriteStartElement("TagBody");
                    writer.WriteString(string.Format("{0}", tag.Tag.Title));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedTo");
                    writer.WriteString(string.Format("{0}", ((ObjectType)tag.ObjectType).ToString()));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToBody");
                    writer.WriteString(string.Format("{0}", sharedObject.Title ?? sharedObject.Body ?? sharedObject.FileTitle));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToType");
                    writer.WriteString(string.Format("{0}", sharedObject.Type));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToId");
                    writer.WriteString(string.Format("{0}", sharedObject.Id));
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
            }
            //writer.WriteEndElement();

            #endregion

            #region Topic

            var topics = ObjectTopicMapperDAL.GetAll();
            //writer.WriteStartElement("Topics");
            foreach (var topic in topics)
            {
                var sharedObject = ManageObject.GetSharedObject(topic.ObjectId, topic.ObjectType);
                if (sharedObject.CourseId == courseId)
                {
                    writer.WriteStartElement("SocialMaterial");

                    writer.WriteStartElement("MaterialType");
                    writer.WriteString("Topic");
                    writer.WriteEndElement();

                    writer.WriteStartElement("CreateUser");
                    writer.WriteString(string.Format("{0} {1}", sharedObject.CreateUser.FirstName, sharedObject.CreateUser.LastName));
                    writer.WriteEndElement();

                    writer.WriteStartElement("CreateDate");
                    writer.WriteString(string.Format("{0}", sharedObject.CreateDate.ToString("MM/dd/yyyy HH:mm:ss")));
                    writer.WriteEndElement();

                    writer.WriteStartElement("TopicBody");
                    writer.WriteString(string.Format("{0}", topic.Topic.Title));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedTo");
                    writer.WriteString(string.Format("{0}", ((ObjectType)topic.ObjectType).ToString()));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToBody");
                    writer.WriteString(string.Format("{0}", sharedObject.Title ?? sharedObject.Body ?? sharedObject.FileTitle));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToType");
                    writer.WriteString(string.Format("{0}", sharedObject.Type));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToId");
                    writer.WriteString(string.Format("{0}", sharedObject.Id));
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
            }
            //writer.WriteEndElement();

            #endregion

            #region Vote

            var votes = VoteParentDAL.GetAll();
            //writer.WriteStartElement("Votes");
            foreach (var vote in votes.Where(x => x.Votes.Any()))
            {
                var sharedObject = ManageObject.GetSharedObject(vote.ObjectId, vote.ObjectType.Value);
                if (sharedObject.CourseId == courseId)
                {
                    writer.WriteStartElement("SocialMaterial");

                    writer.WriteStartElement("MaterialType");
                    writer.WriteString("Vote");
                    writer.WriteEndElement();

                    writer.WriteStartElement("GivenVotes");
                    foreach (var item in vote.Votes)
                    {
                        writer.WriteStartElement("Vote");

                        writer.WriteStartElement("CreateUser");
                        writer.WriteString(string.Format("{0} {1}", item.App_User.FirstName, item.App_User.LastName));
                        writer.WriteEndElement();

                        writer.WriteStartElement("CreateDate");
                        writer.WriteString(string.Format("{0}", item.CreateDate.Value.ToString("MM/dd/yyyy HH:mm:ss")));
                        writer.WriteEndElement();

                        writer.WriteStartElement("NumberVote");
                        writer.WriteString(string.Format("{0}", item.VoteValue));
                        writer.WriteEndElement();

                        writer.WriteStartElement("Like-Dislike");
                        writer.WriteString(string.Format("{0}", item.Updown));
                        writer.WriteEndElement();

                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("Count");
                    writer.WriteString(string.Format("{0}", vote.Count));
                    writer.WriteEndElement();

                    writer.WriteStartElement("UpvoteCount");
                    writer.WriteString(string.Format("{0}", vote.UpvoteCount));
                    writer.WriteEndElement();

                    writer.WriteStartElement("DownvoteCount");
                    writer.WriteString(string.Format("{0}", vote.DownvoteCount));
                    writer.WriteEndElement();

                    writer.WriteStartElement("VoteValueAverage");
                    writer.WriteString(string.Format("{0}", vote.VoteValueAverage));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedTo");
                    writer.WriteString(string.Format("{0}", ((ObjectType)vote.ObjectType).ToString()));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToBody");
                    writer.WriteString(string.Format("{0}", sharedObject.Title ?? sharedObject.Body ?? sharedObject.FileTitle));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToType");
                    writer.WriteString(string.Format("{0}", sharedObject.Type));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToId");
                    writer.WriteString(string.Format("{0}", sharedObject.Id));
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
            }
            //writer.WriteEndElement();

            #endregion

            #region Share

            writer.WriteStartElement("SocialMaterial");

            writer.WriteStartElement("MaterialType");
            writer.WriteString("Share");
            writer.WriteEndElement();

            writer.WriteEndElement();

            #endregion

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            return File("~/Exports/CourseSocialActions.xml", "text/xml", "CourseSocialActions.xml");
        }

        public ActionResult ExportCourseLearningActions(int courseId)
        {
            XmlTextWriter writer = new XmlTextWriter(Server.MapPath("~/Exports/CourseLearningActions.xml"), System.Text.Encoding.UTF8);
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.WriteStartElement("CourseLearningActions");

            writer.WriteStartElement("Course");
            writer.WriteString(string.Format("{0}", CourseDAL.Get(courseId).Title));
            writer.WriteEndElement();

            #region Submission

            var submissions = AssignmentSubmissionDAL.GetAll();
            //writer.WriteStartElement("Submissions");
            foreach (var submission in submissions)
            {
                var sharedObject = ManageObject.GetSharedObject(submission.Id, (int)ObjectType.AssignmentSubmission);
                if (sharedObject.CourseId == courseId)
                {
                    writer.WriteStartElement("LearningMaterial");

                    writer.WriteStartElement("MaterialType");
                    writer.WriteString("Submission");
                    writer.WriteEndElement();

                    writer.WriteStartElement("CreateUser");
                    writer.WriteString(string.Format("{0} {1}", submission.App_User.FirstName, submission.App_User.LastName));
                    writer.WriteEndElement();

                    writer.WriteStartElement("CreateDate");
                    writer.WriteString(string.Format("{0}", submission.CreateDate.ToString("MM/dd/yyyy HH:mm:ss")));
                    writer.WriteEndElement();

                    writer.WriteStartElement("GradeValue");
                    writer.WriteString(string.Format("{0}", GradeDAL.Get(submission.Id, (int)ObjectType.AssignmentSubmission)));
                    writer.WriteEndElement();

                    writer.WriteStartElement("SubmissionBody");
                    writer.WriteString(string.Format("{0}", submission.Body));
                    writer.WriteEndElement();

                    writer.WriteStartElement("SubmissionFile");
                    writer.WriteString(string.Format("{0}", submission.FileTitle));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToBody");
                    writer.WriteString(string.Format("{0}", sharedObject.Title ?? sharedObject.Body ?? sharedObject.FileTitle));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToType");
                    writer.WriteString(string.Format("{0}", sharedObject.Type));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToId");
                    writer.WriteString(string.Format("{0}", sharedObject.Id));
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
            }
            //writer.WriteEndElement();

            #endregion

            #region Grades

            var grades = GradeDAL.GetAllCourse(courseId);
            //writer.WriteStartElement("Grades");
            foreach (var grade in grades)
            {
                var sharedObject = ManageObject.GetSharedObject(grade.ObjectId.Value, grade.ObjectType.Value);
                if (sharedObject.CourseId == courseId)
                {
                    writer.WriteStartElement("LearningMaterial");

                    writer.WriteStartElement("MaterialType");
                    writer.WriteString("Grade");
                    writer.WriteEndElement();

                    writer.WriteStartElement("CreateDate");
                    writer.WriteString(string.Format("{0}", grade.CreateDate.ToString("MM/dd/yyyy HH:mm:ss")));
                    writer.WriteEndElement();

                    writer.WriteStartElement("GradeFor");
                    writer.WriteString(string.Format("{0} {1}", grade.App_User1.FirstName, grade.App_User1.LastName));
                    writer.WriteEndElement();

                    writer.WriteStartElement("GradedBy");
                    writer.WriteString(string.Format("{0} {1}", grade.App_User.FirstName, grade.App_User.LastName));
                    writer.WriteEndElement();

                    writer.WriteStartElement("GradeValue");
                    writer.WriteString(string.Format("{0}", grade.GradeValue));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedTo");
                    writer.WriteString(string.Format("{0}", ((ObjectType)grade.ObjectType).ToString()));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToBody");
                    writer.WriteString(string.Format("{0}", sharedObject.Title ?? sharedObject.Body ?? sharedObject.FileTitle));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToType");
                    writer.WriteString(string.Format("{0}", sharedObject.Type));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToId");
                    writer.WriteString(string.Format("{0}", sharedObject.Id));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToParent");
                    writer.WriteString(string.Format("{0}", ((ObjectType)grade.ParentObjectId).ToString()));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToParentBody");
                    writer.WriteString(string.Format("{0}", sharedObject.CameFromTitle));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToParentType");
                    writer.WriteString(string.Format("{0}", grade.ParentObjectId));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToParentId");
                    writer.WriteString(string.Format("{0}", grade.ParentObjectId));
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
            }
            //writer.WriteEndElement();

            #endregion

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            return File("~/Exports/CourseLearningActions.xml", "text/xml", "CourseLearningActions.xml");

        }

        public ActionResult ExportCourseContents(int courseId)
        {
            XmlTextWriter writer = new XmlTextWriter(Server.MapPath("~/Exports/CourseContents.xml"), System.Text.Encoding.UTF8);
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.WriteStartElement("CourseContents");
            var materials = CourseDAL.GetAllMaterials(courseId);
            writer.WriteStartElement("Course");
            writer.WriteString(string.Format("{0}", CourseDAL.Get(courseId).Title));
            writer.WriteEndElement();
            foreach (var group in materials.GroupBy(x => x.ObjectType))
            {
                //writer.WriteStartElement(((ObjectType)group.First().ObjectType).ToString() + "s");
                foreach (var item in group)
                {
                    var material = ManageObject.GetSharedObject(item.ObjectId, item.ObjectType);
                    if (material.CreateUser != null)
                    {
                        writer.WriteStartElement("Material");

                        writer.WriteStartElement("MaterialType");
                        writer.WriteString(string.Format("{0}", ((ObjectType)item.ObjectType).ToString()));
                        writer.WriteEndElement();

                        //writer.WriteStartElement("Metadata");
                        #region  metadata
                        writer.WriteStartElement("ObjectId");
                        writer.WriteString(string.Format("{0}", item.ObjectId));
                        writer.WriteEndElement();
                        writer.WriteStartElement("ObjectType");
                        writer.WriteString(string.Format("{0}", item.ObjectType));
                        writer.WriteEndElement();
                        writer.WriteStartElement("Popularity");
                        writer.WriteString(string.Format("{0}", ContentPopularityModelDAL.GetScore(item.ObjectId, item.ObjectType, courseId)));
                        writer.WriteEndElement();
                        writer.WriteStartElement("CreateUser");
                        writer.WriteString(string.Format("{0} {1}", material.CreateUser.FirstName, material.CreateUser.LastName));
                        writer.WriteEndElement();
                        writer.WriteStartElement("CreateDate");
                        writer.WriteString(string.Format("{0}", material.CreateDate.ToString("MM/dd/yyyy HH:mm:ss")));
                        writer.WriteEndElement();

                        var topics = ObjectTopicMapperDAL.GetAll(item.ObjectId, item.ObjectType);
                        var topicString = string.Empty;
                        foreach (var topic in topics)
                        {
                            topicString += topic.Title + ", ";
                        }
                        writer.WriteStartElement("Topics");
                        writer.WriteString(string.Format("{0}", topicString));
                        writer.WriteEndElement();

                        var tags = TagMapperDAL.GetAll(item.ObjectId, item.ObjectType);
                        var tagString = string.Empty;
                        foreach (var tag in tags)
                        {
                            tagString += tag.Tag.Title + ", ";
                        }
                        writer.WriteStartElement("Tags");
                        writer.WriteString(string.Format("{0}", tagString));
                        writer.WriteEndElement();

                        writer.WriteStartElement("Vote");
                        writer.WriteString(string.Format("{0}", VoteDAL.GetAllCount(item.ObjectId, item.ObjectType)));
                        writer.WriteEndElement();

                        writer.WriteStartElement("Comment");
                        writer.WriteString(string.Format("{0}", CommentDAL.GetAllCount(item.ObjectId, item.ObjectType)));
                        writer.WriteEndElement();

                        writer.WriteStartElement("Share");
                        writer.WriteString(string.Format("{0}", "0"));
                        writer.WriteEndElement();


                        writer.WriteStartElement("Title");
                        writer.WriteString(string.Format("{0}", material.Title));
                        writer.WriteEndElement();
                        writer.WriteStartElement("Body");
                        writer.WriteString(string.Format("{0}", material.Body));
                        writer.WriteEndElement();
                        writer.WriteStartElement("File");
                        writer.WriteString(string.Format("{0}", material.FileTitle));
                        writer.WriteEndElement();
                        #endregion

                        if (item.ObjectType == (int)ObjectType.Forum)
                        {
                            var parentDiscussions = ForumDAL.Get(item.ObjectId).ForumDiscussions.Where(x => !x.ParentId.HasValue).ToList();
                            foreach (var discParent in parentDiscussions)
                            {
                                writer.WriteStartElement("FroumDisccusions");
                                foreach (var disc in ForumDiscussionDAL.GetAllDiscussions(discParent.Id).OrderBy(x => x.CreateDate))
                                {
                                    writer.WriteStartElement("Discussion");

                                    writer.WriteStartElement("CreateUser");
                                    writer.WriteString(string.Format("{0} {1}", disc.App_User.FirstName, disc.App_User.LastName));
                                    writer.WriteEndElement();
                                    writer.WriteStartElement("CreateDate");
                                    writer.WriteString(string.Format("{0}", disc.CreateDate.Value.ToString("MM/dd/yyyy HH:mm:ss")));
                                    writer.WriteEndElement();
                                    writer.WriteStartElement("Vote");
                                    writer.WriteString(string.Format("{0}", VoteDAL.GetAllCount(disc.Id, (int)ObjectType.ForumDiscussion)));
                                    writer.WriteEndElement();

                                    if (disc.ParentId.HasValue)
                                    {
                                        tags = TagMapperDAL.GetAll(disc.Id, (int)ObjectType.ForumDiscussion);
                                        tagString = string.Empty;
                                        foreach (var tag in tags)
                                        {
                                            tagString += tag.Tag.Title + ", ";
                                        }
                                        writer.WriteStartElement("Tags");
                                        writer.WriteString(string.Format("{0}", tagString));
                                        writer.WriteEndElement();
                                    }

                                    writer.WriteStartElement("Body");
                                    writer.WriteString(string.Format("{0}", disc.Body));
                                    writer.WriteEndElement();
                                    writer.WriteStartElement("File");
                                    writer.WriteString(string.Format("{0}", disc.FileTitle));
                                    writer.WriteEndElement();

                                    writer.WriteEndElement();
                                }
                                writer.WriteEndElement();
                            }

                        }

                        //writer.WriteEndElement();
                        writer.WriteEndElement();
                    }
                }
                // writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            return File("~/Exports/CourseContents.xml", "text/xml", "CourseContents.xml");
        }

        public ActionResult ExportCourseMaterials2(int courseId)
        {
            XmlTextWriter writer = new XmlTextWriter(Server.MapPath("~/Exports/CourseMaterials2.xml"), System.Text.Encoding.UTF8);
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.WriteStartElement("CourseMaterials");
            writer.WriteStartElement("Course");
            writer.WriteString(string.Format("{0}", CourseDAL.Get(courseId).Title));
            writer.WriteEndElement();

            var resources = ResourceDAL.GetAllByCourse(courseId).Where(x => x.IsPublishd && x.IsValid);
            foreach (var item in resources.OrderBy(x => x.CreateDate))
            {
                writer.WriteStartElement("Content");

                writer.WriteStartElement("Action");
                writer.WriteString(string.Format("{0}", "Resource"));
                writer.WriteEndElement();

                writer.WriteStartElement("Id");
                writer.WriteString(string.Format("{0}", item.Id));
                writer.WriteEndElement();

                writer.WriteStartElement("Title");
                if (!string.IsNullOrEmpty(item.Title))
                {
                    writer.WriteString(string.Format("{0}", item.Title));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("Body");
                if (!string.IsNullOrEmpty(item.Body))
                {
                    writer.WriteString(string.Format("{0}", item.Body));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("File");
                if (!string.IsNullOrEmpty(item.FileTitle))
                {
                    writer.WriteString(string.Format("{0}", item.FileTitle));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("CreateUserFirstName");
                writer.WriteString(string.Format("{0}", item.App_User.FirstName));
                writer.WriteEndElement();

                writer.WriteStartElement("CreateUserLastName");
                writer.WriteString(string.Format("{0}", item.App_User.LastName));
                writer.WriteEndElement();

                writer.WriteStartElement("CreateDate");
                writer.WriteString(string.Format("{0}", item.CreateDate.ToString("MM/dd/yyyy HH:mm:ss")));
                writer.WriteEndElement();

                writer.WriteStartElement("CommentsCount");
                writer.WriteString(string.Format("{0}", CommentDAL.GetAllCount(item.Id, (int)ObjectType.Resource)));
                writer.WriteEndElement();

                writer.WriteStartElement("TopicsCount");
                writer.WriteString(string.Format("{0}", ObjectTopicMapperDAL.GetAllCount(item.Id, (int)ObjectType.Resource)));
                writer.WriteEndElement();

                var topics = ObjectTopicMapperDAL.GetAll(item.Id, (int)ObjectType.Resource);
                var topicString = string.Empty;
                foreach (var topic in topics)
                {
                    topicString += topic.Title + " , ";
                }
                writer.WriteStartElement("Topics");
                writer.WriteString(string.Format("{0}", topicString));
                writer.WriteEndElement();

                writer.WriteStartElement("TagsCount");
                writer.WriteString(string.Format("{0}", TagMapperDAL.GetAllCount(item.Id, (int)ObjectType.Resource)));
                writer.WriteEndElement();

                var tags = TagMapperDAL.GetAll(item.Id, (int)ObjectType.Resource);
                var tagString = string.Empty;
                foreach (var tag in tags)
                {
                    tagString += tag.Tag.Title + " , ";
                }
                writer.WriteStartElement("Tags");
                writer.WriteString(string.Format("{0}", tagString));
                writer.WriteEndElement();

                writer.WriteStartElement("VotesCount");
                writer.WriteString(string.Format("{0}", VoteDAL.GetAllCount(item.Id, (int)ObjectType.Resource)));
                writer.WriteEndElement();

                writer.WriteStartElement("Popularity");
                writer.WriteString(string.Format("{0}", ContentPopularityModelDAL.GetScore(item.Id, (int)ObjectType.Resource, courseId)));
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            var assignments = AssignmentDAL.GetAllByCourse(courseId).Where(x => x.IsPublished && x.IsValid);
            foreach (var item in assignments.OrderBy(x => x.CreateDate))
            {
                writer.WriteStartElement("Content");

                writer.WriteStartElement("Action");
                writer.WriteString(string.Format("{0}", "Assignment"));
                writer.WriteEndElement();

                writer.WriteStartElement("Id");
                writer.WriteString(string.Format("{0}", item.Id));
                writer.WriteEndElement();

                writer.WriteStartElement("Title");
                if (!string.IsNullOrEmpty(item.Title))
                {
                    writer.WriteString(string.Format("{0}", item.Title));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("Body");
                if (!string.IsNullOrEmpty(item.Body))
                {
                    writer.WriteString(string.Format("{0}", item.Body));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("File");
                if (!string.IsNullOrEmpty(item.FileTitle))
                {
                    writer.WriteString(string.Format("{0}", item.FileTitle));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("CreateUserFirstName");
                writer.WriteString(string.Format("{0}", item.App_User.FirstName));
                writer.WriteEndElement();

                writer.WriteStartElement("CreateUserLastName");
                writer.WriteString(string.Format("{0}", item.App_User.LastName));
                writer.WriteEndElement();

                writer.WriteStartElement("CreateDate");
                writer.WriteString(string.Format("{0}", item.CreateDate.ToString("MM/dd/yyyy HH:mm:ss")));
                writer.WriteEndElement();

                writer.WriteStartElement("CommentsCount");
                writer.WriteString(string.Format("{0}", CommentDAL.GetAllCount(item.Id, (int)ObjectType.Assignment)));
                writer.WriteEndElement();

                writer.WriteStartElement("TopicsCount");
                writer.WriteString(string.Format("{0}", ObjectTopicMapperDAL.GetAllCount(item.Id, (int)ObjectType.Assignment)));
                writer.WriteEndElement();

                var topics = ObjectTopicMapperDAL.GetAll(item.Id, (int)ObjectType.Assignment);
                var topicString = string.Empty;
                foreach (var topic in topics)
                {
                    topicString += topic.Title + " , ";
                }
                writer.WriteStartElement("Topics");
                writer.WriteString(string.Format("{0}", topicString));
                writer.WriteEndElement();

                writer.WriteStartElement("TagsCount");
                writer.WriteString(string.Format("{0}", TagMapperDAL.GetAllCount(item.Id, (int)ObjectType.Assignment)));
                writer.WriteEndElement();

                var tags = TagMapperDAL.GetAll(item.Id, (int)ObjectType.Assignment);
                var tagString = string.Empty;
                foreach (var tag in tags)
                {
                    tagString += tag.Tag.Title + " , ";
                }
                writer.WriteStartElement("Tags");
                writer.WriteString(string.Format("{0}", tagString));
                writer.WriteEndElement();

                writer.WriteStartElement("VotesCount");
                writer.WriteString(string.Format("{0}", VoteDAL.GetAllCount(item.Id, (int)ObjectType.Assignment)));
                writer.WriteEndElement();

                writer.WriteStartElement("Popularity");
                writer.WriteString(string.Format("{0}", ContentPopularityModelDAL.GetScore(item.Id, (int)ObjectType.Assignment, courseId)));
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            var forums = ForumDAL.GetAllByCourse(courseId).Where(x => x.IsPublishd && x.IsValid);
            foreach (var item in forums.OrderBy(x => x.CreateDate))
            {
                writer.WriteStartElement("Content");

                writer.WriteStartElement("Action");
                writer.WriteString(string.Format("{0}", "Forum"));
                writer.WriteEndElement();

                writer.WriteStartElement("Id");
                writer.WriteString(string.Format("{0}", item.Id));
                writer.WriteEndElement();

                writer.WriteStartElement("Title");
                if (!string.IsNullOrEmpty(item.Title))
                {
                    writer.WriteString(string.Format("{0}", item.Title));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("Body");
                if (!string.IsNullOrEmpty(item.Body))
                {
                    writer.WriteString(string.Format("{0}", item.Body));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("File");
                if (!string.IsNullOrEmpty(item.FileTitle))
                {
                    writer.WriteString(string.Format("{0}", item.FileTitle));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("CreateUserFirstName");
                writer.WriteString(string.Format("{0}", item.App_User.FirstName));
                writer.WriteEndElement();

                writer.WriteStartElement("CreateUserLastName");
                writer.WriteString(string.Format("{0}", item.App_User.LastName));
                writer.WriteEndElement();

                writer.WriteStartElement("CreateDate");
                if (item.CreateDate.HasValue)
                {
                    writer.WriteString(string.Format("{0}", item.CreateDate.Value.ToString("MM/dd/yyyy HH:mm:ss")));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("CommentsCount");
                writer.WriteString(string.Format("{0}", ForumDiscussionDAL.GetDiscussionsCount(item.Id)));
                writer.WriteEndElement();

                writer.WriteStartElement("TopicsCount");
                writer.WriteString(string.Format("{0}", ObjectTopicMapperDAL.GetAllCount(item.Id, (int)ObjectType.Forum)));
                writer.WriteEndElement();

                var topics = ObjectTopicMapperDAL.GetAll(item.Id, (int)ObjectType.Forum);
                var topicString = string.Empty;
                foreach (var topic in topics)
                {
                    topicString += topic.Title + " , ";
                }
                writer.WriteStartElement("Topics");
                writer.WriteString(string.Format("{0}", topicString));
                writer.WriteEndElement();

                writer.WriteStartElement("TagsCount");
                writer.WriteString(string.Format("{0}", TagMapperDAL.GetAllCount(item.Id, (int)ObjectType.Forum)));
                writer.WriteEndElement();

                var tags = TagMapperDAL.GetAll(item.Id, (int)ObjectType.Forum);
                var tagString = string.Empty;
                foreach (var tag in tags)
                {
                    tagString += tag.Tag.Title + " , ";
                }
                writer.WriteStartElement("Tags");
                writer.WriteString(string.Format("{0}", tagString));
                writer.WriteEndElement();

                writer.WriteStartElement("VotesCount");
                writer.WriteString(string.Format("{0}", VoteDAL.GetAllCount(item.Id, (int)ObjectType.Forum)));
                writer.WriteEndElement();

                writer.WriteStartElement("Popularity");
                writer.WriteString(string.Format("{0}", ContentPopularityModelDAL.GetScore(item.Id, (int)ObjectType.Forum, courseId)));
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            var forumDicsAdd = ForumDiscussionDAL.GetAllByCourseNew(courseId).Where(x => x.IsPublishd && x.IsValid);
            foreach (var item in forumDicsAdd.OrderBy(x => x.CreateDate))
            {
                writer.WriteStartElement("Content");

                writer.WriteStartElement("Action");
                writer.WriteString(string.Format("{0}", "ForumDiscussion"));
                writer.WriteEndElement();

                writer.WriteStartElement("Id");
                writer.WriteString(string.Format("{0}", item.Id));
                writer.WriteEndElement();

                writer.WriteStartElement("Title");
                if (!string.IsNullOrEmpty(item.Title))
                {
                    writer.WriteString(string.Format("{0}", item.Title));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("Body");
                if (!string.IsNullOrEmpty(item.Body))
                {
                    writer.WriteString(string.Format("{0}", item.Body));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("File");
                if (!string.IsNullOrEmpty(item.FileTitle))
                {
                    writer.WriteString(string.Format("{0}", item.FileTitle));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("CreateUserFirstName");
                writer.WriteString(string.Format("{0}", item.App_User.FirstName));
                writer.WriteEndElement();

                writer.WriteStartElement("CreateUserLastName");
                writer.WriteString(string.Format("{0}", item.App_User.LastName));
                writer.WriteEndElement();

                writer.WriteStartElement("CreateDate");
                if (item.CreateDate.HasValue)
                {
                    writer.WriteString(string.Format("{0}", item.CreateDate.Value.ToString("MM/dd/yyyy HH:mm:ss")));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                } writer.WriteEndElement();

                writer.WriteStartElement("CommentsCount");
                writer.WriteString(string.Format("{0}", ForumDiscussionDAL.GetRepliesCount(item.Id)));
                writer.WriteEndElement();

                writer.WriteStartElement("TopicsCount");
                writer.WriteString(string.Format("{0}", ObjectTopicMapperDAL.GetAllCount(item.Id, (int)ObjectType.ForumDiscussion)));
                writer.WriteEndElement();

                var topics = ObjectTopicMapperDAL.GetAll(item.Id, (int)ObjectType.ForumDiscussion);
                var topicString = string.Empty;
                foreach (var topic in topics)
                {
                    topicString += topic.Title + " , ";
                }
                writer.WriteStartElement("Topics");
                writer.WriteString(string.Format("{0}", topicString));
                writer.WriteEndElement();

                writer.WriteStartElement("TagsCount");
                writer.WriteString(string.Format("{0}", TagMapperDAL.GetAllCount(item.Id, (int)ObjectType.ForumDiscussion)));
                writer.WriteEndElement();

                var tags = TagMapperDAL.GetAll(item.Id, (int)ObjectType.ForumDiscussion);
                var tagString = string.Empty;
                foreach (var tag in tags)
                {
                    tagString += tag.Tag.Title + " , ";
                }
                writer.WriteStartElement("Tags");
                writer.WriteString(string.Format("{0}", tagString));
                writer.WriteEndElement();

                writer.WriteStartElement("VotesCount");
                writer.WriteString(string.Format("{0}", VoteDAL.GetAllCount(item.Id, (int)ObjectType.ForumDiscussion)));
                writer.WriteEndElement();

                writer.WriteStartElement("Popularity");
                writer.WriteString(string.Format("{0}", ContentPopularityModelDAL.GetScore(item.Id, (int)ObjectType.ForumDiscussion, courseId)));
                writer.WriteEndElement();

                writer.WriteEndElement();
            }



            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            return File("~/Exports/CourseMaterials2.xml", "text/xml", "CourseMaterials2.xml");

        }

        /////// very bad code. only brute force, must be optimized!
        public ActionResult ExportCourseMetada(int courseId)
        {
            XmlTextWriter writer = new XmlTextWriter(Server.MapPath("~/Exports/CourseMetadta.xml"), System.Text.Encoding.UTF8);
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.WriteStartElement("CourseMetadata");
            writer.WriteStartElement("Course");
            writer.WriteString(string.Format("{0}", CourseDAL.Get(courseId).Title));
            writer.WriteEndElement();

            var resources = ResourceDAL.GetAllByCourse(courseId).Where(x => x.IsPublishd && x.IsValid);
            foreach (var item in resources.OrderBy(x => x.CreateDate))
            {
                writer.WriteStartElement("Data");

                writer.WriteStartElement("ActionType");
                writer.WriteString(string.Format("{0}", "Creation"));
                writer.WriteEndElement();

                writer.WriteStartElement("Action");
                writer.WriteString(string.Format("{0}", "New Resource"));
                writer.WriteEndElement();

                writer.WriteStartElement("ActionId");
                writer.WriteString(string.Format("{0}", item.Id));
                writer.WriteEndElement();

                writer.WriteStartElement("ActionBody");
                if (!string.IsNullOrEmpty(item.Title))
                {
                    writer.WriteString(string.Format("{0}", item.Title));
                }
                else if (!string.IsNullOrEmpty(item.Body))
                {
                    writer.WriteString(string.Format("{0}", item.Body));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("More");
                if (!string.IsNullOrEmpty(item.FileTitle))
                {
                    writer.WriteString(string.Format("{0}", item.FileTitle));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("CreateUserFirstName");
                writer.WriteString(string.Format("{0}", item.App_User.FirstName));
                writer.WriteEndElement();

                writer.WriteStartElement("CreateUserLastName");
                writer.WriteString(string.Format("{0}", item.App_User.LastName));
                writer.WriteEndElement();

                writer.WriteStartElement("CreateDate");
                writer.WriteString(string.Format("{0}", item.CreateDate.ToString("MM/dd/yyyy HH:mm:ss")));
                writer.WriteEndElement();

                writer.WriteStartElement("AssignedToType");
                writer.WriteString(string.Format("{0}", string.Empty));
                writer.WriteEndElement();

                writer.WriteStartElement("AssignedToId");
                writer.WriteString(string.Format("{0}", string.Empty));
                writer.WriteEndElement();

                writer.WriteStartElement("AssignedToBody");
                writer.WriteString(string.Format("{0}", string.Empty));
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            var assignments = AssignmentDAL.GetAllByCourse(courseId).Where(x => x.IsPublished && x.IsValid);
            foreach (var item in assignments.OrderBy(x => x.CreateDate))
            {
                writer.WriteStartElement("Data");

                writer.WriteStartElement("ActionType");
                writer.WriteString(string.Format("{0}", "Creation"));
                writer.WriteEndElement();

                writer.WriteStartElement("Action");
                writer.WriteString(string.Format("{0}", "New Assignment"));
                writer.WriteEndElement();

                writer.WriteStartElement("ActionId");
                writer.WriteString(string.Format("{0}", item.Id));
                writer.WriteEndElement();

                writer.WriteStartElement("ActionBody");
                if (!string.IsNullOrEmpty(item.Title))
                {
                    writer.WriteString(string.Format("{0}", item.Title));
                }
                else if (!string.IsNullOrEmpty(item.Body))
                {
                    writer.WriteString(string.Format("{0}", item.Body));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("More");
                if (!string.IsNullOrEmpty(item.FileTitle))
                {
                    writer.WriteString(string.Format("{0}", item.FileTitle));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("CreateUserFirstName");
                writer.WriteString(string.Format("{0}", item.App_User.FirstName));
                writer.WriteEndElement();

                writer.WriteStartElement("CreateUserLastName");
                writer.WriteString(string.Format("{0}", item.App_User.LastName));
                writer.WriteEndElement();

                writer.WriteStartElement("CreateDate");
                writer.WriteString(string.Format("{0}", item.CreateDate.ToString("MM/dd/yyyy HH:mm:ss")));
                writer.WriteEndElement();

                writer.WriteStartElement("AssignedToType");
                writer.WriteString(string.Format("{0}", string.Empty));
                writer.WriteEndElement();

                writer.WriteStartElement("AssignedToId");
                writer.WriteString(string.Format("{0}", string.Empty));
                writer.WriteEndElement();

                writer.WriteStartElement("AssignedToBody");
                writer.WriteString(string.Format("{0}", string.Empty));
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            var forums = ForumDAL.GetAllByCourse(courseId).Where(x => x.IsPublishd && x.IsValid);
            foreach (var item in forums.OrderBy(x => x.CreateDate))
            {
                writer.WriteStartElement("Data");

                writer.WriteStartElement("ActionType");
                writer.WriteString(string.Format("{0}", "Creation"));
                writer.WriteEndElement();

                writer.WriteStartElement("Action");
                writer.WriteString(string.Format("{0}", "New Forum"));
                writer.WriteEndElement();

                writer.WriteStartElement("ActionId");
                writer.WriteString(string.Format("{0}", item.Id));
                writer.WriteEndElement();

                writer.WriteStartElement("ActionBody");
                if (!string.IsNullOrEmpty(item.Title))
                {
                    writer.WriteString(string.Format("{0}", item.Title));
                }
                else if (!string.IsNullOrEmpty(item.Body))
                {
                    writer.WriteString(string.Format("{0}", item.Body));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("More");
                if (!string.IsNullOrEmpty(item.FileTitle))
                {
                    writer.WriteString(string.Format("{0}", item.FileTitle));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("CreateUserFirstName");
                writer.WriteString(string.Format("{0}", item.App_User.FirstName));
                writer.WriteEndElement();

                writer.WriteStartElement("CreateUserLastName");
                writer.WriteString(string.Format("{0}", item.App_User.LastName));
                writer.WriteEndElement();

                writer.WriteStartElement("CreateDate");
                if (item.CreateDate.HasValue)
                {
                    writer.WriteString(string.Format("{0}", item.CreateDate.Value.ToString("MM/dd/yyyy HH:mm:ss")));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("AssignedToType");
                writer.WriteString(string.Format("{0}", string.Empty));
                writer.WriteEndElement();

                writer.WriteStartElement("AssignedToId");
                writer.WriteString(string.Format("{0}", string.Empty));
                writer.WriteEndElement();

                writer.WriteStartElement("AssignedToBody");
                writer.WriteString(string.Format("{0}", string.Empty));
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            var forumDicsAdd = ForumDiscussionDAL.GetAllByCourseNew(courseId).Where(x => x.IsPublishd && x.IsValid);
            foreach (var item in forumDicsAdd.OrderBy(x => x.CreateDate))
            {
                writer.WriteStartElement("Data");

                writer.WriteStartElement("ActionType");
                writer.WriteString(string.Format("{0}", "Creation"));
                writer.WriteEndElement();

                writer.WriteStartElement("Action");
                writer.WriteString(string.Format("{0}", "Add Discussion"));
                writer.WriteEndElement();

                writer.WriteStartElement("ActionId");
                writer.WriteString(string.Format("{0}", item.Id));
                writer.WriteEndElement();

                writer.WriteStartElement("ActionBody");
                if (!string.IsNullOrEmpty(item.Title))
                {
                    writer.WriteString(string.Format("{0}", item.Title));
                }
                else if (!string.IsNullOrEmpty(item.Body))
                {
                    writer.WriteString(string.Format("{0}", item.Body));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("More");
                if (!string.IsNullOrEmpty(item.FileTitle))
                {
                    writer.WriteString(string.Format("{0}", item.FileTitle));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("CreateUserFirstName");
                writer.WriteString(string.Format("{0}", item.App_User.FirstName));
                writer.WriteEndElement();

                writer.WriteStartElement("CreateUserLastName");
                writer.WriteString(string.Format("{0}", item.App_User.LastName));
                writer.WriteEndElement();

                writer.WriteStartElement("CreateDate");
                if (item.CreateDate.HasValue)
                {
                    writer.WriteString(string.Format("{0}", item.CreateDate.Value.ToString("MM/dd/yyyy HH:mm:ss")));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("AssignedToType");
                writer.WriteString(string.Format("{0}", "Forum"));
                writer.WriteEndElement();

                writer.WriteStartElement("AssignedToId");
                writer.WriteString(string.Format("{0}", item.ForumId.ToString()));
                writer.WriteEndElement();

                writer.WriteStartElement("AssignedToBody");
                writer.WriteString(string.Format("{0}", item.Forum.Title));
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            var forumDicsReplies = ForumDiscussionDAL.GetAllByCourseReplies(courseId).Where(x => x.IsPublishd && x.IsValid);
            foreach (var item in forumDicsReplies.OrderBy(x => x.CreateDate))
            {
                writer.WriteStartElement("Data");

                writer.WriteStartElement("ActionType");
                writer.WriteString(string.Format("{0}", "Creation"));
                writer.WriteEndElement();

                writer.WriteStartElement("Action");
                writer.WriteString(string.Format("{0}", "Discussion Reply"));
                writer.WriteEndElement();

                writer.WriteStartElement("ActionId");
                writer.WriteString(string.Format("{0}", item.Id));
                writer.WriteEndElement();

                writer.WriteStartElement("ActionBody");
                if (!string.IsNullOrEmpty(item.Title))
                {
                    writer.WriteString(string.Format("{0}", item.Title));
                }
                else if (!string.IsNullOrEmpty(item.Body))
                {
                    writer.WriteString(string.Format("{0}", item.Body));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("More");
                if (!string.IsNullOrEmpty(item.FileTitle))
                {
                    writer.WriteString(string.Format("{0}", item.FileTitle));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("CreateUserFirstName");
                writer.WriteString(string.Format("{0}", item.App_User.FirstName));
                writer.WriteEndElement();

                writer.WriteStartElement("CreateUserLastName");
                writer.WriteString(string.Format("{0}", item.App_User.LastName));
                writer.WriteEndElement();

                writer.WriteStartElement("CreateDate");
                if (item.CreateDate.HasValue)
                {
                    writer.WriteString(string.Format("{0}", item.CreateDate.Value.ToString("MM/dd/yyyy HH:mm:ss")));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("AssignedToType");
                writer.WriteString(string.Format("{0}", "ForumDiscussion"));
                writer.WriteEndElement();

                writer.WriteStartElement("AssignedToId");
                writer.WriteString(string.Format("{0}", item.ParentId.Value.ToString()));
                writer.WriteEndElement();

                writer.WriteStartElement("AssignedToBody");
                if (!string.IsNullOrEmpty(item.ForumDiscussion1.Title))
                {
                    writer.WriteString(string.Format("{0}", item.ForumDiscussion1.Title));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            var assigSub = AssignmentSubmissionDAL.GetAllByCourse(courseId).Where(x => x.IsPublishd && x.IsValid);
            foreach (var item in assigSub.OrderBy(x => x.CreateDate))
            {
                writer.WriteStartElement("Data");

                writer.WriteStartElement("ActionType");
                writer.WriteString(string.Format("{0}", "Learning"));
                writer.WriteEndElement();

                writer.WriteStartElement("Action");
                writer.WriteString(string.Format("{0}", "Submit Assignment"));
                writer.WriteEndElement();

                writer.WriteStartElement("ActionId");
                writer.WriteString(string.Format("{0}", item.Id));
                writer.WriteEndElement();

                writer.WriteStartElement("ActionBody");
                if (!string.IsNullOrEmpty(item.Body))
                {
                    writer.WriteString(string.Format("{0}", item.Body));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("More");
                if (!string.IsNullOrEmpty(item.FileTitle))
                {
                    writer.WriteString(string.Format("{0}", item.FileTitle));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("CreateUserFirstName");
                writer.WriteString(string.Format("{0}", item.App_User.FirstName));
                writer.WriteEndElement();

                writer.WriteStartElement("CreateUserLastName");
                writer.WriteString(string.Format("{0}", item.App_User.LastName));
                writer.WriteEndElement();

                writer.WriteStartElement("CreateDate");
                writer.WriteString(string.Format("{0}", item.CreateDate.ToString("MM/dd/yyyy HH:mm:ss")));
                writer.WriteEndElement();

                writer.WriteStartElement("AssignedToType");
                writer.WriteString(string.Format("{0}", "Assignment"));
                writer.WriteEndElement();

                writer.WriteStartElement("AssignedToId");
                writer.WriteString(string.Format("{0}", item.AssignmentId));
                writer.WriteEndElement();

                writer.WriteStartElement("AssignedToBody");

                if (!string.IsNullOrEmpty(item.Assignment.Title))
                {
                    writer.WriteString(string.Format("{0}", item.Assignment.Title));
                }
                else if (!string.IsNullOrEmpty(item.Assignment.Body))
                {
                    writer.WriteString(string.Format("{0}", item.Assignment.Body));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            var grades = GradeDAL.GetAllByCourse(courseId);
            foreach (var item in grades.OrderBy(x => x.CreateDate))
            {
                writer.WriteStartElement("Data");

                writer.WriteStartElement("ActionType");
                writer.WriteString(string.Format("{0}", "Learning"));
                writer.WriteEndElement();

                writer.WriteStartElement("Action");
                writer.WriteString(string.Format("{0}", "Grade"));
                writer.WriteEndElement();

                writer.WriteStartElement("ActionId");
                writer.WriteString(string.Format("{0}", item.Id));
                writer.WriteEndElement();

                writer.WriteStartElement("ActionBody");
                if (!string.IsNullOrEmpty(item.Body))
                {
                    writer.WriteString(string.Format("{0}", item.Body));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("More");
                if (item.GradeValue.HasValue)
                {
                    writer.WriteString(string.Format("{0}", item.GradeValue.Value));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("CreateUserFirstName");
                writer.WriteString(string.Format("{0}", item.App_User.FirstName));
                writer.WriteEndElement();

                writer.WriteStartElement("CreateUserLastName");
                writer.WriteString(string.Format("{0}", item.App_User.LastName));
                writer.WriteEndElement();

                writer.WriteStartElement("CreateDate");
                writer.WriteString(string.Format("{0}", item.CreateDate.ToString("MM/dd/yyyy HH:mm:ss")));
                writer.WriteEndElement();

                writer.WriteStartElement("AssignedToType");
                if (item.ObjectType.HasValue && item.ObjectType.Value == (int)ObjectType.ForumDiscussion)
                {
                    writer.WriteString(string.Format("{0}", "ForumDiscussion"));
                }
                else if (item.ObjectType.HasValue && item.ObjectType.Value == (int)ObjectType.AssignmentSubmission)
                {
                    writer.WriteString(string.Format("{0}", "AssignmentSubmission"));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("AssignedToId");
                if (item.ObjectId.HasValue)
                {
                    writer.WriteString(string.Format("{0}", item.ObjectId.Value));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteStartElement("AssignedToBody");
                if (item.ObjectId.HasValue && item.ObjectType.HasValue)
                {
                    var getObject = ManageObject.GetSharedObject(item.ObjectId.Value, item.ObjectType.Value);
                    if (getObject != null)
                        writer.WriteString(string.Format("{0} , {1} , {2}", getObject.Title, getObject.Body, getObject.FileTitle));
                }
                else
                {
                    writer.WriteString(string.Format("{0}", string.Empty));
                }
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            foreach (var item in AssessDAL.GetAll())
            {
                var getObject = ManageObject.GetSharedObject(item.AssessParent.ObjectId, item.AssessParent.ObjectType);
                if (getObject.CourseId == courseId)
                {
                    writer.WriteStartElement("Data");

                    writer.WriteStartElement("ActionType");
                    writer.WriteString(string.Format("{0}", "Learning"));
                    writer.WriteEndElement();

                    writer.WriteStartElement("Action");
                    writer.WriteString(string.Format("{0}", "Asses"));
                    writer.WriteEndElement();

                    writer.WriteStartElement("ActionId");
                    writer.WriteString(string.Format("{0}", item.Id));
                    writer.WriteEndElement();

                    writer.WriteStartElement("ActionBody");
                    writer.WriteString(string.Format("{0}", ((AssessType)item.AssessValue).ToString()));
                    writer.WriteEndElement();

                    writer.WriteStartElement("More");
                    writer.WriteString(string.Format("{0}", item.AssessType));
                    writer.WriteEndElement();

                    writer.WriteStartElement("CreateUserFirstName");
                    writer.WriteString(string.Format("{0}", item.App_User.FirstName));
                    writer.WriteEndElement();

                    writer.WriteStartElement("CreateUserLastName");
                    writer.WriteString(string.Format("{0}", item.App_User.LastName));
                    writer.WriteEndElement();

                    writer.WriteStartElement("CreateDate");
                    if (item.CreateDate.HasValue)
                    {
                        writer.WriteString(string.Format("{0}", item.CreateDate.Value.ToString("MM/dd/yyyy HH:mm:ss")));
                    }
                    else
                    {
                        writer.WriteString(string.Format("{0}", string.Empty));
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToType");
                    writer.WriteString(string.Format("{0}", ((ObjectType)item.AssessParent.ObjectType).ToString()));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToId");
                    writer.WriteString(string.Format("{0}", item.AssessParent.ObjectId));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToBody");
                    if (!string.IsNullOrEmpty(getObject.CameFromTitle))
                    {
                        writer.WriteString(string.Format("{0}", getObject.CameFromTitle));
                    }
                    else
                    {
                        writer.WriteString(string.Format("{0}", string.Empty));
                    }
                    writer.WriteEndElement();

                    writer.WriteEndElement();

                }
            }

            foreach (var item in CommentDAL.GetAll().OrderBy(x => x.CreateDate))
            {
                var getObject = ManageObject.GetSharedObject(item.Id, (int)ObjectType.Comment);
                if (getObject.CourseId == courseId)
                {
                    writer.WriteStartElement("Data");

                    writer.WriteStartElement("ActionType");
                    writer.WriteString(string.Format("{0}", "Social"));
                    writer.WriteEndElement();

                    writer.WriteStartElement("Action");
                    writer.WriteString(string.Format("{0}", "Comment"));
                    writer.WriteEndElement();

                    writer.WriteStartElement("ActionId");
                    writer.WriteString(string.Format("{0}", item.Id));
                    writer.WriteEndElement();

                    writer.WriteStartElement("ActionBody");
                    if (!string.IsNullOrEmpty(item.Title))
                    {
                        writer.WriteString(string.Format("{0}", item.Title));
                    }
                    else
                    {
                        writer.WriteString(string.Format("{0}", string.Empty));
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("More");
                    writer.WriteString(string.Format("{0}", string.Empty));
                    writer.WriteEndElement();

                    writer.WriteStartElement("CreateUserFirstName");
                    writer.WriteString(string.Format("{0}", item.App_User.FirstName));
                    writer.WriteEndElement();

                    writer.WriteStartElement("CreateUserLastName");
                    writer.WriteString(string.Format("{0}", item.App_User.LastName));
                    writer.WriteEndElement();

                    writer.WriteStartElement("CreateDate");
                    if (item.CreateDate.HasValue)
                    {
                        writer.WriteString(string.Format("{0}", item.CreateDate.Value.ToString("MM/dd/yyyy HH:mm:ss")));
                    }
                    else
                    {
                        writer.WriteString(string.Format("{0}", string.Empty));
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToType");
                    writer.WriteString(string.Format("{0}", ((ObjectType)item.Type).ToString()));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToId");
                    writer.WriteString(string.Format("{0}", item.ObjectId));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToBody");
                    if (!string.IsNullOrEmpty(getObject.CameFromTitle))
                    {
                        writer.WriteString(string.Format("{0}", getObject.CameFromTitle));
                    }
                    else
                    {
                        writer.WriteString(string.Format("{0}", string.Empty));
                    } writer.WriteEndElement();

                    writer.WriteEndElement();
                }
            }

            foreach (var item in VoteDAL.GetAll())
            {
                if (item.VoteParent.ObjectType.HasValue)
                {
                    var getObject = ManageObject.GetSharedObject(item.Id, (int)ObjectType.Vote);

                    if (getObject.CourseId == courseId)
                    {
                        writer.WriteStartElement("Data");

                        writer.WriteStartElement("ActionType");
                        writer.WriteString(string.Format("{0}", "Social"));
                        writer.WriteEndElement();

                        writer.WriteStartElement("Action");
                        writer.WriteString(string.Format("{0}", "Vote"));
                        writer.WriteEndElement();

                        writer.WriteStartElement("ActionId");
                        writer.WriteString(string.Format("{0}", item.Id));
                        writer.WriteEndElement();

                        writer.WriteStartElement("ActionBody");
                        writer.WriteString(string.Format("{0}", item.VoteValue));
                        writer.WriteEndElement();

                        writer.WriteStartElement("More");
                        writer.WriteString(string.Format("{0}", string.Empty));
                        writer.WriteEndElement();

                        writer.WriteStartElement("CreateUserFirstName");
                        writer.WriteString(string.Format("{0}", item.App_User.FirstName));
                        writer.WriteEndElement();

                        writer.WriteStartElement("CreateUserLastName");
                        writer.WriteString(string.Format("{0}", item.App_User.LastName));
                        writer.WriteEndElement();

                        writer.WriteStartElement("CreateDate");
                        if (item.CreateDate.HasValue)
                        {
                            writer.WriteString(string.Format("{0}", item.CreateDate.Value.ToString("MM/dd/yyyy HH:mm:ss")));
                        }
                        else
                        {
                            writer.WriteString(string.Format("{0}", string.Empty));
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("AssignedToType");
                        if (item.VoteParent.ObjectType.HasValue)
                        {
                            writer.WriteString(string.Format("{0}", ((ObjectType)item.VoteParent.ObjectType.Value).ToString()));
                        }
                        else
                        {
                            writer.WriteString(string.Format("{0}", string.Empty));
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("AssignedToId");
                        writer.WriteString(string.Format("{0}", item.VoteParent.ObjectId));
                        writer.WriteEndElement();

                        writer.WriteStartElement("AssignedToBody");
                        if (!string.IsNullOrEmpty(getObject.CameFromTitle))
                        {
                            writer.WriteString(string.Format("{0}", getObject.CameFromTitle));
                        }
                        else
                        {
                            writer.WriteString(string.Format("{0}", string.Empty));
                        }
                        writer.WriteEndElement();

                        writer.WriteEndElement();
                    }
                }
            }

            foreach (var item in ShareDAL.GetAll())
            {
                if (item.Type.HasValue)
                {
                    var getObject = ManageObject.GetSharedObject(item.Id, (int)ObjectType.Share);
                    if (getObject.CourseId == courseId)
                    {
                        writer.WriteStartElement("Data");

                        writer.WriteStartElement("ActionType");
                        writer.WriteString(string.Format("{0}", "Social"));
                        writer.WriteEndElement();

                        writer.WriteStartElement("Action");
                        writer.WriteString(string.Format("{0}", "Share"));
                        writer.WriteEndElement();

                        writer.WriteStartElement("ActionId");
                        writer.WriteString(string.Format("{0}", item.Id));
                        writer.WriteEndElement();

                        writer.WriteStartElement("ActionBody");
                        writer.WriteString(string.Format("{0}", string.Empty));
                        writer.WriteEndElement();

                        writer.WriteStartElement("More");
                        writer.WriteString(string.Format("{0}", string.Empty));
                        writer.WriteEndElement();

                        writer.WriteStartElement("CreateUserFirstName");
                        writer.WriteString(string.Format("{0}", item.App_User.FirstName));
                        writer.WriteEndElement();

                        writer.WriteStartElement("CreateUserLastName");
                        writer.WriteString(string.Format("{0}", item.App_User.LastName));
                        writer.WriteEndElement();

                        writer.WriteStartElement("CreateDate");
                        if (item.CreateDate.HasValue)
                        {
                            writer.WriteString(string.Format("{0}", item.CreateDate.Value.ToString("MM/dd/yyyy HH:mm:ss")));
                        }
                        else
                        {
                            writer.WriteString(string.Format("{0}", string.Empty));
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("AssignedToType");
                        if (item.Type.HasValue)
                        {
                            writer.WriteString(string.Format("{0}", ((ObjectType)item.Type.Value).ToString()));
                        }
                        else
                        {
                            writer.WriteString(string.Format("{0}", string.Empty));
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("AssignedToId");
                        writer.WriteString(string.Format("{0}", item.ObjectId));
                        writer.WriteEndElement();

                        writer.WriteStartElement("AssignedToBody");
                        if (!string.IsNullOrEmpty(getObject.CameFromTitle))
                        {
                            writer.WriteString(string.Format("{0}", getObject.CameFromTitle));
                        }
                        else
                        {
                            writer.WriteString(string.Format("{0}", string.Empty));
                        }
                        writer.WriteEndElement();

                        writer.WriteEndElement();
                    }
                }
            }

            foreach (var item in ObjectTopicMapperDAL.GetAll())
            {
                var getObject = ManageObject.GetSharedObject(item.Id, (int)ObjectType.ObjectTopicMapper);

                if (getObject.CourseId == courseId)
                {
                    writer.WriteStartElement("Data");

                    writer.WriteStartElement("ActionType");
                    writer.WriteString(string.Format("{0}", "MetaData"));
                    writer.WriteEndElement();

                    writer.WriteStartElement("Action");
                    writer.WriteString(string.Format("{0}", "Topic"));
                    writer.WriteEndElement();

                    writer.WriteStartElement("ActionId");
                    writer.WriteString(string.Format("{0}", item.Topic.Id));
                    writer.WriteEndElement();

                    writer.WriteStartElement("ActionBody");
                    if (!string.IsNullOrEmpty(item.Topic.Title))
                    {
                        writer.WriteString(string.Format("{0}", item.Topic.Title));
                    }
                    else
                    {
                        writer.WriteString(string.Format("{0}", string.Empty));
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("More");
                    writer.WriteString(string.Format("{0}", string.Empty));
                    writer.WriteEndElement();

                    writer.WriteStartElement("CreateUserFirstName");
                    writer.WriteString(string.Format("{0}", string.Empty));
                    writer.WriteEndElement();

                    writer.WriteStartElement("CreateUserLastName");
                    writer.WriteString(string.Format("{0}", string.Empty));
                    writer.WriteEndElement();

                    writer.WriteStartElement("CreateDate");
                    writer.WriteString(string.Format("{0}", string.Empty));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToType");
                    writer.WriteString(string.Format("{0}", ((ObjectType)item.ObjectType).ToString()));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToId");
                    writer.WriteString(string.Format("{0}", item.ObjectId));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToBody");
                    if (!string.IsNullOrEmpty(getObject.CameFromTitle))
                    {
                        writer.WriteString(string.Format("{0}", getObject.CameFromTitle));
                    }
                    else
                    {
                        writer.WriteString(string.Format("{0}", string.Empty));
                    } writer.WriteEndElement();

                    writer.WriteEndElement();
                }
            }

            foreach (var item in TagMapperDAL.GetAll())
            {
                var getObject = ManageObject.GetSharedObject(item.Id, (int)ObjectType.TagMapper);

                if (getObject.CourseId == courseId)
                {
                    writer.WriteStartElement("Data");

                    writer.WriteStartElement("ActionType");
                    writer.WriteString(string.Format("{0}", "MetaData"));
                    writer.WriteEndElement();

                    writer.WriteStartElement("Action");
                    writer.WriteString(string.Format("{0}", "Tag"));
                    writer.WriteEndElement();

                    writer.WriteStartElement("ActionId");
                    writer.WriteString(string.Format("{0}", item.Tag.Id));
                    writer.WriteEndElement();

                    writer.WriteStartElement("ActionBody");
                    if (!string.IsNullOrEmpty(item.Tag.Title))
                    {
                        writer.WriteString(string.Format("{0}", item.Tag.Title));
                    }
                    else
                    {
                        writer.WriteString(string.Format("{0}", string.Empty));
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("More");
                    writer.WriteString(string.Format("{0}", string.Empty));
                    writer.WriteEndElement();

                    writer.WriteStartElement("CreateUserFirstName");
                    writer.WriteString(string.Format("{0}", item.App_User.FirstName));
                    writer.WriteEndElement();

                    writer.WriteStartElement("CreateUserLastName");
                    writer.WriteString(string.Format("{0}", item.App_User.LastName));
                    writer.WriteEndElement();

                    writer.WriteStartElement("CreateDate");
                    if (item.CreateDate.HasValue)
                    {
                        writer.WriteString(string.Format("{0}", item.CreateDate.Value.ToString("MM/dd/yyyy HH:mm:ss")));
                    }
                    else
                    {
                        writer.WriteString(string.Format("{0}", string.Empty));
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToType");
                    writer.WriteString(string.Format("{0}", ((ObjectType)item.ObjectType).ToString()));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToId");
                    writer.WriteString(string.Format("{0}", item.ObjectId));
                    writer.WriteEndElement();

                    writer.WriteStartElement("AssignedToBody");
                    if (!string.IsNullOrEmpty(getObject.CameFromTitle))
                    {
                        writer.WriteString(string.Format("{0}", getObject.CameFromTitle));
                    }
                    else
                    {
                        writer.WriteString(string.Format("{0}", string.Empty));
                    }
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            return File("~/Exports/CourseMetadta.xml", "text/xml", "CourseMetadta.xml");
        }

    }
}
