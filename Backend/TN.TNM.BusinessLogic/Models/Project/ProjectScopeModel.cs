using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Project;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.BusinessLogic.Models.Project
{
    public class ProjectScopeModel : BaseModel<DataAccess.Databases.Entities.ProjectScope>
    {
        public Guid ProjectScopeId { get; set; }
        public string ProjectScopeCode { get; set; }
        public string ProjectScopeName { get; set; }
        public Guid? ResourceType { get; set; }
        public string Description { get; set; }
        public string StyleClass { get; set; }
        public int Level { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? ParentId { get; set; }
        public List<string> ListTask { get; set; }
        public List<string> ListEmployee { get; set; }
        public ProjectScopeModel() { }

        public ProjectScopeModel(DataAccess.Models.Project.ProjectScopeModel model)
        {
            Mapper(model, this);
        }

        public override ProjectScope ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.ProjectScope();
            Mapper(this, entity);
            return entity;
        }
    }
}
