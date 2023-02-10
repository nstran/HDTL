using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class KeHoachOtPhongBan
    {
        public int Id { get; set; }
        public Guid OrganizationId { get; set; }
        public int KeHoachOtId { get; set; }
        public Guid? TenantId { get; set; }
        public int? TrangThai { get; set; }
    }
}
