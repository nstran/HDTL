using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class GetInventoryNumberRequest : BaseRequest<GetInventoryNumberParameter>
    {
        public Guid? WareHouseId { get; set; }
        public Guid? ProductId { get; set; }
        public override GetInventoryNumberParameter ToParameter()
        {
            return new GetInventoryNumberParameter
            {
                WareHouseId = this.WareHouseId,
                ProductId = this.ProductId,
                UserId = this.UserId
            };
        }
    }
}
