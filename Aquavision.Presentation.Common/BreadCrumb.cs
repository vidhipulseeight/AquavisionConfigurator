using System.Collections.Generic;

namespace Aquavision.Presentation.Common {
	public class BreadCrumb {
		public string Name { get; }
		public string URL { get; }
		public List<MenuItem> MenuList { set; get; }

		public BreadCrumb(string name, string url = "") {
			Name = name;
			URL = url;
		}
	}
}