using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class MucDanhGia
    {
        public int MucDanhGiaId { get; set; }
        public string TenMucDanhGia { get; set; }
        public decimal? DiemDanhGia { get; set; }
        public DateTime NgayApDung { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
