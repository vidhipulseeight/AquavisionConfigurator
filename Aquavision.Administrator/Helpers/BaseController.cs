using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Aquavision.Administration.Helpers {
	public abstract class BaseController : Controller {
		protected string DEFAULT_META = "Aquavision";
		protected string DEFAULT_TITLE = "Aquavision";
		protected string DEFAULT_THEME = "Aquavision";
		protected bool myAppendDefaultTitle = true;

		private readonly List<BreadCrumb> myBreadCrumbs = new List<BreadCrumb>();

		protected virtual bool InsertLog(string message, Exception exception) { return false; }

		[SuppressMessage("ReSharper", "InconsistentNaming")]
		public static class Settings {
			public static string NSDataCenterURL = "NSDataCenterURL";
		}

		protected void AddBreadCrumb(BreadCrumb crumb) {
			myBreadCrumbs.Add(crumb);
		}

		protected void AddBreadCrumb(string name, string url = "") {
			AddBreadCrumb(new BreadCrumb(name, url));
		}

		protected override void OnActionExecuted(ActionExecutedContext filterContext) {
			ViewBag.BreadCrumbs = myBreadCrumbs;
			if (!string.IsNullOrEmpty(ViewBag.Error)) {
				ViewBag.Error = ViewBag.Error + "<br />" + TempData["Error"];
			} else {
				ViewBag.Error = TempData["Error"];
			}
			if (!string.IsNullOrEmpty(ViewBag.Warning)) {
				ViewBag.Warning = ViewBag.Warning + "<br />" + TempData["Warning"];
			} else {
				ViewBag.Warning = TempData["Warning"];
			}
			if (!string.IsNullOrEmpty(ViewBag.Success)) {
				ViewBag.Success = ViewBag.Success + "<br />" + TempData["Success"];
			} else {
				ViewBag.Success = TempData["Success"];
			}
			if (ViewBag.Title == DEFAULT_TITLE && !string.IsNullOrWhiteSpace(ViewBag.PageTitle.ToString())) {
				ViewBag.Title = ViewBag.PageTitle.ToString();
			}
			if (ViewBag.Title != DEFAULT_TITLE && myAppendDefaultTitle) {
				ViewBag.Title += " - " + DEFAULT_TITLE;
			}
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext) {
#if DEBUG
			ViewBag.Debug = true;
#else
			ViewBag.Debug = false;
#endif
			ViewBag.ThemeName = DEFAULT_THEME;
			if (string.IsNullOrEmpty(ViewBag.Title)) {
				ViewBag.Title = DEFAULT_TITLE;
			}
			if (string.IsNullOrEmpty(ViewBag.PageTitle)) {
				ViewBag.PageTitle = string.Empty;
			}
			if (string.IsNullOrEmpty(ViewBag.PageSubTitle)) {
				ViewBag.PageSubTitle = string.Empty;
			}
			base.OnActionExecuting(filterContext);
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

		protected override void OnException(ExceptionContext filterContext) {
			if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled) {
				return;
			}

			if (IsAjax(filterContext)) {
				filterContext.Result = new JsonResult {
					Data = filterContext.Exception.Message,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet
				};

				filterContext.ExceptionHandled = true;
				filterContext.HttpContext.Response.Clear();
			} else {
				InsertLog("Exception Raised, Not Handled", filterContext.Exception);
				//MailHelper.SendInternalMail("Exception Raised, Not Handled", filterContext.Exception.ToString());

				filterContext.ExceptionHandled = true;
				View("Error").ExecuteResult(ControllerContext);
				filterContext.HttpContext.Response.Clear();
				filterContext.HttpContext.Response.StatusCode = 500;
				filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
				base.OnException(filterContext);
			}
		}

		private static bool IsAjax(ExceptionContext filterContext) {
			return filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
		}
		protected string GetCurrentUserName() {
			return User.Identity.Name.Substring(User.Identity.Name.IndexOf('\\') + 1);
		}
	}
}
