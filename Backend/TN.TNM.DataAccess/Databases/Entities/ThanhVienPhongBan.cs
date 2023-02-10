using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ThanhVienPhongBan
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public int IsManager { get; set; }
        public Guid? TenantId { get; set; }
    }
}
