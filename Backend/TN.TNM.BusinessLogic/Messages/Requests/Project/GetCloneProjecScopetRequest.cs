using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class GetCloneProjecScopetRequest : BaseRequest<GetCloneProjectScopeParameter>
    {
        public Guid NewProjectId { get; set; }
        public Guid OldProjectId { get; set; }
        public override GetCloneProjectScopeParameter ToParameter()
        {
            return new GetCloneProjectScopeParameter
            {
                NewProjectId = NewProjectId,
                OldProjectId = OldProjectId
            };
        }
    }
}
