using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class GetAllProductionOrderResponse : BaseResponse
    {
        public List<ProductionOrderModel> ListProductionOrder { get; set; }
    }
}
