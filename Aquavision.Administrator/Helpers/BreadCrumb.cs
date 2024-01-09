using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Aquavision.Administration.Helpers {
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