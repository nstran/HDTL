using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Project
{
    public class ProjectTaskEntityModel
    {
        public Guid TaskId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid ScopeId { get; set; }
        public Guid TaskCode { get; set; }
        public int CountNote { get; set; }
        public int TaskComplate { get; set; }
        public string TaskName { get; set; }
        public string ScopeName { get; set; }
        public string Employee { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal? Hour { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public decimal? ActualHour { get; set; }
        public decimal? CompleteRate { get; set; }
        public decimal? EstimateHour { get; set; }
        public string Description { get; set; }
        public Guid? Status { get; set; }
        public string StatusName { get; set; }
        public int? Priority { get; set; }
        public Guid? MilestonesId { get; set; }
        public bool? IncludeWeekend { get; set; }       
        public bool? IsSendMail { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
