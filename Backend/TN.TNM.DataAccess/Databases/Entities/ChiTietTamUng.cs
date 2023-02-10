using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ChiTietTamUng
    {
        public int ChiTietTamUngId { get; set; }
        public int? ChiTietTamUngParentId { get; set; }
        public int DeNghiTamUngId { get; set; }
        public string NoiDung { get; set; }
        public Guid? OrganizationId { get; set; }
        public decimal? SoTienTamUng { get; set; }
        public decimal? Vat { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
