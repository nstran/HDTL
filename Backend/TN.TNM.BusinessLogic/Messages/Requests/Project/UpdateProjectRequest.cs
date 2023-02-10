using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Project;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class UpdateProjectRequest : BaseRequest<UpdateProjectParameter>
    {
        public ProjectModel Project { get; set; }
        public List<ProjectTargetModel>  ListProjectTarget { get; set; }
        public override UpdateProjectParameter ToParameter()
        {
            List<DataAccess.Databases.Entities.ProjectObjective> lst = new List<DataAccess.Databases.Entities.ProjectObjective>();
            ListProjectTarget.ForEach(target => {
                lst.Add(target.ToEntity());
            });
            return new UpdateProjectParameter()
            {
                //Project = Project.ToEntity(),
                UserId = UserId,
                //ListEmployeeSm = Project.EmployeeSM,
                //ListEmployeeSub = Project.EmployeeSub,
                //ListProjectTarget = lst
            };
        }
    }
}
