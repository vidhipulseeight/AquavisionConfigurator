//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Aquavision.Data.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public System.Guid CustomerGUID { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string SaltKey { get; set; }
        public bool Deleted { get; set; }
    }
}
