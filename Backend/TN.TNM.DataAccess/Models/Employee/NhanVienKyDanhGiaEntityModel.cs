using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class NhanVienKyDanhGiaEntityModel
    {
        public int? NhanVienKyDanhGiaId { get; set; }
        public int? ParentId { get; set; }
        public bool? IsTruongBoPhan { get; set; }
        public int? Level { get; set; }
        public int? KyDanhGiaId { get; set; }
        public Guid? OrgParentId { get; set; }
        public Guid? RootOrgId { get; set; }
        public Guid? NguoiDanhGiaId { get; set; }
        public Guid? PositionId { get; set; }
        public string NguoiDanhGiaName { get; set; }
        public Guid? NguoiDuocDanhGiaId { get; set; }
        public string NguoiDuocDanhGiaName { get; set; }
        public int? BuocHienTai { get; set; }
        public Guid? OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public decimal? QuyLuong { get; set; }
        public bool? isEmp { get; set; }
        public bool? XemLuong { get; set; }
        public string ChucVu { get; set; }
        public int? TrangThai { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? MucDanhGiaMasterDataId { get; set; }
        public int? TrangThaiDanhGia { get; set; }
        public int? DanhGiaNhanVienId { get; set; }

        public int? ChiTietDanhGiaNhanVienId { get; set; }
        
        public decimal? TongDiemTuDanhGia { get; set; }
        public decimal? TongDiemDanhGia { get; set; }
        public decimal? TongKetQua { get; set; }
        public decimal? MucLuongCu { get; set; }
        public decimal? MucLuongDeXuatQuanLy { get; set; }
        public decimal? MucLuongDeXuatTruongPhong { get; set; }
        public decimal? MucTangTruongPhong { get; set; }
        public decimal? ThangDiemDanhGiaId { get; set; }
        public MucDanhGiaDanhGiaMappingEntityModel MucDanhGiaApDung { get; set; }
        public List<MucDanhGiaDanhGiaMappingEntityModel> ListMucDanhGia { get; set; }
        public bool? DuocXemLuong { get; set; }
        public bool? IsShowButtonDanhGia { get; set; }
        public int? TrangThaiPBValue { get; set; }
        public string TrangThaiPBName { get; set; }
        public bool? IsNguoiDanhGia { get; set; }
        public string TongKetQuaName { get; set; }
        public string TongDiemTuDanhGiaName { get; set; }
        public string TongDiemDanhGiaName { get; set; }


    }
}
