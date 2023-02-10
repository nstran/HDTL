using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class NotificationConfiguration
    {
        public Guid Id { get; set; }
        public Guid? ServicePacketId { get; set; }
        public Guid? CategoryId { get; set; }
        public bool? ServiceRequestMaker { get; set; }
        public bool? ServiceManagement { get; set; }
        public bool? ServiceSupporter { get; set; }
        public bool? Supporter { get; set; }
        public bool? Reporter { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid TenantId { get; set; }
    }
}
