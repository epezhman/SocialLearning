using System.Web;
using System.Web.Optimization;

namespace UT.SL.UI.WebUI
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.cookie.js",
                        "~/Scripts/jquery.scrollTo.js",
                        "~/Scripts/jquery.jscroll.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/bootsrap").Include(
                        "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/authorCode").Include(
                        "~/Scripts/buttons.js",
                       "~/Scripts/UTPlugins.js",
                        "~/Scripts/AuthorCode.js"
                       ));

            bundles.Add(new ScriptBundle("~/bundles/otherJs").Include(
                        "~/Scripts/jquery.fancybox.pack.js",
                         "~/Scripts/jquery.annotate.js"
                       ));

            bundles.Add(new ScriptBundle("~/bundles/rtl").Include(
                       "~/Scripts/jquery.ui.datepicker-cc.all.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
                        "~/Scripts/kendo/2013.1.319/kendo.core.min.js",
                        "~/Scripts/kendo/2013.1.319/kendo.web.min.js",
                        "~/Scripts/kendo/2013.1.319/kendo.editor.min.js",
                        "~/Scripts/kendo/2013.1.319/kendo.autocomplete.min.js",
                        "~/Scripts/kendo/2013.1.319/kendo.dropdownlist.min.js",
                        "~/Scripts/kendo/2013.1.319/kendo.upload.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/highcharts").Include(
                        "~/Scripts/highcharts/highcharts.js",
                        "~/Scripts/highcharts/highcharts-more.js",
                        "~/Scripts/highcharts/modules/exporting.js",
                        "~/Scripts/highcharts/modules/data.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/framework").Include("~/Content/Framework.css"));

            bundles.Add(new StyleBundle("~/Content/sitecss").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/sitecssrtl").Include("~/Content/sitertl.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/cssCSS").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

            bundles.Add(new StyleBundle("~/Content/themes/jquery-uiCSS").Include(
                        "~/Content/themes/jquery-ui.css"));

            bundles.Add(new StyleBundle("~/Content/themes/bootstrapCSS").Include(
                        "~/Content/themes/bootstrap.css",
                        "~/Content/themes/bootstrap-responsive.css"));

            bundles.Add(new StyleBundle("~/Content/bootstrapCSS").Include(
                        "~/Content/bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/bootstrapCSSRTL").Include(
                        "~/Content/bootstrap.css",
                       "~/Content/bootstrap-rtl.css"));


            bundles.Add(new StyleBundle("~/Content/themes/bootstraprtlCSS").Include(
                        "~/Content/themes/bootstrap.rtl.css",
                        "~/Content/themes/bootstrap-responsive.rtl.css",
                        "~/Content/kendo/2013.1.319/kendo.rtl.min.css"));

            //bundles.Add(new StyleBundle("~/Content/kendo/2013.1.319CSS").Include(
            //           "~/Content/kendo/2013.1.319/kendo.common.min.css",
            //           "~/Content/kendo/2013.1.319/kendo.bootstrap.min.css"
            //           ));

            bundles.Add(new StyleBundle("~/Content/kendoCSS").Include(
                      "~/Content/kendo.common.min.css",
                      "~/Content/kendo.bootstrap.min.css"
                      ));

            bundles.Add(new StyleBundle("~/Content/other/fancyBox/fancyBoxCSS").Include(
                       "~/Content/other/fancyBox/jquery.fancybox.css"));

            bundles.Add(new StyleBundle("~/Content/other/annotationCSS").Include(
                      "~/Content/other/annotation/annotation.css"));

            bundles.Add(new StyleBundle("~/Content/Customize").Include(
                      "~/Content/Customize.css"));

            // Clear all items from the ignore list to allow minified CSS and JavaScript files in debug mode
            bundles.IgnoreList.Clear();


            // Add back the default ignore list rules sans the ones which affect minified files and debug mode
            bundles.IgnoreList.Ignore("*.intellisense.js");
            bundles.IgnoreList.Ignore("*-vsdoc.js");
            bundles.IgnoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);

        }
    }
}