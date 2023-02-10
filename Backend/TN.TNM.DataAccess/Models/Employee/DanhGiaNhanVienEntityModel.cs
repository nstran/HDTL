using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class DanhGiaNhanVienEntityModel
    {
        public int? DanhGiaNhanVienId { get; set; }
        public int? NhanVienKyDanhGiaId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? PositionId { get; set; }
        public decimal? TongDiemTuDanhGia { get; set; }
        public decimal? TongDiemDanhGia { get; set; }
        public decimal? TongKetQua { get; set; }
        public decimal? MucLuongCu { get; set; }
        public decimal? MucLuongDeXuatQuanLy { get; set; }
        public decimal? MucLuongDeXuatTruongPhong { get; set; }
        public string NhanXetTruongPhong { get; set; }
        public Guid? MucDanhGiaMasterDataId { get; set; }
        public int? TrangThaiId { get; set; }
        public Guid? TenantId { get; set; }
        public string PositionName { get; set; }
        public string OrganizationName { get; set; }
        public string MucDanhGiaName { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }

        public string NguoiDanhGiaName { get; set; }
        public string NguoiDanhGiaCode { get; set; }
        public string NguoiDanhGiaPositionName { get; set; }
        public string NguoiDanhGiaOrganizationName { get; set; }

    }
}
