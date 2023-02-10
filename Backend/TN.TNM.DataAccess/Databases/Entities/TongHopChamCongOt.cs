using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class TongHopChamCongOt
    {
        public int TongHopChamCongOtId { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime NgayChamCong { get; set; }
        public Guid LoaiOtId { get; set; }
        public decimal SoPhut { get; set; }
        public decimal SoGio { get; set; }
        public Guid? TenantId { get; set; }
    }
}
