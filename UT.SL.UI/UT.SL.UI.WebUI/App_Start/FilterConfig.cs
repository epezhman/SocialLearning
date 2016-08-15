using System.Web;
using System.Web.Mvc;

namespace UT.SL.UI.WebUI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {

            filters.Add(new HandleErrorAttribute());
            filters.Add(new Infrastructure.CustomHandleErrorAttribute());
         
        }
    }
}