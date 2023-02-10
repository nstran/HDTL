using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ManagerPacketService
    {
        public Guid Id { get; set; }
        public Guid PackId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
