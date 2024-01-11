using AquavisionConfigurator.App_Start;
using System.Diagnostics;
using System.Web.Mvc;

namespace AquavisionConfigurator.Helpers {
	public abstract class BaseController : Controller {
		protected override void OnActionExecuted(ActionExecutedContext filterContext) {
			if (!(filterContext.Result is RedirectToRouteResult)) {
				if (!AquavisionContext.Current.SessionSaveToClient()) {
					Debug.WriteLine("Failed to Save Session to Client");
				}
			}
			base.OnActionExecuted(filterContext);
		}
	}
}