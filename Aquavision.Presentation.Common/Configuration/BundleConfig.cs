using System.Web.Optimization;

namespace Aquavision.Presentation.Common.Configuration {
	public class BundleConfig {
		public static void RegisterBundles(BundleCollection bundles) {
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
						"~/Scripts/jquery-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
						"~/Scripts/bootstrap.js",
						"~/Scripts/bootstrap-select.js",
						"~/sharedassets/plugins/bootstrap-modal/js/bootstrap-modalmanager.js",
						"~/sharedassets/plugins/bootstrap-modal/js/bootstrap-modal.js"));

			bundles.Add(new ScriptBundle("~/bundles/plugins").Include(
						"~/Scripts/owl.carousel.js",
						"~/Scripts/superfish.js",
						"~/assets/plugins/jquery.sticky.js",
						"~/assets/plugins/prettyphoto/js/jquery.prettyPhoto.js",
						"~/Scripts/jquery.easing.{version}.js",
						"~/Scripts/jquery.cookie.js"));

			bundles.Add(new ScriptBundle("~/bundles/store").Include(
						"~/assets/js/core.js",
						"~/Scripts/parallax.js",
						"~/Scripts/smooth-scroll.min.js",
						"~/Scripts/scripts.js",
						"~/Scripts/nicepage.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
						"~/Scripts/jquery-ui-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/slick-plugin").Include(
						"~/Scripts/Slick/slick.js").Include("~/assets/js/theme.js"));

			bundles.Add(new StyleBundle("~/Content/main-css").Include(
					  "~/Content/bootstrap.css",
					  "~/Content/bootstrap-select.css",
					  "~/sharedassets/plugins/bootstrap-modal/css/bootstrap-modal-bs3patch.css",
					  "~/sharedassets/plugins/bootstrap-modal/css/bootstrap-modal.css",
					  "~/Content/OwlCarousel/owl.carousel.css",
					  "~/Content/OwlCarousel/owl.theme.default.css",
					  "~/Content/Slick/slick.css",
					  "~/Content/Slick/slick-theme.css",
					  "~/Content/animate.css",
					  "~/assets/plugins/prettyphoto/css/prettyPhoto.css").Include("~/css/font-awesome.css"));


			bundles.Add(new StyleBundle("~/Content/store-branding").Include(
				"~/assets/css/theme-store.css",
				"~/assets/css/common.css",
				"~/assets/css/fonts.css",
				"~/css/theme-charcoal.css",
				"~/css/iconsmind.css",
				"~/css/stack-interface.css",
				"~/css/nicepage.css",
				"~/css/News-and-Articles.css",
				"~/css/Blogs.css"));


			bundles.Add(new StyleBundle("~/Content/my-branding").Include(
				"~/assets/css/common.css",
				"~/assets/css/portlets.css",
				"~/assets/css/theme-my.css"));
#if DEBUG
			BundleTable.EnableOptimizations = false;
#else
			BundleTable.EnableOptimizations = true;
#endif
		}
	}
}
