using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class TaskConstraint
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
        public Guid? TenantId { get; set; }
    }
}
