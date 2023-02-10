using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class TotalProductionOrder
    {
        public Guid TotalProductionOrderId { get; set; }
        public string Code { get; set; }
        public Guid? PeriodId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
    }
}
