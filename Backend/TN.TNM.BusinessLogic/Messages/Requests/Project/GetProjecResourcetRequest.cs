using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class GetProjectResourceRequest : BaseRequest<GetProjectResourceParameter>
    {
        public Guid ProjectId { get; set; }
        public override GetProjectResourceParameter ToParameter()
        {
            return new GetProjectResourceParameter
            {
                ProjectId = ProjectId,
                UserId = this.UserId,
            };
        }
    }
}
