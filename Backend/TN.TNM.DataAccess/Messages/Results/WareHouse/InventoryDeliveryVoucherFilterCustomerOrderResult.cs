using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Order;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class InventoryDeliveryVoucherFilterCustomerOrderResult:BaseResult
    {
        public List<CustomerOrderEntityModel> listCustomerOrder { get; set; }

    }
}
