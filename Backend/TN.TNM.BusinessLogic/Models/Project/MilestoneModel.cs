using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.BusinessLogic.Models.Project
{
    public class ProjectMilestoneModel : BaseModel<DataAccess.Databases.Entities.ProjectMilestone>
    {
        public Guid MilestonesId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? ProjectId { get; set; }       
        public Guid CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public List<ProjectTaskEntityModel> ListTask { get; set; }
        public int TotalTask { get; set; }
        public int TotalInProcess { get; set; }
        public int TotalDone { get; set; }

        public ProjectMilestoneModel() { }

        public ProjectMilestoneModel(ProjectMilestoneEntityModel model)
        {
            Mapper(model,this);
        }

        public override ProjectMilestone ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.ProjectMilestone();
            Mapper(this, entity);
            return entity;
        }
    }
}
