using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class GetMasterUpdateProjectRequest : BaseRequest<GetMasterUpdateProjectParameter>
    {
        public Guid ProjectId { get; set; }
        public override GetMasterUpdateProjectParameter ToParameter()
        {
            return new GetMasterUpdateProjectParameter
            {
                ProjectId = ProjectId,
                UserId  = this.UserId
            };
        }
    }
}
