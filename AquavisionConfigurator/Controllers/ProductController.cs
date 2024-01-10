using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Aquavision.Data.Models;
using Microsoft.Ajax.Utilities;

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

		public class ImagesClass {
			public int Id { get; set; }
			public int OptionId { get; set; }
			public int GroupId { get; set; }
			public string ProductImage { get; set; }
			public string ImageData { get; set; }
		}

		public JsonResult GetProductImage(int Id) {
			var productImage = myDB.Images.Where(i => i.ProductOptionId == Id && i.ImageData != null).FirstOrDefault();
			if (productImage != null) {
				var option = myDB.ProductOptions.First(o => o.Id == productImage.ProductOptionId);
				var image = new ImagesClass {
					Id = productImage.Id,
					OptionId = productImage.ProductOptionId,
					ProductImage = productImage.ProductImage,
					GroupId = option.ProductOptionGroupId,
					ImageData = Convert.ToBase64String(productImage.ImageData)
				};
				try {
					return Json(new { Result = true, Image = image }, JsonRequestBehavior.AllowGet);
					//return Json(new { ImageURL = productImage.ProductImage }, JsonRequestBehavior.AllowGet);
				} catch { return Json(new { Result = true, Image = string.Empty }, JsonRequestBehavior.AllowGet); }
			}
			return Json(new { Result = false, Image = string.Empty }, JsonRequestBehavior.AllowGet);
		}

		

		public JsonResult GetProductDefaultImages(int Id) {
			var product = myDB.Products.FirstOrDefault(p => p.Id == Id);
			var productImage = Convert.ToBase64String(product.Image);
			var optionGroups = myDB.ProductOptionGroups.Where(prg => prg.ProductId == Id).Select(prg => prg.Id);
			var options = myDB.ProductOptions.Where(op => optionGroups.Contains(op.ProductOptionGroupId) && op.DefaultOption).Select(op => op.Id);
			var productDefaultImages = myDB.Images.Where(i => options.Contains(i.ProductOptionId) && i.ImageData != null);
			var imagesList = new List<ImagesClass>();
			foreach(var image in productDefaultImages) {
				var option = myDB.ProductOptions.First(o => o.Id == image.ProductOptionId);
				imagesList.Add(new ImagesClass {
					Id = image.Id,
					OptionId = image.ProductOptionId,
					ProductImage = image.ProductImage,
					GroupId = option.ProductOptionGroupId,
					ImageData = Convert.ToBase64String(image.ImageData)
				});
			}
			if (productDefaultImages != null) {
				try {
					return Json(new { ProductImage = productImage, Images = imagesList }, JsonRequestBehavior.AllowGet);
				} catch { return Json(new { ProductImage = productImage, Images = string.Empty }, JsonRequestBehavior.AllowGet); }
			}
			return Json(new { Images = string.Empty }, JsonRequestBehavior.AllowGet);
		}

	}
}