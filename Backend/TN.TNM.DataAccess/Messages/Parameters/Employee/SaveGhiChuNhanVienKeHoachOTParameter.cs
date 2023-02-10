using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class SaveGhiChuNhanVienKeHoachOTParameter: BaseParameter
    {
        public int ThanhVienOtId { get; set; }
        public string GhiChu { get; set; }
    }
}
