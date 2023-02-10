using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LuongCtLoaiTroCapKhac
    {
        public int LuongCtLoaiTroCapKhacId { get; set; }
        public int LuongCtTroCapKhacId { get; set; }
        public int KyLuongId { get; set; }
        public Guid LoaiTroCapId { get; set; }
        public decimal MucTroCap { get; set; }
        public bool IsEdit { get; set; }
        public Guid? TenantId { get; set; }
    }
}
