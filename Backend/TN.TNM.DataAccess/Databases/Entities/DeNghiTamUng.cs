using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class DeNghiTamUng
    {
        public int DeNghiTamUngId { get; set; }
        public DateTime NgayDeNghi { get; set; }
        public int HoSoCongTacId { get; set; }
        public Guid NguoiDeNghi { get; set; }
        public Guid NguoiPheDuyetId { get; set; }
        public bool? IsTamUng { get; set; }
        public string LyDoThanhToan { get; set; }
        public string NoiDungThanhToan { get; set; }
        public decimal? SoTienCanThanhToan { get; set; }
        public decimal? SoTienTamUng { get; set; }
        public string GhiChu { get; set; }
        public int? TrangThaiTamUng { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
