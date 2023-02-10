using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CauHinhThueTncn
    {
        public int CauHinhThueTncnId { get; set; }
        public decimal SoTienTu { get; set; }
        public decimal SoTienDen { get; set; }
        public decimal PhanTramThue { get; set; }
        public decimal SoBiTruTheoCongThuc { get; set; }
        public Guid? TenantId { get; set; }
    }
}
