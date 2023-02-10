using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class GetMasterDataProjectInformationRequest : BaseRequest<GetMasterDataProjectInformationParameter>
    {
        public Guid ProjectId { get; set; }

        public override GetMasterDataProjectInformationParameter ToParameter()
        {
            return new GetMasterDataProjectInformationParameter
            {
                ProjectId = this.ProjectId,
                UserId = this.UserId
            };
        }
    }
}
