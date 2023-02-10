using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class TroCapCoDinh
    {
        public int TroCapCoDinhId { get; set; }
        public int KyLuongId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid PositionId { get; set; }
        public decimal? SoNgayNghi { get; set; }
        public int? SoLanDmvs { get; set; }
        public decimal? AnTrua { get; set; }
        public decimal? DiLai { get; set; }
        public decimal? DienThoai { get; set; }
        public decimal? ChuyenCanNgayCong { get; set; }
        public decimal? ChuyenCanDmvs { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid TenantId { get; set; }
    }
}
