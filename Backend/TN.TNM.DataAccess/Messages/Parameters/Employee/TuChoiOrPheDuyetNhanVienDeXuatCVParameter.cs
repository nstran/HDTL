using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class TuChoiOrPheDuyetNhanVienDeXuatCVParameter: BaseParameter
    {
        public List<DeXuatChucVuNhanVienEntityModel> ListNV { get; set; }
        public int DeXuatChucVuId { get; set; }
        public bool IsXacNhan { get; set; }
        public string LyDoTuChoi { get; set; }

    }
}
