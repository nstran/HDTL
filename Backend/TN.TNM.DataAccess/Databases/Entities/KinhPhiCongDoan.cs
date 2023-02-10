using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class KinhPhiCongDoan
    {
        public int KinhPhiCongDoanId { get; set; }
        public decimal MucDongNhanVien { get; set; }
        public decimal MucDongCongTy { get; set; }
        public Guid? TenantId { get; set; }
    }
}
