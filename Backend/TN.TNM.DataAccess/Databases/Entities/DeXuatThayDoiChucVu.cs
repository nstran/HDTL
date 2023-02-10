using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class DeXuatThayDoiChucVu
    {
        public int DeXuatThayDoiChucVuId { get; set; }
        public string TenDeXuat { get; set; }
        public DateTime NgayDeXuat { get; set; }
        public Guid? NguoiDeXuatId { get; set; }
        public Guid? TaiLieuLienQuanId { get; set; }
        public int TrangThai { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public DateTime? NgayApDung { get; set; }
        public bool? Active { get; set; }
    }
}
