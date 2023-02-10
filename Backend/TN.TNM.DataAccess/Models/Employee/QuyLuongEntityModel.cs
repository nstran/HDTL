using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class QuyLuongEntityModel
    {
        public int? QuyLuongId { get; set; }
        public int? Nam { get; set; }
        public decimal? QuyLuong { get; set; }
        public decimal? Acv { get; set; }
        public bool? Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
