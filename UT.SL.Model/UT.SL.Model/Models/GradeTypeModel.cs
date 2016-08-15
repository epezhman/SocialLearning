using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UT.SL.Data.LINQ;

namespace UT.SL.Model
{
    public class GradeTypeModel
    {
        public string Title { get; set; }
        public int? MyGrade { get; set; }
        public double? MinGrade { get; set; }
        public double? MaxGrade { get; set; }
        //public double AverageGrade { get; set; }
        public DateTime CreateDate { get; set; }
        public double? MyGradeDouble { get; set; }
        public ObjectViewModel GradedObject { get; set; }
        public string FullTitle { get; set; }
        public Topic Topic { get; set; }
        public List<App_User> MaxGardeUsers { get; set; }
        public App_User GradeGiver { get; set; }
        public int GradedObjectType { get; set; }
        public List<GradeTypeModel> GradedObjects { get; set; }

        public GradeTypeModel() {
            MaxGardeUsers = new List<App_User>();
            GradedObjects = new List<GradeTypeModel>();
        }
    }
}
