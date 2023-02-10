using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class TaiSan
    {
        public int TaiSanId { get; set; }
        public string MaTaiSan { get; set; }
        public DateTime? NgayVaoSo { get; set; }
        public Guid? DonViTinhId { get; set; }
        public decimal? SoLuong { get; set; }
        public Guid? HangSxid { get; set; }
        public Guid? NuocSxid { get; set; }
        public string Model { get; set; }
        public string SoHieu { get; set; }
        public string SoSerial { get; set; }
        public int? ThoiHanBaoHanh { get; set; }
        public string ThongTinNoiMua { get; set; }
        public string ThongTinNoiBaoHanh { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public int? NamSx { get; set; }
        public string TenTaiSan { get; set; }
        public string MoTa { get; set; }
        public string MaCode { get; set; }
        public DateTime? NgayMua { get; set; }
        public DateTime? ThoiDiemBdtinhKhauHao { get; set; }
        public int? TiLeKhauHao { get; set; }
        public Guid? PhanLoaiTaiSanId { get; set; }
        public int? HienTrangTaiSan { get; set; }
        public int? BaoDuongDinhKy { get; set; }
        public decimal? GiaTriNguyenGia { get; set; }
        public int? PhuongPhapTinhKhauHao { get; set; }
        public decimal? ThoiGianKhauHao { get; set; }
        public decimal? GiaTriTinhKhauHao { get; set; }
        public Guid? KhuVucTaiSanId { get; set; }
    }
}
