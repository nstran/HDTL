using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class GetMasterDataCommonDashboardProjectRequest : BaseRequest<GetMasterDataCommonDashboardProjectParameter>
    {
        public Guid ProjectId { get; set; }
        public override GetMasterDataCommonDashboardProjectParameter ToParameter()
        {
            return new GetMasterDataCommonDashboardProjectParameter
            {
                ProjectId = this.ProjectId,
                UserId = this.UserId
            };
        }
    }
}
