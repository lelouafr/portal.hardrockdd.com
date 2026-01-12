using System.Web.Optimization;

namespace portal
{
    public static class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            if (bundles == null)
            {
                throw new System.ArgumentNullException(nameof(bundles));
            }

            #region Styles
            // Font Awesome icons
            bundles.Add(new StyleBundle("~/cssbase/font-awesome").Include(
                      "~/Content/assets/plugins/font-awesome/css/all.min.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/cssbase/font-google", "https://fonts.googleapis.com/css?family=Open+Sans:300,400,600,700").Include("~/Content/assets/plugins/google/css/opensans.css"));

            bundles.Add(new StyleBundle("~/cssbase/theme")
                .Include(
                      "~/Content/assets/plugins/jquery-ui/jquery-ui.min.css",
                      //"~/Content/assets/plugins/bootstrap/css/bootstrap.min.css",
                      "~/Content/bootstrap.min.css",
                      "~/Content/assets/plugins/animate/animate.min.css")
                );

            bundles.Add(new StyleBundle("~/cssbase/style")
                .Include("~/Content/assets/css/default/style.css", new CssRewriteUrlTransform())
                .Include("~/Content/assets/css/default/style-responsive.css", new CssRewriteUrlTransform())
                .Include("~/Content/assets/css/default/theme/default.css", new CssRewriteUrlTransform())
                .Include("~/Content/assets/css/forum/style.css", new CssRewriteUrlTransform())
                .Include("~/Content/assets/css/forum/style-responsive.css", new CssRewriteUrlTransform())
                );

            //bundles.Add(new StyleBundle("~/cssbase/forum")
            //    .Include("~/Content/assets/css/forum/style.css", new CssRewriteUrlTransform())
            //    .Include("~/Content/assets/css/forum/style-responsive.css", new CssRewriteUrlTransform())
            //    );

            // Notifications
            bundles.Add(new StyleBundle("~/cssbase/notifications").Include(
                        "~/Content/assets/plugins/gritter/css/jquery.gritter.css",
                        "~/Content/assets/plugins/toastr/toastr.min.css"));

            // dataTables css styles
            bundles.Add(new StyleBundle("~/csspage/dataTables").Include(
                      "~/Content/plugins/dataTables/datatables.min.css"));
            
            // Date Picker
            bundles.Add(new StyleBundle("~/csspage/datepicker").Include(
                        "~/Content/assets/plugins/bootstrap-datepicker/css/bootstrap-datepicker.min.css", new CssRewriteUrlTransform()));

            // switchery styles
            bundles.Add(new StyleBundle("~/csspage/switcheryStyles").Include(
                      "~/Content/plugins/switchery/switchery.css"));
            
            // wysihtml5
            bundles.Add(new StyleBundle("~/csspage/wysihtml5").Include(
                      "~/Content/plugins/bootstrap-wysihtml5/src/bootstrap3-wysihtml5.min.css"));

            // dropZone styles
            bundles.Add(new StyleBundle("~/csspage/dropzone").Include(
                      "~/Content/assets/plugins/dropzone/min/dropzone.min.css"));



            // Input Control Styles styles
            bundles.Add(new StyleBundle("~/csspage/inputcontrols").Include(
                      "~/Content/assets/plugins/bootstrap-datepicker/css/bootstrap-datepicker.min.css",
                      "~/Content/assets/plugins/bootstrap-daterangepicker/daterangepicker.css",
                      "~/Content/assets/plugins/bootstrap-select/bootstrap-select.min.css",
                      "~/Content/assets/plugins/switchery/switchery.min.css",
                      "~/Content/assets/plugins/tagsinput/bootstrap-tagsinput.css",
                      "~/Content/assets/plugins/bootstrap-wysihtml5/dist/bootstrap3-wysihtml5.css")
                );
            #endregion

            #region Scripts
            bundles.Add(new ScriptBundle("~/jsbase/jquery").Include(
                        "~/Scripts/jquery-3.3.1.min.js",
                        "~/Content/assets/plugins/jquery-ui/jquery-ui.min.js",
                        "~/Content/assets/plugins/jquery-ui-touch-punch/jquery.ui.touch-punch.min.js"));

            bundles.Add(new ScriptBundle("~/jsbase/bootstrap").Include(
                      //"~/Content/assets/plugins/bootstrap/js/bootstrap.bundle.min.js"
                      "~/Scripts/bootstrap.bundle.min.js"

                      ));

