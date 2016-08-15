using System.Web.Mvc;

namespace UT.SL.UI.WebUI.Areas.Sandbox
{
    public class SandboxAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Sandbox";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Sandbox_default",
                "Sandbox/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
