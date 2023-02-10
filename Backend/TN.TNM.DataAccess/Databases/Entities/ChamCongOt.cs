using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ChamCongOt
    {
        public int ChamCongOtId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid LoaiOtId { get; set; }
        public DateTime NgayChamCong { get; set; }
        public Guid? TenantId { get; set; }
        public TimeSpan? GioVaoSang { get; set; }
        public TimeSpan? GioRaSang { get; set; }
        public TimeSpan? GioVaoChieu { get; set; }
        public TimeSpan? GioRaChieu { get; set; }
        public TimeSpan? GioVaoToi { get; set; }
        public TimeSpan? GioRaToi { get; set; }
    }
}
