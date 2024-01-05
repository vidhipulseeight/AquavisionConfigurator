using AquavisionConfigurator.Models;
using System.Collections.Generic;
using System.Linq;

namespace AquavisionConfigurator.Helpers {
	public partial class ProductHelper {
		public static List<ProductOptionGroup> GetOptionGroup(int productId) {
			AquavisionEntities myDB = new AquavisionEntities();
			return myDB.ProductOptionGroups.Where(og => og.ProductId == productId).ToList();
		}
	}
}