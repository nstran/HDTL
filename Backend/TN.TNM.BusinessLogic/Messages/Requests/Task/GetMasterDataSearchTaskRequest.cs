using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Task;

namespace TN.TNM.BusinessLogic.Messages.Requests.Task
{
    public class GetMasterDataSearchTaskRequest : BaseRequest<GetMasterDataSearchTaskParameter>
    {
        public Guid ProjectId { get; set; }
        public override GetMasterDataSearchTaskParameter ToParameter()
        {
            return new GetMasterDataSearchTaskParameter
            {
                ProjectId = this.ProjectId,
                UserId = this.UserId
            };
        }
    }
}
