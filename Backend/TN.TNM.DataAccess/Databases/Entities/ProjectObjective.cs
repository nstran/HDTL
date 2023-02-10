using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ProjectObjective
    {
        public Guid ProjectObjectId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? ProjectObjectType { get; set; }
        public string ProjectObjectName { get; set; }
        public Guid? ProjectObjectUnit { get; set; }
        public decimal? ProjectObjectValue { get; set; }
        public string ProjectObjectDescription { get; set; }
        public int? OrderNumber { get; set; }
        public Guid? TenantId { get; set; }
    }
}
