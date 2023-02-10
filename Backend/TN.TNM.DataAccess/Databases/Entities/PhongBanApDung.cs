using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class PhongBanApDung
    {
        public Guid Id { get; set; }
        public Guid CacBuocApDungId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid CacBuocQuyTrinhId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
