using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class HoanLaiSauThue
    {
        public int HoanLaiId { get; set; }
        public int? KyLuongId { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? PositionId { get; set; }
        public decimal? ThueTncnhoan { get; set; }
        public decimal? KhoanCongLai { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
