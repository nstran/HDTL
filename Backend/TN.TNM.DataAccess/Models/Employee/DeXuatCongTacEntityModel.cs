using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class DeXuatCongTacEntityModel
    {
        public int? DeXuatCongTacId { get; set; }
        public string TenDeXuat { get; set; }
        public string MaDeXuat { get; set; }
        public DateTime NgayDeXuat { get; set; }
        public string NguoiDeXuat { get; set; }
        public Guid NguoiDeXuatId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? PositionId { get; set; }
        public int? TrangThai { get; set; }
        public string TrangThaiString { get; set; }
        public string BackgroundColorForStatus { get; set; }
        public bool? Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public string DonVi { get; set; }
        public string DiaDiem { get; set; }
        public string PhuongTien { get; set; }
        public string NhiemVu { get; set; }
        public string LyDo { get; set; }
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
        public Guid? NguoiGuiXacNhanId { get; set; }
        public List<DeXuatCongTacChiTietEntityModel> ListDeXuatCongTacChiTiet { get; set; }
    }
}
