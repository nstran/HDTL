using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class GetProjectScopeRequest : BaseRequest<GetProjectScopeParameter>
    {
        public Guid ProjectId { get; set; }
        public override GetProjectScopeParameter ToParameter()
        {
            return new GetProjectScopeParameter
            { 
                ProjectId = ProjectId,
                UserId = this.UserId,
            };
        }
    }
}
