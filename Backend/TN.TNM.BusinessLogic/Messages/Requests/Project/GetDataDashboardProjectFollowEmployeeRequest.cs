using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class GetDataDashboardProjectFollowEmployeeRequest : BaseRequest<GetDataDashboardProjectFollowEmployeeParameter>
    {
        public Guid ProjectId { get; set; }

        public string Mode { get; set; }

        public override GetDataDashboardProjectFollowEmployeeParameter ToParameter()
        {
            return new GetDataDashboardProjectFollowEmployeeParameter
            {
                ProjectId = this.ProjectId,
                Mode = this.Mode,
                UserId = this.UserId
            };
        }
    }
}
