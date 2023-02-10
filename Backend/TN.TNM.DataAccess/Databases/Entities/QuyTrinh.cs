using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class QuyTrinh
    {
        public Guid Id { get; set; }
        public string TenQuyTrinh { get; set; }
        public string MaQuyTrinh { get; set; }
        public int DoiTuongApDung { get; set; }
        public bool HoatDong { get; set; }
        public string MoTa { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? TenantId { get; set; }
    }
}
