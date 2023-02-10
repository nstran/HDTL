using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Cost
    {
        public Guid CostId { get; set; }
        public string CostCode { get; set; }
        public string CostName { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? StatusId { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public decimal SoLuongToiThieu { get; set; }
        public decimal DonGia { get; set; }
        public DateTime NgayHieuLuc { get; set; }
        public DateTime? NgayHetHan { get; set; }
    }
}