            bundles.Add(new ScriptBundle("~/jsbase/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            // IE 9
            bundles.Add(new ScriptBundle("~/jsbase/ie9").Include(
                      "~/Content/assets/crossbrowserjs/html5shiv.js",
                      "~/Content/assets/crossbrowserjs/respond.min.js",
                      "~/Content/assets/crossbrowserjs/excanvas.min.js"));

            // Page Loader
            bundles.Add(new ScriptBundle("~/jsbase/pageloader").Include(
                      "~/Content/assets/plugins/pace/pace.min.js"));

            bundles.Add(new ScriptBundle("~/jsbase/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // SlimScroll
            bundles.Add(new ScriptBundle("~/jsbase/slimScroll").Include(
                      "~/Content/assets/plugins/slimscroll/jquery.slimscroll.min.js"));


            // lsCache
            bundles.Add(new ScriptBundle("~/jsbase/lsCache").Include(
                      "~/Content/assets/plugins/lsCache/lsCache.min.js"));

            // Cookies
            bundles.Add(new ScriptBundle("~/jsbase/jscookie").Include(
                      "~/Content/assets/plugins/js-cookie/js.cookie.js"));

            // Custom Code
            bundles.Add(new ScriptBundle("~/jsbase/custom").Include(
                        "~/Scripts/custom/custom.js"));

            // Theme
            bundles.Add(new ScriptBundle("~/jsbase/theme").Include(
                        "~/Content/assets/js/theme/default.min.js",
                        "~/Content/assets/js/apps.js",
                        "~/Content/assets/js/custom.js"));

            // Notifications
            bundles.Add(new ScriptBundle("~/jsbase/notifications").Include(
                        "~/Content/assets/plugins/gritter/js/jquery.gritter.min.js",
                        "~/Content/assets/plugins/toastr/toastr.min.js"));

            bundles.Add(new ScriptBundle("~/jspage/datepicker").Include(
                        "~/Content/assets/plugins/bootstrap-datepicker/js/bootstrap-datepicker.min.js"));

            bundles.Add(new ScriptBundle("~/jsbase/custom").Include(
                                "~/Content/lib/document.js",
                                "~/Content/lib/ajax.js",
                                "~/Content/lib/combo.js",
                                "~/Content/lib/form.js",
                                "~/Content/lib/func.js",
                                "~/Content/lib/inputPlugins.js",
                                "~/Content/lib/jsext.js",
                                "~/Content/lib/table.js",
                                "~/Content/lib/tree.js"
                                ));

            bundles.Add(new ScriptBundle("~/jsbase/inputmask").Include(
                    "~/Content/assets/plugins/inputmask/min/inputmask.min.js",
                    "~/Content/assets/plugins/inputmask/min/jquery.inputmask.min.js",
                    "~/Content/assets/plugins/inputmask/min/inputmask.extensions.min.js",
                    "~/Content/assets/plugins/inputmask/min/inputmask.date.extensions.min.js",
                    "~/Content/assets/plugins/inputmask/min/inputmask.numeric.extensions.min.js"));

            bundles.Add(new ScriptBundle("~/jspage/inputcontrols").Include(
                    "~/Content/assets/plugins/bootstrap-datepicker/js/bootstrap-datepicker.min.js",
                    "~/Content/assets/plugins/bootstrap-daterangepicker/daterangepicker.js",
                    "~/Content/assets/plugins/switchery/switchery.js",
                    "~/Content/assets/plugins/bootstrap-select/bootstrap-select.min.js",
                    "~/Content/assets/plugins/tagsinput/bootstrap-tagsinput.js",
                    "~/Content/assets/plugins/bootstrap-wysihtml5/dist/bootstrap3-wysihtml5.all.min.js",
                    "~/Content/assets/plugins/tagsinput/typeahead.bundle.js"));

            // Chart
            bundles.Add(new ScriptBundle("~/jspage/chart-flot").Include(
                        "~/Content/assets/plugins/flot/jquery.flot.min.js",
                        "~/Content/assets/plugins/flot/jquery.flot.time.min.js",
                        "~/Content/assets/plugins/flot/jquery.flot.resize.min.js",
                        "~/Content/assets/plugins/flot/jquery.flot.pie.min.js",
                        "~/Content/assets/plugins/sparkline/jquery.sparkline.js"));

            // dropZone 
            bundles.Add(new ScriptBundle("~/jspage/dropZone").Include(
                      "~/Content/assets/plugins/dropzone/min/dropzone.min.js",
                      "~/Content/assets/plugins/heic2any/heic2any.min.js"));

            // wysihtml5 
            bundles.Add(new ScriptBundle("~/jspage/wysihtml5").Include(
                      "~/Content/assets/plugins/bootstrap-wysihtml5/dist/bootstrap3-wysihtml5.all.min.js"));

            // switchery 
            bundles.Add(new ScriptBundle("~/jspage/switchery").Include(
                      "~/Scripts/plugins/switchery/switchery.js"));

            // Datatables plugins
            bundles.Add(new ScriptBundle("~/jspage/datatableExt")
                .Include("~/Content/assets/plugins/DataTables/extensions/TreeGrid/dataTables.treegrid.js"));
            

            //bundles.Add(new ScriptBundle("~/jspage/datatables").Include(
            //         "https://cdn.datatables.net/1.10.24/js/jquery.dataTables.min.js",
            //         "https://cdn.datatables.net/1.10.24/js/dataTables.bootstrap4.min.js",
            //         "~/plugins/DataTables/extensions/TreeGrid/dataTables.treegrid.js"));


            
            #endregion

            //BundleTable.EnableOptimizations = false;
        }
    }
}
