using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ThongKeDiMuonVeSom
    {
        public int ThongKeDiMuonVeSomId { get; set; }
        public Guid EmployeeId { get; set; }
        public int LoaiDmvs { get; set; }
        public int ThoiGian { get; set; }
        public DateTime NgayDmvs { get; set; }
        public Guid? TenantId { get; set; }
    }
}
