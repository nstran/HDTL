using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LuongCtThuNhapTinhThue
    {
        public int LuongCtThuNhapTinhThueId { get; set; }
        public int KyLuongId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid PositionId { get; set; }
        public decimal NetToGross { get; set; }
        public decimal Month13 { get; set; }
        public decimal Gift { get; set; }
        public decimal Other { get; set; }
        public Guid? TenantId { get; set; }
    }
}
