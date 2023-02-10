using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Task
{
    public class TaskConstraintEntityModel
    {
        public Guid TaskConstraintId { get; set; }
        public Guid TaskId { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? ConstraintType { get; set; }
        public bool? ConstraingRequired { get; set; }
        public decimal? DelayTime { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }

        public string TaskCode { get; set; }
        public string TaskName { get; set; }
        public Guid? Status { get; set; }
        public string StatusName { get; set; }
        public DateTime? PlanStartTime { get; set; }
        public DateTime? PlanEndTime { get; set; }
        public string BackgroundColorForStatus { get; set; }
    }
}
