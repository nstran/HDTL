using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class DangKyOTOrHuyDangKyOTParameter: BaseParameter
    {
        public int KeHoachOtId { get; set; }
        public bool Type { get; set; }
    }
}
