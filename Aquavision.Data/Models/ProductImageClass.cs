using System;

namespace Aquavision.Data.Models {
	public partial class Image {
		public string GetProductOptionImage() {
			if (ImageData != null) {
				string base64Image = Convert.ToBase64String(ImageData);
				return $"data:image/jpeg;base64,{base64Image}";
			} else { return "../assets/img/noimage.png"; }
		}
	}
}
