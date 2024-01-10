using Aquavision.Administration.Helpers;
using Aquavision.Data.Models;
using System;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace Aquavision.Administrator.Controllers {
	[Authorize]
	public class ProductController : BaseController {
		AquavisionEntities myDB = new AquavisionEntities();
		public ActionResult Index() {
			ViewBag.PageTitle = "Products";
			AddBreadCrumb(new BreadCrumb("Products"));
			var productList = myDB.Products.Where(c => !c.Deleted).ToList();
			return View(productList);
		}
		public ActionResult New() {
			ViewBag.PageTitle = "Product";
			AddBreadCrumb(new BreadCrumb("Products", Url.Action("Index", "Product")));
			AddBreadCrumb(new BreadCrumb("New"));
			var categoryList = myDB.Categories.Where(c => !c.Deleted).OrderBy(c => c.Name).ToList();
			ViewBag.CategoryId = new SelectList(categoryList, "Id", "Name");
			return View(new Product());
		}
		[HttpPost]
		public ActionResult New(Product product, HttpPostedFileBase file) {
			var checkProductSKU = myDB.Products.Where(p => p.SKU == product.SKU);
			if (checkProductSKU.Any()) {
				ModelState.AddModelError("SKU", "Product SKU is already exists.");
			}
			var ms = new MemoryStream();
			file.InputStream.CopyTo(ms);
			var pictureBytes = ms.ToArray();

			if (ModelState.IsValid) {
				product.Image = pictureBytes;
				product.ImageMimeType = file.ContentType;
				myDB.Products.Add(product);
				try {
					myDB.SaveChanges();
				} catch (DbEntityValidationException ex) {
					foreach (var dbValidationError in ex.EntityValidationErrors.SelectMany(error => error.ValidationErrors)) {
						Debug.WriteLine(dbValidationError.ErrorMessage);
						ModelState.AddModelError(dbValidationError.PropertyName, dbValidationError.ErrorMessage);
					}
				}
				return RedirectToAction("Index");
			}
			ViewBag.PageTitle = "Product";
			AddBreadCrumb(new BreadCrumb("Products", Url.Action("Index", "Product")));
			AddBreadCrumb(new BreadCrumb("New"));
			var categoryList = myDB.Categories.Where(c => !c.Deleted).OrderBy(c => c.Name).ToList();
			ViewBag.CategoryId = new SelectList(categoryList, "Id", "Name");
			return View(product);
		}

		public ActionResult Edit(int id) {
			var product = myDB.Products.FirstOrDefault(p => p.Id == id);
			if (product == null) {
				return RedirectToAction("Index");
			}
			ViewBag.PageTitle = "Product";
			var categoryList = myDB.Categories.Where(c => !c.Deleted).OrderBy(c => c.Name).ToList();
			ViewBag.CategoryId = new SelectList(categoryList, "Id", "Name", product.CategoryId);
			AddBreadCrumb(new BreadCrumb("Products", Url.Action("Index", "Product")));
			AddBreadCrumb(new BreadCrumb(product.Name));
			AddBreadCrumb(new BreadCrumb("Edit"));
			return View(product);
		}

		[HttpPost]
		public ActionResult Edit(Product product, HttpPostedFileBase file) {
			var originalProduct = myDB.Products.FirstOrDefault(c => c.Id == product.Id);
			if (originalProduct == null) {
				return RedirectToAction("Index");
			}
			var checkProductSKU = myDB.Products.Where(p => p.SKU == product.SKU && p.Id != product.Id);
			if (checkProductSKU.Any()) {
				ModelState.AddModelError("SKU", "Product SKU is already exists.");
			}
			if (ModelState.IsValid) {
				originalProduct.CategoryId = product.CategoryId;
				originalProduct.SKU = product.SKU;
				originalProduct.Name = product.Name;
				originalProduct.Description = product.Description;
				originalProduct.Price = product.Price;
				if (file != null) {
					var ms = new MemoryStream();
					file.InputStream.CopyTo(ms);
					var pictureBytes = ms.ToArray();
					originalProduct.Image = pictureBytes;
					originalProduct.ImageMimeType = file.ContentType;
				}
				try {
					myDB.SaveChanges();
				} catch (DbEntityValidationException ex) {
					foreach (var dbValidationError in ex.EntityValidationErrors.SelectMany(error => error.ValidationErrors)) {
						Debug.WriteLine(dbValidationError.ErrorMessage);
						ModelState.AddModelError(dbValidationError.PropertyName, dbValidationError.ErrorMessage);
					}
				}
				return RedirectToAction("Index");
			}
			ViewBag.PageTitle = "Product";
			var categoryList = myDB.Categories.Where(c => !c.Deleted).OrderBy(c => c.Name).ToList();
			ViewBag.CategoryId = new SelectList(categoryList, "Id", "Name", product.CategoryId);
			AddBreadCrumb(new BreadCrumb("Products", Url.Action("Index", "Product")));
			AddBreadCrumb(new BreadCrumb(product.Name));
			AddBreadCrumb(new BreadCrumb("Edit"));
			return View(product);
		}
		[HttpPost]
		public ActionResult ProductOptionGroup(FormCollection collection) {
			var productId = Convert.ToInt32(collection["Id"]);
			var groupId = Convert.ToInt32(collection["GroupId"]);
			var groupName = collection["ProductOptionGroupName"].Trim();
			var product = myDB.Products.FirstOrDefault(p => p.Id == productId);
			if (product == null) {
				return RedirectToAction("Index");
			}
			if (groupId > 0) {
				var productOptionGroup = myDB.ProductOptionGroups.FirstOrDefault(p => p.Id == groupId);
				if (productOptionGroup == null) {
					TempData["Error"] = "Invalid Product Group.";
					return RedirectToAction("Edit", new { id = productId });
				}
				var checkOptionGroup = myDB.ProductOptionGroups.Where(p => p.Name.Trim() == groupName && p.ProductId == productId && p.Id != groupId).FirstOrDefault();
				if (checkOptionGroup != null) {
					TempData["Error"] = "Option group already exits for this product.";
					return RedirectToAction("Edit", new { id = productId });
				}
				productOptionGroup.Name = groupName;
				myDB.SaveChanges();
				TempData["Success"] = "Option group updated.";
			} else {
				var checkOptionGroup = myDB.ProductOptionGroups.Where(p => p.Name == groupName && p.ProductId == productId).FirstOrDefault();
				if (checkOptionGroup != null) {
					TempData["Error"] = "Option group already exits for this product.";
					return RedirectToAction("Edit", new { id = productId });
				}
				var productOptionGroup = new ProductOptionGroup {
					Name = groupName,
					ProductId = productId
				};
				myDB.ProductOptionGroups.Add(productOptionGroup);
				myDB.SaveChanges();
				TempData["Success"] = "Option group added.";
			}
			return RedirectToAction("Edit", new { id = productId });
		}
		public ActionResult DeleteProductGroup(int id) {
			var productId = 0;
			var productGroup = myDB.ProductOptionGroups.FirstOrDefault(p => p.Id == id);
			if (productGroup != null) {
				productId = productGroup.ProductId;
				var productOption = myDB.ProductOptions.Where(i => i.ProductOptionGroupId == id).ToList();
				if (productOption.Any()) {
					var optionImages = myDB.Images.Where(i => i.ProductOptionId == id).ToList();
					if (optionImages.Any()) {
						myDB.Images.RemoveRange(optionImages);
					}
					myDB.ProductOptions.RemoveRange(productOption);
				}
				myDB.ProductOptionGroups.Remove(productGroup);
				myDB.SaveChanges();
			}
			return RedirectToAction("Edit", "Product", new { Id = productId });
		}
		public ActionResult ProductOptions(int productGroupId, int productId) {
			ViewBag.PageTitle = "Product Option";
			AddBreadCrumb(new BreadCrumb("Products", Url.Action("Index", "Product")));
			AddBreadCrumb(new BreadCrumb(myDB.Products.FirstOrDefault(p => p.Id == productId)?.Name, Url.Action("Edit", "Product", new { id = productId })));
			AddBreadCrumb(new BreadCrumb("Edit", Url.Action("Edit", "Product", new { id = productId })));
			AddBreadCrumb(new BreadCrumb("Options"));
			ViewBag.ProductGroupId = productGroupId;
			ViewBag.ProductId = productId;
			var optionGroups = myDB.ProductOptions.Where(p => p.ProductOptionGroupId == productGroupId).ToList();
			return View(optionGroups);
		}
		public ActionResult NewProductOptions(int productGroupId, int productId) {
			ViewBag.PageTitle = "New Product Option";
			AddBreadCrumb(new BreadCrumb("Products", Url.Action("Index", "Product")));
			AddBreadCrumb(new BreadCrumb(myDB.Products.FirstOrDefault(p => p.Id == productId)?.Name, Url.Action("Edit", "Product", new { id = productId })));
			AddBreadCrumb(new BreadCrumb("Edit", Url.Action("Edit", "Product", new { id = productId })));
			AddBreadCrumb(new BreadCrumb("Options", Url.Action("ProductOptions", "Product", new { productGroupId, productId })));
			AddBreadCrumb(new BreadCrumb("OptionNew"));
			ViewBag.ProductGroupId = productGroupId;
			ViewBag.ProductId = productId;
			return View(new ProductOption());
		}
		[HttpPost]
		public ActionResult NewProductOptions(ProductOption productOption,FormCollection formCollection) {
			var productGroupId = Convert.ToInt32(formCollection["ProductGroupId"]);
			var productId = Convert.ToInt32(formCollection["ProductId"]);
			var defaultOption = formCollection["chkDefaultOption"] == "on";

			if (ModelState.IsValid) {
				try {
					var checkOptionSKU = myDB.ProductOptions.Where(p => p.SKU == productOption.SKU).FirstOrDefault();
					if (checkOptionSKU != null) {
						TempData["Error"] = "SKU is already exits.";
						return RedirectToAction("NewProductOptions", "Product", new { productGroupId, productId });
					}
					if (defaultOption) {
						var checkDefaultOption = myDB.ProductOptions.Where(p => p.ProductOptionGroupId == productGroupId && p.DefaultOption).FirstOrDefault();
						if (checkDefaultOption != null) {
							TempData["Error"] = checkDefaultOption.SKU + " is already selected as default option.";
							return RedirectToAction("NewProductOptions", "Product", new { productGroupId, productId });
						}
					}			
					productOption.ProductOptionGroupId = productGroupId;
					productOption.DefaultOption = defaultOption;
					myDB.ProductOptions.Add(productOption);
					myDB.SaveChanges();

					var files = Request.Files;
					if (files != null && files.Count > 0) {
						for (int i = 0; i < files.Count; i++) {
							var file = files[i];
							if (file != null && file.ContentLength > 0) {
								var ms = new MemoryStream();
								file.InputStream.CopyTo(ms);
								var pictureBytes = ms.ToArray();

								//var fileNameFromForm = formCollection["files[" + i + "].FileName"];
								//var fileName = Path.GetFileName(file.FileName);
								//var path = Path.Combine(Server.MapPath("~/OptionImages"), fileName);
								//file.SaveAs(path);
								//var filepath = "../OptionImages/" + fileName;
								var optionImages = new Image {
									ProductOptionId = productOption.Id,
									ProductImage = "",
									ImageData= pictureBytes,
									ImageMimeType = file.ContentType
								};
								myDB.Images.Add(optionImages);
								myDB.SaveChanges();
							}
						}
					}
					return RedirectToAction("ProductOptions", "Product", new { productGroupId, productId });
				} catch (DbEntityValidationException ex) {
					foreach (var dbValidationError in ex.EntityValidationErrors.SelectMany(error => error.ValidationErrors)) {
						Debug.WriteLine(dbValidationError.ErrorMessage);
						ModelState.AddModelError(dbValidationError.PropertyName, dbValidationError.ErrorMessage);
					}
				}
			}
			return RedirectToAction("NewProductOptions", "Product", new { productGroupId, productId });
		}
		public ActionResult DeleteProductOption(int id,int productGroupId,int productId) {
			var productOption = myDB.ProductOptions.FirstOrDefault(p => p.Id == id);
			if (productOption != null) {
				var optionImages = myDB.Images.Where(i => i.ProductOptionId == id).ToList();
				if (optionImages.Any()) {
					myDB.Images.RemoveRange(optionImages);
				}
				myDB.ProductOptions.Remove(productOption);
				myDB.SaveChanges();
			}
			return RedirectToAction("ProductOptions", "Product", new { productGroupId, productId });
		}
		public ActionResult EditProductOption(int id) {
			var productOption = myDB.ProductOptions.FirstOrDefault(p => p.Id == id);
			if (productOption == null) {
				return RedirectToAction("Index");
			}
			ViewBag.PageTitle = "Edit Product Option";
			AddBreadCrumb(new BreadCrumb("Products", Url.Action("Index", "Product")));
			AddBreadCrumb(new BreadCrumb(myDB.Products.FirstOrDefault(p => p.Id == productOption.ProductOptionGroup.ProductId)?.Name, Url.Action("Edit", "Product", new { id = productOption.ProductOptionGroup.ProductId })));
			AddBreadCrumb(new BreadCrumb("Edit", Url.Action("Edit", "Product", new { id = productOption.ProductOptionGroup.ProductId })));
			AddBreadCrumb(new BreadCrumb("Options", Url.Action("ProductOptions", "Product", new { productGroupId = productOption.ProductOptionGroupId, productId = productOption.ProductOptionGroup.ProductId })));
			AddBreadCrumb(new BreadCrumb("OptionEdit"));
			return View(productOption);
		}
		[HttpPost]
		public ActionResult EditProductOption(ProductOption productOption, FormCollection formCollection) {
			var defaultOption = formCollection["chkDefaultOption"] == "on";
			var originalProductOption = myDB.ProductOptions.FirstOrDefault(c => c.Id == productOption.Id);
			if (originalProductOption == null) {
				return RedirectToAction("Index");
			}
			ViewBag.PageTitle = "Edit Product Option";
			AddBreadCrumb(new BreadCrumb("Products", Url.Action("Index", "Product")));
			AddBreadCrumb(new BreadCrumb(myDB.Products.FirstOrDefault(p => p.Id == originalProductOption.ProductOptionGroup.ProductId)?.Name, Url.Action("Edit", "Product", new { id = originalProductOption.ProductOptionGroup.ProductId })));
			AddBreadCrumb(new BreadCrumb("Edit", Url.Action("Edit", "Product", new { id = originalProductOption.ProductOptionGroup.ProductId })));
			AddBreadCrumb(new BreadCrumb("Options", Url.Action("ProductOptions", "Product", new { productGroupId = originalProductOption.ProductOptionGroupId, productId = originalProductOption.ProductOptionGroup.ProductId })));
			AddBreadCrumb(new BreadCrumb("OptionEdit"));
			if (ModelState.IsValid) {
				try {
					var checkOptionSKU = myDB.ProductOptions.Where(p => p.SKU == productOption.SKU && p.Id != originalProductOption.Id).FirstOrDefault();
					if (checkOptionSKU != null) {
						TempData["Error"] = "SKU is already exits.";
						return RedirectToAction("EditProductOption", "Product", new { originalProductOption.Id });
					}
					if (defaultOption) {
						var checkDefaultOption = myDB.ProductOptions.Where(p => p.ProductOptionGroupId == originalProductOption.ProductOptionGroupId && p.DefaultOption && p.Id != originalProductOption.Id).FirstOrDefault();
						if (checkDefaultOption != null) {
							TempData["Error"] = checkDefaultOption.SKU + " is already selected as default option.";
							return RedirectToAction("EditProductOption", "Product", new { originalProductOption.Id });
						}
					}
					originalProductOption.Name = productOption.Name;
					originalProductOption.SKU = productOption.SKU;
					originalProductOption.Price = productOption.Price;
					originalProductOption.DefaultOption = defaultOption;
					myDB.SaveChanges();

					var files = Request.Files;
					if (files != null && files.Count > 0) {
						for (int i = 0; i < files.Count; i++) {
							var file = files[i];
							if (file != null && file.ContentLength > 0) {
								var ms = new MemoryStream();
								file.InputStream.CopyTo(ms);
								var pictureBytes = ms.ToArray();
								foreach (var optionImage in originalProductOption.Images) {
									optionImage.ImageData = pictureBytes;
									optionImage.ImageMimeType = file.ContentType;
								}
								myDB.SaveChanges();
							}
						}
					}
				} catch (DbEntityValidationException ex) {
					foreach (var dbValidationError in ex.EntityValidationErrors.SelectMany(error => error.ValidationErrors)) {
						Debug.WriteLine(dbValidationError.ErrorMessage);
						ModelState.AddModelError(dbValidationError.PropertyName, dbValidationError.ErrorMessage);
					}
				}
			}
			return RedirectToAction("ProductOptions", "Product", new { productGroupId = originalProductOption.ProductOptionGroupId, productId = originalProductOption.ProductOptionGroup.ProductId });
		}

		[HttpPost]
		public ActionResult AddOptionImages(int Id, HttpPostedFileBase file) {
			var productOption = myDB.ProductOptions.FirstOrDefault(c => c.Id == Id);
			if (productOption == null) {
				return RedirectToAction("Index");
			}
			var ms = new MemoryStream();
			file.InputStream.CopyTo(ms);
			var pictureBytes = ms.ToArray();
			var fileName = Path.GetFileName(file.FileName);
			var path = Path.Combine(Server.MapPath("~/OptionImages"), fileName);
			file.SaveAs(path);
			var filepath = "../OptionImages/" + fileName;

			var optionImage = new Image {
				ProductOptionId = Id,
				ProductImage = filepath,
				ImageData = pictureBytes,
				ImageMimeType = file.ContentType
			};
			myDB.Images.Add(optionImage);
			myDB.SaveChanges();
			return RedirectToAction("EditProductOption", "Product", new { Id });
		}
		public ActionResult DeleteOptionImage(int id) {
			var productOptionId = 0;
			var optionImage = myDB.Images.FirstOrDefault(p => p.Id == id);
			if (optionImage != null) {
				productOptionId = optionImage.ProductOptionId;
				myDB.Images.Remove(optionImage);
				myDB.SaveChanges();
			}
			return RedirectToAction("EditProductOption", "Product", new { Id = productOptionId });
		}
	}
}