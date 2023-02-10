using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class QuyenLoiBaoHiemLoftCare
    {
        public int QuyenLoiBaoHiemLoftCareId { get; set; }
        public int NhomBaoHiemLoftCareId { get; set; }
        public string TenQuyenLoi { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
