using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class NhanVienDeXuatThayDoiChucVu
    {
        public int NhanVienDeXuatThayDoiChucVuId { get; set; }
        public int DeXuatThayDoiChucVuId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid ChucVuDeXuatId { get; set; }
        public Guid? ChucVuHienTaiId { get; set; }
        public string LyDoDeXuat { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public int TrangThai { get; set; }
        public bool? Active { get; set; }
        public string LyDo { get; set; }
        public string NghiaVu { get; set; }
    }
}
