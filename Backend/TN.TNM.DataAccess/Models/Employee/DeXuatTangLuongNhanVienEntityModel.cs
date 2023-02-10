using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class DeXuatTangLuongNhanVienEntityModel
    {
        public int? DeXuatTangLuongNhanVienId { get; set; }
        public int? DeXuatTangLuongId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid? PhongBanId { get; set; }
        public Guid? ChucVuId { get; set; }
        public decimal? LuongHienTai { get; set; }
        public decimal? LuongDeXuat { get; set; }
        public decimal? MucChechLech { get; set; }
        public string LyDoDeXuat { get; set; }
        public DateTime? NgayBatDauTang { get; set; }
        public short? TrangThai { get; set; }
        public string LyDo { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCodeName { get; set; }
        public string OrganizationName { get; set; }
        public string PositionName { get; set; }
        public bool? Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //Đề xuất sau kỳ đán hgias
        public int? PhieuTuDanhGiaId { get; set; }
        public string DiemDanhGia { get; set; }
        public decimal? TongDiemTuDanhGia { get; set; }
        public decimal? TongDiemDanhGia { get; set; }
        public decimal? MucLuongDeXuatQuanLy { get; set; }
        public string NguoiDanhGiaName { get; set; }
        public decimal? TongKetQua { get; set; }
        public decimal? MucLuongCu { get; set; }
        public decimal? MucLuongDeXuatTruongPhong { get; set; }
        public decimal? MucTangTruongPhong { get; set; }

    }
}
