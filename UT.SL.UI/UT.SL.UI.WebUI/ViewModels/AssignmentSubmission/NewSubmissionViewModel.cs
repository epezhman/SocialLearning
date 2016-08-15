using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UT.SL.DAL;
using UT.SL.Helper;
using UT.SL.UI.WebUI.Controllers;

namespace ViewModels.AssignmentSubmission
{
    public class NewSubmissionViewModel
    {
        public NewSubmissionViewModel()
        {
        }

        public NewSubmissionViewModel(UT.SL.Data.LINQ.AssignmentSubmission assignmentSubmission,
                                       UT.SL.Data.LINQ.Grade grade,
                                       UT.SL.Data.LINQ.Comment comment,
                                       UT.SL.Data.LINQ.Assess selfAssess,
                                        UT.SL.Data.LINQ.Assess expertAssess
                                       )
        {
            AssignmentSubmission = assignmentSubmission;
            Grade  = grade;
            Comment = comment;
            SelfAssess = selfAssess;
            ExpertAssess = expertAssess;
        }

       
        public UT.SL.Data.LINQ.AssignmentSubmission AssignmentSubmission { get; set; }
        public UT.SL.Data.LINQ.Grade  Grade { get; set; }
        public UT.SL.Data.LINQ.Comment Comment { get; set; }
        public UT.SL.Data.LINQ.Assess SelfAssess { get; set; }
        public UT.SL.Data.LINQ.Assess ExpertAssess { get; set; }
    }

}