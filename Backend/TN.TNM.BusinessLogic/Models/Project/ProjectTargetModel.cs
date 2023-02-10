using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.BusinessLogic.Models.Project
{
    public class ProjectTargetModel : BaseModel<DataAccess.Databases.Entities.ProjectObjective>
    {
        public Guid ProjectObjectId { get; set; }
        public Guid ProjectId { get; set; }
        public int? OrderNumber { get; set; }
        public Guid? ProjectObjectType { get; set; }
        public string ProjectObjectName { get; set; }
        public Guid? ProjectObjectUnit { get; set; }
        public decimal? ProjectObjectValue { get; set; }
        public string ProjectObjectDescription { get; set; }
        public string TargetTypeDisplay { get; set; }
        public string TargetUnitDisplay { get; set; }
        public Guid? TenantId { get; set; }

        public ProjectTargetModel() { }

        public ProjectTargetModel(ProjectTargetEntityModel model)
        {
            Mapper(model, this);
        }

        public override ProjectObjective ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.ProjectObjective();
            Mapper(this, entity);
            return entity;
        }
    }
}
