

using System;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class DeNghiTamHoanUngEntityModel
    {
        public int? DeNghiTamHoanUngId { get; set; }
        public string MaDeNghi { get; set; }
        public string NguoiThuHuong { get; set; }
        public decimal? TongTienThanhToan { get; set; }
        public int? TrangThai { get; set; }
        public string TrangThaiString { get; set; }
        public string BackgroundColorForStatus { get; set; }
        public string HienTrangTaiSanString { get; set; }
        public Guid NguoiThuHuongId { get; set; }
        public int LoaiDeNghi { get; set; }
        public DateTime NgayDeNghi { get; set; }
        public int HoSoCongTacId { get; set; }
        public string LyDo { get; set; }
        public decimal? TienTamUng { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? NguoiDeXuatId { get; set; }
        public string NguoiDeXuat { get; set; }
        public Guid? NguoiPheDuyetId { get; set; }
        public Guid? NguoiGuiXacNhanId { get; set; }
        public string PhongBan { get; set; }
    }
}
