using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CauHinhBaoHiemLoftCare
    {
        public int CauHinhBaoHiemLoftCareId { get; set; }
        public Guid? TenantId { get; set; }
        public string NamCauHinh { get; set; }
    }
}
