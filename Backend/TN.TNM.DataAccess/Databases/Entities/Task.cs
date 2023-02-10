using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Task
    {
        public Guid TaskId { get; set; }
        public Guid ProjectId { get; set; }
        public string TaskCode { get; set; }
        public string TaskName { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public decimal? ActualHour { get; set; }
        public string Description { get; set; }
        public Guid? Status { get; set; }
        public int? Priority { get; set; }
        public Guid? MilestonesId { get; set; }
        public bool? IncludeWeekend { get; set; }
        public Guid CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? TenantId { get; set; }
        public DateTime? PlanStartTime { get; set; }
        public DateTime? PlanEndTime { get; set; }
        public decimal? EstimateHour { get; set; }
        public decimal? EstimateCost { get; set; }
        public decimal? TaskComplate { get; set; }
        public Guid? ProjectScopeId { get; set; }
        public bool? IsSendMail { get; set; }
        public Guid? TaskTypeId { get; set; }
        public string TimeType { get; set; }
        public Guid? ParentId { get; set; }
        public DateTime? CompleteDate { get; set; }
        public int? SoLanMoLai { get; set; }
    }
}
