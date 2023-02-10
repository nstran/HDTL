using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Salary
{
    public class UpdateMucTroCapKhacParameter : BaseParameter
    {
        public decimal MucTroCap { get; set; }
        public int LuongCtLoaiTroCapKhacId { get; set; }
    }
}
