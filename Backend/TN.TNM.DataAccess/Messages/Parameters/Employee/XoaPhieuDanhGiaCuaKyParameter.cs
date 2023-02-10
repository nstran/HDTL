using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class XoaPhieuDanhGiaCuaKyParameter: BaseParameter
    {
        public int KyDanhGiaId { get; set; }
        public int PhieuDanhGiaId { get; set; }
    }
}
