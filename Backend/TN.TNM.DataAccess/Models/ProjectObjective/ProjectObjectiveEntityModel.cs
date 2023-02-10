using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.ProjectObjective
{
    public class ProjectObjectiveEntityModel : BaseModel<DataAccess.Databases.Entities.ProjectObjective>
    {
        public Guid ProjectObjectId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? ProjectObjectType { get; set; }
        public string ProjectObjectName { get; set; }
        public Guid? ProjectObjectUnit { get; set; }
        public decimal? ProjectObjectValue { get; set; }
        public string ProjectObjectDescription { get; set; }
        public int? OrderNumber { get; set; }
        public Guid? TenantId { get; set; }
        public ProjectObjectiveEntityModel() { }

        public ProjectObjectiveEntityModel(DataAccess.Databases.Entities.ProjectObjective model)
        {
            Mapper(model, this);
        }

        public override DataAccess.Databases.Entities.ProjectObjective ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.ProjectObjective();
            Mapper(this, entity);
            return entity;
        }
    }
}
