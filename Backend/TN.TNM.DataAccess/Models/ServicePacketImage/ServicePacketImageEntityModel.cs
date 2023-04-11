using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.ServicePacketImage
{
    public class ServicePacketImageEntityModel
    {
        public Guid Id { get; set; }
        public Guid ServicePacketId { get; set; }
        public string MainImage { get; set; }
        public string MainImageName { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid TenantId { get; set; }
        public string BackgroundImage { get; set; }
        public string BackgroundImageName { get; set; }
        public string Icon { get; set; }
        public string IconName { get; set; }
    }
}
