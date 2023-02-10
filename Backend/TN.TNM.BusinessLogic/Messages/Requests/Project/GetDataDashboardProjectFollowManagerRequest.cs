using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class GetDataDashboardProjectFollowManagerRequest : BaseRequest<GetDataDashboardProjectFollowManagerParameter>
    {
        public Guid ProjectId { get; set; }
        public string Mode { get; set; }
        public override GetDataDashboardProjectFollowManagerParameter ToParameter()
        {
            return new GetDataDashboardProjectFollowManagerParameter
            {
                ProjectId = this.ProjectId,
                Mode = this.Mode
            };
        }
    }
}
