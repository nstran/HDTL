using System;
using TN.TNM.DataAccess.Messages.Parameters.Task;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class DeleteProjectScopeRequest : BaseRequest<DeleteProjectScopeParameter>
    {
        public Guid ProjectScopeId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid ParentId { get; set; }
        public override DeleteProjectScopeParameter ToParameter()
        {
            return new DeleteProjectScopeParameter
            {
                ProjectScopeId = ProjectScopeId,
                ProjectId = ProjectId,
                ParentId = ParentId,
                UserId = UserId
            };
        }
    }
}
