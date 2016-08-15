using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UT.SL.Model.Enumeration;

namespace UT.SL.Model
{
    public class SharedVia
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public SharedType SharedType { get; set; }
    }
}
