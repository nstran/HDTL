using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class TroCap
    {
        public int TroCapId { get; set; }
        public int TypeId { get; set; }
        public Guid LoaiTroCapId { get; set; }
        public decimal MucTroCap { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
