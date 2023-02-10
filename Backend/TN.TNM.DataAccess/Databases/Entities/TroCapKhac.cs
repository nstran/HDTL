using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class TroCapKhac
    {
        public int TroCapKhacId { get; set; }
        public int KyLuongId { get; set; }
        public int Stt { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid PositionId { get; set; }
        public Guid LoaiTroCapId { get; set; }
        public decimal SoTien { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid TenantId { get; set; }
    }
}
