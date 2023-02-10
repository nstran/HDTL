using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class GetTrackProductionResult : BaseResult
    {
        public List<TrackProductionEntityModel> ListTrackProduction { get; set; }
        public int TotalRecords { get; set; }
        public List<ProductionOrderEntityModel> ListProductionOrder { get; set; }
    }
}
