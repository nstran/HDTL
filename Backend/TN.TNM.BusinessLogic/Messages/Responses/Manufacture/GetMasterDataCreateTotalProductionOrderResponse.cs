using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class GetMasterDataCreateTotalProductionOrderResponse : BaseResponse
    {
        public List<CategoryModel> ListStatus { get; set; }
        public List<ProductionOrderModel> ListProductionOrder { get; set; }
    }
}
