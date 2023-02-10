using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Project;
using TN.TNM.DataAccess.Messages.Parameters.Project;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class UpdateProjectScopeRequest : BaseRequest<UpdateProjectScopeParameter>
    {


        public Models.Project.ProjectScopeModel ProjectScope { get; set; }
        public Guid ParentId { get; set; }

        public override UpdateProjectScopeParameter ToParameter()
        {
            DataAccess.Models.Project.ProjectScopeModel obj = new DataAccess.Models.Project.ProjectScopeModel();
            obj.ProjectScopeId = ProjectScope.ProjectScopeId;
            obj.ParentId = ProjectScope.ParentId;
            obj.Description = ProjectScope.Description;
            obj.ProjectId = ProjectScope.ProjectId;
            obj.ProjectScopeCode = ProjectScope.ProjectScopeCode;
            obj.ProjectScopeName = ProjectScope.ProjectScopeName;
            obj.ResourceType = ProjectScope.ResourceType;
            obj.TenantId = ProjectScope.TenantId;
            obj.ListTask = ProjectScope.ListTask;
            obj.Level = ProjectScope.Level;

            return new UpdateProjectScopeParameter()
            {
                ProjectScope = obj,
                ParentId = ParentId,
                UserId = UserId
            };
        }
    }
}
