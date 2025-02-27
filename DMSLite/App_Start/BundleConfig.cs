﻿using System.Web;
using System.Web.Optimization;

namespace DMSLite
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-unobtrusive").Include(
                        "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/notify").Include(
                        "~/Scripts/notify.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/font-awesome",
               "https://maxcdn.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css").Include(
                      "~/css/font-awesome.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/select2.css"));

            bundles.Add(new ScriptBundle("~/bundles/select2").Include(
                "~/Scripts/select2.js"));

            //Custom Scripts
            bundles.Add(new ScriptBundle("~/bundles/home").Include(
                "~/Scripts/Custom/homeJavascript.js"));

            bundles.Add(new ScriptBundle("~/bundles/suggestionBar").Include(
                "~/Scripts/Custom/suggestionbar.js"));
        }
    }
}
