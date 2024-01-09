using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace Aquavision.Administration.Helpers {
	public static class HtmlHelpers {
		private static string GetActiveString() {
			var activeString = ConfigurationManager.AppSettings["MenuActiveString"];
			return !string.IsNullOrEmpty(activeString) ? activeString : "active";
		}

		public static MvcHtmlString MenuLink(this HtmlHelper htmlHelper, string controllerName, string areaName) {
			return MenuLink(htmlHelper, new[] {controllerName}, areaName);
		}

		public static MvcHtmlString MenuLink(this HtmlHelper htmlHelper, string[] controllerNames, string areaName) {
			var currentController = htmlHelper.ViewContext.RouteData.GetRequiredString("controller");
			var area = (string)htmlHelper.ViewContext.RouteData.DataTokens["area"] ?? string.Empty;
			return controllerNames.Any(controllerName => controllerName.ToLower() == currentController.ToLower() && area == areaName) ? new MvcHtmlString(GetActiveString()) : new MvcHtmlString(string.Empty);
		}

		public static bool ActiveMenuLink(this HtmlHelper htmlHelper, string controllerName, string areaName) {
			return ActiveMenuLink(htmlHelper, new[] {controllerName}, areaName);
		}

		public static bool ActiveMenuLink(this HtmlHelper htmlHelper, string[] controllerNames, string areaName) {
			var currentController = htmlHelper.ViewContext.RouteData.GetRequiredString("controller");
			var area = (string)htmlHelper.ViewContext.RouteData.DataTokens["area"] ?? string.Empty;
			return controllerNames.Any(controllerName => controllerName == currentController && area == areaName);
		}

		public static bool ActiveSubMenuLink(this HtmlHelper htmlHelper, string actionName, string controllerName, string areaName) {
			var action = htmlHelper.ViewContext.RouteData.GetRequiredString("action");
			var currentController = htmlHelper.ViewContext.RouteData.GetRequiredString("controller");
			var area = (string)htmlHelper.ViewContext.RouteData.DataTokens["area"] ?? string.Empty;
			return action == actionName && controllerName == currentController && area == areaName;
		}

		public static bool ActiveSubMenuLinkWithProperty(this HtmlHelper htmlHelper, string actionName, string controllerName, string areaName, string propertyName, string propertyValue) {
			var action = htmlHelper.ViewContext.RouteData.GetRequiredString("action");
			var currentController = htmlHelper.ViewContext.RouteData.GetRequiredString("controller");
			var area = (string)htmlHelper.ViewContext.RouteData.DataTokens["area"] ?? string.Empty;
			if (!htmlHelper.ViewContext.RouteData.Values.TryGetValue(propertyName, out var property) || property.ToString() != propertyValue) {
				return false;
			}
			return action == actionName && controllerName == currentController && area == areaName;
		}

		public static MvcHtmlString MenuAreaLink(this HtmlHelper htmlHelper, string areaName) {
			return (string)htmlHelper.ViewContext.RouteData.DataTokens["area"] == (areaName ?? string.Empty) ? new MvcHtmlString(GetActiveString()) : new MvcHtmlString(string.Empty);
		}

		public static bool ActiveMenuAreaLink(this HtmlHelper htmlHelper, string areaName) {
			return (string)htmlHelper.ViewContext.RouteData.DataTokens["area"] == (areaName ?? string.Empty);
		}

		public static MvcHtmlString SubMenuLink(this HtmlHelper htmlHelper, string actionName, string controllerName, string areaName) {
			var action = htmlHelper.ViewContext.RouteData.GetRequiredString("action");
			var currentController = htmlHelper.ViewContext.RouteData.GetRequiredString("controller");
			var area = (string)htmlHelper.ViewContext.RouteData.DataTokens["area"] ?? string.Empty;
			return action == actionName && controllerName.ToLower() == currentController.ToLower() && area == areaName ? new MvcHtmlString(GetActiveString()) : new MvcHtmlString(string.Empty);
		}

		public static MvcHtmlString SubMenuLinkWithProperty(this HtmlHelper htmlHelper, string actionName, string controllerName, string areaName, string propertyName, string propertyValue) {
			var action = htmlHelper.ViewContext.RouteData.GetRequiredString("action");
			var currentController = htmlHelper.ViewContext.RouteData.GetRequiredString("controller");
			var area = (string)htmlHelper.ViewContext.RouteData.DataTokens["area"] ?? string.Empty;
			if (!htmlHelper.ViewContext.RouteData.Values.TryGetValue(propertyName, out var property) || property.ToString() != propertyValue) {
				return new MvcHtmlString(string.Empty);
			}
			return action == actionName && controllerName == currentController && area == areaName ? new MvcHtmlString(GetActiveString()) : new MvcHtmlString(string.Empty);
		}
		
		public static MvcHtmlString IsSite(this HtmlHelper htmlHelper, string siteName) {
			return new MvcHtmlString(ConfigurationManager.AppSettings["SiteName"] == siteName ? GetActiveString() : string.Empty);
		}
	}
}