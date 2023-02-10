using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CauHinhChamCongOt
    {
        public int CauHinhChamCongOtId { get; set; }
        public decimal SoPhut { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
