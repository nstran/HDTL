using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Project
{
    public class GetDataDashboardProjectFollowEmployeeParameter : BaseParameter
    {
        public Guid ProjectId { get; set; }

        // Day - ngày; Week - Tuần; Month - Tháng; Year - Năm
        public string Mode { get; set; }
    }
}
