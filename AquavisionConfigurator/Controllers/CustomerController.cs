using AquavisionConfigurator.App_Start;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Cryptography;
using static System.Web.Security.FormsAuthentication;
using Aquavision.Data.Models;
using AquavisionConfigurator.Helpers;
using System.Collections.Generic;

namespace AquavisionConfigurator.Controllers {
	public class CustomerController : BaseController {
		AquavisionEntities myDB = new AquavisionEntities();
		public ActionResult Login() {
			return View();
		}

		[HttpPost]
		public JsonResult Login(FormCollection collection) {
			var email = collection["customerEmail"];
			var password = collection["customerPassword"];
			var rememberme = collection["rememberme"];

			if (!CommonHelper.IsValidEmail(email)) {
				return Json(new { Result = false, Message = "Please enter a valid email address" }, JsonRequestBehavior.AllowGet);
			}
			var customer = GetCustomerByEmail(email);

			if (customer == null || customer.Deleted) {
				return Json(new { Result = false, Message = "Sorry this account doesnt exist or is no longer available" }, JsonRequestBehavior.AllowGet);
			}

			string passwordHash = CreatePasswordHash(password, customer.SaltKey);
			if (customer.PasswordHash.Equals(passwordHash)) {
				SetAuthCookie(email, rememberme != null);			
				var customerSession = myDB.CustomerSessions.FirstOrDefault(cs => cs.CustomerId == customer.Id);
				if (customerSession != null) {
					customerSession.IsExpired = false;
					customerSession.LastAccessed = DateTime.UtcNow;
					customerSession.CustomerId = customer.Id;
					AquavisionContext.Current.Session = customerSession;
					myDB.SaveChanges();
				}

				return Json(new { Result = true, CustomerId = customer.Id, Message = string.Empty }, JsonRequestBehavior.AllowGet);
			}
			return Json(new { Result = false, Message = "Sorry your username or password is incorrect" }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult Register(FormCollection collection) {
			var name = collection["customerRegisterFullName"];
			var phone = collection["customerRegisterPhone"];
			var email = collection["customerRegisterEmail"];
			var password = collection["customerRegisterPassword"];
			var confirmpassword = collection["customerRegisterConfirmPassword"];

			ViewBag.Email = email;

			if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmpassword)) {
				return Json(new { Result = false, Message = "None of the above fields can be blank." }, JsonRequestBehavior.AllowGet);
			}
			if (!CommonHelper.IsValidEmail(email)) {
				return Json(new { Result = false, Message = "Please enter a valid email address" }, JsonRequestBehavior.AllowGet);
			}
			if (!CommonHelper.IsValidPhone(phone)) {
				return Json(new { Result = false, Message = "Please enter a valid phone number" }, JsonRequestBehavior.AllowGet);
			}
			if (password != confirmpassword) {
				return Json(new { Result = false, Message = "Your password doesn't match, please re-enter" }, JsonRequestBehavior.AllowGet);
			}

			if (password.Length < 6) {
				return Json(new { Result = false, Message = "Please enter a password at least 6 characters long." }, JsonRequestBehavior.AllowGet);
			}

			var existingEmail = myDB.Customers.FirstOrDefault(c => c.Email == email);
			if (existingEmail != null) {
				return Json(new { Result = false, Message = $"You already appear to have an account, <a href=\"{Url.Action("ForgotPass", "Home")}\">Forgot Password?</a>" }, JsonRequestBehavior.AllowGet);
			}
			MembershipCreateStatus status;
			email = email.Trim();
			if (GetCustomerByEmail(email) != null) {
				status = MembershipCreateStatus.DuplicateEmail;
				return null;
			}

			if (!CommonHelper.IsValidEmail(email)) {
				status = MembershipCreateStatus.InvalidEmail;
				return null;
			}

			if (email.Length > 100) {
				status = MembershipCreateStatus.InvalidEmail;
				return null;
			}

			string saltKey = CreateSalt(5);
			string passwordHash = CreatePasswordHash(password, saltKey);
			var customer = CreateCustomer(name, phone, Guid.NewGuid(), email, passwordHash, saltKey);

			status = MembershipCreateStatus.Success;

			if (status == MembershipCreateStatus.Success) {
				SetAuthCookie(email, false);
				//WebSecurity.CreateAccount(email, password, false);
				return Json(new { Result = true, CustomerId = customer.Id, Message = string.Empty }, JsonRequestBehavior.AllowGet);
			}

			return Json(new { Result = false, Message = "Something went wrong creating your new account, please try again later" }, JsonRequestBehavior.AllowGet);
		}

		public JsonResult LinkCustomerBuild(int? productId, List<ProductOption> productOptions, int? customerId) {
			if (productId.HasValue) {
				var customerDesign = new CustomerDesign {
					CustomerId = customerId.Value,
					ProductId = productId.Value
				};
				myDB.CustomerDesigns.Add(customerDesign);
				myDB.SaveChanges();
				foreach (var option in productOptions) {
					var productOption = myDB.ProductOptions.FirstOrDefault(p => p.Id == option.Id);
					if (productOption != null) {
						var customerDesignSpec = new CustomerDesignSpec {
							CustomerDesignId = customerDesign.Id,
							ProductOptionId = productOption.Id
						};
						myDB.CustomerDesignSpecs.Add(customerDesignSpec);
						myDB.SaveChanges();
					}
				}
				return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
			}
			return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
		}

