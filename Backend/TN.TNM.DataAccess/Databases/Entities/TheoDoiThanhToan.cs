using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class TheoDoiThanhToan
    {
        public Guid TheoDoiThanhToanId { get; set; }
        public Guid ContractId { get; set; }
        public string LanThanhToan { get; set; }
        public string DieuKienThanhToan { get; set; }
        public DateTime? NgayThanhToan { get; set; }
        public decimal? SoTienBaoGomVat { get; set; }
        public int? TrangThai { get; set; }
        public Guid? TenantId { get; set; }
    }
}
