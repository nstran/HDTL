using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class PhieuDanhGia
    {
        public int PhieuDanhGiaId { get; set; }
        public string TenPhieuDanhGia { get; set; }
        public int? TrangThaiPhieuDanhGia { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public int? ThangDiemDanhGiaId { get; set; }
        public int? CachTinhTong { get; set; }
        public string MaPhieuDanhGia { get; set; }
    }
}
