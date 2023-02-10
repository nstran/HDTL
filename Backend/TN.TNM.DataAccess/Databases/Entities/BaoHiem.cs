using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class BaoHiem
    {
        public int BaoHiemId { get; set; }
        public int? KyLuongId { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? PositionId { get; set; }
        public decimal? Bhxh { get; set; }
        public decimal? Bhyt { get; set; }
        public decimal? Bhtn { get; set; }
        public decimal? Bhtnnn { get; set; }
        public decimal? KhoanBuTru { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
