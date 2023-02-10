using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ProductCategoryMapping
    {
        public Guid ProductCategoryMappingId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ProductCategoryId { get; set; }
        public bool? Active { get; set; }
        public Guid? TenantId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public Product Product { get; set; }
        public ProductCategory ProductCategory { get; set; }
    }
}
