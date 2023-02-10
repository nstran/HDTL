using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Task;

namespace TN.TNM.BusinessLogic.Messages.Requests.Task
{
    public class GetMasterDataCreateOrUpdateTaskRequest : BaseRequest<GetMasterDataCreateOrUpdateTaskParameter>
    {
        public Guid? ProjectId { get; set; }
        public Guid? TaskId { get; set; }
        public override GetMasterDataCreateOrUpdateTaskParameter ToParameter()
        {
            return new GetMasterDataCreateOrUpdateTaskParameter
            {
                UserId = this.UserId,
                ProjectId = this.ProjectId,
                TaskId = this.TaskId
            };
        }
    }
}
