using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class HopDongNhanSu
    {
        public int HopDongNhanSuId { get; set; }
        public Guid LoaiHopDongId { get; set; }
        public string SoHopDong { get; set; }
        public string SoPhuLuc { get; set; }
        public DateTime NgayKyHopDong { get; set; }
        public DateTime NgayBatDauLamViec { get; set; }
        public DateTime? NgayKetThucHopDong { get; set; }
        public Guid PositionId { get; set; }
        public decimal MucLuong { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public Guid EmployeeId { get; set; }
    }
}
