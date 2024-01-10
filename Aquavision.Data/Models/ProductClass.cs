using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Aquavision.Data.Models {
	public partial class Product {
		public List<ProductOption> GetProductOption() {
			AquavisionEntities myDB = new AquavisionEntities();
			return myDB.ProductOptions.Where(p => p.ProductOptionGroupId == Id).ToList();
		}
		public string GetProductImage() {
			if (Image != null) {
				string base64Image = Convert.ToBase64String(Image);
				return $"data:image/jpeg;base64,{base64Image}";
			} else { return "../../assets/img/noimage.png"; }
		}
	}
}