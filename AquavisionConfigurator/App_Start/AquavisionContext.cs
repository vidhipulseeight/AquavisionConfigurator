using System.Threading;
using System.Web;

namespace AquavisionConfigurator.App_Start {
	public class AquavisionContext {
		private const string AQUAVISIONCONTEXT = "AquavisionContext";
		public static AquavisionContext Current {
			get {
				if (HttpContext.Current == null) {
					var data = Thread.GetData(Thread.GetNamedDataSlot(AQUAVISIONCONTEXT));
					if (data != null) {
						return (AquavisionContext)data;
					}
					var context = new AquavisionContext();
					Thread.SetData(Thread.GetNamedDataSlot(AQUAVISIONCONTEXT), context);
					return context;
				}
				if (HttpContext.Current.Items[AQUAVISIONCONTEXT] == null) {
					var context = new AquavisionContext();
					HttpContext.Current.Items.Add(AQUAVISIONCONTEXT, context);
					return context;
				}
				return (AquavisionContext)HttpContext.Current.Items[AQUAVISIONCONTEXT];
			}
		}
	}
}