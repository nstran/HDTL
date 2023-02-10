using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class KichHoatTinhHuongChiTiet
    {
        public Guid Id { get; set; }
        public Guid KichHoatTinhHuongId { get; set; }
        public string HoVaTen { get; set; }
        public string Sdt { get; set; }
        public string NoiDung { get; set; }
        public string PhanHoi { get; set; }
        public string Session { get; set; }
        public Guid? TenantId { get; set; }
    }
}
