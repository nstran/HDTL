using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class KeHoachOt
    {
        public int KeHoachOtId { get; set; }
        public string TenKeHoach { get; set; }
        public DateTime? NgayDeXuat { get; set; }
        public Guid? NguoiDeXuatId { get; set; }
        public Guid? DonViId { get; set; }
        public Guid? ChucVuId { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public TimeSpan? GioBatDau { get; set; }
        public TimeSpan? GioKetThuc { get; set; }
        public string LyDo { get; set; }
        public DateTime? HanDangKy { get; set; }
        public string GhiChu { get; set; }
        public short? TrangThai { get; set; }
        public bool? Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public DateTime? HanPheDuyetKeHoach { get; set; }
        public DateTime? HanPheDuyetDangKy { get; set; }
        public string DiaDiem { get; set; }
        public Guid? LoaiOtId { get; set; }
        public int? LoaiCaId { get; set; }
    }
}
