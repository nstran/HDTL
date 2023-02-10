using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CauHinhOtCaNgay
    {
        public int CauHinhOtCaNgayId { get; set; }
        public TimeSpan GioVaoSang { get; set; }
        public TimeSpan GioRaSang { get; set; }
        public TimeSpan GioKetThucSang { get; set; }
        public TimeSpan GioVaoChieu { get; set; }
        public TimeSpan GioRaChieu { get; set; }
        public TimeSpan GioKetThucChieu { get; set; }
        public Guid? TenantId { get; set; }
    }
}
