using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class GetDataEVNProjectDashboardRequest : BaseRequest<GetDataEVNProjectDashboardParameter>
    {
        public Guid ProjectId { get; set; }
        // Day - ngày; Week - Tuần; Month - Tháng; Year - Năm
        public string Mode { get; set; }
        public override GetDataEVNProjectDashboardParameter ToParameter()
        {
            return new GetDataEVNProjectDashboardParameter
            {
                ProjectId = this.ProjectId,
                Mode = this.Mode,
                UserId = this.UserId
            };
        }
    }
}
