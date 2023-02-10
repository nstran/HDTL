using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class GetStatusResourceProjectRequest : BaseRequest<GetStatusResourceProjectParameter>
    {
        public Guid? ProjectResourceId { get; set; }
        public override GetStatusResourceProjectParameter ToParameter()
        {
            return new GetStatusResourceProjectParameter()
            {
                ProjectResourceId = this.ProjectResourceId,
            };
        }

    }
}
