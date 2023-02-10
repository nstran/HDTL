using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Milestones
    {
        public Guid MilestonesId { get; set; }
        public string Name { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public Guid? TenantId { get; set; }
        public Guid ProjectId { get; set; }
    }
}
