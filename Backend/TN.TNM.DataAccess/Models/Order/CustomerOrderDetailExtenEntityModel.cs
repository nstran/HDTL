using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Helper;

namespace TN.TNM.DataAccess.Models.Order
{
    public class CustomerOrderDetailExtenEntityModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public Guid? ServicePacketId { get; set; }
        public Guid? OrderId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public int? Status { get; set; }
        public string StatusName { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public bool? Edit { get; set; }
        public TrangThaiGeneral StatusObject { get; set; }
        public int? Stt { get; set; }
    }
}
