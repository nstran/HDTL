using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CaLamViec
    {
        public int CaLamViecId { get; set; }
        public TimeSpan GioVao { get; set; }
        public TimeSpan GioRa { get; set; }
        public bool? Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public int LoaiCaLamViecId { get; set; }
        public TimeSpan ThoiGianKetThucCa { get; set; }
    }
}
