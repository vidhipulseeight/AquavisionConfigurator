//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AquavisionConfigurator.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProductOption
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductOption()
        {
            this.CustomerDesignSpecs = new HashSet<CustomerDesignSpec>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProductOptionGroupId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CustomerDesignSpec> CustomerDesignSpecs { get; set; }
        public virtual ProductOptionGroup ProductOptionGroup { get; set; }
    }
}
