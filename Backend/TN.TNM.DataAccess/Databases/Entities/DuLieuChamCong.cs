using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class DuLieuChamCong
    {
        public int DuLieuChamCongId { get; set; }
        public Guid EmployeeId { get; set; }
        public int? LoaiDmvs { get; set; }
        public int? TongThoiGian { get; set; }
        public DateTime NgayDmvs { get; set; }
        public int CaLamViecId { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
