using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Project
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectCode { get; set; }
        public Guid? ProjectManagerId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? ContractId { get; set; }
        public string Description { get; set; }
        public Guid? ProjectType { get; set; }
        public Guid? ProjectScope { get; set; }
        public Guid? ProjectStatus { get; set; }
        public Guid? ProjectSize { get; set; }
        public decimal? Butget { get; set; }
        public int? ButgetType { get; set; }
        public int? Priority { get; set; }
        public DateTime? ProjectStartDate { get; set; }
        public DateTime? ProjectEndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public bool? IncludeWeekend { get; set; }
        public Guid CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? TenantId { get; set; }
        public decimal? BudgetVnd { get; set; }
        public decimal? BudgetUsd { get; set; }
        public decimal? BudgetNgayCong { get; set; }
        public decimal GiaBanTheoGio { get; set; }
        public bool? ProjectStatusPlan { get; set; }
        public DateTime? LastChangeActivityDate { get; set; }
        public DateTime? NgayKyNghiemThu { get; set; }
    }
}
