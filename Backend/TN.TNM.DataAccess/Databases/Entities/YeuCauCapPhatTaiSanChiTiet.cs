using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class YeuCauCapPhatTaiSanChiTiet
    {
        public int YeuCauCapPhatTaiSanChiTietId { get; set; }
        public int TaiSanId { get; set; }
        public int YeuCauCapPhatTaiSanId { get; set; }
        public Guid LoaiTaiSanId { get; set; }
        public string MoTa { get; set; }
        public Guid NhanVienYeuCauId { get; set; }
        public Guid MucDichSuDungId { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string LyDo { get; set; }
        public int? TrangThai { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public decimal? SoLuong { get; set; }
        public decimal? SoLuongPheDuyet { get; set; }
        public bool? Active { get; set; }
    }
}
