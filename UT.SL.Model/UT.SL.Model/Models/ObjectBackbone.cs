using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UT.SL.Model
{
    public class ObjectBackbone
    {
        public int Id { get; set; }
        public Model.Enumeration.ObjectType Type { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
