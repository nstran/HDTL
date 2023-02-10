using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class NoiDungKyDanhGiaEntityModel
    {
        public int? NoiDungKyDanhGiaId { get; set; }
        public int? KyDanhGiaId { get; set; }
        public int? PhieuDanhGiaId { get; set; }
        public Guid? PositionId { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
