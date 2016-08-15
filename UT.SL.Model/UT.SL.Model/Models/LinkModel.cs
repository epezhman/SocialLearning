using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UT.SL.Model
{
    public class LinkModel
    {
        public string Title { get; set; }
        public string Area { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public List<Object> Datas { get; set; }

        public bool ExtraLink { get; set; }
        public bool IsActive { get; set; }
        public int Rank { get; set; }

        public int Id { get; set; }
        public Guid guid { get; set; }
        public string Filter { get; set; }

        public bool IsIcon  { get; set; }
        public string IconTitle { get; set; }

        public bool IsBold { get; set; }

        public LinkModel()
        {
            Datas = new List<object>();
        }
    }
}
