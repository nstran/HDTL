using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class PermissionConfiguration
    {
        public Guid Id { get; set; }
        public int? StepId { get; set; }
        public Guid? ServicePacketId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid TenantId { get; set; }
        public Guid? RoleId { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
