using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class DeNghiTamHoanUngChiTiet
    {
        public int DeNghiTamHoanUngChiTietId { get; set; }
        public int DeNghiTamHoanUngId { get; set; }
        public DateTime? NgayThang { get; set; }
        public string SoHoaDon { get; set; }
        public string NoiDung { get; set; }
        public decimal? VanChuyenXm { get; set; }
        public decimal? TienDonHnnb { get; set; }
        public decimal? TienDonDn { get; set; }
        public decimal? KhachSan { get; set; }
        public decimal? ChiPhiKhac { get; set; }
        public decimal? TongTienTruocVat { get; set; }
        public decimal? Vat { get; set; }
        public decimal? TienSauVat { get; set; }
        public Guid? HinhThucThanhToan { get; set; }
        public string GhiChu { get; set; }
        public int? ParentId { get; set; }
        public string DinhKemCt { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public int Level { get; set; }
    }
}
