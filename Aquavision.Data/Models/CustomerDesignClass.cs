using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Aquavision.Data.Models {
	public partial class CustomerDesign {
		public List<CustomerDesignSpec> GetDesignSpecs() {
			AquavisionEntities myDB = new AquavisionEntities();
			return myDB.CustomerDesignSpecs.Where(cds => cds.CustomerDesignId == Id).ToList();
		}
	}
}