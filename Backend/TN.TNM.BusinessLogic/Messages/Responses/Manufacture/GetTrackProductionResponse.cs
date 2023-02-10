using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Manufacture;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class GetTrackProductionResponse : BaseResponse
    {
        public List<TrackProductionModel> ListTrackProduction { get; set; }
        public int TotalRecords { get; set; }
        public List<ProductionOrderEntityModel> ListProductionOrder { get; set; }
    }
}
