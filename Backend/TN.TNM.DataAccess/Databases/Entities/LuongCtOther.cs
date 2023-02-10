using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LuongCtOther
    {
        public int LuongCtOtherId { get; set; }
        public int KyLuongId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid PositionId { get; set; }
        public decimal KhoanBuTruThangTruoc { get; set; }
        public decimal TroCapKhacKhongTinhThue { get; set; }
        public decimal KhauTruHoanLaiTruocThue { get; set; }
        public decimal LuongTamUng { get; set; }
        public Guid? TenantId { get; set; }
    }
}
