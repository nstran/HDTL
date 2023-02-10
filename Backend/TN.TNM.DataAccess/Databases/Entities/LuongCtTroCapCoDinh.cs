using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LuongCtTroCapCoDinh
    {
        public int LuongCtTroCapCoDinhId { get; set; }
        public int KyLuongId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid PositionId { get; set; }
        public Guid? LoaiHopDongId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
