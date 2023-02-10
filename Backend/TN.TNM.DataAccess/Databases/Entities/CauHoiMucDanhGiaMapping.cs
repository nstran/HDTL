using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CauHoiMucDanhGiaMapping
    {
        public int CauHoiMucDanhGiaMappingId { get; set; }
        public int CauHoiDanhGiaId { get; set; }
        public int MucDanhGiaId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
