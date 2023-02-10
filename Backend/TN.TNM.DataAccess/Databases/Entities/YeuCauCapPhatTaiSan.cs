using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class YeuCauCapPhatTaiSan
    {
        public int YeuCauCapPhatTaiSanId { get; set; }
        public string MaYeuCau { get; set; }
        public DateTime NgayDeXuat { get; set; }
        public Guid NguoiDeXuatId { get; set; }
        public int TrangThai { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? NguoiGuiXacNhanId { get; set; }
        public bool? Active { get; set; }
    }
}
