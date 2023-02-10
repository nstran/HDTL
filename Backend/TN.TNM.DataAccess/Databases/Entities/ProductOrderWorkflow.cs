using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ProductOrderWorkflow
    {
        public Guid ProductOrderWorkflowId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool? IsDefault { get; set; }
        public string Description { get; set; }
        public bool? Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? ParentId { get; set; }
    }
}
