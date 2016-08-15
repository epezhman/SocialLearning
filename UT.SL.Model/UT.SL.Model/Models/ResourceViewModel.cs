using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UT.SL.Data.LINQ;

namespace UT.SL.Model
{
    public class ResourceViewModel
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
        public int CourseId { get; set; }
    }
}
