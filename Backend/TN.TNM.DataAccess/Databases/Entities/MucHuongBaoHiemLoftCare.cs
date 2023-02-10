using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class MucHuongBaoHiemLoftCare
    {
        public int Id { get; set; }
        public int DoiTuongHuong { get; set; }
        public decimal MucHuong { get; set; }
        public int? DonVi { get; set; }
        public decimal LePhi { get; set; }
        public decimal PhiCoDinh { get; set; }
        public decimal PhiTheoLuong { get; set; }
        public decimal MucGiam { get; set; }
        public Guid? TenantId { get; set; }
        public int? QuyenLoiBaoHiemLoftCareId { get; set; }
    }
}
