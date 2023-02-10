using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.MilestoneConfiguration
{
    public class MilestoneConfigurationEntityModel : BaseModel<DataAccess.Databases.Entities.MilestoneConfiguration>
    {
        public Guid Id { get; set; }
        public string ScopeReport { get; set; }
        public string Name { get; set; }
        public bool? ClientView { get; set; }
        public DateTime? Deadline { get; set; }
        public string Status { get; set; }
        public string Content { get; set; }
        public Guid? EmployeeId { get; set; }
        public int? WorkingOrder { get; set; }
        public string Note { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; } 
        public Guid TenantId { get; set; }
        public bool IsActive { get; set; }
        public Guid OptionId { get; set; }
        public int SortOrder { get; set; }

        public MilestoneConfigurationEntityModel()
        {
        }
        public override Databases.Entities.MilestoneConfiguration ToEntity()
        {
            var entity = new Databases.Entities.MilestoneConfiguration();
            Mapper(this, entity);
            return entity;
        }
        public MilestoneConfigurationEntityModel(Databases.Entities.MilestoneConfiguration model)
        {
            Mapper(model, this);
        }
    }
}
