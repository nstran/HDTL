using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LuongCtDieuKienTroCapKhac
    {
        public int LuongCtDieuKienTroCapKhacId { get; set; }
        public int LuongCtTroCapKhacId { get; set; }
        public int LuongCtLoaiTroCapKhacId { get; set; }
        public int KyLuongId { get; set; }
        public Guid DieuKienHuongId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
