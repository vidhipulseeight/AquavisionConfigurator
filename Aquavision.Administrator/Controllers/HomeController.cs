using Aquavision.Administration.Helpers;
using System.Web.Mvc;
namespace Aquavision.Administration.Controllers {
	[Authorize]
	public class HomeController : BaseController {
		public ActionResult Index() {
			var user = GetCurrentUserName();
			return View();
		}
	}
}