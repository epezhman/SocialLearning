using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UT.SL.Data.LINQ
{

    [MetadataType(typeof(App_SiteMenu_Annotations))]
    public partial class App_SiteMenu
    {

    }

    public partial class App_SiteMenu_Annotations
    {
        public App_SiteMenu_Annotations() { }

        [Display(Name = "ActionId", ResourceType = typeof(UT.SL.Model.Resource.App_SiteMenu))]
        public virtual int ActionId { get; set; }

        [Display(Name = "MenuTitle", ResourceType = typeof(UT.SL.Model.Resource.App_SiteMenu))]
        public string MenuTitle { get; set; }

        [Display(Name = "BrifDescription", ResourceType = typeof(UT.SL.Model.Resource.App_SiteMenu))]
        public string BrifDescription { get; set; }

        [Display(Name = "ExternalLink", ResourceType = typeof(UT.SL.Model.Resource.App_SiteMenu))]
        public string ExternalLink { get; set; }

        [Display(Name = "PageTitle", ResourceType = typeof(UT.SL.Model.Resource.App_SiteMenu))]
        public string PageTitle { get; set; }

        public System.DateTime CreateDate { get; set; }

        [Display(Name = "Rank", ResourceType = typeof(UT.SL.Model.Resource.App_SiteMenu))]
        public short Rank { get; set; }

        [Display(Name = "MenuPosition", ResourceType = typeof(UT.SL.Model.Resource.App_SiteMenu))]
        public byte MenuPosition { get; set; }

        [Display(Name = "IsActive", ResourceType = typeof(UT.SL.Model.Resource.App_SiteMenu))]
        public bool IsActive { get; set; }

        [Display(Name = "PageFooter", ResourceType = typeof(UT.SL.Model.Resource.App_SiteMenu))]
        public string PageFooter { get; set; }

        [Display(Name = "PageHeader", ResourceType = typeof(UT.SL.Model.Resource.App_SiteMenu))]
        public string PageHeader { get; set; }

        [Display(Name = "Description", ResourceType = typeof(UT.SL.Model.Resource.App_SiteMenu))]
        public string Description { get; set; }

        [Display(Name = "Help", ResourceType = typeof(UT.SL.Model.Resource.App_SiteMenu))]
        public string Help { get; set; }
    }
}
