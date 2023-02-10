using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Manufacture;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class MinusItemRequest : BaseRequest<MinusItemParameter>
    {
        public ProductionOrderHistoryModel ProductionOrderHistory { get; set; }
        public List<Guid> ListItemId { get; set; }
        public override MinusItemParameter ToParameter()
        {
            return new MinusItemParameter()
            {
                UserId = UserId,
                ProductionOrderHistory = ProductionOrderHistory.ToEntity(),
                ListItemId = ListItemId
            };
        }
    }
}
