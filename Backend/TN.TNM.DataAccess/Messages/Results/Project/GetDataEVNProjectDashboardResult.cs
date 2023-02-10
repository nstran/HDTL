using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.DataAccess.Messages.Results.Project
{
    public class GetDataEVNProjectDashboardResult : BaseResult
    {
        public List<ChartEvn> ListChartEvn { get; set; }
        public List<PerformanceCost> ListPerformanceCost { get; set; }
    }
}
