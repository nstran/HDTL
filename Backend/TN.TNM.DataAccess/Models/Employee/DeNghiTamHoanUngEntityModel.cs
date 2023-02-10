

using System;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class DeNghiTamHoanUngChiTietEntityModel
    {
        public int DeNghiTamHoanUngChiTietId { get; set; }
        public int DeNghiTamHoanUngId { get; set; }
        public string SoHoaDon { get; set; }
        public string NoiDung { get; set; }
        public Guid? HinhThucThanhToan { get; set; }
        public DateTime? NgayThang { get; set; }
        public decimal VanChuyenXM { get; set; }
        public decimal TienDonHNNB { get; set; }
        public decimal TienDonDN { get; set; }
        public decimal KhachSan { get; set; }
        public decimal ChiPhiKhac { get; set; }
        public decimal TongTienTruocVAT { get; set; }
        public decimal VAT { get; set; }
        public decimal TienSauVAT { get; set; }
        public string GhiChu { get; set; }
        public int ParentId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public string DinhKemCT { get; set; }
        public bool? IsNewLine { get; set; }
    }
}
