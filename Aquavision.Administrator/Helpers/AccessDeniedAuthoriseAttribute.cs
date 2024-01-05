using System.Web.Mvc;

namespace Aquavision.Administration.Helpers {
	public class AccessDeniedAuthorizeAttribute : AuthorizeAttribute {
		public override void OnAuthorization(AuthorizationContext filterContext) {
			base.OnAuthorization(filterContext);

			if (filterContext.Result is HttpUnauthorizedResult) {
				filterContext.Result = filterContext.HttpContext.Request.IsAuthenticated ? (ActionResult)new ViewResult { ViewName = "~/Views/Home/AccessDenied.cshtml", MasterName = null } : new RedirectResult("~/");
			}
		}
	}
}