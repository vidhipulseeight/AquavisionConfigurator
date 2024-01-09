using Aquavision.Data.Models;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Aquavision.Administration.Helpers;
namespace Aquavision.Administrator.Controllers {
	[Authorize]
	public class CategoryController :BaseController {
		AquavisionEntities myDB = new AquavisionEntities();
		public ActionResult Index() {
			ViewBag.PageTitle = "Categories";
			AddBreadCrumb(new BreadCrumb("Categories"));
			var categoryList = myDB.Categories.Where(c=>!c.Deleted).ToList();
			return View(categoryList);
        }
		public ActionResult New() {
			ViewBag.PageTitle = "Categories";
			AddBreadCrumb(new BreadCrumb("Categories", Url.Action("Index", "Category")));
			AddBreadCrumb(new BreadCrumb("New"));
			return View(new Category());
		}
		[HttpPost]
		public ActionResult New(Category category) {
			var checkCategoryExits = myDB.Categories.Where(c => c.Name == category.Name && !c.Deleted).ToList();
			if (checkCategoryExits.Any()) {
				ModelState.AddModelError("Name", "Category is already exists.");
			}
			if (ModelState.IsValid) {
				myDB.Categories.Add(category);
				try {
					myDB.SaveChanges();
				} catch (DbEntityValidationException ex) {
					foreach (var dbValidationError in ex.EntityValidationErrors.SelectMany(error => error.ValidationErrors)) {
						Debug.WriteLine(dbValidationError.ErrorMessage);
						ModelState.AddModelError(dbValidationError.PropertyName, dbValidationError.ErrorMessage);
					}
				}
				if (ModelState.IsValid) {
					return RedirectToAction("Index");
				}
			}
			ViewBag.PageTitle = "Categories";
			AddBreadCrumb(new BreadCrumb("Categories", Url.Action("Index", "Category")));
			AddBreadCrumb(new BreadCrumb("New"));
			return View(category);
		}
		public ActionResult Edit(int id) {
			var category = myDB.Categories.FirstOrDefault(c => c.Id == id);
			if (category == null) {
				return RedirectToAction("Index");
			}
			ViewBag.PageTitle = "Categories";			
			AddBreadCrumb(new BreadCrumb("Categories", Url.Action("Index", "Category")));
			AddBreadCrumb(new BreadCrumb(category.Name));
			AddBreadCrumb(new BreadCrumb("Edit"));
			return View(category);
		}
		[HttpPost]
		public ActionResult Edit(Category category) {
			var originalCategory = myDB.Categories.FirstOrDefault(c => c.Id == category.Id);
			if (originalCategory == null) {
				return RedirectToAction("Index");
			}
			ViewBag.PageTitle = "Categories";
			AddBreadCrumb(new BreadCrumb("Categories", Url.Action("Index", "Category")));
			AddBreadCrumb(new BreadCrumb(originalCategory.Name));
			AddBreadCrumb(new BreadCrumb("Edit"));

			var checkCategoryExits = myDB.Categories.Where(c => c.Name == category.Name && !c.Deleted && c.Id != category.Id).ToList();
			if (checkCategoryExits.Any()) {
				ModelState.AddModelError("Name", "Category is already exists.");
			}

			if (ModelState.IsValid) {
				originalCategory.Name = category.Name;
				try {
					myDB.SaveChanges();
				} catch (DbEntityValidationException ex) {
					foreach (var dbValidationError in ex.EntityValidationErrors.SelectMany(error => error.ValidationErrors)) {
						Debug.WriteLine(dbValidationError.ErrorMessage);
						ModelState.AddModelError(dbValidationError.PropertyName, dbValidationError.ErrorMessage);
					}
				}
				if (ModelState.IsValid) {
					return RedirectToAction("Index");
				}
			}
			return View(category);
		}
		public ActionResult Delete(int id) {
			var category = myDB.Categories.FirstOrDefault(c => c.Id == id);
			if (category != null) {
				category.Deleted = true;
				myDB.SaveChanges();
			}
			return RedirectToAction("Index");
		}
	}
}