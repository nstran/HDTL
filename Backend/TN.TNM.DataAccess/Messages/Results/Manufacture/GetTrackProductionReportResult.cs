using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class GetTrackProductionReportResult: BaseResult
    {
        public List<Models.Manufacture.TrackProductionReportModel> ListTrackProductionReport { get; set; }
    }
}
