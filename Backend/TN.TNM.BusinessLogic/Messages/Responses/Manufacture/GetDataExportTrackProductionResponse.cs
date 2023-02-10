using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class GetDataExportTrackProductionResponse: BaseResponse
    {
        public List<DataAccess.Models.Manufacture.ProductionOrderEntityModel> ListProductionOrder { get; set; }
    }
}
