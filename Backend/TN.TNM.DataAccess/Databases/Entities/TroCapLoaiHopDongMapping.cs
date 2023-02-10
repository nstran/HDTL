using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class TroCapLoaiHopDongMapping
    {
        public int TroCapLoaiHopDongMappingId { get; set; }
        public int TroCapId { get; set; }
        public Guid LoaiHopDongId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
