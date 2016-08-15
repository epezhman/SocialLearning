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
    public class GradeViewModel
    {
        public GradeViewModel()
        {
        }

        public GradeViewModel(UT.SL.Data.LINQ.AssignmentSubmission assignmentSubmission,
                                   UT.SL.Data.LINQ.Grade grade
                                    )
        {
            AssignmentSubmission = assignmentSubmission;
            Grade = grade;
        }


        public UT.SL.Data.LINQ.AssignmentSubmission AssignmentSubmission { get; set; }
        public UT.SL.Data.LINQ.Grade Grade { get; set; }
    }
}