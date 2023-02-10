using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Asset
{
    public class DotKiemKeSearchParameter: BaseParameter
    {
        public string TenDotKiemKe { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public List<int> ListTrangThai { get; set; }
    }
}
