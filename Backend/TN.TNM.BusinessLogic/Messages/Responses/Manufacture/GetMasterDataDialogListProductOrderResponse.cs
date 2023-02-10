using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class GetMasterDataDialogListProductOrderResponse:BaseResponse
    {
        public List<ProductionOrderMappingModel> ListProduct { get; set; }
    }
}
