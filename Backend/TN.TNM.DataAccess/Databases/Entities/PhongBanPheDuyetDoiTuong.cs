using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class PhongBanPheDuyetDoiTuong
    {
        public int PhongBanPheDuyetDoiTuongId { get; set; }
        public int DoiTuongApDung { get; set; }
        public int? ObjectNumber { get; set; }
        public Guid? ObjectId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
