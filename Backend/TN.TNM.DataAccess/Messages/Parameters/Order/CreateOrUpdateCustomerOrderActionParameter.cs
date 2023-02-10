using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.CustomerOrder;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class CreateOrUpdateCustomerOrderActionParameter: BaseParameter
    {
        public Guid? CustomerOrderActionId { get; set; }
        public Guid CustomerOrderId { get; set; }
        public List<CustomerOrderTaskEntityModel> ListSettingEmpToTask { get; set; }
        public int? StatusOrderAction { get; set; }
    }
}
