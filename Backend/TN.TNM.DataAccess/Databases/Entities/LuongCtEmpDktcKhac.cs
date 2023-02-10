using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LuongCtEmpDktcKhac
    {
        public int LuongCtEmpDktcKhacId { get; set; }
        public Guid EmployeeId { get; set; }
        public int KyLuongId { get; set; }
        public Guid DieuKienHuongId { get; set; }
        public bool DuDieuKien { get; set; }
        public Guid? TenantId { get; set; }
    }
}
