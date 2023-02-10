using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class TechniqueRequest
    {
        public Guid TechniqueRequestId { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? OrganizationId { get; set; }
        public string TechniqueName { get; set; }
        public string Description { get; set; }
        public bool? Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
        public string TechniqueRequestCode { get; set; }
    }
}
