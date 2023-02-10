using System;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class UpdateProjectStatusRequest : BaseRequest<UpdateProjectStatusParameter>
    {
        public Guid ProjectId { get; set; }
        public string Status { get; set; }
        public override UpdateProjectStatusParameter ToParameter()
        {
            return new UpdateProjectStatusParameter
            {
                ProjectId = ProjectId,
                UserId = UserId,
                Status =Status
            };
        }
    }
}
