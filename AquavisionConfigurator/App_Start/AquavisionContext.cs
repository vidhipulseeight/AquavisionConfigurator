using Aquavision.Data.Models;
using System;
using System.Linq;
using System.Threading;
using System.Web;

namespace AquavisionConfigurator.App_Start {
	public class AquavisionContext {
		private const string AQUAVISIONCONTEXT = "AquavisionContext";
		private const int COOKIE_EXPIRES = 128;
		private const string CUSTOMERSESSION = "CustomerSession";
		private const string CUSTOMERSESSIONCOOKIE = "CustomerSessionGUIDCookie";
		private readonly HttpContext myContext = HttpContext.Current;

		public static AquavisionContext Current {
			get {
				if (HttpContext.Current == null) {
					var data = Thread.GetData(Thread.GetNamedDataSlot(AQUAVISIONCONTEXT));
					if (data != null) {
						return (AquavisionContext)data;
					}
					var context = new AquavisionContext();
					Thread.SetData(Thread.GetNamedDataSlot(AQUAVISIONCONTEXT), context);
					return context;
				}
				if (HttpContext.Current.Items[AQUAVISIONCONTEXT] == null) {
					var context = new AquavisionContext();
					HttpContext.Current.Items.Add(AQUAVISIONCONTEXT, context);
					return context;
				}
				return (AquavisionContext)HttpContext.Current.Items[AQUAVISIONCONTEXT];
			}
		}
		public object this[string key] {
			get => myContext?.Items[key];
			set {
				if (myContext != null) {
					myContext.Items.Remove(key);
					myContext.Items.Add(key, value);
				}
			}
		}
		public CustomerSession Session {
			get => GetSession(false);
			set => Current[CUSTOMERSESSION] = value;
		}

		public Customer Customer { get; set; }
		public CustomerSession GetSession(bool createInDatabase) {
			CustomerSession byId = null;
			object obj2 = Current[CUSTOMERSESSION];
			if (obj2 != null) {
				byId = (CustomerSession)obj2;
			}
			if (byId == null && createInDatabase) {
				byId = SaveSessionToDatabase();
			}
			string customerSessionCookieValue = string.Empty;
			if (HttpContext.Current.Request.Cookies[CUSTOMERSESSIONCOOKIE] != null && HttpContext.Current.Request.Cookies[CUSTOMERSESSIONCOOKIE].Value != null) {
				customerSessionCookieValue = HttpContext.Current.Request.Cookies[CUSTOMERSESSIONCOOKIE].Value;
			}
			if (byId == null && !string.IsNullOrEmpty(customerSessionCookieValue)) {
				byId = GetCustomerSessionByGUID(new Guid(customerSessionCookieValue));
			}
			Current[CUSTOMERSESSION] = byId;
			return byId;
		}
		private CustomerSession SaveSessionToDatabase() {
			var sessionId = Guid.NewGuid();
			while (GetCustomerSessionByGUID(sessionId) != null) {
				sessionId = Guid.NewGuid();
			}
			var session = new CustomerSession {
				GUID = sessionId,
				CustomerId = Customer?.Id ?? 0,
				LastAccessed = DateTime.UtcNow,
				IsExpired = false
			};

			using (var db = new AquavisionEntities()) {
				db.CustomerSessions.Add(session);
				db.SaveChanges();
			}
			return session;
		}
		private static CustomerSession GetCustomerSessionByGUID(Guid guid) {
			using (var db = new AquavisionEntities()) {
				return db.CustomerSessions.FirstOrDefault(c => c.GUID == guid);
			}
		}
		public bool SessionSaveToClient() {
			if (HttpContext.Current != null && Session != null) {
				SetCookie(HttpContext.Current.ApplicationInstance, CUSTOMERSESSIONCOOKIE, Session.GUID.ToString());
				return true;
			}
			return false;
		}
		private static void SetCookie(HttpApplication application, string key, string val) {
			var cookie = new HttpCookie(key) {
				Value = val,
				Expires = string.IsNullOrEmpty(val) ? DateTime.Now.AddMonths(-1) : DateTime.Now.AddHours(COOKIE_EXPIRES)
			};
			application.Response.Cookies.Remove(key);
			application.Response.Cookies.Add(cookie);
		}
		public void ResetSession() {
			if (HttpContext.Current != null) {
				SetCookie(HttpContext.Current.ApplicationInstance, CUSTOMERSESSIONCOOKIE, string.Empty);
			}
			Session = null;
			Customer = null;
		}
	}
}