using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LuongCtLoaiTroCapCoDinh
    {
        public int LuongCtLoaiTroCapCoDinhId { get; set; }
        public int LuongCtTroCapCoDinhId { get; set; }
        public int KyLuongId { get; set; }
        public Guid LoaiTroCapId { get; set; }
        public decimal MucTroCap { get; set; }
        public Guid? TenantId { get; set; }
    }
}
