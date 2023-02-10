using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ReportPoint
    {
        public Guid Id { get; set; }
        public Guid? ServicePacketMappingOptionsId { get; set; }
        public string Name { get; set; }
        public Guid? OptionId { get; set; }
        public int Order { get; set; }
        public Guid EmpId { get; set; }
        public DateTime? Deadline { get; set; }
        public bool? IsCusView { get; set; }
        public string Content { get; set; }
        public int? Status { get; set; }
        public Guid? OrderActionId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid TenantId { get; set; }
    }
}
