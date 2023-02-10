using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ProjectTask
    {
        public Guid TaskId { get; set; }
        public Guid ScopeId { get; set; }
        public Guid TaskCode { get; set; }
        public string TaskName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal? Hour { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public decimal? ActualHour { get; set; }
        public string Description { get; set; }
        public Guid? TaskType { get; set; }
        public Guid? Status { get; set; }
        public int? Priority { get; set; }
        public Guid? MilestonesId { get; set; }
        public bool? IncludeWeekend { get; set; }
        public Guid CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
