using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class DeXuatTangLuong
    {
        public int DeXuatTangLuongId { get; set; }
        public string TenDeXuat { get; set; }
        public short LoaiDeXuat { get; set; }
        public DateTime NgayDeXuat { get; set; }
        public Guid NguoiDeXuatId { get; set; }
        public Guid? PhongBanId { get; set; }
        public Guid? ChucVuId { get; set; }
        public string GhiChu { get; set; }
        public short? TrangThai { get; set; }
        public bool? Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public DateTime? NgayApDung { get; set; }
        public int? KyDanhGiaId { get; set; }
    }
}
