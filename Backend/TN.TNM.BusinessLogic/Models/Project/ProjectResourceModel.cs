using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Project;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.BusinessLogic.Models.Project
{
    public class ProjectResourceModel : BaseModel<DataAccess.Databases.Entities.ProjectResource>
    {
        public Guid ProjectResourceId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? ObjectId { get; set; }
        public Guid ResourceType { get; set; }
        public string ResourceTypeName { get; set; }
        public Guid ResourceName { get; set; }
        public string NameResource { get; set; }
        public Guid? ResourceRole { get; set; }
        public Guid? EmployeeRole { get; set; }
        public string ResourceRoleName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool? IncludeWeekend { get; set; }
        public int? Allowcation { get; set; }
        public double? WorkDay { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? RoleId { get; set; }
        public int Stt { get; set; }
        public string EmployeeRoleName { get; set; }
        public bool? IsCreateVendor { get; set; }
        public bool? IsOverload { get; set; }
        public List<ContactEntityModel> ListContact { get; set; }
        public List<ProjectVendorEntityModel> ListProjectVendor { get; set; }
        public VendorEntityModel Vendor { get; set; }
        public string BackgroundColorForStatus { get; set; }
        public ProjectResourceModel() { }

        public ProjectResourceModel(ProjectResourceEntityModel model)
        {
            Mapper(model, this);
        }

        public override ProjectResource ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.ProjectResource();
            Mapper(this, entity);
            return entity;
        }
    }
}
