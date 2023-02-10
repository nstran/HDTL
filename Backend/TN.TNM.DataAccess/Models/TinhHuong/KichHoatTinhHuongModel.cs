using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.TinhHuong
{
    public class KichHoatTinhHuongModel
    {
        public Guid? Id { get; set; }
        public string NoiDung { get; set; }
        public DateTime ThoiDiemKichHoat { get; set; }
        public int SoLuongNguoi { get; set; }
        public string Session { get; set; }
        public Guid? TenantId { get; set; }
    }
}
