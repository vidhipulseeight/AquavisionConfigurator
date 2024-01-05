using Aquavision.Presentation.Common.Configuration;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebMatrix.WebData;

namespace Aquavision.Administration {
	public class MvcApplication : HttpApplication {
		private static SimpleMembershipInitializer myAuthenticationInitializer;
		private static object myAuthenticationInitializerLock = new object();
		private static bool myIsAuthenticationInitialized;

		protected void Application_Start() {
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			AdministrationBundleConfig.RegisterBundles(BundleTable.Bundles);

			BundleTable.Bundles.Add(new ScriptBundle("~/adminJS").Include(
				"~/sharedassets/plugins/bootbox/bootbox.js",
				"~/Scripts/croppic.js"));

			BundleTable.Bundles.Add(new StyleBundle("~/admin").Include("~/Content/croppic.css"));

			LazyInitializer.EnsureInitialized(ref myAuthenticationInitializer, ref myIsAuthenticationInitialized, ref myAuthenticationInitializerLock);
		}

		public class SimpleMembershipInitializer {
			public SimpleMembershipInitializer() {
				if (!WebSecurity.Initialized) {
					WebSecurity.InitializeDatabaseConnection("AccountManagement", "UserProfile", "UserId", "UserName", false);
				}
			}
		}
	}
}
