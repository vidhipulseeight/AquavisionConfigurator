using System.Linq;
using System.Web.Mvc;
using Aquavision.Data.Models;
using AquavisionConfigurator.Helpers;

namespace AquavisionConfigurator.Controllers {
	public class HomeController : BaseController {
		AquavisionEntities myDB = new AquavisionEntities();
		public ActionResult Index() {
			ViewBag.Categories = myDB.Categories.ToList();
			ViewBag.Products = myDB.Products.Take(4).ToList();
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
			ViewBag.ProductId = productId;
			ViewBag.ProductOptionGroups = myDB.ProductOptionGroups.Where(p => p.ProductId == productId).ToList();
			return View();
		}
		public JsonResult GetProductImage(int Id) {
			var productImage = myDB.Images.Where(p => p.ProductOptionId == Id).FirstOrDefault();
			if (productImage != null) {
				try {
					return Json(new { ImageURL = productImage.ProductImage }, JsonRequestBehavior.AllowGet);
				} catch { return Json(new { ImageURL = string.Empty }, JsonRequestBehavior.AllowGet); }
			}
			return Json(new { ImageURL = string.Empty }, JsonRequestBehavior.AllowGet);
		}
	}
}