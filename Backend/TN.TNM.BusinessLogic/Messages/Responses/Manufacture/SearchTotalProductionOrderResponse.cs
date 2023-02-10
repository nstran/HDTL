using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class SearchTotalProductionOrderResponse : BaseResponse
    {
        public List<TotalProductionOrderModel> ListTotalProductionOrder { get; set; }
        public int TotalRecords { get; set; }
    }
}
