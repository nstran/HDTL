using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class DatVeMoiKeHoachOtParameter : BaseParameter
    {
        public int KeHoachOtId { get; set; }
        public bool IsClearEmp { get; set; }
    }
}
