using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetMasterDataListItemDialogRequest : BaseRequest<GetMasterDataListItemDialogParameter>
    {
        public Guid ProductionOrderId { get; set; }
        public override GetMasterDataListItemDialogParameter ToParameter()
        {
            return new GetMasterDataListItemDialogParameter()
            {
                UserId = UserId,
                ProductionOrderId = ProductionOrderId
            };
        }
    }
}
