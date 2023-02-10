using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class NoiDungKyDanhGia
    {
        public int NoiDungKyDanhGiaId { get; set; }
        public int KyDanhGiaId { get; set; }
        public int PhieuDanhGiaId { get; set; }
        public Guid? PositionId { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
