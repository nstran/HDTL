using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class NhomBaoHiemLoftCare
    {
        public int NhomBaoHiemLoftCareId { get; set; }
        public int CauHinhBaoHiemLoftCareId { get; set; }
        public string TenNhom { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
