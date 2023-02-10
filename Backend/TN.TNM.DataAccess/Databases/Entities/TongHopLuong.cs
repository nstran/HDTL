using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class TongHopLuong
    {
        public int TongHopLuongId { get; set; }
        public int? KyLuongId { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? PositionId { get; set; }
        public decimal? LuongThucTe { get; set; }
        public decimal? BuTruThangTruoc { get; set; }
        public decimal? OtTinhThue { get; set; }
        public decimal? OtKhongThue { get; set; }
        public decimal? TongTroCapTinhThue { get; set; }
        public decimal? TongTroCapKhongThue { get; set; }
        public decimal? KhauTruDaThanhToan { get; set; }
        public decimal? GiamTru { get; set; }
        public decimal? ThuNhapChiTinhThue { get; set; }
        public decimal? ThueTncn { get; set; }
        public decimal? Bhxh { get; set; }
        public decimal? GiamTruSauThue { get; set; }
        public decimal? HoanLaiSauThue { get; set; }
        public decimal? ThucNhan { get; set; }
        public decimal? CtyPhaiTra { get; set; }
        public decimal? TongCtyTra { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