		public JsonResult IsRegistered() {
			if(GetCurrentCustomerData() != null) {
				return Json(new { Result = true, CustomerId = GetCurrentCustomerData().Id }, JsonRequestBehavior.AllowGet);
			} else {
				return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
			}
		}

		protected Customer GetCustomerByEmail(string emailAddress) {
			return myDB.Customers.FirstOrDefault(c => c.Email == emailAddress);
		}

		protected Customer AddCustomer(string name, string phone, string email, string password, out MembershipCreateStatus status) {
			email = email.Trim();
			if (GetCustomerByEmail(email) != null) {
				status = MembershipCreateStatus.DuplicateEmail;
				return null;
			}

			if (!CommonHelper.IsValidEmail(email)) {
				status = MembershipCreateStatus.InvalidEmail;
				return null;
			}

			if (email.Length > 100) {
				status = MembershipCreateStatus.InvalidEmail;
				return null;
			}

			string saltKey = CreateSalt(5);
			string passwordHash = CreatePasswordHash(password, saltKey);
			var customer = CreateCustomer(name, phone, Guid.NewGuid(), email, passwordHash, saltKey);

			status = MembershipCreateStatus.Success;

			//SendWelcomeEmail(customer);
			return customer;
		}

		public Customer CreateCustomer(string name, string phone, Guid customerGuid, string email, string passwordHash, string saltKey) {
			email = (email ?? string.Empty).Trim();
			email = CommonHelper.EnsureMaximumLength(email, 255);
			passwordHash = passwordHash ?? string.Empty;
			passwordHash = CommonHelper.EnsureMaximumLength(passwordHash, 255);
			saltKey = saltKey ?? string.Empty;
			saltKey = CommonHelper.EnsureMaximumLength(saltKey, 255);

			var customer = new Customer {
				Name = name,
				Address = string.Empty,
				Phone = phone,
				CustomerGUID = customerGuid,
				Email = email,
				PasswordHash = passwordHash,
				SaltKey = saltKey,
				Deleted = false
			};

			myDB.Customers.Add(customer);
			myDB.SaveChanges();

			var session = new CustomerSession {
				GUID = Guid.NewGuid(),
				CustomerId = customer.Id,
				LastAccessed = DateTime.UtcNow,
				IsExpired = false
			};
			myDB.CustomerSessions.Add(session);
			myDB.SaveChanges();

			return customer;
		}

		protected static string CreateSalt(int size) {
			var provider = new RNGCryptoServiceProvider();
			var data = new byte[size];
			provider.GetBytes(data);
			return Convert.ToBase64String(data);
		}

		protected static string CreatePasswordHash(string password, string salt) => HashPasswordForStoringInConfigFile(password + salt, "SHA1");

		//protected void SendWelcomeEmail(Customer customer) {
		//	var mandrill = new MandrillEmail();
		//	var primaryAddress = customer.Addresses.FirstOrDefault();
		//	var customerName = "Pulse-Eight Groupie";
		//	var firstName = string.Empty;
		//	if (primaryAddress != null) {
		//		customerName = primaryAddress.GetName();
		//		firstName = primaryAddress.FirstName + ", ";
		//	}
		//	mandrill.SendTemplate(new MailAddress(customer.Email, customerName), null, $"{firstName}Thanks for registering", "New Customer Welcome",
		//		new List<Tuple<string, string>> {
		//						new Tuple<string, string>("MYACCOUNT", BASE_SECURE_PUBLIC_SITE_URL + Url.Action("Index", "Account"))
		//					},

		//		new Dictionary<string, string>());
		//}
		public ActionResult Logout() {
			DoLogout();
			return RedirectToAction("Index", "Home");
		}
		public ActionResult Details() {
			var currentCustomer = GetCurrentCustomerData();
			if (currentCustomer != null) {
				var customerDesigns = myDB.CustomerDesigns.Where(cd => cd.CustomerId == currentCustomer.Id).ToList();
				return View(customerDesigns);
			}
			return RedirectToAction("Login","Customer");
			//if (AquavisionContext.Current.Session == null) {
			//	return RedirectToAction("Index", "Home");
			//}
			//var buidcart = myDB.BuildCarts.Where(b => b.CustomerSessionGUID == AquavisionContext.Current.Session.GUID).ToList();
			//return View(buidcart);
		}

		public ActionResult BuildDetails(int id) {
			var currentCustomer = GetCurrentCustomerData();
			if (currentCustomer != null) {
				var customerDesign = myDB.CustomerDesigns.FirstOrDefault(cd => cd.Id == id);
				if (customerDesign != null) {
					return View(customerDesign);
				}
				TempData["Error"] = "Unable to fine the product data.";
				return RedirectToAction("Details", "Customer");
			}
			return null;
			//if (AquavisionContext.Current.Session == null) {
			//	return RedirectToAction("Index", "Home");
			//}
			//var buidcart = myDB.BuildCarts.Where(b => b.CustomerSessionGUID == AquavisionContext.Current.Session.GUID).ToList();
			//return View(buidcart);
		}
	}
}