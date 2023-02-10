using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class OrderTechniqueMapping
    {
        public Guid OrderTechniqueMappingId { get; set; }
        public Guid ProductOrderWorkflowId { get; set; }
        public Guid TechniqueRequestId { get; set; }
        public short? TechniqueOrder { get; set; }
        public short? Rate { get; set; }
        public bool? IsDefault { get; set; }
        public bool? Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
    }
}
