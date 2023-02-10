using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.BusinessLogic.Models.Project
{
    public class ProjectModel: BaseModel<DataAccess.Databases.Entities.Project>
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? ContractId { get; set; }
        public Guid? ProjectManagerId { get; set; }
        public string EmployeeName { get; set; }
        public string Description { get; set; }
        public Guid? ProjectType { get; set; }
        public string ProjectTypeName { get; set; }
        public string ProjectStatusCode { get; set; }
        public Guid? ProjectSize { get; set; }
        public Guid? ProjectStatus { get; set; }
        public string BackgroundColorForStatus { get; set; }
        public string ProjectStatusName { get; set; }

        public decimal? TaskComplate { get; set; }
        
        public int? Priority { get; set; }
        public string PriorityName { get; set; }
        public DateTime? ProjectStartDate { get; set; }
        public DateTime? ProjectEndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public bool? IncludeWeekend { get; set; }
        public Guid CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? TenantId { get; set; }
        public string ProjectCode { get; set; }
        public List<Guid> EmployeeSM { get; set; }
        public List<Guid> EmployeeSub { get; set; }

        public decimal? EstimateCompleteTime { get; set; }

        public decimal? BudgetVnd { get; set; }

        public decimal? BudgetUsd { get; set; }

        public decimal? BudgetNgayCong { get; set; }

        public decimal GiaBanTheoGio { get; set; }

        public bool? ProjectStatusPlan { get; set; }

        public ProjectModel() { }

        public ProjectModel(ProjectEntityModel model)
        {
            Mapper(model,this);
        }

        public override DataAccess.Databases.Entities.Project ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.Project();
            Mapper(this, entity);
            return entity;
        }
    }
}
