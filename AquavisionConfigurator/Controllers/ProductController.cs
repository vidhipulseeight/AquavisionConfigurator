using System.Linq;
using System.Web.Mvc;
using Aquavision.Data.Models;

namespace AquavisionConfigurator.Controllers {
	public class ProductController : Controller {
		AquavisionEntities myDB = new AquavisionEntities();
		public ActionResult List(int? categoryId) {
			ViewBag.Category = myDB.Categories.ToList();
			var products = myDB.Products.Where(p => !p.Deleted).ToList();
			if(categoryId.HasValue) {
				products = products.Where(p => p.CategoryId.HasValue && p.CategoryId == categoryId).ToList();
			}
			return View(products);
		}
		//public ActionResult Shop(int? productId) {
		//	ViewBag.Products = myDB.Products.ToList();
		//	if (productId == null) {
		//		var defaultProduct = myDB.Products.FirstOrDefault();
		//		if (defaultProduct != null) {
		//			productId = defaultProduct.Id;
		//		}
		//	}
		//	ViewBag.ProductId = productId;
		//	ViewBag.ProductOptionGroups = myDB.ProductOptionGroups.Where(p => p.ProductId == productId).ToList();
		//	return View();
		//}

		public ActionResult Customise(int productId) {
			ViewBag.Products = myDB.Products.ToList();

			var product = myDB.Products.FirstOrDefault(p => p.Id == productId);
			if (product == null) {
				TempData["Error"] = "Failed to find matching product";
				return RedirectToAction("List");
			}
			ViewBag.ProductId = productId;
			ViewBag.ProductOptionGroups = myDB.ProductOptionGroups.Where(p => p.ProductId == productId).ToList();

			return View(product);
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