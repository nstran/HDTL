using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.BusinessLogic.Models.Project
{
    public class ResourceModel : BaseModel<DataAccess.Databases.Entities.Resource>
    {
        public Guid ResourceId { get; set; }
        public string ResourceName { get; set; }
        public string ResourceDescription { get; set; }
        public Guid CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? TenantId { get; set; }

        public ResourceModel() { }

        public ResourceModel(ResourceEntityModel model)
        {
            Mapper(model, this);
        }

        public override Resource ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.Resource();
            Mapper(this, entity);
            return entity;
        }
    }
}
