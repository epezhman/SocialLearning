using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UT.SL.Model
{
    public class ObjectModel
    {
        public string ObjectName { get; set; }
        public int ObjectId { get; set; }
        public int Type { get; set; }
        public int Count { get; set; }
        public UT.SL.Model.Enumeration.ObjectType ObjectType { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int UserId { get; set; }
        public byte[] FileContent { get; set; }
        public string FileTitle { get; set; }
        public string FileMime { get; set; }
        public int CourseId { get; set; }
        public byte ViewType { get; set; }
        public int OwnerId { get; set; }
        public bool IsNew { get; set; }

        public ObjectModel() {
            IsNew = false;
        }

    }
}
