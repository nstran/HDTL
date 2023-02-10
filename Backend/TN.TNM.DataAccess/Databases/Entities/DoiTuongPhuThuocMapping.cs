using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class DoiTuongPhuThuocMapping
    {
        public int Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
