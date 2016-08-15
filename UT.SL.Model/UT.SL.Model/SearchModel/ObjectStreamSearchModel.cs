using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UT.SL.Helper;
using System.ComponentModel.DataAnnotations;
using UT.SL.Model;

namespace UT.SL.Model
{


    public class ObjectStreamSearchModel : PagingItems
    {
        public int UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public int ObjectType { get; set; }
        public int ObjectId { get; set; }
        public bool IsCourse { get; set; }
        public bool? IsRead { get; set; }
    }
}
