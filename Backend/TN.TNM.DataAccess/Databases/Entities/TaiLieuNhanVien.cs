using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class TaiLieuNhanVien
    {
        public int TaiLieuNhanVienId { get; set; }
        public string TenTaiLieu { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime? NgayNop { get; set; }
        public DateTime? NgayHen { get; set; }
        public bool? Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public int? CauHinhChecklistId { get; set; }
    }
}
