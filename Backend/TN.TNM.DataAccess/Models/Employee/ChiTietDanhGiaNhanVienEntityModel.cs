using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class ChiTietDanhGiaNhanVienEntityModel
    {
        public int ChiTietDanhGiaNhanVienId { get; set; }
        public int DanhGiaNhanVienId { get; set; }
        public string CauTraLoiText { get; set; }
        public bool? CauTraLoiLuaChon { get; set; }
        public decimal? DiemTuDanhGia { get; set; }
        public decimal? DiemDanhGia { get; set; }
        public decimal? KetQua { get; set; }

        //CauHoiPhieuDanhGiaMapping
        public int? CauHoiPhieuDanhGiaMappingId { get; set; }
        public int? PhieuDanhGiaId { get; set; }
        public int? CauHoiDanhGiaId { get; set; }
        public int? NguoiDanhGia { get; set; }
        public string NoiDungCauHoi { get; set; }
        public decimal? TiLe { get; set; }
        public int? LoaiCauTraLoiId { get; set; }
        public decimal? Stt { get; set; }
        public int? ParentId { get; set; }
        public List<CategoryEntityModel> DanhMucItem { get; set; }
        public List<CategoryEntityModel> ListDanhMucItemChose { get; set; }
    }
}
