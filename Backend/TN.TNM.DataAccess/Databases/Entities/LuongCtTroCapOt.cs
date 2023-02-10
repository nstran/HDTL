using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LuongCtTroCapOt
    {
        public int LuongCtTroCapOtId { get; set; }
        public int KyLuongId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid PositionId { get; set; }
        public decimal MucLuongHienTai { get; set; }
        public Guid? TenantId { get; set; }
    }
}
