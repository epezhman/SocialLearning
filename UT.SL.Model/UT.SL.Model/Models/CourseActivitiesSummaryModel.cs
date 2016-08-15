using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UT.SL.Data.LINQ;

namespace UT.SL.Model
{
    public class CourseActivitiesSummaryModel
    {
        public List<Quiz> Quizes { get; set; }
        public List<Assignment> Assignments { get; set; }
        public List<Forum> Forumes { get; set; }
    }
}
