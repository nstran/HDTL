using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class DeXuatXinNghi
    {
        public int DeXuatXinNghiId { get; set; }
        public string Code { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid? PositionId { get; set; }
        public int LoaiDeXuatId { get; set; }
        public string LyDo { get; set; }
        public int TrangThaiId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public string LyDoTuChoi { get; set; }
    }
}
