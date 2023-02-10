using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class RelateTaskMapping
    {
        public Guid RelateTaskMappingId { get; set; }
        public Guid TaskId { get; set; }
        public Guid RelateTaskId { get; set; }
        public Guid? ProjectId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
    }
}
