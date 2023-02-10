using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ProjectResource
    {
        public Guid ProjectResourceId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid ObjectId { get; set; }
        public Guid ResourceType { get; set; }
        public Guid? ResourceRole { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public Guid CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? TenantId { get; set; }
        public bool? IsCreateVendor { get; set; }
        public Guid? EmployeeRole { get; set; }
        public int Allowcation { get; set; }
        public bool? IsOverload { get; set; }
        public decimal ChiPhiTheoGio { get; set; }
        public bool? IncludeWeekend { get; set; }
    }
}
