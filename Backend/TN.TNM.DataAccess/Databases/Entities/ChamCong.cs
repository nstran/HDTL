using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ChamCong
    {
        public int ChamCongId { get; set; }
        public Guid? EmployeeId { get; set; }
        public TimeSpan? VaoSang { get; set; }
        public TimeSpan? RaSang { get; set; }
        public TimeSpan? VaoChieu { get; set; }
        public TimeSpan? RaChieu { get; set; }
        public bool? Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public DateTime NgayChamCong { get; set; }
        public int? KyHieuVaoSang { get; set; }
        public int? KyHieuRaSang { get; set; }
        public int? KyHieuVaoChieu { get; set; }
        public int? KyHieuRaChieu { get; set; }
    }
}
