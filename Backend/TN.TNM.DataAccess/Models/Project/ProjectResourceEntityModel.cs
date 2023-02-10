using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Models.Project
{
    public class ProjectResourceEntityModel : BaseModel<DataAccess.Databases.Entities.ProjectResource>
    {
        public Guid ProjectResourceId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? ObjectId { get; set; }
        public Guid ResourceType { get; set; }
        public string ResourceTypeName { get; set; }
        public string EmployeeRoleName { get; set; }        
        public Guid ResourceName { get; set; }
        public string NameResource { get; set; }
        public Guid? ResourceRole { get; set; }
        public Guid? EmployeeRole { get; set; }
        public string ResourceRoleName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool? IsCreateVendor { get; set; }
        public int? Allowcation { get; set; }
        public bool? IncludeWeekend { get; set; }
        public Guid? TenantId { get; set; }
        public int Stt { get; set; }
        public Guid CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool? IsPic { get; set; }
        public bool? IsCheck { get; set; }
        public bool? IsOverload { get; set; }
        public decimal Hours { get; set; }
        public double? WorkDay { get; set; }
        public string BackgroundColorForStatus { get; set; }
        public bool? IsActive { get; set; }

        public decimal HourUsed { get; set; }
        public decimal TotalMoney { get; set; }
        public List<ContactEntityModel> ListContact { get; set; }
        public VendorEntityModel Vendor { get; set; }

        public List<ProjectVendorEntityModel> ListProjectVendor { get; set; }

        public decimal ThoiGianPhanBoDenHienTai { get; set; }
        
        public decimal ChiPhiTheoGio { get; set; }

        public string ProjectCode { get; set; }
        
        public string ProjectName { get; set; }
        public double? WorkDayActual { get; set; }

        public ProjectResourceEntityModel()
        {
            WorkDayActual = 0;
        }

        public ProjectResourceEntityModel(ProjectResourceEntityModel model)
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
