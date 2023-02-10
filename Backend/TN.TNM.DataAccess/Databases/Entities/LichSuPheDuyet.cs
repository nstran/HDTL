using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LichSuPheDuyet
    {
        public Guid Id { get; set; }
        public Guid ObjectId { get; set; }
        public int DoiTuongApDung { get; set; }
        public DateTime NgayTao { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid? OrganizationId { get; set; }
        public string LyDo { get; set; }
        public int TrangThai { get; set; }
        public Guid? TenantId { get; set; }
        public int? ObjectNumber { get; set; }
    }
}
