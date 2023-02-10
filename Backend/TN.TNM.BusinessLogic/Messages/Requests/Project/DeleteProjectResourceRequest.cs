using System;
using TN.TNM.DataAccess.Messages.Parameters.Task;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class DeleteProjectResourceRequest : BaseRequest<DeleteProjectResourceParameter>
    {
        public Guid ProjectResourceId { get; set; }
        public override DeleteProjectResourceParameter ToParameter()
        {
            return new DeleteProjectResourceParameter
            {
                ProjectResourceId = this.ProjectResourceId,
                UserId = this.UserId
            };
        }
    }
}
