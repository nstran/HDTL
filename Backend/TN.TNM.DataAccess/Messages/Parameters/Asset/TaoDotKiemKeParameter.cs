using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Asset
{
    public class TaoDotKiemKeParameter: BaseParameter
    {
        public int? DotKiemKeId { get; set; }
        public string TenDotKiemKe { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
    }
}
