using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ServicePacketMappingOptions
    {
        public Guid Id { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid TenantId { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        public Guid? OptionId { get; set; }
        public Guid? ServicePacketId { get; set; }
        public int? SortOrder { get; set; }
    }
}
