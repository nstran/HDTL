using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Task;

namespace TN.TNM.BusinessLogic.Messages.Requests.Task
{
    public class GetAllTaskByProjectScopeIdRequest : BaseRequest<GetAllTaskByProjectIdParameter>
    {
        public Guid ProjectId { get; set; }
        public Guid ProjectScopeId { get; set; }
        public override GetAllTaskByProjectIdParameter ToParameter()
        {
            return new GetAllTaskByProjectIdParameter
            {
                ProjectId = this.ProjectId,
                ProjectScopeId = this.ProjectScopeId
            };
        }
    }
}
