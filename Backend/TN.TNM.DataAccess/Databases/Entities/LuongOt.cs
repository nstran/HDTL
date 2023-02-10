using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LuongOt
    {
        public int LuongOtId { get; set; }
        public int? KyLuongId { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? PositionId { get; set; }
        public Guid? LoaiOt { get; set; }
        public TimeSpan? GioOt { get; set; }
        public decimal? TienOt { get; set; }
        public decimal? TienOtTinhThue { get; set; }
        public bool? Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
