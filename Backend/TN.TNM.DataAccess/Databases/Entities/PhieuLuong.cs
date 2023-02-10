using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class PhieuLuong
    {
        public int PhieuLuongId { get; set; }
        public string PhieuLuongCode { get; set; }
        public int KyLuongId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid PositionId { get; set; }
        public decimal SoNgayLamViec { get; set; }
        public string NgayBatDauKyLuong { get; set; }
        public string ThangBatDauKyLuong { get; set; }
        public string NamBatDauKyLuong { get; set; }
        public string NgayKetThucKyLuong { get; set; }
        public string ThangKetThucKyLuong { get; set; }
        public string NamKetThucKyLuong { get; set; }
        public string ThangTruoc { get; set; }
        public string ThangTruocTiengAnh { get; set; }
        public string NamTheoThangTruoc { get; set; }
        public decimal CauHinhGiamTruCaNhan { get; set; }
        public decimal CauHinhGiamTruNguoiPhuThuoc { get; set; }
        public decimal PhanTramBaoHiemCty { get; set; }
        public decimal PhanTramBaoHiemNld { get; set; }
        public decimal PhanTramKinhPhiCongDoanCty { get; set; }
        public decimal LuongCoBan { get; set; }
        public decimal LuongCoBanSau { get; set; }
        public decimal MucDieuChinh { get; set; }
        public decimal NgayLamViecThucTe { get; set; }
        public decimal NgayNghiPhep { get; set; }
        public decimal NgayNghiLe { get; set; }
        public decimal NgayNghiKhongLuong { get; set; }
        public decimal NgayDmvs { get; set; }
        public decimal NgayKhongHuongChuyenCan { get; set; }
        public string DuocHuongTroCapKpi { get; set; }
        public decimal SoLuongDkGiamTruGiaCanh { get; set; }
        public decimal GiamTruGiaCanh { get; set; }
        public decimal LuongTheoNgayHocViec { get; set; }
        public decimal TroCapDiLai { get; set; }
        public decimal TroCapDienThoai { get; set; }
        public decimal TroCapAnTrua { get; set; }
        public decimal TroCapNhaO { get; set; }
        public decimal TroCapChuyenCan { get; set; }
        public decimal ThuongKpi { get; set; }
        public decimal ThuongCuoiNam { get; set; }
        public decimal TroCapTrachNhiem { get; set; }
        public decimal TroCapHocViec { get; set; }
        public decimal OtTinhThue { get; set; }
        public decimal OtKhongTinhThue { get; set; }
        public decimal LuongThang13 { get; set; }
        public decimal QuaBocTham { get; set; }
        public decimal TongThueTncn { get; set; }
        public decimal BaoHiem { get; set; }
        public decimal ThucNhan { get; set; }
        public decimal CtyTraBh { get; set; }
        public decimal KinhPhiCongDoan { get; set; }
        public decimal TongChiPhiNhanVien { get; set; }
        public decimal CauHinhTroCapCc { get; set; }
        public decimal CauHinhTroCapDmvs { get; set; }
        public string DuocHuongTroCapChuyenCan { get; set; }
        public decimal NghiKhongPhep { get; set; }
        public decimal TongSoNgayKhongTinhLuong { get; set; }
        public decimal TroCapCcncTheoNgayLvtt { get; set; }
        public decimal SoLanTruChuyenCan { get; set; }
        public decimal TroCapChuyenCanDmvs { get; set; }
        public Guid? CreatedByEmpId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
