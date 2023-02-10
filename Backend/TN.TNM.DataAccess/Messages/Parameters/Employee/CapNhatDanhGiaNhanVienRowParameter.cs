using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class CapNhatDanhGiaNhanVienRowParameter: BaseParameter
    {
        public int DanhGiaNhanVienId { get; set; }
        public decimal? MucLuongDeXuatQuanLy { get; set; }
    }
}
