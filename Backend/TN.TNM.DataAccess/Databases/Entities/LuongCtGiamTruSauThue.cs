using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LuongCtGiamTruSauThue
    {
        public int LuongCtGiamTruSauThueId { get; set; }
        public int KyLuongId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid PositionId { get; set; }
        public decimal KinhPhiCongDoan { get; set; }
        public decimal QuyetToanThueTncn { get; set; }
        public decimal Other { get; set; }
        public Guid? TenantId { get; set; }
    }
}
