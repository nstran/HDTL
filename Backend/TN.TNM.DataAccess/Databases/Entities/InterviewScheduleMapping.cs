using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class InterviewScheduleMapping
    {
        public Guid InterviewScheduleMappingId { get; set; }
        public Guid InterviewScheduleId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
