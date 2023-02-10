using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class TroCapDieuKienHuongMapping
    {
        public int TroCapDieuKienHuongMappingId { get; set; }
        public int TroCapId { get; set; }
        public Guid DieuKienHuongId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
