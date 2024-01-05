using System.Web.Optimization;

namespace Aquavision.Presentation.Common.Configuration {
	public class AdministrationBundleConfig {
		public static void RegisterBundles(BundleCollection bundles) {
			bundles.Add(new ScriptBundle("~/bundles/allJS").Include(
				"~/Scripts/jquery-{version}.js",
				"~/Scripts/jquery-ui-{version}.js",
				"~/Scripts/jquery-sortable.js",
				"~/Scripts/jquery-confirm.js",
				"~/Scripts/bootstrap.min.js",
				"~/Scripts/bootstrap-datepicker.js",
				"~/sharedassets/plugins/bootstrap-modal/js/bootstrap-modalmanager.js",
				"~/sharedassets/plugins/bootstrap-modal/js/bootstrap-modal.js",
				"~/sharedassets/plugins/jquery-slimscroll/jquery.slimscroll.js",
				"~/Scripts/contextual.js",
				"~/assets/scripts/core/app.js"
			));
			bundles.Add(new ScriptBundle("~/bundles/loginJS").Include(
				"~/Scripts/jquery.validate.min.js",
				"~/sharedassets/plugins/select2/select2.min.js",
				"~/assets/scripts/core/app.js",
				"~/assets/scripts/custom/login.js"
			));

			bundles.Add(new StyleBundle("~/css-admin").Include(
				"~/Content/bootstrap.css",
				"~/Content/bootstrap-datepicker3.css",
				"~/Content/themes/base/jquery-ui.css",
				"~/sharedassets/plugins/bootstrap-modal/css/bootstrap-modal-bs3patch.css",
				"~/sharedassets/plugins/bootstrap-modal/css/bootstrap-modal.css",
				"~/assets/css/style-metronic.css",
				"~/assets/css/style.css",
				"~/assets/css/style-responsive.css",
				"~/assets/css/plugins.css",
				"~/assets/css/default.css",
				"~/assets/css/error.css",
				"~/assets/css/timeline.css",
				"~/assets/css/custom.css",
				"~/css/jquery-confirm.css",
				"~/Content/contextual.theme.css",
				"~/Content/contextual.css"
			).Include("~/css/font-awesome.css").Include("~/Content/all.css"));

			bundles.Add(new StyleBundle("~/css/login").Include(
				"~/assets/css/login.css",
				"~/sharedassets/plugins/select2/select2.css",
				"~/sharedassets/plugins/select2/select2-metronic.css"
			));
		}
	}
}
