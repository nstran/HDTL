using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class GetMasterDataAddProductionOrderDialogResponse : BaseResponse
    {
        public List<ProductionOrderModel> ListProductionOrder { get; set; }
    }
}
