using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CustomerOrderDetailExten
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid? ServicePacketId { get; set; }
        public string Name { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public int Status { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid TenantId { get; set; }
        public int? Stt { get; set; }
    }
}
