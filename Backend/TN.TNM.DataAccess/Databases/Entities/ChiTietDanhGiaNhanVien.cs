using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ChiTietDanhGiaNhanVien
    {
        public int ChiTietDanhGiaNhanVienId { get; set; }
        public int DanhGiaNhanVienId { get; set; }
        public int? NguoiDanhGia { get; set; }
        public int? CauHoiPhieuDanhGiaMappingId { get; set; }
        public int? LoaiCauTraLoiId { get; set; }
        public string CauTraLoiText { get; set; }
        public bool? CauTraLoiLuaChon { get; set; }
        public decimal? DiemTuDanhGia { get; set; }
        public decimal? DiemDanhGia { get; set; }
        public decimal? KetQua { get; set; }
        public Guid? TenantId { get; set; }
    }
}
