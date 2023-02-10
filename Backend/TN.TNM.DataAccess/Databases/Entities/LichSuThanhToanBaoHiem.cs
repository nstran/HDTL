using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LichSuThanhToanBaoHiem
    {
        public int LichSuThanhToanBaoHiemId { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime? NgayThanhToan { get; set; }
        public int? LoaiBaoHiem { get; set; }
        public decimal? SoTienThanhToan { get; set; }
        public string GhiChu { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
