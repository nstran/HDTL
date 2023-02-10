using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class DeXuatCongTac
    {
        public int DeXuatCongTacId { get; set; }
        public string TenDeXuat { get; set; }
        public DateTime NgayDeXuat { get; set; }
        public Guid NguoiDeXuatId { get; set; }
        public Guid NguoiPheDuyetId { get; set; }
        public int? TrangThaiDeXuat { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? PositionId { get; set; }
        public Guid? OrganizationId { get; set; }
        public string DonVi { get; set; }
        public string DiaDiem { get; set; }
        public string PhuongTien { get; set; }
        public string NhiemVu { get; set; }
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
        public string MaDeXuat { get; set; }
        public bool? Active { get; set; }
        public int? TrangThai { get; set; }
        public Guid? NguoiGuiXacNhanId { get; set; }
    }
}
