using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class PartItemMapping
    {
        public Guid PartItemMappingId { get; set; }
        public Guid? ProductionOrderHistoryId { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? ProductionOrderMappingId { get; set; }
        public bool? HasCreated { get; set; }
        public Guid? TenantId { get; set; }
    }
}
