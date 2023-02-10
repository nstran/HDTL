using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CauHoiPhieuDanhGiaMappingDanhMucItem
    {
        public int CauHoiPhieuDanhGiaMappingDanhMucItemId { get; set; }
        public Guid DanhMucId { get; set; }
        public int? CauHoiPhieuDanhGiaMappingId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
