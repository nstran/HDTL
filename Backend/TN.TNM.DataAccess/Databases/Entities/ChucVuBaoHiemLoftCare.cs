using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ChucVuBaoHiemLoftCare
    {
        public int ChucVuBaoHiemLoftCareId { get; set; }
        public Guid PositionId { get; set; }
        public decimal? SoNamKinhNghiem { get; set; }
        public Guid? TenantId { get; set; }
        public int? NhomBaoHiemLoftCareId { get; set; }
    }
}
