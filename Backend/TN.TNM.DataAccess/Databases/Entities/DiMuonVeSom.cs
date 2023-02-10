using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class DiMuonVeSom
    {
        public int DiMuonVeSomId { get; set; }
        public Guid EmployeeId { get; set; }
        public int? SoPhutDmvs { get; set; }
        public decimal? SoNgayDmvs { get; set; }
        public decimal? TongNgayNghiCoPhep { get; set; }
        public decimal? TongNgayNghiKhongPhep { get; set; }
        public int? SoLanDmvs { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
