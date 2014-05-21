using System.Web;
using System.Web.Optimization;

namespace PhotoGallery2
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/boostrap-image-gallery")
                                        .Include("~/Scripts/blueimp-gallery/blueimp-gallery.js",
                                                 "~/Scripts/blueimp-gallery/blueimp-gallery-fullscreen.js",
                                                 "~/Scripts/blueimp-gallery/blueimp-gallery-indicator.js",
                                                 "~/Scripts/blueimp-gallery/blueimp-helper.js",
                                                 "~/Scripts/blueimp-gallery/jquery.blueimp-gallery.js",
                                                 "~/Scripts/blueimp-gallery/bootstrap-image-gallery.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/bootstrap.css",
                        "~/Content/site.css"));
            bundles.Add(
                new StyleBundle("~/Content/bootstrap-image-gallery")
                               .Include("~/Content/bootstrap-image-gallery.css",
                                        "~/Content/blueimp-gallery.css",
                                        "~/Content/blueimp-gallery-indicator.css"));
        }
    }
}
