using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class LichSuThayDoiChucVuModel
    {
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public Guid PositionId { get; set; }
        public string PositionName { get; set; }
    }
}
