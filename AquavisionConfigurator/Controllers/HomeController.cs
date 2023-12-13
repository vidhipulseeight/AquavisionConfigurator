using AquavisionConfigurator.Models;
using System.Linq;
using System.Web.Mvc;
namespace AquavisionConfigurator.Controllers {
	public class HomeController : Controller {
		AquavisionEntities myDB = new AquavisionEntities();
		public ActionResult Index() {
			ViewBag.Category = myDB.Categories.ToList();
			return View();
		}
		public ActionResult Shop(int? productId) {
			ViewBag.Products = myDB.Products.ToList();
			if (productId == null) {
				var defaultProduct = myDB.Products.FirstOrDefault();
				if (defaultProduct != null) {
					productId = defaultProduct.Id;
				}
			}
			ViewBag.ProductOptionGroups = myDB.ProductOptionGroups.Where(p => p.ProductId == productId).ToList();
			return View();
		}
	}
}