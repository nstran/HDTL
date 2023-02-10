using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class MilestoneConfiguration
    {
        public Guid Id { get; set; }
        public string ScopeReport { get; set; }
        public string Name { get; set; }
        public bool? ClientView { get; set; }
        public DateTime? Deadline { get; set; }
        public string Status { get; set; }
        public string Content { get; set; }
        public Guid? EmployeeId { get; set; }
        public int? WorkingOrder { get; set; }
        public string Note { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid TenantId { get; set; }
        public Guid OptionId { get; set; }
        public int SortOrder { get; set; }
    }
}
