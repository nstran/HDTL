using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class CheckTonKhoSanPhamParameter:BaseParameter
    {
        public Guid OrderId { get; set; }
        public List<CustomerOrderDetailEntityModel> CustomerOrderDetail { get; set; }
    }
}
