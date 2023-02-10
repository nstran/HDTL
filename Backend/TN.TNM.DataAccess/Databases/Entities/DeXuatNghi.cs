using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class DeXuatNghi
    {
        public int DeXuatNghiId { get; set; }
        public Guid EmployeeId { get; set; }
        public short LoaiDeXuat { get; set; }
        public string MaDeXuat { get; set; }
        public DateTime? TuNgay { get; set; }
        public TimeSpan? TuCaSang { get; set; }
        public TimeSpan? TuCaChieu { get; set; }
        public DateTime? DenNgay { get; set; }
        public TimeSpan? DenCaSang { get; set; }
        public TimeSpan? DenCaChieu { get; set; }
        public decimal? TongNgayNghi { get; set; }
        public decimal? SoPhepConLai { get; set; }
        public string LyDo { get; set; }
        public Guid? NguoiThongBao { get; set; }
        public Guid? NguoiPheDuyet { get; set; }
        public int? TrangThai { get; set; }
        public short? LanGuiLai { get; set; }
        public bool? Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
