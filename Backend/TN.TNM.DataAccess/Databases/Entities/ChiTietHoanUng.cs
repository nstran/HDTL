using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ChiTietHoanUng
    {
        public int ChiTietHoanUngId { get; set; }
        public int DeNghiHoanUngId { get; set; }
        public DateTime? Ngay { get; set; }
        public string SoHoaDon { get; set; }
        public string TomTatMucDichChi { get; set; }
        public Guid? OrganizationId { get; set; }
        public string HinhThucThanhToan { get; set; }
        public string VanChuyenXeMay { get; set; }
        public string TienDonHnNd { get; set; }
        public string TienDonDn { get; set; }
        public string KhachSan { get; set; }
        public string ChiPhiKhac { get; set; }
        public string GhiChu { get; set; }
        public decimal? SoTienTamUng { get; set; }
        public decimal? Vat { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
