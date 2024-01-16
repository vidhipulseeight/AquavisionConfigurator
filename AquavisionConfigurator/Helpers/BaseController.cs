using Aquavision.Data.Models;
using AquavisionConfigurator.App_Start;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using static System.Web.Security.FormsAuthentication;

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
		protected string GetCurrentCustomer() {
			if (AquavisionContext.Current.Session != null) {
				var customerId = AquavisionContext.Current.Session.CustomerId;
				var myDB = new AquavisionEntities();
				var customer = myDB.Customers.FirstOrDefault(c => c.Id == customerId);
				if (customer != null) {
					return customer.Name;
				}
			}
			return string.Empty;
		}

		protected Customer GetCurrentCustomerData() {
			if (AquavisionContext.Current.Session != null) {
				var customerId = AquavisionContext.Current.Session.CustomerId;
				var myDB = new AquavisionEntities();
				var customer = myDB.Customers.FirstOrDefault(c => c.Id == customerId);
				if (customer != null) {
					return customer;
				}
			}
			return null;
		}
		protected override void OnAuthentication(AuthenticationContext filterContext) {
			var myDB = new AquavisionEntities();
			if (User?.Identity.IsAuthenticated ?? false) {
				var name = User.Identity.Name;
				var customer = myDB.Customers.FirstOrDefault(c => c.Email == name);
				if (customer != null) {
					if (!string.IsNullOrEmpty(User.Identity.Name) && !customer.Deleted) {
						AquavisionContext.Current.Customer = customer;

						//set current customer session
						var customerSession = myDB.CustomerSessions.FirstOrDefault(cs => cs.CustomerId == AquavisionContext.Current.Customer.Id);
						if (customerSession == null) {
							customerSession = AquavisionContext.Current.GetSession(true);
							customerSession.IsExpired = false;
							customerSession.LastAccessed = DateTime.UtcNow;
							customerSession.CustomerId = AquavisionContext.Current.Customer.Id;
							myDB.SaveChanges();
						}
						AquavisionContext.Current.Session = customerSession;
					} else {
						DoLogout();
					}
				} else {
					DoLogout();
				}
			} else {
				if (AquavisionContext.Current.Session != null) {
					var guestCustomer = myDB.Customers.FirstOrDefault(c => c.Id == AquavisionContext.Current.Session.CustomerId);
					if (guestCustomer != null && !guestCustomer.Deleted) {
						AquavisionContext.Current.Customer = guestCustomer;
					}
				}
			}
			ViewBag.CurrentCustomer = GetCurrentCustomer();
			base.OnAuthentication(filterContext);
		}
		protected void DoLogout() {
			Session.Abandon();
			SignOut();
			if (AquavisionContext.Current != null) {
				AquavisionContext.Current.ResetSession();
				Response.Redirect("/");
			}
		}

	}
}