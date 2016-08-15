using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace UT.SL.Model
{
    public class AddGroupMemberModel
    {
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public int GroupeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public Guid Member { get; set; }

        public int IsAdmin { get; set; }
    }

    public class AddSocialGroupMemberModel
    {
        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public int GroupeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public string Member { get; set; }

        public int IsAdmin { get; set; }
    }

    public class AddCourseMembershipModel
    {
        public int CourseId { get; set; }

        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public string Member { get; set; }

        public int MembershipType { get; set; }
    }

    public class AddLearningGroupMemberModel
    {
        public int GroupeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(UT.SL.Model.Resource.App_Errors), ErrorMessageResourceName = "FieldIsRequired")]
        public string Member { get; set; }

    }
}
