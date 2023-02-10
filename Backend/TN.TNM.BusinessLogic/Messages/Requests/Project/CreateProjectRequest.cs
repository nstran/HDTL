using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Project;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class CreateProjectRequest : BaseRequest<CreateProjectParameter>
    {
        public ProjectModel Project { get; set; }
        public List<ProjectTargetModel>  ListProjectTarget { get; set; }
        public override CreateProjectParameter ToParameter()
        {
            List<DataAccess.Databases.Entities.ProjectObjective> lst = new List<DataAccess.Databases.Entities.ProjectObjective>();
            ListProjectTarget.ForEach(target => {
                lst.Add(target.ToEntity());
            });
            return new CreateProjectParameter()
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
