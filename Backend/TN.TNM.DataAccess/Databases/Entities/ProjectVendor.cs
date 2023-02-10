using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ProjectVendor
    {
        public Guid ProjectVendorId { get; set; }
        public Guid ProjectResourceId { get; set; }
        public Guid VendorId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? ContactId { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
