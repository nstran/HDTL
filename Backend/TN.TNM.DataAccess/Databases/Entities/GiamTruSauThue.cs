using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class GiamTruSauThue
    {
        public int GiamTruSauThueId { get; set; }
        public int? KyLuongId { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? PositionId { get; set; }
        public decimal? PhiCongDoan { get; set; }
        public decimal? QuyetToanThueTncn { get; set; }
        public decimal? KhoanKhac { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
