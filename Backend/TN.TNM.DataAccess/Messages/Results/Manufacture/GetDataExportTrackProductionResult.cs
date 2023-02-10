using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class GetDataExportTrackProductionResult: BaseResult
    {
        public List<Models.Manufacture.ProductionOrderEntityModel> ListProductionOrder { get; set; }
    }
}
