using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GiaHanPheDuyetKeHoachOTParameter: BaseParameter
    {
        public int KeHoachOtId { get; set; }
        public DateTime HanPheDuyetKeHoachOt { get; set; }
        public DateTime HanDangKyKeHoachOT { get; set; }
        public DateTime HanPheDuyetDangKyOt { get; set; }
    }
}
