using System;

namespace TN.TNM.DataAccess.Models.Asset
{
    public class YeuCauCapPhatTaiSanChiTietEntityModel
    {
        public int YeuCauCapPhatTaiSanChiTietId { get; set; }
        public int TaiSanId { get; set; }
        public int YeuCauCapPhatTaiSanId { get; set; }
        public Guid LoaiTaiSanId { get; set; }
        public string LoaiTaiSan { get; set; }
        public string MoTa { get; set; }
        public Guid NhanVienYeuCauId { get; set; }
        public string MaNV { get; set; }
        public string TenNhanVien { get; set; }
        public string PhongBan { get; set; }
        public string ViTriLamViec { get; set; }
        public Guid MucDichSuDungId { get; set; }
        public string MucDichSuDung { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string LyDo { get; set; }
        public int? TrangThai { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public decimal? SoLuong { get; set; }
        public decimal? SoLuongPheDuyet { get; set; }
        public int? ParentPartId { get; set; }
        public int? TotalChild { get; set; }
        public string MaTaiSan { get; set; }
        public int? CapPhatTaiSanId { get; set; }
    }
}
