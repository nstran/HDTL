using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LuongCtLoaiTroCapOt
    {
        public int LuongCtLoaiTroCapOtId { get; set; }
        public int LuongCtTroCapOtId { get; set; }
        public int KyLuongId { get; set; }
        public Guid LoaiOtId { get; set; }
        public decimal MucTroCap { get; set; }
        public decimal SoGioOt { get; set; }
        public Guid? TenantId { get; set; }
    }
}
