using System;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.BusinessLogic.Models.Project
{
    public class ProductVendorModel : BaseModel<DataAccess.Databases.Entities.ProjectVendor>
    {
        public Guid ProjectVendorId { get; set; }
        public Guid VendorId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? ContactId { get; set; }
        public Guid PaymentMethodId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
      

        public ProductVendorModel() { }

        public ProductVendorModel(ProjectVendorEntityModel model)
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
