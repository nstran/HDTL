using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class KichHoatTinhHuong
    {
        public Guid Id { get; set; }
        public string NoiDung { get; set; }
        public DateTime ThoiDiemKichHoat { get; set; }
        public int SoLuongNguoi { get; set; }
        public string Session { get; set; }
        public Guid? TenantId { get; set; }
    }
}
