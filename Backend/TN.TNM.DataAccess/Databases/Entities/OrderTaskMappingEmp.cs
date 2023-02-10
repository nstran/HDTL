using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class OrderTaskMappingEmp
    {
        public Guid Id { get; set; }
        public Guid CustomerOrderTaskId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
