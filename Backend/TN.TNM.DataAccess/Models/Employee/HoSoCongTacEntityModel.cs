
using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class HoSoCongTacEntityModel
    {
        public int HoSoCongTacId { get; set; }
        public string MaHoSoCongTac { get; set; }
        public int DeXuatCongTacId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public int? TrangThai { get; set; }
        public bool? Active { get; set; }
        public string TrangThaiString { get; set; }
        public string BackgroundColorForStatus { get; set; }
        public string NoiCongTac { get; set; }
        public string ThoiGian { get; set; }
        public string TenDeXuat { get; set; }
        public string DonVi { get; set; }
        public string DiaDiem { get; set; }
        public string PhuongTien { get; set; }
        public string NhiemVu { get; set; }
        public string LyDo { get; set; }
        public string KetQua { get; set; }
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
        public List<EmployeeEntityModel> ListNhanVienCT { get; set; }
        public string MaCTTenDX { get; set; }
        public string TrangThaiText { get; set; }
    }
}
