using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class DanhGiaNhanVien
    {
        public int DanhGiaNhanVienId { get; set; }
        public int NhanVienKyDanhGiaId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid? PositionId { get; set; }
        public decimal? TongDiemTuDanhGia { get; set; }
        public decimal? TongDiemDanhGia { get; set; }
        public decimal? TongKetQua { get; set; }
        public decimal? MucLuongCu { get; set; }
        public decimal? MucLuongDeXuatQuanLy { get; set; }
        public decimal? MucLuongDeXuatTruongPhong { get; set; }
        public string NhanXetTruongPhong { get; set; }
        public Guid? MucDanhGiaMasterDataId { get; set; }
        public int TrangThaiId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
