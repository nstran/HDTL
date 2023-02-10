using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class TaskMilestonesMapping
    {
        public Guid ProjectMilestonesMappingId { get; set; }
        public Guid MilestonesId { get; set; }
        public Guid TaskId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
