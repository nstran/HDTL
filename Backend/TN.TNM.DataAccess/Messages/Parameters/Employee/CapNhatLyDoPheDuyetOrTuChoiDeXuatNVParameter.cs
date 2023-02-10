using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class CapNhatLyDoPheDuyetOrTuChoiDeXuatNVParameter: BaseParameter
    {
        public int DeXuatNhanVienId { get; set; }
        public string GhiChu { get; set; }
        public string NghiaVu { get; set; }
        public int LoaiDeXuat { get; set; }
    }
}
