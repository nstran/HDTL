using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class KeHoachOtPhongBanEntityModel
    {
        public int? Id { get; set; }
        public Guid? OrganizationId { get; set; }
        public int? KeHoachOtId { get; set; }
        public int? TrangThai { get; set; }
        public Guid? TenantId { get; set; }
    }
}
