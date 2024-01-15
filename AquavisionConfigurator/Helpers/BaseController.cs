using AquavisionConfigurator.App_Start;
using System.Diagnostics;
using System.IO;
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
		protected string RenderPartialViewToString(string viewName, object model) {
			if (string.IsNullOrEmpty(viewName)) {
				viewName = ControllerContext.RouteData.GetRequiredString("action");
			}

			ViewData.Model = model;

			using (var sw = new StringWriter()) {
				var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
				var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
				viewResult.View.Render(viewContext, sw);

				return sw.GetStringBuilder().ToString();
			}
		}
	}
}