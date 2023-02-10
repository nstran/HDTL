using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.DataAccess.Messages.Results.Project
{
    public class GetDataDashboardProjectFollowEmployeeResult : BaseResult
    {
        public List<ChartTaskFollowTime> ListTaskFollowTime { get; set; }
        
        public List<ChartProjectFollowResource> ListProjectFollowResource { get; set; }

        // public List<ChartTaskFollowStatus> ListTaskFollowStatus { get; set; }
        // public List<CharFollowTaskType> ListTaskFollowTaskType { get; set; }
        // public List<ChartTaskFollowResource> ListTaskFollowResource { get; set; }
        // public List<ChartTimeFollowResource> ListChartTimeFollowResource { get; set; }
    }
}
