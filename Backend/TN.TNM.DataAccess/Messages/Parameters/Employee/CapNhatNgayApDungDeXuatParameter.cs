using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class CapNhatNgayApDungDeXuatParameter: BaseParameter
    {
        public DateTime NgayApDung { get; set; }
        public int DeXuatId { get; set; }
        public int LoaiDeXuat { get; set; }
    }
}
