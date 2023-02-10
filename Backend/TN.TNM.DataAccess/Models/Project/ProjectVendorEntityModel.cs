using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Models.Project
{
    public class ProjectVendorEntityModel : BaseModel<DataAccess.Databases.Entities.ProjectVendor>
    {
        public Guid ProjectVendorId { get; set; }
        public Guid ProjectResourceId { get; set; }
        public Guid VendorId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? ContactId { get; set; }
        public Guid PaymentMethodId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public ProjectVendorEntityModel() { }

        public ProjectVendorEntityModel(ProjectVendorEntityModel model)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(model, this);
        }

        public override DataAccess.Databases.Entities.ProjectVendor ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.ProjectVendor();
            Mapper(this, entity);
            return entity;
        }
    }
}
