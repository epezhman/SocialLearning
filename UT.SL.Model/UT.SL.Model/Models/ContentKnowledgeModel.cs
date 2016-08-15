using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UT.SL.Model
{
    public class ContentKnowledgeModel
    {
        public int Id { get; set; }
        public int ObjectId { get; set; }
        public int ObjectType { get; set; }
        public int UserId { get; set; }
        public double UserKnowledgeValue { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string FileTitle { get; set; }
        public int ContentCreateUserId { get; set; }
        public DateTime ContentCreateDate { get; set; }
        public List<ContentKnowledgeModel> ContentKnowledgeModels { get; set; }
        public DateTime? ContentDueDate { get; set; }
    }

}
