using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class TotalProductionOrderMapping
    {
        public Guid TotalProductionOrderMappingId { get; set; }
        public Guid TotalProductionOrderId { get; set; }
        public Guid ProductionOrderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
    }
}
