using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.CustomerOrder;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetMasterDataOrderActionDetailResult: BaseResult
    {
        public CustomerOrderEntityModel CustomerOrderAction { get; set; }
        public CustomerOrderEntityModel CustomerOrder { get; set; }
        public List<CustomerOrderTaskEntityModel> ListCustomerOrderTask { get; set; }
        public List<ReportPointEntityModel> ListReportPoint { get; set; }
        public string EmpNameCreator { get; set; }
        public bool IsActionStep { get; set; }
        public bool IsConfirmStep { get; set; }


    }
}
