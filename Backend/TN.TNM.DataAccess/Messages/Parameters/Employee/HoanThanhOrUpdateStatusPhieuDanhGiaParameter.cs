using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class HoanThanhOrUpdateStatusPhieuDanhGiaParameter: BaseParameter
    {
        public int PhieuDanhGiaId { get; set; }
        public int? TrangThaiPhieuDanhGia { get; set; }
        public int? Type { get; set; }
    }
}
