using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class DanhMucCauTraLoiDanhGiaMapping
    {
        public int DanhMucCauTraLoiDanhGiaMappingId { get; set; }
        public int ChiTietDanhGiaNhanVienId { get; set; }
        public Guid DanhMucId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
