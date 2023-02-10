using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Task;

namespace TN.TNM.BusinessLogic.Messages.Requests.Task
{
    public class GetProjectScopeByProjectIdRequest : BaseRequest<GetProjectScopeByProjectIdParameter>
    {
        public Guid ProjectId { get; set; }
        public override GetProjectScopeByProjectIdParameter ToParameter()
        {
            return new GetProjectScopeByProjectIdParameter
            {
                UserId = this.UserId,
                ProjectId = this.ProjectId
            };
        }
    }
}
