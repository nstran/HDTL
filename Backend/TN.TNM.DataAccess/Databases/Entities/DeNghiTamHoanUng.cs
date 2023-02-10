using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class DeNghiTamHoanUng
    {
        public int DeNghiTamHoanUngId { get; set; }
        public string MaDeNghi { get; set; }
        public int HoSoCongTacId { get; set; }
        public DateTime NgayDeNghi { get; set; }
        public string LyDo { get; set; }
        public int? TrangThai { get; set; }
        public bool? Active { get; set; }
        public decimal? TongTienThanhToan { get; set; }
        public decimal? TienTamUng { get; set; }
        public int? LoaiDeNghi { get; set; }
        public Guid NguoiThuHuongId { get; set; }
        public Guid NguoiPheDuyetId { get; set; }
        public Guid? NguoiGuiXacNhanId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public Guid NguoiDeXuatId { get; set; }
    }
}
