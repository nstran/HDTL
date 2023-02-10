using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class PhieuDanhGiaEntityModel
    {
        public int? PhieuDanhGiaId { get; set; }
        public string TenPhieuDanhGia { get; set; }
        public string MaPhieuDanhGia { get; set; }
        public int? TrangThaiPhieuDanhGia { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public int? ThangDiemDanhGiaId { get; set; }
        public int? CachTinhTong { get; set; }
        public bool? HoatDong { get; set; }
    }
}
