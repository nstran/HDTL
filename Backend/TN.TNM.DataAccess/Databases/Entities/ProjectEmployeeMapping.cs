using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ProjectEmployeeMapping
    {
        public Guid ProjectResourceMappingId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid ProjectId { get; set; }
        public int Type { get; set; }
        public Guid? TenantId { get; set; }
    }
}
