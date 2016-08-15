using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Web.Mvc;
using System.Globalization;

namespace UT.SL.Model.Enumeration
{
    public class EnumStruct
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Utility
    {

        public static string GetTitleFromResource(string enumName, string key)
        {
            try
            {
                var s = EnumResource.ResourceManager.GetString(enumName + "_" + key);
                if (string.IsNullOrEmpty(s))
                    s = key;
                return s;
            }
            catch
            {
                return key;
            }
        }

        public static bool ConfirmSelected(Type enumType, int selected)
        {
            try
            {
                foreach (var item in enumType.GetFields())
                {
                    if (item.FieldType != enumType)
                        continue;
                    if (selected.ToString() == item.GetRawConstantValue().ToString())
                        return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        public static List<EnumStruct> GetAllTitlesFromResource(Type enumType)
        {
            var names = new List<EnumStruct>();
            try
            {
                foreach (var item in enumType.GetFields())
                {
                    if (item.FieldType != enumType)
                        continue;
                    names.Add(new EnumStruct { Name = Utility.GetTitleFromResource(enumType.Name, item.Name), Value = item.GetRawConstantValue().ToString() });
                }
            }
            catch
            {
                return new List<EnumStruct>();
            }

            return names;
        }

        public static List<SelectListItem> GetYearsSelectList(int? year, string emptyText = "", string selectedValue = "0")
        {
            List<SelectListItem> sl = new List<SelectListItem>();
            SelectListItem sli;
            var pc = new PersianCalendar();

            if (!string.IsNullOrEmpty(emptyText))
            {
                sli = new SelectListItem();
                sli.Value = selectedValue;
                sli.Text = emptyText;
                sl.Add(sli);
            }

            if (year.HasValue)
            {
                for (int i = year.Value - 30; i < year.Value + 30; i++)
                {
                    sli = new SelectListItem();

                    sli.Text = i.ToString();
                    sli.Value = i.ToString();
                    sli.Selected = sli.Value == selectedValue;

                    sl.Add(sli);
                }
            }
            else
            {
                for (int i = pc.GetYear(DateTime.Now) - 30; i < pc.GetYear(DateTime.Now) + 30; i++)
                {
                    sli = new SelectListItem();

                    sli.Text = i.ToString();
                    sli.Value = i.ToString();
                    sli.Selected = sli.Value == selectedValue;

                    sl.Add(sli);
                }
            }

            return sl;
        }

        public static List<SelectListItem> GetSelectList(Type type, string emptyText = "", bool validation = false, string selectedValue = "0")
        {
            List<SelectListItem> sl = new List<SelectListItem>();
            SelectListItem sli;

            if (!string.IsNullOrEmpty(emptyText))
            {
                sli = new SelectListItem();
                sli.Text = emptyText;
                if (validation)
                    sli.Value = String.Empty;
                else
                    sli.Value = selectedValue;
                sl.Add(sli);
            }

            foreach (var item in type.GetFields())
            {
                if (item.FieldType != type)
                    continue;

                sli = new SelectListItem();

                sli.Text = Utility.GetTitleFromResource(type.Name, item.Name);
                sli.Value = item.GetRawConstantValue().ToString();
                sli.Selected = sli.Value == selectedValue;

                sl.Add(sli);
            }

            return sl;
        }

        public class EnumlistModel
        {
            public byte Id { get; set; }
            public string Title { get; set; }
        }

        public static List<EnumlistModel> ToList(Type enumType, string selectedItem = "", string ContainItemsValue = "", string ExceptItemsValue = "", bool AddEmptyRow = true)
        {
            List<EnumlistModel> items = new List<EnumlistModel>();
            if (AddEmptyRow)
                items.Add(new EnumlistModel() { Title = "" });

            string[] ContainValues = ContainItemsValue.Split(',');
            string[] ExceptValues = ExceptItemsValue.Split(',');

            foreach (var item in Enum.GetValues(enumType))
            {
                string value = ((byte)item).ToString();
                if (string.IsNullOrEmpty(ExceptItemsValue) || ExceptValues.Length == 0 || ExceptValues.Where(u => u == value).Count() == 0)
                {
                    if (string.IsNullOrEmpty(ContainItemsValue) || ContainValues.Length == 0 || (ContainValues.Where(u => u == value).Count() > 0))
                    {
                        FieldInfo fi = enumType.GetField(item.ToString());
                        var attribute = fi.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
                        var title = attribute == null ? item.ToString() : ((DescriptionAttribute)attribute).Description;
                        var listItem = new EnumlistModel
                        {
                            Id = byte.Parse(value),
                            Title = GetTitleFromResource(enumType.Name, title)
                        };
                        items.Add(listItem);
                    }
                }
            }

            return items;
        }

    }

    public enum Gender : byte { Male = 1, Female = 2 };

    public enum GenderAll : byte { Boy = 1, Girl = 2, Both = 3 };

    public enum Marriage : byte { Single = 1, Married = 2, Divorced = 3, Myl = 4, Breadwinner = 5 };

    public enum Language : byte { Fa = 1, En = 2, Fr = 3, Es = 4, De = 5 };

    public enum Sexuality : byte { Male = 1, Female = 2, ThirdGender = 3 };

    public enum MenuPosition : byte { AdminSideBar = 1, TeacherSideBar = 2, TASideBar = 4, StudentSideBar = 8, SocialSideBar = 16, LearningSideBar = 32 };

    public enum MenuType : byte { MyHome = 1, AllCourses = 2, Calendar = 3, Messages = 4, Notifications = 5, User = 6, Admin = 7, MyCourses = 8, UserProfile = 9, Account = 10, SocialGroup = 11, Survey = 12, FirstPage = 13, Advrt = 14 };

    public enum NavLinks : byte { Show = 1, Hidden = 2, Category = 3, Tag = 4, Courses = 5, AppActiopn = 6, Users = 7, FeedBack = 8, Email = 9, SocialGroup = 10, Account = 11, Profile = 12, Survey = 12, ActionEvaluation = 13, SurveyReport = 14 };

    public enum ContentType : byte { PageHeader = 1, PageFooter = 2, Description = 3, Help = 4 };

    public enum WeekDays : short { Saturday = 0, Sunday = 1, Monday = 2, Tuesday = 3, Wednesday = 4, Thursday = 5, Friday = 6 };

    public enum ContactType : byte { Email = 1, Mobile = 2, WorkPhone = 3, WorkFax = 4, HomePhone = 5, Website = 6, Pager = 7 };

    public enum AddressType : byte { Work = 1, Home = 2 };

    public enum ObjectType : int
    {
        Course = 1, Resource = 2, User = 3, Comment = 4, Forum = 5, Assignment = 6,
        Quiz = 7, ForumDiscussion = 8, postReply = 9, ImageAnnotation = 10, Category = 11,
        Wiki = 12, VoteParent = 13, Vote = 14, TagMapper = 15, Tag = 16, SocialGroup = 17,
        Share = 18, App_User = 19, LearningGroup = 20, ForumDiscussionPost = 21, AssignmentSubmission = 22,
        Grade = 23, Feedback = 24, Email = 25, Survey = 26, UserEnrolement = 26, ObjectStream = 27, App_Action = 28,
        App_UserInRole = 29, App_UserEnrolement = 30, App_UserInfo = 31, UserTopicMapper = 32, App_ActionEvaulation = 33,
        App_Permission = 34, AssessParent = 35, Assess = 36, ObjectTopicMapper = 37, CourseAbstract = 38, Topic = 39,
        CourseTopcMapper = 40, CourseSchedule = 41, CategoryMapper = 42, Answer = 43, GroupMember = 44, SurveyAnswer = 45,
        SurveyUserSummary = 46, ObjectStreamGroupMapper = 47, ObjectGroupMapper = 48, ObjectStreamCourse = 49, Message = 50,
        MessageThread = 51, MessageContact = 52, Notification = 53, App_UserProfile = 54
    };

    public enum ActivityType : int { Create = 1, Comment = 2, Tag = 3, Vote = 4 };

    public enum SharedType : int { FromCourse = 1, FromLearningGroup = 2, FromSocialGroup = 3, FromUser = 4 };

    public enum MemebrshipType : int { Teacher = 1, TA = 2, Student = 3 };

    public enum ReactionType : int { Awesome = 1, LikeIt = 2, Interesting = 3, Tough = 4, NotThought = 5, NeedTime = 6, Bored = 7, NeedHelp = 8, LostInterest = 9 };

    public enum ResourceReactionType : int {LikeIt = 1, Informative = 2, Acceptable = 3, Incomplete = 4, NeedsImprovement = 5}; // خوشایند، بدردبخور، قابل قبول، ناکامل، نیازمند اصلاح

    public enum AssignmentReactionType : int { LikeIt = 1, Tough = 2, NeedsTime = 3, NeedsHelp = 4, Useless = 5 };//خوشایند، سخت، نیاز به زمان بیشتر، نیاز به کمک، بدردنخور

    public enum ForumReactionType : int { LikeIt = 1, Challenging = 2, Motivating = 3, Communicating = 4 };

    public enum DiscussionReactionType : int { LikeIt = 1, Inspiring = 2, Evaluating = 3, Good_Summary = 4, Informative_Approve = 5, Informativ_Dissaprove = 6, Organizing = 7, Clearifying = 8, Weak_Contribution = 9 };

    public enum PostReactionType : int { Novel_Inspiring = 1, Goal_Direct = 2, Challenging = 3, Informative_Assert = 4, NoNewIdea = 5, WeakContribution = 6, Unrelated = 7 };

    public enum PersonTitleType : int { Mr = 1, Mrs = 2, Dr = 3 };

    public enum ImageSizeType : int { Small = 1, Medium = 2, Large = 3, X_Small = 4, GetPanelPic = 5 };

    public enum ProfileLinkType : int { FullName = 1, FirstName = 2, LastName = 2, UserName = 2 };

    public enum SelfAssignmentSubmissionReactionType : int { I_have_some_ambiguty = 1, I_have_understand_one_concept = 2, I_have_understand_some_concepts = 3, I_have_understand_some_concepts_and_their_relationships = 4, I_am_able_now_to_create_new_idea_based_on_my_mastery_of_the_subject = 5 };

    public enum ExpertAssignmentSubmissionReactionType : int { The_student_has_some_ambiguty = 1, The_student_has_understand_one_concept = 2, The_student_has_understand_some_concepts = 3, The_student_has_understand_some_concepts_and_their_relationships = 4, The_student_is_able_now_to_create_new_idea_based_on_my_mastery_of_the_subject = 5 };

    public enum AssessType : int { self = 1, peer = 2, expert = 3 };

    public enum ActivityStatusType : int { Open = 1, UnGraded = 2, Graded = 3 };

    public enum MailServerType : int { DoosMooc = 1, Gmail = 2, Yahoo = 3, Hotmail = 4 };

    public enum SentEmailType : int { Confirmation = 1, Notification = 2, UnreadMessages = 3, UncategorizedEmail = 4};

    public enum ActionType : int { Vote = 1, Tag = 2, Comment = 3, Create = 4, Delete = -4, DeleteVote = -1, DeleteTag = -2, DeleteComment = -3, Grade = 5 };

}


