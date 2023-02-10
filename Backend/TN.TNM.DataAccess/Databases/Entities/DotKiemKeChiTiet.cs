using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class DotKiemKeChiTiet
    {
        public int DotKiemKeChiTietId { get; set; }
        public int DotKiemKeId { get; set; }
        public int TaiSanId { get; set; }
        public Guid NguoiKiemKeId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
