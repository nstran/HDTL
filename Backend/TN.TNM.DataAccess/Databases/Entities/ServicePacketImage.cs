using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ServicePacketImage
    {
        public Guid Id { get; set; }
        public Guid ServicePacketId { get; set; }
        public string MainImage { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid TenantId { get; set; }
        public string BackgroundImage { get; set; }
        public string Icon { get; set; }
    }
}
