using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Manufacture;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class UpdateTotalProductionOrderRequest : BaseRequest<UpdateTotalProductionOrderParameter>
    {
        public TotalProductionOrderModel TotalProductionOrder { get; set; }
        public List<Guid> ListProductionOrderId { get; set; }
        public string OldStatusCodeFe { get; set; }
        public override UpdateTotalProductionOrderParameter ToParameter()
        {
            return new UpdateTotalProductionOrderParameter()
            {
                UserId = UserId,
                TotalProductionOrder = TotalProductionOrder.ToEntity(),
                ListProductionOrderId = ListProductionOrderId,
                OldStatusCodeFe = OldStatusCodeFe
            };
        }
    }
}
