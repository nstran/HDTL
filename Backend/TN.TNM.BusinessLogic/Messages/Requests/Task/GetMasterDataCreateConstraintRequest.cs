using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Task;

namespace TN.TNM.BusinessLogic.Messages.Requests.Task
{
    public class GetMasterDataCreateConstraintRequest : BaseRequest<GetMasterDataCreateConstraintParameter>
    {
        public Guid? TaskId { get; set; }
        public Guid ProjectId { get; set; }
        public override GetMasterDataCreateConstraintParameter ToParameter()
        {
            return new GetMasterDataCreateConstraintParameter
            {
                ProjectId = this.ProjectId,
                TaskId = this.TaskId,
                UserId = this.UserId
            };
        }
    }
}
