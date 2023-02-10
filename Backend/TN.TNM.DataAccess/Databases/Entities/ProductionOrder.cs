using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ProductionOrder
    {
        public Guid ProductionOrderId { get; set; }
        public string ProductionOrderCode { get; set; }
        public Guid OrderId { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public string PlaceOfDelivery { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Note { get; set; }
        public string NoteTechnique { get; set; }
        public bool? Especially { get; set; }
        public Guid StatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? ProductionOrderHistoryId { get; set; }
    }
}
