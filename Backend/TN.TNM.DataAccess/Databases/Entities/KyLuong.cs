using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class KyLuong
    {
        public int KyLuongId { get; set; }
        public string TenKyLuong { get; set; }
        public decimal SoNgayLamViec { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public int TrangThai { get; set; }
        public string LyDoTuChoi { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
