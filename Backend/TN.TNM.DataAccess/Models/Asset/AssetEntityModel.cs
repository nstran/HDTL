using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Models.Asset
{
    public class AssetEntityModel
    {
        public int TaiSanId { get; set; }
        public string MaTaiSan { get; set; }
        public Guid? PhanLoaiTaiSanId { get; set; }
        public string PhanLoaiTaiSan { get; set; }
        public DateTime? NgayVaoSo { get; set; }
        public Guid? DonViTinhId { get; set; }
        public decimal? SoLuong { get; set; }
        public Guid? HangSXId { get; set; }
        public Guid? NuocSXId { get; set; }
        public string Model { get; set; }
        public string SoHieu { get; set; }
        public string SoSerial { get; set; }
        public int? ThoiHanBaoHanh { get; set; }
        public string ThongTinNoiMua { get; set; }
        public string ThongTinNoiBaoHanh { get; set; }
        public string BackgroundColorForStatus { get; set; }
        public int? NamSX { get; set; }
        public string TenTaiSan { get; set; }
        public string TenTaiSanCode { get; set; }
        public string MoTa { get; set; }
        public string MaCode { get; set; }
        public int TiLeKhauHao { get; set; }
        public int HienTrangTaiSan { get; set; }
        public string HienTrangTaiSanString { get; set; }
        public DateTime? NgayMua { get; set; }
        public DateTime? ThoiDiemBDTinhKhauHao { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public string MaNV { get; set; }
        public string PhongBan { get; set; }
        public string HoVaTen { get; set; }
        public string ViTriLamViec { get; set; }
        public Guid? NguoiSuDungId { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string NguoiCapPhat { get; set; }
        public int? BaoDuongDinhKy { get; set; }
        public decimal? GiaTriNguyenGia { get; set; }
        public decimal? GiaTriTinhKhauHao { get; set; }
        public int? PhuongPhapTinhKhauHao { get; set; }
        public decimal? ThoiGianKhauHao { get; set; }
        public string LoaiCapPhat { get; set; }
        public string LyDo { get; set; }
        public Guid? KhuVucTaiSanId { get; set; }
        public DateTime? NgayHetHanBaoHanh { get; set; }
        public List<DateTime> ListThoiDiemBaoDuong { get; set; }
        public string Account { get; set; }
        public string Dept { get; set; }


    }
}
