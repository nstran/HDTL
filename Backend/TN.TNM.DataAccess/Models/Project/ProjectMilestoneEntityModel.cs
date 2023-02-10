using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Project
{
    public class ProjectMilestoneEntityModel : BaseModel<DataAccess.Databases.Entities.ProjectMilestone>
    {
        public Guid ProjectMilestonesId { get; set; }
        public string Name { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public Guid ProjectId { get; set; }
        public Guid CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }

        public string UpdateTimeStr { get; set; }

        public int? TaskNumber { get; set; }
        public int? TaskInProgressNumber { get; set; }
        public int? TaskCloseNumber { get; set; }
        
        public string CreateByName { get; set; }
        public double? DelayNumber { get; set; }
        public decimal? ProjectMilestoneComplete { get; set; }

        public ProjectMilestoneEntityModel() { }

        public ProjectMilestoneEntityModel(DataAccess.Databases.Entities.ProjectMilestone model)
        {
            Mapper(model, this);
        }

        public override DataAccess.Databases.Entities.ProjectMilestone ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.ProjectMilestone();
            Mapper(this, entity);
            return entity;
        }
    }
}
