using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UT.SL.Data.LINQ {
    
    
    [MetadataType(typeof(GroupMember_Annotation))]
	public partial class GroupMember
	{
	}

	 public partial class GroupMember_Annotation {
        
        public GroupMember_Annotation() { }
        
        [Display(Name = "Id", ResourceType = typeof(UT.SL.Model.Resource.GroupMember))]
        public int Id { get; set; }
        
        [Display(Name = "Type", ResourceType = typeof(UT.SL.Model.Resource.GroupMember))]
        public System.Nullable<int> Type { get; set; }
        
        [Display(Name = "IsCircleAdmin", ResourceType = typeof(UT.SL.Model.Resource.GroupMember))]
        public System.Nullable<int> IsCircleAdmin { get; set; }
    }
}
