using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LuongCtHoanLaiSauThue
    {
        public int LuongCtHoanLaiSauThueId { get; set; }
        public int KyLuongId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid PositionId { get; set; }
        public decimal ThueTncn { get; set; }
        public decimal Other { get; set; }
        public Guid? TenantId { get; set; }
    }
}
