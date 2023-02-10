using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class TaskResourceMapping
    {
        public Guid TaskResourceMappingId { get; set; }
        public Guid TaskId { get; set; }
        public Guid ResourceId { get; set; }
        public decimal? Hours { get; set; }
        public Guid? TenantId { get; set; }
        public bool? IsPersonInCharge { get; set; }
        public bool? IsChecker { get; set; }
    }
}
