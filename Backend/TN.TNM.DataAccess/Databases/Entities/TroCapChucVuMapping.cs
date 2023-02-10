using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class TroCapChucVuMapping
    {
        public int TroCapChucVuMappingId { get; set; }
        public int TroCapId { get; set; }
        public Guid PositionId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
