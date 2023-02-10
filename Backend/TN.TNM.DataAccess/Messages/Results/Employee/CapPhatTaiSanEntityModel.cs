using System;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class CapPhatTaiSanEntityModel
    {
        public int CapPhatTaiSanId { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string MaTaiSan { get; set; }
        public string TenTaiSan { get; set; }
        public string MucDichSuDung { get; set; }
        public string MoTa { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
