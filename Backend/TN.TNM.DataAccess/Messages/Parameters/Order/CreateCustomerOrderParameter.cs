using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.CustomerOrder;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class CreateCustomerOrderParameter : BaseParameter
    {
        public Guid? OrderId { get; set; }
        public CustomList Vat { get; set; }
        public int? DiscountType { get; set; }
        public int? DiscountValue { get; set; }
        public CustomerOrderEntityModel CusOrder { get; set; }
        public List<CustomerOrderDetailEntityModel> ListCustomerDetail { get; set; }
        public List<CustomerOrderExtensionEntityModel> ListAttrPackAndOption { get; set; }
        public List<CustomerOrderDetailExtenEntityModel> ListOrderDetailExten { get; set; }
        public Guid? OrderProcessId { get; set; }
        public Guid? ServicePacketId { get; set; }
        public bool? IsMobile { get; set; }
    }

    public class CustomList
    {
        public int value { get; set; }
        public string name { get; set; }
    }
}
