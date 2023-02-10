using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class GetAllProductionOrderResult : BaseResult
    {
        public List<ProductionOrderEntityModel> ListProductionOrder { get; set; }
    }
}
