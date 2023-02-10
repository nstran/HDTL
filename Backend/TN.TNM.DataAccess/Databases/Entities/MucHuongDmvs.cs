using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class MucHuongDmvs
    {
        public int MucHuongDmvsId { get; set; }
        public int TroCapId { get; set; }
        public int HinhThucTru { get; set; }
        public decimal MucTru { get; set; }
        public decimal SoLanTu { get; set; }
        public decimal? SoLanDen { get; set; }
        public Guid? TenantId { get; set; }
    }
}
