using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UT.SL.Data.LINQ;

namespace UT.SL.Model
{
    public class CourseRecommendedSummaryModel
    {
        public List<ContentInterestModel> ContentInterestModels { get; set; }
        public List<ContentKnowledgeModel> ContentKnowledgeModels { get; set; }

    }
}
