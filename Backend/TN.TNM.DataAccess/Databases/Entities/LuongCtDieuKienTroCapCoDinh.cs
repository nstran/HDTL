using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LuongCtDieuKienTroCapCoDinh
    {
        public int LuongCtDieuKienTroCapCoDinhId { get; set; }
        public int LuongCtTroCapCoDinhId { get; set; }
        public int LuongCtLoaiTroCapCoDinhId { get; set; }
        public int KyLuongId { get; set; }
        public Guid DieuKienHuongId { get; set; }
        public bool DuDieuKien { get; set; }
        public Guid? TenantId { get; set; }
    }
}
