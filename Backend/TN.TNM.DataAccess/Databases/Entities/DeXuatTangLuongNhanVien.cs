using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class DeXuatTangLuongNhanVien
    {
        public int DeXuatTangLuongNhanVienId { get; set; }
        public int DeXuatTangLuongId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid? PhongBanId { get; set; }
        public Guid? ChucVuId { get; set; }
        public decimal LuongHienTai { get; set; }
        public decimal LuongDeXuat { get; set; }
        public string LyDoDeXuat { get; set; }
        public short? TrangThai { get; set; }
        public string LyDo { get; set; }
        public bool? Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
