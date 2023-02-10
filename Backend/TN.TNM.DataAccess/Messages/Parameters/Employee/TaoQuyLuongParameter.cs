using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class TaoQuyLuongParameter: BaseParameter
    {
        public int? QuyLuongId { get; set; }
        public int Nam { get; set; }
        public Decimal QuyLuong { get; set; }
    }
}
