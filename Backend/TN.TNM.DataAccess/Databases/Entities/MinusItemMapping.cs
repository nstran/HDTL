using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class MinusItemMapping
    {
        public Guid MinusItemMappingId { get; set; }
        public Guid? ProductionOrderId { get; set; }
        public Guid? ProductionOrderMappingId { get; set; }
        public int? MinusType { get; set; }
        public bool? IsWork { get; set; }
        public double? Quantity { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
    }
}
