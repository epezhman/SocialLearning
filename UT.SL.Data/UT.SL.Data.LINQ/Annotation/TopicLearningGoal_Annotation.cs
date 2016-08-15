using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(TopicLearningGoal_Annotation))]
	public partial class TopicLearningGoal
	{
	}

	 public partial class TopicLearningGoal_Annotation {
        
        public TopicLearningGoal_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.TopicLearningGoal))]
        public int Id { get; set; }
        
        [Display(Name = "LevelId", ResourceType = typeof(UT.SL.Model.Resource.TopicLearningGoal))]
        public System.Nullable<int> LevelId { get; set; }
        
        [Display(Name = "QuizId", ResourceType = typeof(UT.SL.Model.Resource.TopicLearningGoal))]
        public System.Nullable<int> QuizId { get; set; }
        
        [Display(Name = "TopicId", ResourceType = typeof(UT.SL.Model.Resource.TopicLearningGoal))]
        public System.Nullable<int> TopicId { get; set; }
        
        [Display(Name = "ImpactId", ResourceType = typeof(UT.SL.Model.Resource.TopicLearningGoal))]
        public System.Nullable<int> ImpactId { get; set; }
    }
}
