using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Order
{
    public class OrderProcessDetailEntityModel
    {
        public Guid? Id { get; set; }
        public int? StepId { get; set; }
        public Guid? ServicePacketId { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryCode { get; set; }
        public int? Status { get; set; }
        public string StatusName { get; set; }
        public Guid? OrderProcessId { get; set; }
        public Guid? OrderId { get; set; }
        public Guid? OrderActionId { get; set; }
        public List<CustomerOrderEntityModel> ListExtenOrder { get; set; }
        public bool? IsAction { get; set; } //Người đăng nhập có quyền thao tác tại bước này


    }
}
