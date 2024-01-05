using System.Collections.Generic;
using System.Linq;

namespace Aquavision.Data.Models {
	public partial class ProductOptionGroup {
		public List<ProductOption> GetProductOption() {
			AquavisionEntities myDB = new AquavisionEntities();
			return myDB.ProductOptions.Where(p => p.ProductOptionGroupId == Id).ToList();
		}
	}
}