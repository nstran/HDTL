using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.BusinessLogic.Messages.Responses.Project
{
    public class GetDataEVNProjectDashboardResponse : BaseResponse
    {
        public List<ChartEvn> ListChartEvn { get; set; }
        public List<PerformanceCost> ListPerformanceCost { get; set; }
    }
}
