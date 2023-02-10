using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CongThucTinhLuong
    {
        public int CongThucTinhLuongId { get; set; }
        public string CongThuc { get; set; }
        public Guid? TenantId { get; set; }
    }
}
