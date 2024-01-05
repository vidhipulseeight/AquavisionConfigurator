using System.Collections.Generic;
using System.Linq;

namespace Aquavision.Data.Models {
	public partial class Product {
		public List<ProductOption> GetProductOption() {
			AquavisionEntities myDB = new AquavisionEntities();
			return myDB.ProductOptions.Where(p => p.ProductOptionGroupId == Id).ToList();
		}
	}
}