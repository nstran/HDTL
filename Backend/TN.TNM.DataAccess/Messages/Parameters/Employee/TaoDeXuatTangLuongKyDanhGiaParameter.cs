using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class TaoDeXuatTangLuongKyDanhGiaParameter: BaseParameter
    {
        public int KyDanhGiaId { get; set; }
        public Guid OrgId { get; set; }
    }
}
