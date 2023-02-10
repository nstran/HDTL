using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ChiTietDeXuatCongTac
    {
        public int ChiTietDeXuatCongTacId { get; set; }
        public int DeXuatCongTacId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? PositionId { get; set; }
        public Guid? OrganizationId { get; set; }
        public string LyDo { get; set; }
        public bool? Active { get; set; }
    }
}
