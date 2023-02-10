using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class GetPermissionRequest : BaseRequest<GetPermissionParameter>
    {
        public Guid ProjectId { get; set; }

        public override GetPermissionParameter ToParameter()
        {
            return new GetPermissionParameter
            {
                UserId = this.UserId,
                ProjectId = this.ProjectId
            };
        }
    }
}
