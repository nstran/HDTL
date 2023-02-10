using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ProductMappingOptions
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid OptionId { get; set; }
        public Guid? VendorId { get; set; }
        public decimal? Price { get; set; }
        public decimal? Amount { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid TenantId { get; set; }
    }
}
