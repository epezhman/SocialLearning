using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UT.SL.Model
{
    public class ViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Tag { get; set; }
        public object Value { get; set; }
    }

    public class SectionSummery
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Contents { get; set; }
        public int ChildSections { get; set; }
    }
}
