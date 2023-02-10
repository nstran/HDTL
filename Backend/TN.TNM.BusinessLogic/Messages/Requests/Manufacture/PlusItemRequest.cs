using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Manufacture;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class PlusItemRequest : BaseRequest<PlusItemParameter>
    {
        public ProductionOrderHistoryModel ProductionOrderHistory { get; set; }
        public override PlusItemParameter ToParameter()
        {
            return new PlusItemParameter()
            {
                UserId = UserId,
                ProductionOrderHistory = ProductionOrderHistory.ToEntity()
            };
        }
    }
}
