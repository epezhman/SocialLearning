using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UT.SL.Data.LINQ;

namespace UT.SL.Model
{
    public class ObjectViewModel
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public int CameFromId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreateDate { get; set; }
        public byte[] FileContent { get; set; }
        public string FileTitle { get; set; }
        public string FileMime { get; set; }
        public App_User CreateUser { get; set; }
        public string CameFromTitle { get; set; }
        public int? CourseId { get; set; }
        public string CourseTitle { get; set; }
        public int AAssignmentScore { get; set; }
        public double Score { get; set; }
        public bool IsWide { get; set; }
        public String BadgetTitle { get; set; }
        public int CameFromType { get; set; }
        public App_User CameFromUser { get; set; }
        public string ExtraInfo { get; set; }
        public App_User GradeUser { get; set; }
        public bool IsReaden { get; set; }
        public DateTime ReadenDate { get; set; }
        public DateTime? ClickedDate { get; set; }
        public bool IsEdited { get; set; }
    }


    public class ObjectViewModelList
    {
        public List<ObjectViewModel> ObjectViewModels { get; set; }
        public bool IsWide { get; set; }

        public ObjectViewModelList()
        { 
            ObjectViewModels = new List<ObjectViewModel>();
            IsWide = false;
        }
    }

}
