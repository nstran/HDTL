using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class MucHuongTheoNgayNghi
    {
        public int MucHuongTheoNgayNghiId { get; set; }
        public int TroCapId { get; set; }
        public decimal MucHuongPhanTram { get; set; }
        public int? LoaiNgayNghi { get; set; }
        public decimal SoNgayTu { get; set; }
        public decimal? SoNgayDen { get; set; }
        public Guid? TenantId { get; set; }
    }
}
