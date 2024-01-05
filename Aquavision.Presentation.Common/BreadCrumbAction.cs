namespace Aquavision.Presentation.Common {
	public class BreadCrumbAction {
		public string Action { set; get; }
		public string Controller { set; get; }
		public object RouteValues { set; get; }
		public string LinkText { set; get; }
		public string Icon { set; get; }
		public bool Divider { set; get; }
	}
}