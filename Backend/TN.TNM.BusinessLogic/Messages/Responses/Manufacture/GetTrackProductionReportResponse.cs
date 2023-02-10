using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class GetTrackProductionReportResponse: BaseResponse
    {
        public List<DataAccess.Models.Manufacture.TrackProductionReportModel> ListTrackProductionReport { get; set; }
    }
}
