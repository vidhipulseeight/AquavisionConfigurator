using Aquavision.Administration.Models;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace Aquavision.Administration.Controllers {
	[Authorize]
	public class AccountController : Controller {
		[AllowAnonymous]
		public ActionResult Login() {
			ViewBag.Title = "Log in";
			ViewBag.ReturnUrl = string.Empty;
			return View();
		}
		[HttpPost]
		[AllowAnonymous]
		public ActionResult Login(LoginModel model, string returnUrl) {
			if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, model.RememberMe)) {
				return RedirectToLocal(returnUrl);
			}

			ModelState.AddModelError("", "The user name or password provided is incorrect.");
			return View(model);
		}
		private ActionResult RedirectToLocal(string returnUrl) {
			if (Url.IsLocalUrl(returnUrl)) {
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Home");
		}
		[HttpPost]
		[AllowAnonymous]
		public ActionResult Register(RegisterModel model) {
			try {
				WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new {
					model.FullName,
					model.Email
				});
				WebSecurity.Login(model.UserName, model.Password);
			} catch (MembershipCreateUserException e) {
				ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
			}
			return RedirectToAction("Index", "Home");
		}
		private static string ErrorCodeToString(MembershipCreateStatus createStatus) {
			switch (createStatus) {
				case MembershipCreateStatus.DuplicateUserName:
					return "User name already exists. Please enter a different user name.";

				case MembershipCreateStatus.DuplicateEmail:
					return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

				case MembershipCreateStatus.InvalidPassword:
					return "The password provided is invalid. Please enter a valid password value.";

				case MembershipCreateStatus.InvalidEmail:
					return "The e-mail address provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidAnswer:
					return "The password retrieval answer provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidQuestion:
					return "The password retrieval question provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidUserName:
					return "The user name provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.ProviderError:
					return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				case MembershipCreateStatus.UserRejected:
					return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				default:
					return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
			}
		}
		
		public ActionResult LogOff() {
			WebSecurity.Logout();
			return RedirectToAction("Index", "Home");
		}
	}
}