using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GiaHanThemKeHoachOTParameter: BaseParameter
    {
        public int KeHoachOtId { get; set; }
        public int Type { get; set; }
        public string GiaHanThemType { get; set; }
        
        public DateTime? GiaHanDangKyOT { get; set; }
        public DateTime? GiaHanPheDuyetDangKyOT { get; set; }
    }
}
