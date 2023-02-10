using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.BusinessLogic.Models.Project
{
    public class TaskModel : BaseModel<DataAccess.Databases.Entities.Task>
    {
        public Guid TaskId { get; set; }
        public Guid ProjectScopeId { get; set; }
        public string TaskCode { get; set; }
        public string TaskName { get; set; }
        public string ProjectScopeName { get; set; }
        public string ProjectScopeCode { get; set; }
        public string Employee { get; set; }
        public DateTime? PlanStartTime { get; set; }
        public DateTime? PlanEndTime { get; set; }
        public decimal? Hour { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public decimal? ActualHour { get; set; }
        public decimal? TaskComplate { get; set; }
        public string Description { get; set; }
        public Guid? Status { get; set; }
        public string StatusName { get; set; }
        public string TaskTyeName { get; set; }
        public string CreateByName { get; set; }
        public int? Priority { get; set; }
        public Guid? MilestonesId { get; set; }
        public decimal? EstimateHour { get; set; }
        public bool CanEdit { get; set; }
        public string BackgroundColorForStatus { get; set; }
        public List<ProjectScopeModel> ListProjectScope { get; set; }
        public TaskModel() { }
        public TaskModel(TaskEntityModel model)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(model, this);
        }

        public override Task ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.Task();
            Mapper(this, entity);
            return entity;
        }
    }
}
