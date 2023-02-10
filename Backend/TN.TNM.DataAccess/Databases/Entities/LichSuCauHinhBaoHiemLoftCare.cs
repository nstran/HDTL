using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LichSuCauHinhBaoHiemLoftCare
    {
        public int LichSuCauHinhBaoHiemLoftCareId { get; set; }
        public int CauHinhBaoHiemLoftCareId { get; set; }
        public string TenQuyenLoiCu { get; set; }
        public Guid PositionCuId { get; set; }
        public decimal SoNamLamViecCu { get; set; }
        public int MucDongTypeCu { get; set; }
        public decimal? MucDongCu { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
